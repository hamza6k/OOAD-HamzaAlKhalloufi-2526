using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace DokterspraktijkLib
{
    /// <summary>
    /// Stelt een patiënt voor in het systeem. Erft van <see cref="Gebruiker"/>.
    /// Bevat patiënt-specifieke gegevens en alle CRUD-operaties.
    /// </summary>
    public class Patient : Gebruiker
    {
        private string _geslacht;
        private DateTime _geboortedatum;
        private Notificaties _notificaties;

        /// <summary>Geslacht van de patiënt.</summary>
        public string Geslacht
        {
            get { return _geslacht; }
            set { _geslacht = value; }
        }

        /// <summary>Geboortedatum van de patiënt.</summary>
        public DateTime Geboortedatum
        {
            get { return _geboortedatum; }
            set { _geboortedatum = value; }
        }

        /// <summary>Notificatie-voorkeur van de patiënt.</summary>
        public Notificaties Notificaties
        {
            get { return _notificaties; }
            set { _notificaties = value; }
        }

        /// <summary>
        /// Probeert een patiënt aan te melden via e-mail en wachtwoord.
        /// Het opgegeven wachtwoord wordt via SHA256 vergeleken met de opgeslagen hash.
        /// </summary>
        /// <param name="email">E-mailadres van de patiënt.</param>
        /// <param name="wachtwoord">Plain-text wachtwoord.</param>
        /// <returns>Een ingevuld <see cref="Patient"/>-object bij succesvolle login, anders null.</returns>
        public static Patient Login(string email, string wachtwoord)
        {
            string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            string query = @"SELECT id, voornaam, achternaam, geslacht, gsm, email, paswoord,
                                    geboortedatum, profielfotodata, notificaties
                             FROM Patient
                             WHERE email = @email";

            Patient resultaat = null;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@email", email);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string opgeslagenHash = reader["paswoord"].ToString();

                            // Wachtwoord controleren voor de rij wordt uitgelezen
                            if (VerifieerWachtwoord(wachtwoord, opgeslagenHash))
                            {
                                resultaat = new Patient();
                                resultaat.Id = (int)reader["id"];
                                resultaat.Voornaam = reader["voornaam"].ToString();
                                resultaat.Achternaam = reader["achternaam"].ToString();
                                resultaat.Geslacht = reader["geslacht"].ToString();
                                resultaat.Gsm = reader["gsm"].ToString();
                                resultaat.Email = reader["email"].ToString();
                                resultaat.Paswoord = opgeslagenHash;
                                resultaat.Geboortedatum = (DateTime)reader["geboortedatum"];
                                resultaat.Profielfotodata = reader["profielfotodata"] as byte[];
                                resultaat.Notificaties = (Notificaties)Convert.ToInt32(reader["notificaties"]);
                            }
                        }
                    }
                }
            }

            return resultaat;
        }

        /// <summary>
        /// Haalt alle patiënten op uit de database, gesorteerd op achternaam.
        /// </summary>
        /// <returns>Lijst van alle <see cref="Patient"/>-objecten.</returns>
        public static List<Patient> GetAll()
        {
            string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            string query = @"SELECT id, voornaam, achternaam, geslacht, gsm, email, paswoord,
                                    geboortedatum, profielfotodata, notificaties
                             FROM Patient
                             ORDER BY achternaam";

            List<Patient> lijst = new List<Patient>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Patient patient = new Patient();
                            patient.Id = (int)reader["id"];
                            patient.Voornaam = reader["voornaam"].ToString();
                            patient.Achternaam = reader["achternaam"].ToString();
                            patient.Geslacht = reader["geslacht"].ToString();
                            patient.Gsm = reader["gsm"].ToString();
                            patient.Email = reader["email"].ToString();
                            patient.Paswoord = reader["paswoord"].ToString();
                            patient.Geboortedatum = (DateTime)reader["geboortedatum"];
                            patient.Profielfotodata = reader["profielfotodata"] as byte[];
                            patient.Notificaties = (Notificaties)Convert.ToInt32(reader["notificaties"]);
                            lijst.Add(patient);
                        }
                    }
                }
            }

            return lijst;
        }

        /// <summary>
        /// Haalt één patiënt op via zijn primaire sleutel.
        /// </summary>
        /// <param name="id">Het id van de op te halen patiënt.</param>
        /// <returns><see cref="Patient"/>-object als gevonden, anders null.</returns>
        public static Patient GetById(int id)
        {
            string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            string query = @"SELECT id, voornaam, achternaam, geslacht, gsm, email, paswoord,
                                    geboortedatum, profielfotodata, notificaties
                             FROM Patient
                             WHERE id = @id";

            Patient resultaat = null;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            resultaat = new Patient();
                            resultaat.Id = (int)reader["id"];
                            resultaat.Voornaam = reader["voornaam"].ToString();
                            resultaat.Achternaam = reader["achternaam"].ToString();
                            resultaat.Geslacht = reader["geslacht"].ToString();
                            resultaat.Gsm = reader["gsm"].ToString();
                            resultaat.Email = reader["email"].ToString();
                            resultaat.Paswoord = reader["paswoord"].ToString();
                            resultaat.Geboortedatum = (DateTime)reader["geboortedatum"];
                            resultaat.Profielfotodata = reader["profielfotodata"] as byte[];
                            resultaat.Notificaties = (Notificaties)Convert.ToInt32(reader["notificaties"]);
                        }
                    }
                }
            }

            return resultaat;
        }

        /// <summary>
        /// Zoekt patiënten op voornaam of achternaam via een gedeeltelijke overeenkomst (LIKE).
        /// </summary>
        /// <param name="zoekterm">De tekst om op te zoeken (hoofdletterongevoelig).</param>
        /// <returns>Lijst van overeenkomende <see cref="Patient"/>-objecten, gesorteerd op achternaam.</returns>
        public static List<Patient> Zoek(string zoekterm)
        {
            string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            string query = @"SELECT id, voornaam, achternaam, geslacht, gsm, email, paswoord,
                                    geboortedatum, profielfotodata, notificaties
                             FROM Patient
                             WHERE voornaam LIKE @zoekterm OR achternaam LIKE @zoekterm
                             ORDER BY achternaam";

            List<Patient> lijst = new List<Patient>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Wildcards toevoegen voor gedeeltelijke overeenkomst
                    cmd.Parameters.AddWithValue("@zoekterm", "%" + zoekterm + "%");

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Patient patient = new Patient();
                            patient.Id = (int)reader["id"];
                            patient.Voornaam = reader["voornaam"].ToString();
                            patient.Achternaam = reader["achternaam"].ToString();
                            patient.Geslacht = reader["geslacht"].ToString();
                            patient.Gsm = reader["gsm"].ToString();
                            patient.Email = reader["email"].ToString();
                            patient.Paswoord = reader["paswoord"].ToString();
                            patient.Geboortedatum = (DateTime)reader["geboortedatum"];
                            patient.Profielfotodata = reader["profielfotodata"] as byte[];
                            patient.Notificaties = (Notificaties)Convert.ToInt32(reader["notificaties"]);
                            lijst.Add(patient);
                        }
                    }
                }
            }

            return lijst;
        }

        /// <summary>
        /// Voegt de huidige patiënt in de database in en slaat het gegenereerde id op in <see cref="Gebruiker.Id"/>.
        /// Het wachtwoord moet vooraf gehashed zijn via <see cref="Gebruiker.HashWachtwoord"/>.
        /// </summary>
        public void Insert()
        {
            string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            string query = @"INSERT INTO Patient (voornaam, achternaam, geslacht, gsm, email, paswoord,
                                                  geboortedatum, profielfotodata, notificaties)
                             VALUES (@voornaam, @achternaam, @geslacht, @gsm, @email, @paswoord,
                                     @geboortedatum, @profielfotodata, @notificaties);
                             SELECT CAST(SCOPE_IDENTITY() AS INT)";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@voornaam", Voornaam);
                    cmd.Parameters.AddWithValue("@achternaam", Achternaam);
                    cmd.Parameters.AddWithValue("@geslacht", Geslacht);
                    cmd.Parameters.AddWithValue("@gsm", Gsm);
                    cmd.Parameters.AddWithValue("@email", Email);
                    cmd.Parameters.AddWithValue("@paswoord", Paswoord);
                    cmd.Parameters.AddWithValue("@geboortedatum", Geboortedatum);

                    // Gebruik DBNull als er geen profielfoto is
                    if (Profielfotodata != null)
                        cmd.Parameters.AddWithValue("@profielfotodata", Profielfotodata);
                    else
                        cmd.Parameters.AddWithValue("@profielfotodata", DBNull.Value);

                    cmd.Parameters.AddWithValue("@notificaties", (int)Notificaties);

                    Id = (int)cmd.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Werkt de gegevens van de huidige patiënt bij in de database op basis van <see cref="Gebruiker.Id"/>.
        /// </summary>
        public void Update()
        {
            string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            string query = @"UPDATE Patient
                             SET voornaam = @voornaam, achternaam = @achternaam, geslacht = @geslacht,
                                 gsm = @gsm, email = @email, paswoord = @paswoord,
                                 geboortedatum = @geboortedatum, profielfotodata = @profielfotodata,
                                 notificaties = @notificaties
                             WHERE id = @id";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@voornaam", Voornaam);
                    cmd.Parameters.AddWithValue("@achternaam", Achternaam);
                    cmd.Parameters.AddWithValue("@geslacht", Geslacht);
                    cmd.Parameters.AddWithValue("@gsm", Gsm);
                    cmd.Parameters.AddWithValue("@email", Email);
                    cmd.Parameters.AddWithValue("@paswoord", Paswoord);
                    cmd.Parameters.AddWithValue("@geboortedatum", Geboortedatum);

                    // Gebruik DBNull als er geen profielfoto is
                    if (Profielfotodata != null)
                        cmd.Parameters.AddWithValue("@profielfotodata", Profielfotodata);
                    else
                        cmd.Parameters.AddWithValue("@profielfotodata", DBNull.Value);

                    cmd.Parameters.AddWithValue("@notificaties", (int)Notificaties);
                    cmd.Parameters.AddWithValue("@id", Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Verwijdert de huidige patiënt en alle gekoppelde afspraken uit de database.
        /// Beide DELETE-statements worden uitgevoerd binnen één transactie.
        /// </summary>
        public void Delete()
        {
            string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();

                // Eerst gekoppelde afspraken verwijderen (foreign key constraint)
                string deleteAfspraken = "DELETE FROM Afspraak WHERE patient_id = @id";
                using (SqlCommand cmdAfspraken = new SqlCommand(deleteAfspraken, conn, trans))
                {
                    cmdAfspraken.Parameters.AddWithValue("@id", Id);
                    cmdAfspraken.ExecuteNonQuery();
                }

                // Dan de patiënt zelf verwijderen
                string deletePatient = "DELETE FROM Patient WHERE id = @id";
                using (SqlCommand cmdPatient = new SqlCommand(deletePatient, conn, trans))
                {
                    cmdPatient.Parameters.AddWithValue("@id", Id);
                    cmdPatient.ExecuteNonQuery();
                }

                trans.Commit();
            }
        }
    }
}
