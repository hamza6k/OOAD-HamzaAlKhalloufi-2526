namespace CLHelpdesk;

/// <summary>
/// Beheert het laden en opslaan van tickets via een CSV-bestand.
/// </summary>
public class TicketRepository
{
    /// <summary>Verwacht aantal kolommen per CSV-regel.</summary>
    private const int AantalKolommen = 11;

    /// <summary>
    /// Laadt alle tickets uit een CSV-bestand.
    /// </summary>
    /// <param name="pad">Het volledige pad naar het CSV-bestand.</param>
    /// <returns>Een lijst van Ticket-objecten geladen uit het bestand.</returns>
    /// <exception cref="FileNotFoundException">Gegooid als het bestand niet bestaat.</exception>
    /// <exception cref="IOException">Gegooid bij een leesfout.</exception>
    public static List<Ticket> LaadTickets(string pad)
    {
        List<Ticket> tickets = new List<Ticket>();
        try
        {
            using StreamReader reader = new StreamReader(pad);
            reader.ReadLine(); // sla header over

            string? lijn;
            while ((lijn = reader.ReadLine()) != null)
            {
                string[] delen = lijn.Split(';');

                // Sla ongeldige regels over die niet het juiste aantal kolommen hebben
                if (delen.Length != AantalKolommen)
                    continue;

                int id = int.Parse(delen[0]);
                string titel = delen[1];
                string melderVoornaam = delen[2];
                string melderAchternaam = delen[3];
                string gebruikersnaam = delen[4];

                Enum.TryParse(delen[5], out TicketPrioriteit prioriteit);
                bool isAfgesloten = bool.Parse(delen[6]);
                string type = delen[7];
                string extraInfo = delen[8];

                DateTime datumAangemaakt = DateTime.ParseExact(delen[9], "yyyy-MM-dd HHmm", null);
                // Lege waarde betekent dat het ticket nog niet afgesloten is
                DateTime? datumAfgesloten = string.IsNullOrEmpty(delen[10])
                    ? null
                    : DateTime.ParseExact(delen[10], "yyyy-MM-dd HHmm", null);

                Medewerker melder = new Medewerker
                {
                    Voornaam = melderVoornaam,
                    Achternaam = melderAchternaam
                };

                Ticket ticket;
                if (type == "Hardware")
                {
                    ticket = new HardwareTicket
                    {
                        Id = id,
                        Titel = titel,
                        Melder = melder,
                        Prioriteit = prioriteit,
                        IsAfgesloten = isAfgesloten,
                        DatumAangemaakt = datumAangemaakt,
                        DatumAfgesloten = datumAfgesloten,
                        Toestel = extraInfo
                    };
                }
                else
                {
                    ticket = new SoftwareTicket
                    {
                        Id = id,
                        Titel = titel,
                        Melder = melder,
                        Prioriteit = prioriteit,
                        IsAfgesloten = isAfgesloten,
                        DatumAangemaakt = datumAangemaakt,
                        DatumAfgesloten = datumAfgesloten,
                        Applicatie = extraInfo
                    };
                }

                tickets.Add(ticket);
            }
        }
        catch (FileNotFoundException)
        {
            throw;
        }
        catch (IOException)
        {
            throw;
        }
        catch (Exception)
        {
            throw;
        }

        return tickets;
    }

    /// <summary>
    /// Herschrijft het volledige CSV-bestand met alle opgegeven tickets.
    /// Gebruikt bij het bijwerken van bestaande tickets (bv. afsluiten).
    /// </summary>
    /// <param name="pad">Het volledige pad naar het CSV-bestand.</param>
    /// <param name="tickets">De volledige lijst van tickets om op te slaan.</param>
    /// <exception cref="IOException">Gegooid bij een schrijffout.</exception>
    public static void SlaAlleTicketsOp(string pad, List<Ticket> tickets)
    {
        try
        {
            // append: false overschrijft het bestand volledig
            using StreamWriter writer = new StreamWriter(pad, append: false);
            writer.WriteLine("id;titel;melderVoornaam;melderAchternaam;gebruikersnaam;prioriteit;isAfgesloten;type;extraInfo;datumAangemaakt;datumAfgesloten");

            foreach (Ticket ticket in tickets)
            {
                writer.WriteLine(BouwCsvRegel(ticket));
            }
        }
        catch (IOException)
        {
            throw;
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Voegt één ticket toe aan het einde van het CSV-bestand.
    /// Gebruikt bij het aanmaken van een nieuw ticket.
    /// </summary>
    /// <param name="pad">Het volledige pad naar het CSV-bestand.</param>
    /// <param name="ticket">Het ticket om toe te voegen.</param>
    /// <exception cref="IOException">Gegooid bij een schrijffout.</exception>
    public static void SlaTicketOp(string pad, Ticket ticket)
    {
        try
        {
            using StreamWriter writer = new StreamWriter(pad, append: true);
            writer.WriteLine(BouwCsvRegel(ticket));
        }
        catch (IOException)
        {
            throw;
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Bouwt een CSV-regel op voor het opgegeven ticket.
    /// </summary>
    /// <param name="ticket">Het ticket waarvoor de regel opgebouwd wordt.</param>
    /// <returns>Een string in het CSV-formaat voor dit ticket.</returns>
    private static string BouwCsvRegel(Ticket ticket)
    {
        string type;
        string extraInfo;
        if (ticket is HardwareTicket ht)
        {
            type = "Hardware";
            extraInfo = ht.Toestel;
        }
        else
        {
            type = "Software";
            extraInfo = ((SoftwareTicket)ticket).Applicatie;
        }

        string datumAfgesloten = ticket.DatumAfgesloten.HasValue
            ? ticket.DatumAfgesloten.Value.ToString("yyyy-MM-dd HHmm")
            : "";

        return $"{ticket.Id};{ticket.Titel};{ticket.Melder.Voornaam};{ticket.Melder.Achternaam};{ticket.Melder.Id};{ticket.Prioriteit};{ticket.IsAfgesloten};{type};{extraInfo};{ticket.DatumAangemaakt:yyyy-MM-dd HHmm};{datumAfgesloten}";
    }
}
