using Microsoft.Data.Sqlite;

namespace CLImmo;

/// <summary>
/// Zorgt voor alle communicatie met de SQLite-databank van het immokantoor:
/// makelaars en panden ophalen (eventueel gefilterd), een nieuw pand invoegen,
/// en een pand als verkocht markeren. Dit is de enige klasse in het project die
/// rechtstreeks SQL-commando's uitvoert; de UI-laag (WpfImmo) roept enkel deze
/// methodes aan en werkt zelf nooit rechtstreeks met SqliteConnection/SqliteCommand.
/// </summary>
public class PandRepository
{
    // De volledige connection string naar het db-bestand, bv.
    // "Data Source=C:\...\Data\ImmoDB.db".
    private readonly string connectionString;

    /// <summary>
    /// Maakt een nieuwe repository aan die verbindt met het opgegeven
    /// SQLite-bestand.
    /// </summary>
    /// <param name="dbBestandsPad">Volledig pad naar het .db-bestand.</param>
    public PandRepository(string dbBestandsPad)
    {
        connectionString = "Data Source=" + dbBestandsPad;
    }

    /// <summary>
    /// Haalt alle makelaars op uit de databank, gesorteerd op voornaam. Wordt
    /// gebruikt om de makelaar-combobox in de UI te vullen en om panden aan hun
    /// makelaar te koppelen.
    /// </summary>
    public List<Makelaar> GeefAlleMakelaars()
    {
        List<Makelaar> resultaat = new List<Makelaar>();

        using (SqliteConnection connectie = new SqliteConnection(connectionString))
        {
            connectie.Open();

            using (SqliteCommand commando = connectie.CreateCommand())
            {
                commando.CommandText = "SELECT id, voornaam, achternaam, telefoon, email FROM makelaars ORDER BY voornaam;";

                using (SqliteDataReader reader = commando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string id = reader.GetString(0);
                        string voornaam = reader.GetString(1);
                        string achternaam = reader.GetString(2);
                        string telefoon = reader.GetString(3);
                        string email = reader.GetString(4);

                        Makelaar makelaar = new Makelaar(id, voornaam + " " + achternaam, telefoon, email);
                        resultaat.Add(makelaar);
                    }
                }
            }
        }

