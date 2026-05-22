using Microsoft.Data.SqlClient;

namespace DokterspraktijkLib
{
    /// <summary>
    /// Modelklasse voor een dokter. Bevat alle CRUD-operaties voor de Dokters-tabel.
    /// </summary>
    public class Dokter
    {
        public int DokterId { get; set; }
        public string Naam { get; set; }
        public string Voornaam { get; set; }
        public string Email { get; set; }
        public string Wachtwoord { get; set; }
        public string Specialisatie { get; set; }
        public string Telefoon { get; set; }

        public Dokter() { Naam = ""; Voornaam = ""; Email = ""; Wachtwoord = ""; Specialisatie = ""; Telefoon = ""; }

        public Dokter(int dokterId, string naam, string voornaam, string email,
                      string wachtwoord, string specialisatie, string telefoon)
        {
            DokterId = dokterId;
            Naam = naam;
            Voornaam = voornaam;
            Email = email;
            Wachtwoord = wachtwoord;
            Specialisatie = specialisatie;
            Telefoon = telefoon;
        }

        /// <summary>
        /// Voegt deze dokter toe aan de databank. Het wachtwoord wordt gehasht voor opslag.
        /// Geeft de foutmelding terug als string, of null bij succes.
        /// </summary>
        public string? Toevoegen()
        {
            try
            {
                using (SqlConnection conn = Database.GetConnection())
                {
                    string query = "INSERT INTO Dokters (Naam, Voornaam, Email, Wachtwoord, Specialisatie, Telefoon) " +
                                   "VALUES (@Naam, @Voornaam, @Email, @Wachtwoord, @Specialisatie, @Telefoon)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Naam", Naam);
                    cmd.Parameters.AddWithValue("@Voornaam", Voornaam);
                    cmd.Parameters.AddWithValue("@Email", Email);
                    cmd.Parameters.AddWithValue("@Wachtwoord", WachtwoordHelper.Hash(Wachtwoord));
                    cmd.Parameters.AddWithValue("@Specialisatie", Specialisatie);
                    cmd.Parameters.AddWithValue("@Telefoon", Telefoon);
                    cmd.ExecuteNonQuery();
                }
                return null;
            }
            catch (SqlException ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Werkt de gegevens van deze dokter bij in de databank op basis van DokterId.
        /// Geeft de foutmelding terug als string, of null bij succes.
        /// </summary>
        public string? Bijwerken()
        {
            try
            {
                using (SqlConnection conn = Database.GetConnection())
                {
                    string query = "UPDATE Dokters SET Naam=@Naam, Voornaam=@Voornaam, Email=@Email, " +
                                   "Specialisatie=@Specialisatie, Telefoon=@Telefoon WHERE DokterId=@DokterId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Naam", Naam);
                    cmd.Parameters.AddWithValue("@Voornaam", Voornaam);
                    cmd.Parameters.AddWithValue("@Email", Email);
                    cmd.Parameters.AddWithValue("@Specialisatie", Specialisatie);
                    cmd.Parameters.AddWithValue("@Telefoon", Telefoon);
                    cmd.Parameters.AddWithValue("@DokterId", DokterId);
                    cmd.ExecuteNonQuery();
                }
                return null;
            }
            catch (SqlException ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Werkt het wachtwoord van deze dokter bij. Het nieuwe wachtwoord wordt gehasht voor opslag.
        /// Geeft de foutmelding terug als string, of null bij succes.
        /// </summary>
        public string? WachtwoordBijwerken(string nieuwWachtwoord)
        {
            try
            {
                using (SqlConnection conn = Database.GetConnection())
                {
                    string query = "UPDATE Dokters SET Wachtwoord=@Wachtwoord WHERE DokterId=@DokterId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Wachtwoord", WachtwoordHelper.Hash(nieuwWachtwoord));
                    cmd.Parameters.AddWithValue("@DokterId", DokterId);
                    cmd.ExecuteNonQuery();
                }
                return null;
            }
            catch (SqlException ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Verwijdert deze dokter uit de databank op basis van DokterId.
        /// Geeft de foutmelding terug als string, of null bij succes.
        /// </summary>
        public string? Verwijderen()
        {
            try
            {
                using (SqlConnection conn = Database.GetConnection())
                {
                    string query = "DELETE FROM Dokters WHERE DokterId=@DokterId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@DokterId", DokterId);
                    cmd.ExecuteNonQuery();
                }
                return null;
            }
            catch (SqlException ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Haalt alle dokters op uit de databank.
        /// Geeft een lege lijst terug bij een fout.
        /// </summary>
        public static List<Dokter> GetAlles()
        {
            List<Dokter> dokters = new List<Dokter>();
            try
            {
                using (SqlConnection conn = Database.GetConnection())
                {
                    string query = "SELECT DokterId, Naam, Voornaam, Email, Wachtwoord, Specialisatie, Telefoon FROM Dokters";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Dokter dokter = new Dokter(
                            (int)reader["DokterId"],
                            (string)reader["Naam"],
                            (string)reader["Voornaam"],
                            (string)reader["Email"],
                            (string)reader["Wachtwoord"],
                            (string)reader["Specialisatie"],
                            (string)reader["Telefoon"]
                        );
                        dokters.Add(dokter);
                    }
                }
            }
            catch (SqlException)
            {
                // Geef lege lijst terug; foutafhandeling gebeurt in de UI via TextBlock
            }
            return dokters;
        }

        /// <summary>
        /// Haalt één dokter op uit de databank op basis van DokterId.
        /// Geeft null terug als de dokter niet gevonden werd.
        /// </summary>
        public static Dokter? GetOpId(int dokterId)
        {
            try
            {
                using (SqlConnection conn = Database.GetConnection())
                {
                    string query = "SELECT DokterId, Naam, Voornaam, Email, Wachtwoord, Specialisatie, Telefoon " +
                                   "FROM Dokters WHERE DokterId=@DokterId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@DokterId", dokterId);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        return new Dokter(
                            (int)reader["DokterId"],
                            (string)reader["Naam"],
                            (string)reader["Voornaam"],
                            (string)reader["Email"],
                            (string)reader["Wachtwoord"],
                            (string)reader["Specialisatie"],
                            (string)reader["Telefoon"]
                        );
                    }
                }
            }
            catch (SqlException)
            {
                // Geef null terug; foutafhandeling gebeurt in de UI via TextBlock
            }
            return null;
        }

        /// <summary>
        /// Controleert de inloggegevens van een dokter.
        /// Geeft het Dokter-object terug bij succes, null bij fout of ongeldig wachtwoord.
        /// </summary>
        public static Dokter? Inloggen(string email, string wachtwoord)
        {
            try
            {
                using (SqlConnection conn = Database.GetConnection())
                {
                    string query = "SELECT DokterId, Naam, Voornaam, Email, Wachtwoord, Specialisatie, Telefoon " +
                                   "FROM Dokters WHERE Email=@Email";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Email", email);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        string opgeslagenHash = (string)reader["Wachtwoord"];
                        if (WachtwoordHelper.Verificeer(wachtwoord, opgeslagenHash))
                        {
                            return new Dokter(
                                (int)reader["DokterId"],
                                (string)reader["Naam"],
                                (string)reader["Voornaam"],
                                (string)reader["Email"],
                                opgeslagenHash,
                                (string)reader["Specialisatie"],
                                (string)reader["Telefoon"]
                            );
                        }
                    }
                }
            }
            catch (SqlException)
            {
                // Geef null terug; foutafhandeling gebeurt in de UI via TextBlock
            }
            return null;
        }
    }
}



