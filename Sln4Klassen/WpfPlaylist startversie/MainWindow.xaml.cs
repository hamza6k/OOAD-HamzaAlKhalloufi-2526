using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfPlaylist
{
    public partial class MainWindow : Window
    {
        List<Artist> artiesten;
        List<Song> playlist;
        MediaPlayer mediaPlayer = new();

        public MainWindow()
        {
            InitializeComponent();

            artiesten = Artist.LaadUitBestand("Files/Artists.tsv");
            playlist = Song.LaadUitBestand("Files/Songs.tsv", artiesten);

            // voeg songs toe aan listbox
            for (int i = 0; i < playlist.Count; i++)
            {
                ListBoxItem newItem = new();
                newItem.Content = playlist[i].ToString();
                lbxSongs.Items.Add(newItem);
            }

            mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
        }

        private void MediaPlayer_MediaEnded(object? sender, EventArgs e)
        {
            btnStop.IsEnabled = false;
            btnPlay.IsEnabled = lbxSongs.SelectedItem != null;
            txtMessage.Text = "";
        }

        private void LbxSongs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            imgArtist.Source = null;
            ListBoxItem selectedItem = lbxSongs.SelectedItem as ListBoxItem;
            btnPlay.IsEnabled = selectedItem != null;
            if (selectedItem == null) return;

            Song song = playlist[lbxSongs.SelectedIndex];
            imgArtist.Source = new BitmapImage(new Uri($"Photos/{song.Artiest.Foto}", UriKind.Relative));
            txtArtist.Text = song.Artiest.Naam;
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
            Song song = playlist[lbxSongs.SelectedIndex];
            mediaPlayer.Open(new Uri($"Mp3/{song.Mp3}", UriKind.Relative));
            mediaPlayer.Play();
            txtMessage.Text = $"Now playing: \"{song.Naam}\" by {song.Artiest.Naam}";
            btnPlay.IsEnabled = false;
            btnStop.IsEnabled = true;
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
            txtMessage.Text = "";
            btnStop.IsEnabled = false;
            btnPlay.IsEnabled = lbxSongs.SelectedItem != null;
        }

        private void Slider_ValueChanged(object sender, EventArgs e)
        {
            mediaPlayer.Volume = (double)sldVolume.Value / 100.0;
        }
    }
}