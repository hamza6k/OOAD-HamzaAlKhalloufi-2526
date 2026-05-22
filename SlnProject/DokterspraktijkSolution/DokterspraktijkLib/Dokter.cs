using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace DokterspraktijkLib
{
    /// <summary>
    /// Stelt een dokter voor in het systeem. Erft van <see cref="Gebruiker"/>.
    /// Bevat dokter-specifieke gegevens en statische opzoekmethodes.
    /// </summary>
    public class Dokter : Gebruiker
    {
        private string _rizivnummer;
        private bool _isGeconventioneerd;

        /// <summary>RIZIV-nummer van de dokter.</summary>
        public string Rizivnummer
        {
            get { return _rizivnummer; }
            set { _rizivnummer = value; }
        }

        /// <summary>Geeft aan of de dokter geconventioneerd is.</summary>
        public bool IsGeconventioneerd
        {
            get { return _isGeconventioneerd; }
            set { _isGeconventioneerd = value; }
        }

        /// <summary>
        /// Probeert een dokter aan te melden via e-mail en wachtwoord.
        /// Het opgegeven wachtwoord wordt via SHA256 vergeleken met de opgeslagen hash.
        /// </summary>
        /// <param name="email">E-mailadres van de dokter.</param>
        /// <param name="wachtwoord">Plain-text wachtwoord.</param>
        /// <returns>Een ingevuld <see cref="Dokter"/>-object bij succesvolle login, anders null.</returns>
        public static Dokter Login(string email, string wachtwoord)
        {
            string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            string query = @"SELECT id, voornaam, achternaam, gsm, email, paswoord, profielfotodata,
                                    rizivnummer, isgeconventioneerd
                             FROM Dokter
                             WHERE email = @email";

            Dokter resultaat = null;

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
                                resultaat = new Dokter();
                                resultaat.Id = (int)reader["id"];
                                resultaat.Voornaam = reader["voornaam"].ToString();
                                resultaat.Achternaam = reader["achternaam"].ToString();
                                resultaat.Gsm = reader["gsm"].ToString();
                                resultaat.Email = reader["email"].ToString();
                                resultaat.Paswoord = opgeslagenHash;
                                resultaat.Profielfotodata = reader["profielfotodata"] as byte[];
                                resultaat.Rizivnummer = reader["rizivnummer"].ToString();
                                resultaat.IsGeconventioneerd = (bool)reader["isgeconventioneerd"];
                            }
                        }
                    }
                }
            }

            return resultaat;
        }

        /// <summary>
        /// Haalt alle dokters op uit de database, gesorteerd op achternaam.
        /// </summary>
        /// <returns>Lijst van alle <see cref="Dokter"/>-objecten.</returns>
        public static List<Dokter> GetAll()
        {
            string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            string query = @"SELECT id, voornaam, achternaam, gsm, email, paswoord, profielfotodata,
                                    rizivnummer, isgeconventioneerd
                             FROM Dokter
                             ORDER BY achternaam";

            List<Dokter> lijst = new List<Dokter>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Dokter dokter = new Dokter();
                            dokter.Id = (int)reader["id"];
                            dokter.Voornaam = reader["voornaam"].ToString();
                            dokter.Achternaam = reader["achternaam"].ToString();
                            dokter.Gsm = reader["gsm"].ToString();
                            dokter.Email = reader["email"].ToString();
                            dokter.Paswoord = reader["paswoord"].ToString();
                            dokter.Profielfotodata = reader["profielfotodata"] as byte[];
                            dokter.Rizivnummer = reader["rizivnummer"].ToString();
                            dokter.IsGeconventioneerd = (bool)reader["isgeconventioneerd"];
                            lijst.Add(dokter);
                        }
                    }
                }
            }

            return lijst;
        }

        /// <summary>
        /// Haalt één dokter op via zijn primaire sleutel.
        /// </summary>
        /// <param name="id">Het id van de op te halen dokter.</param>
        /// <returns><see cref="Dokter"/>-object als gevonden, anders null.</returns>
        public static Dokter GetById(int id)
        {
            string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            string query = @"SELECT id, voornaam, achternaam, gsm, email, paswoord, profielfotodata,
                                    rizivnummer, isgeconventioneerd
                             FROM Dokter
                             WHERE id = @id";

            Dokter resultaat = null;

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
                            resultaat = new Dokter();
                            resultaat.Id = (int)reader["id"];
                            resultaat.Voornaam = reader["voornaam"].ToString();
                            resultaat.Achternaam = reader["achternaam"].ToString();
                            resultaat.Gsm = reader["gsm"].ToString();
                            resultaat.Email = reader["email"].ToString();
                            resultaat.Paswoord = reader["paswoord"].ToString();
                            resultaat.Profielfotodata = reader["profielfotodata"] as byte[];
                            resultaat.Rizivnummer = reader["rizivnummer"].ToString();
                            resultaat.IsGeconventioneerd = (bool)reader["isgeconventioneerd"];
                        }
                    }
                }
            }

            return resultaat;
        }
    }
}
