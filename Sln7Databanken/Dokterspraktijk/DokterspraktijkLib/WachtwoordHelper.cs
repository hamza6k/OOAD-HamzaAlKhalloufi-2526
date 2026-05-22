using System.Security.Cryptography;
using System.Text;

namespace DokterspraktijkLib
{
    /// <summary>
    /// Hulpklasse voor het hashen van wachtwoorden met SHA256.
    /// </summary>
    public static class WachtwoordHelper
    {
        /// <summary>
        /// Zet een plaintext wachtwoord om naar een SHA256-hash (hexadecimale string).
        /// </summary>
        public static string Hash(string wachtwoord)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(wachtwoord));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }

        /// <summary>
        /// Vergelijkt een plaintext wachtwoord met een opgeslagen SHA256-hash.
        /// </summary>
        public static bool Verificeer(string wachtwoord, string opgeslagenHash)
        {
            string hash = Hash(wachtwoord);
            return hash == opgeslagenHash;
        }
    }
}
