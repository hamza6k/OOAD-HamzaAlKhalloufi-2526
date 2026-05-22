using Microsoft.Data.SqlClient;

namespace DokterspraktijkLib
{
    /// <summary>
    /// Centrale klasse voor het beheren van de databaseverbinding.
    /// Alle SQL-queries in de applicatie maken gebruik van deze klasse.
    /// </summary>
    public class Database
    {
        private static string _connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=DokterspraktijkDB;Integrated Security=True;TrustServerCertificate=True;";

        /// <summary>
        /// Geeft een open SqlConnection terug naar DokterspraktijkDB.
        /// De aanroeper is verantwoordelijk voor het sluiten van de verbinding (using-blok).
        /// </summary>
        public static SqlConnection GetConnection()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection;
        }

        /// <summary>
        /// Test of een verbinding met de databank mogelijk is.
        /// Geeft true terug bij succes, false bij een fout.
        /// </summary>
        public static bool TestConnection()
        {
            try
            {
                using (SqlConnection connection = GetConnection())
                {
                    return true;
                }
            }
            catch (SqlException)
            {
                return false;
            }
        }
    }
}