        return resultaat;
    }

    /// <summary>
    /// Haalt alle panden op, zonder filter. Gemakshalve gebouwd bovenop
    /// GeefGefilterdePanden.
    /// </summary>
    public List<Pand> GeefAllePanden()
    {
        return GeefGefilterdePanden(null, null, false);
    }

    /// <summary>
    /// Haalt panden op uit de databank, met optionele filters: op energielabel,
    /// op makelaar (via diens Id), en/of enkel panden die nog te koop staan.
    /// Een filterparameter die null is (of enkelTeKoop=false) wordt genegeerd.
    /// </summary>
    /// <param name="energielabel">Enkel panden met dit energielabel, of null voor alle labels.</param>
    /// <param name="makelaarId">Enkel panden van deze makelaar, of null voor alle makelaars.</param>
    /// <param name="enkelTeKoop">Als true: enkel panden die nog niet verkocht zijn.</param>
    public List<Pand> GeefGefilterdePanden(Energielabel? energielabel, string? makelaarId, bool enkelTeKoop)
    {
        // Eerst alle makelaars ophalen, zodat we elk pand meteen aan het juiste
        // Makelaar-object kunnen koppelen zonder voor elk pand apart de
        // databank te moeten bevragen.
        List<Makelaar> alleMakelaars = GeefAlleMakelaars();

        List<Pand> resultaat = new List<Pand>();

        using (SqliteConnection connectie = new SqliteConnection(connectionString))
        {
            connectie.Open();

            using (SqliteCommand commando = connectie.CreateCommand())
            {
                string query = "SELECT id, adres, makelaarId, prijs, oppervlakte, energielabel, bouwjaar, "
                    + "type, tuinoppervlakte, heeftLift, foto, isVerkocht FROM panden WHERE 1 = 1";

                if (energielabel.HasValue)
                {
                    query += " AND energielabel = @energielabel";
                }

                if (!string.IsNullOrEmpty(makelaarId))
                {
                    query += " AND makelaarId = @makelaarId";
                }

                if (enkelTeKoop)
                {
                    query += " AND isVerkocht = 0";
                }

                query += " ORDER BY adres;";

                commando.CommandText = query;

                if (energielabel.HasValue)
                {
                    commando.Parameters.AddWithValue("@energielabel", energielabel.Value.ToString());
                }

                if (!string.IsNullOrEmpty(makelaarId))
                {
                    commando.Parameters.AddWithValue("@makelaarId", makelaarId);
                }

                using (SqliteDataReader reader = commando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Pand pand = MaakPandVanRij(reader, alleMakelaars);
                        resultaat.Add(pand);
                    }
                }
            }
        }

        return resultaat;
    }

    /// <summary>
    /// Voegt een nieuw pand (Huis of Appartement) toe aan de databank. Na het
    /// invoegen wordt de Id van het pand-object gevuld met de door SQLite
    /// toegekende sleutel.
    /// </summary>
    public void VoegPandToe(Pand pand)
    {
        using (SqliteConnection connectie = new SqliteConnection(connectionString))
        {
            connectie.Open();

            using (SqliteCommand commando = connectie.CreateCommand())
            {
                commando.CommandText =
                    "INSERT INTO panden (adres, makelaarId, prijs, oppervlakte, energielabel, bouwjaar, type, "
                    + "tuinoppervlakte, heeftLift, foto, isVerkocht, datumVerkocht) "
                    + "VALUES (@adres, @makelaarId, @prijs, @oppervlakte, @energielabel, @bouwjaar, @type, "
                    + "@tuinoppervlakte, @heeftLift, @foto, 0, NULL); "
                    + "SELECT last_insert_rowid();";

                commando.Parameters.AddWithValue("@adres", pand.Adres);
                commando.Parameters.AddWithValue("@makelaarId", pand.Makelaar.Id);
                commando.Parameters.AddWithValue("@prijs", pand.Prijs);
                commando.Parameters.AddWithValue("@oppervlakte", pand.Oppervlakte);
                commando.Parameters.AddWithValue("@energielabel", pand.Energielabel.ToString());
                commando.Parameters.AddWithValue("@bouwjaar", pand.BouwJaar);

                if (string.IsNullOrEmpty(pand.FotoPad))
                {
                    commando.Parameters.AddWithValue("@foto", DBNull.Value);
                }
                else
                {
                    commando.Parameters.AddWithValue("@foto", pand.FotoPad);
                }

                // Type-specifieke kolommen: bij een Huis vullen we de
                // tuinoppervlakte in en laten we heeftLift leeg (NULL); bij een
                // Appartement is het net omgekeerd.
                if (pand is Huis huis)
                {
                    commando.Parameters.AddWithValue("@type", "Huis");
                    commando.Parameters.AddWithValue("@tuinoppervlakte", huis.Tuinoppervlakte);
                    commando.Parameters.AddWithValue("@heeftLift", DBNull.Value);
                }
                else if (pand is Appartement appartement)
                {
                    commando.Parameters.AddWithValue("@type", "Appartement");
                    commando.Parameters.AddWithValue("@tuinoppervlakte", DBNull.Value);
                    commando.Parameters.AddWithValue("@heeftLift", appartement.HeeftLift ? 1 : 0);
                }

                object? nieuweId = commando.ExecuteScalar();
                pand.Id = Convert.ToInt32(nieuweId);
            }
        }
    }

    /// <summary>
    /// Markeert het opgegeven pand als verkocht: zowel op het object zelf
    /// (via Pand.MarkeerAlsVerkocht()) als in de databank, samen met de datum
    /// van vandaag.
    /// </summary>
    public void MarkeerAlsVerkocht(Pand pand)
    {
        pand.MarkeerAlsVerkocht();

        using (SqliteConnection connectie = new SqliteConnection(connectionString))
        {
            connectie.Open();

            using (SqliteCommand commando = connectie.CreateCommand())
            {
                commando.CommandText = "UPDATE panden SET isVerkocht = 1, datumVerkocht = @datum WHERE id = @id;";
                commando.Parameters.AddWithValue("@datum", DateTime.Now.ToString("s"));
                commando.Parameters.AddWithValue("@id", pand.Id);
                commando.ExecuteNonQuery();
            }
        }
    }

    /// <summary>
    /// Bouwt op basis van één rij uit het resultaat van een SELECT op de
    /// panden-tabel het juiste Pand-object (Huis of Appartement) op, inclusief
    /// het gekoppelde Makelaar-object.
    /// </summary>
    private Pand MaakPandVanRij(SqliteDataReader reader, List<Makelaar> alleMakelaars)
    {
        int id = reader.GetInt32(0);
        string adres = reader.GetString(1);
        string makelaarId = reader.GetString(2);
        double prijs = reader.GetDouble(3);
        double oppervlakte = reader.GetDouble(4);
        string energielabelTekst = reader.GetString(5);
        int bouwjaar = reader.GetInt32(6);
        string type = reader.GetString(7);

        double tuinoppervlakte = 0;
        if (!reader.IsDBNull(8))
        {
            tuinoppervlakte = reader.GetDouble(8);
        }

        bool heeftLift = false;
        if (!reader.IsDBNull(9))
        {
            heeftLift = reader.GetInt32(9) != 0;
        }

        string? foto = null;
        if (!reader.IsDBNull(10))
        {
            foto = reader.GetString(10);
        }

        bool isVerkocht = reader.GetInt32(11) != 0;

        Energielabel energielabel = Enum.Parse<Energielabel>(energielabelTekst);
        Makelaar makelaar = ZoekMakelaar(alleMakelaars, makelaarId);

        Pand pand;
        if (type == "Huis")
        {
            pand = new Huis(adres, prijs, oppervlakte, energielabel, makelaar, bouwjaar, tuinoppervlakte, foto, isVerkocht);
        }
        else
        {
            pand = new Appartement(adres, prijs, oppervlakte, energielabel, makelaar, bouwjaar, heeftLift, foto, isVerkocht);
        }

        pand.Id = id;
        return pand;
    }

    /// <summary>
    /// Zoekt de makelaar met het opgegeven Id in de meegegeven lijst (gewone
    /// foreach-lus, geen LINQ). Als er - door inconsistente data - geen
    /// makelaar met dat Id bestaat, wordt een duidelijke "onbekende makelaar"
    /// teruggegeven zodat de UI hier nooit op crasht.
    /// </summary>
    private Makelaar ZoekMakelaar(List<Makelaar> alleMakelaars, string makelaarId)
    {
        foreach (Makelaar makelaar in alleMakelaars)
        {
            if (makelaar.Id == makelaarId)
            {
                return makelaar;
            }
        }

        return new Makelaar(makelaarId, "Onbekende makelaar", "-", "-");
    }
}
