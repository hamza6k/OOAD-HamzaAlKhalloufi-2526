using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace DokterspraktijkLib
{
    /// <summary>
    /// Stelt een afspraak voor tussen een patiënt en een dokter.
    /// Bevat alle CRUD-operaties en opzoekmethodes.
    /// </summary>
    public class Afspraak
    {
        private int _id;
        private int _dokterId;
        private int _patientId;
        private DateTime _moment;
        private string _klacht;

        /// <summary>Unieke identifier van de afspraak.</summary>
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>Id van de dokter gekoppeld aan deze afspraak.</summary>
        public int DokterId
        {
            get { return _dokterId; }
            set { _dokterId = value; }
        }

        /// <summary>Id van de patiënt gekoppeld aan deze afspraak.</summary>
        public int PatientId
        {
            get { return _patientId; }
            set { _patientId = value; }
        }

        /// <summary>Datum en tijdstip van de afspraak.</summary>
        public DateTime Moment
        {
            get { return _moment; }
            set { _moment = value; }
        }

        /// <summary>Beschrijving van de klacht waarvoor de afspraak is gemaakt.</summary>
        public string Klacht
        {
            get { return _klacht; }
            set { _klacht = value; }
        }

        /// <summary>
        /// Haalt alle afspraken op voor een bepaalde dokter op een bepaalde dag,
        /// gesorteerd op tijdstip.
        /// </summary>
        /// <param name="dokterId">Het id van de dokter.</param>
        /// <param name="datum">De dag waarvoor afspraken worden opgezocht (tijdscomponent wordt genegeerd).</param>
        /// <returns>Lijst van <see cref="Afspraak"/>-objecten op die dag voor die dokter.</returns>
        public static List<Afspraak> GetByDokterEnDatum(int dokterId, DateTime datum)
        {
            string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            string query = @"SELECT id, moment, klacht, patient_id, dokter_id
                             FROM Afspraak
                             WHERE dokter_id = @dokterId AND CAST(moment AS DATE) = CAST(@datum AS DATE)
                             ORDER BY moment";

            List<Afspraak> lijst = new List<Afspraak>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@dokterId", dokterId);
                    cmd.Parameters.AddWithValue("@datum", datum.Date);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Afspraak afspraak = new Afspraak();
                            afspraak.Id = (int)reader["id"];
                            afspraak.Moment = (DateTime)reader["moment"];
                            afspraak.Klacht = reader["klacht"].ToString();
                            afspraak.PatientId = (int)reader["patient_id"];
                            afspraak.DokterId = (int)reader["dokter_id"];
                            lijst.Add(afspraak);
                        }
                    }
                }
            }

            return lijst;
        }

        /// <summary>
        /// Haalt alle afspraken op voor een bepaalde patiënt, gesorteerd op tijdstip.
        /// </summary>
        /// <param name="patientId">Het id van de patiënt.</param>
        /// <returns>Lijst van <see cref="Afspraak"/>-objecten voor die patiënt.</returns>
        public static List<Afspraak> GetByPatient(int patientId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            string query = @"SELECT id, moment, klacht, patient_id, dokter_id
                             FROM Afspraak
                             WHERE patient_id = @patientId
                             ORDER BY moment";

            List<Afspraak> lijst = new List<Afspraak>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@patientId", patientId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Afspraak afspraak = new Afspraak();
                            afspraak.Id = (int)reader["id"];
                            afspraak.Moment = (DateTime)reader["moment"];
                            afspraak.Klacht = reader["klacht"].ToString();
                            afspraak.PatientId = (int)reader["patient_id"];
                            afspraak.DokterId = (int)reader["dokter_id"];
                            lijst.Add(afspraak);
                        }
                    }
                }
            }

            return lijst;
        }

        /// <summary>
        /// Voegt de huidige afspraak in de database in en slaat het gegenereerde id op in <see cref="Id"/>.
        /// </summary>
        public void Insert()
        {
            string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            string query = @"INSERT INTO Afspraak (moment, klacht, patient_id, dokter_id)
                             VALUES (@moment, @klacht, @patientId, @dokterId);
                             SELECT CAST(SCOPE_IDENTITY() AS INT)";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@moment", Moment);
                    cmd.Parameters.AddWithValue("@klacht", Klacht);
                    cmd.Parameters.AddWithValue("@patientId", PatientId);
                    cmd.Parameters.AddWithValue("@dokterId", DokterId);

                    Id = (int)cmd.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Verwijdert de huidige afspraak uit de database op basis van <see cref="Id"/>.
        /// </summary>
        public void Delete()
        {
            string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            string query = "DELETE FROM Afspraak WHERE id = @id";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
