using Microsoft.Data.SqlClient;

namespace DokterspraktijkLib
{
    /// <summary>
    /// Modelklasse voor een patiënt. Bevat alle CRUD-operaties voor de Patients-tabel.
    /// </summary>
    public class Patient
    {
        public int PatientId { get; set; }
        public string Naam { get; set; }
        public string Voornaam { get; set; }
        public DateTime GeboorteDatum { get; set; }
        public string Email { get; set; }
        public string Wachtwoord { get; set; }
        public string Telefoon { get; set; }
        public string Adres { get; set; }

        public Patient() { Naam = ""; Voornaam = ""; Email = ""; Wachtwoord = ""; Telefoon = ""; Adres = ""; }

        public Patient(int patientId, string naam, string voornaam, DateTime geboorteDatum,
                       string email, string wachtwoord, string telefoon, string adres)
        {
            PatientId = patientId;
            Naam = naam;
            Voornaam = voornaam;
            GeboorteDatum = geboorteDatum;
            Email = email;
            Wachtwoord = wachtwoord;
            Telefoon = telefoon;
            Adres = adres;
        }

        /// <summary>
        /// Voegt deze patiënt toe aan de databank. Het wachtwoord wordt gehasht voor opslag.
        /// Geeft de foutmelding terug als string, of null bij succes.
        /// </summary>
        public string? Toevoegen()
        {
            try
            {
                using (SqlConnection conn = Database.GetConnection())
                {
                    string query = "INSERT INTO Patients (Naam, Voornaam, GeboorteDatum, Email, Wachtwoord, Telefoon, Adres) " +
                                   "VALUES (@Naam, @Voornaam, @GeboorteDatum, @Email, @Wachtwoord, @Telefoon, @Adres)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Naam", Naam);
                    cmd.Parameters.AddWithValue("@Voornaam", Voornaam);
                    cmd.Parameters.AddWithValue("@GeboorteDatum", GeboorteDatum);
                    cmd.Parameters.AddWithValue("@Email", Email);
                    cmd.Parameters.AddWithValue("@Wachtwoord", WachtwoordHelper.Hash(Wachtwoord));
                    cmd.Parameters.AddWithValue("@Telefoon", Telefoon);
                    cmd.Parameters.AddWithValue("@Adres", Adres);
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
        /// Werkt de gegevens van deze patiënt bij in de databank op basis van PatientId.
        /// Geeft de foutmelding terug als string, of null bij succes.
        /// </summary>
        public string? Bijwerken()
        {
            try
            {
                using (SqlConnection conn = Database.GetConnection())
                {
                    string query = "UPDATE Patients SET Naam=@Naam, Voornaam=@Voornaam, GeboorteDatum=@GeboorteDatum, " +
                                   "Email=@Email, Telefoon=@Telefoon, Adres=@Adres WHERE PatientId=@PatientId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Naam", Naam);
                    cmd.Parameters.AddWithValue("@Voornaam", Voornaam);
                    cmd.Parameters.AddWithValue("@GeboorteDatum", GeboorteDatum);
                    cmd.Parameters.AddWithValue("@Email", Email);
                    cmd.Parameters.AddWithValue("@Telefoon", Telefoon);
                    cmd.Parameters.AddWithValue("@Adres", Adres);
                    cmd.Parameters.AddWithValue("@PatientId", PatientId);
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
        /// Werkt het wachtwoord van deze patiënt bij. Het nieuwe wachtwoord wordt gehasht voor opslag.
        /// Geeft de foutmelding terug als string, of null bij succes.
        /// </summary>
        public string? WachtwoordBijwerken(string nieuwWachtwoord)
        {
            try
            {
                using (SqlConnection conn = Database.GetConnection())
                {
                    string query = "UPDATE Patients SET Wachtwoord=@Wachtwoord WHERE PatientId=@PatientId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Wachtwoord", WachtwoordHelper.Hash(nieuwWachtwoord));
                    cmd.Parameters.AddWithValue("@PatientId", PatientId);
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
        /// Verwijdert deze patiënt uit de databank op basis van PatientId.
        /// Geeft de foutmelding terug als string, of null bij succes.
        /// </summary>
        public string? Verwijderen()
        {
            try
            {
                using (SqlConnection conn = Database.GetConnection())
                {
                    string query = "DELETE FROM Patients WHERE PatientId=@PatientId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PatientId", PatientId);
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
        /// Haalt alle patiënten op uit de databank.
        /// Geeft een lege lijst terug bij een fout.
        /// </summary>
        public static List<Patient> GetAlles()
        {
            List<Patient> patienten = new List<Patient>();
            try
            {
                using (SqlConnection conn = Database.GetConnection())
                {
                    string query = "SELECT PatientId, Naam, Voornaam, GeboorteDatum, Email, Wachtwoord, Telefoon, Adres FROM Patients";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Patient patient = new Patient(
                            (int)reader["PatientId"],
                            (string)reader["Naam"],
                            (string)reader["Voornaam"],
                            (DateTime)reader["GeboorteDatum"],
                            (string)reader["Email"],
                            (string)reader["Wachtwoord"],
                            (string)reader["Telefoon"],
                            (string)reader["Adres"]
                        );
                        patienten.Add(patient);
                    }
                }
            }
            catch (SqlException)
            {
                // Geef lege lijst terug; foutafhandeling gebeurt in de UI via TextBlock
            }
            return patienten;
        }

        /// <summary>
        /// Haalt één patiënt op uit de databank op basis van PatientId.
        /// Geeft null terug als de patiënt niet gevonden werd.
        /// </summary>
        public static Patient? GetOpId(int patientId)
        {
            try
            {
                using (SqlConnection conn = Database.GetConnection())
                {
                    string query = "SELECT PatientId, Naam, Voornaam, GeboorteDatum, Email, Wachtwoord, Telefoon, Adres " +
                                   "FROM Patients WHERE PatientId=@PatientId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PatientId", patientId);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        return new Patient(
                            (int)reader["PatientId"],
                            (string)reader["Naam"],
                            (string)reader["Voornaam"],
                            (DateTime)reader["GeboorteDatum"],
                            (string)reader["Email"],
                            (string)reader["Wachtwoord"],
                            (string)reader["Telefoon"],
                            (string)reader["Adres"]
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
        /// Controleert de inloggegevens van een patiënt.
        /// Geeft het Patient-object terug bij succes, null bij fout of ongeldig wachtwoord.
        /// </summary>
        public static Patient? Inloggen(string email, string wachtwoord)
        {
            try
            {
                using (SqlConnection conn = Database.GetConnection())
                {
                    string query = "SELECT PatientId, Naam, Voornaam, GeboorteDatum, Email, Wachtwoord, Telefoon, Adres " +
                                   "FROM Patients WHERE Email=@Email";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Email", email);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        string opgeslagenHash = (string)reader["Wachtwoord"];
                        if (WachtwoordHelper.Verificeer(wachtwoord, opgeslagenHash))
                        {
                            return new Patient(
                                (int)reader["PatientId"],
                                (string)reader["Naam"],
                                (string)reader["Voornaam"],
                                (DateTime)reader["GeboorteDatum"],
                                (string)reader["Email"],
                                opgeslagenHash,
                                (string)reader["Telefoon"],
                                (string)reader["Adres"]
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



