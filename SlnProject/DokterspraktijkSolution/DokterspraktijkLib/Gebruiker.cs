using System.Security.Cryptography;
using System.Text;

namespace DokterspraktijkLib
{
    /// <summary>
    /// Abstracte basisklasse voor alle gebruikers in het systeem (Dokter en Patient).
    /// Bevat gemeenschappelijke eigenschappen en statische wachtwoordhashing via SHA256.
    /// </summary>
    public abstract class Gebruiker
    {
        private int _id;
        private string _voornaam;
        private string _achternaam;
        private string _gsm;
        private string _email;
        private string _paswoord;
        private byte[] _profielfotodata;

        /// <summary>Unieke identifier van de gebruiker.</summary>
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>Voornaam van de gebruiker.</summary>
        public string Voornaam
        {
            get { return _voornaam; }
            set { _voornaam = value; }
        }

        /// <summary>Achternaam van de gebruiker.</summary>
        public string Achternaam
        {
            get { return _achternaam; }
            set { _achternaam = value; }
        }

        /// <summary>GSM-nummer van de gebruiker.</summary>
        public string Gsm
        {
            get { return _gsm; }
            set { _gsm = value; }
        }

        /// <summary>E-mailadres van de gebruiker.</summary>
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        /// <summary>Gehashed wachtwoord (SHA256-hex) van de gebruiker.</summary>
        public string Paswoord
        {
            get { return _paswoord; }
            set { _paswoord = value; }
        }

        /// <summary>Profielfoto als binaire data. Kan null zijn.</summary>
        public byte[] Profielfotodata
        {
            get { return _profielfotodata; }
            set { _profielfotodata = value; }
        }

        /// <summary>
        /// Hashed een plain-text wachtwoord via SHA256 en geeft het terug als lowercase hexadecimale string.
        /// </summary>
        /// <param name="wachtwoord">Het plain-text wachtwoord om te hashen.</param>
        /// <returns>Lowercase hexadecimale SHA256-hash van het wachtwoord.</returns>
        public static string HashWachtwoord(string wachtwoord)
        {
            SHA256 sha = SHA256.Create();
            byte[] invoerBytes = Encoding.UTF8.GetBytes(wachtwoord);
            byte[] hashBytes = sha.ComputeHash(invoerBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Verifieert of een plain-text wachtwoord overeenkomt met een opgeslagen SHA256-hash.
        /// </summary>
        /// <param name="wachtwoord">Het plain-text wachtwoord om te controleren.</param>
        /// <param name="hash">De opgeslagen SHA256-hash als hexadecimale string.</param>
        /// <returns>True als het wachtwoord overeenkomt met de hash, anders false.</returns>
        public static bool VerifieerWachtwoord(string wachtwoord, string hash)
        {
            string berekend = HashWachtwoord(wachtwoord);
            return string.Equals(berekend, hash, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
