using System.IO;
namespace WpfPlaylist
{
    public class Artist
    {
        public string Naam { get; set; }
        public string Geboortedatum { get; set; }
        public string Foto { get; set; }
        public string Bio { get; set; }

        public Artist(string naam, string geboortedatum, string foto, string bio)
        {
            Naam = naam;
            Geboortedatum = geboortedatum;
            Foto = foto;
            Bio = bio;
        }

        // Lees alle artiesten uit Artists.tsv
        public static List<Artist> LaadUitBestand(string pad)
        {
            List<Artist> artiesten = new List<Artist>();
            string[] lijnen = File.ReadAllLines(pad);

            for (int i = 1; i < lijnen.Length; i++) // i=1 om de header te overslaan
            {
                string[] kolommen = lijnen[i].Split('\t');
                artiesten.Add(new Artist(kolommen[0], kolommen[1], kolommen[2], kolommen[3]));
            }
            return artiesten;
        }
    }
}