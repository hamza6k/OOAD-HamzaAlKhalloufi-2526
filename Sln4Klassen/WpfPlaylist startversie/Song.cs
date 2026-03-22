using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfPlaylist
{
    public class Song
    {
        public string Naam { get; set; }
        public Artist Artiest { get; set; }  // compositie!
        public string Jaar { get; set; }
        public string Duur { get; set; }
        public string Mp3 { get; set; }

        public Song(string naam, Artist artiest, string jaar, string duur, string mp3)
        {
            Naam = naam;
            Artiest = artiest;
            Jaar = jaar;
            Duur = duur;
            Mp3 = mp3;
        }

        // Lees alle songs uit Songs.tsv
        public static List<Song> LaadUitBestand(string pad, List<Artist> artiesten)
        {
            List<Song> songs = new List<Song>();
            string[] lijnen = File.ReadAllLines(pad);

            for (int i = 1; i < lijnen.Length; i++) // i=1 om de header te overslaan
            {
                string[] kolommen = lijnen[i].Split('\t');
                string artiestSleutel = kolommen[1]; // bv. "anouar"

                // zoek de bijhorende artiest
                Artist gevondenArtiest = null;
                for (int j = 0; j < artiesten.Count; j++)
                {
                    if (artiesten[j].Naam.ToLower().StartsWith(artiestSleutel))
                    {
                        gevondenArtiest = artiesten[j];
                        break;
                    }
                }

                songs.Add(new Song(kolommen[0], gevondenArtiest, kolommen[2], kolommen[3], kolommen[4]));
            }
            return songs;
        }

        public override string ToString()
        {
            return $"{Naam} - {Artiest.Naam} ({Jaar}, {Duur})";
        }
    }
}
