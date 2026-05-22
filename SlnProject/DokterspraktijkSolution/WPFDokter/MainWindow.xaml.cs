using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using DokterspraktijkLib;

namespace WPFDokter
{
    /// <summary>
    /// Hoofdvenster van de doktersapplicatie.
    /// Beheert de navigatie tussen pagina's en de staat van de ingelogde dokter.
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Dokter _ingelogdeDokter;

        /// <summary>
        /// De momenteel ingelogde dokter. Null als niemand is ingelogd.
        /// Statisch zodat alle pagina's de ingelogde dokter kunnen raadplegen.
        /// </summary>
        public static Dokter IngelogdeDokter
        {
            get { return _ingelogdeDokter; }
            set { _ingelogdeDokter = value; }
        }

        /// <summary>
        /// Initialiseert het hoofdvenster en navigeert meteen naar de startpagina.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new StartPage());
        }

        /// <summary>
        /// Toont de naam en profielfoto van de ingelogde dokter rechtsboven
        /// en schakelt de navigatieknoppen in.
        /// Wordt aangeroepen vanuit StartPage na een succesvolle login.
        /// </summary>
        public void ToonIngelogdeDokter()
        {
            TxtDokterNaam.Text = _ingelogdeDokter.Voornaam + " " + _ingelogdeDokter.Achternaam;

            // Profielfoto tonen als er binaire data aanwezig is
            if (_ingelogdeDokter.Profielfotodata != null && _ingelogdeDokter.Profielfotodata.Length > 0)
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = new MemoryStream(_ingelogdeDokter.Profielfotodata);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    ImgProfielfoto.Source = bitmap;
                }
                catch (Exception)
                {
                    ImgProfielfoto.Source = null;
                }
            }

            PanelDokterInfo.Visibility = Visibility.Visible;
            BtnAfspraken.IsEnabled = true;
            BtnPatienten.IsEnabled = true;
            BtnUitloggen.IsEnabled = true;
        }

        /// <summary>
        /// Navigeert naar de startpagina (login).
        /// </summary>
        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new StartPage());
        }

        /// <summary>
        /// Navigeert naar de afsprakenpagina.
        /// Alleen beschikbaar na login.
        /// </summary>
        private void BtnAfspraken_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AfsprakenPage());
        }

        /// <summary>
        /// Navigeert naar de patiëntenpagina.
        /// Alleen beschikbaar na login.
        /// </summary>
        private void BtnPatienten_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new PatientenOverzichtPage());
        }

        /// <summary>
        /// Logt de dokter uit: wist de sessie, verbergt de profielbalk,
        /// schakelt beveiligde knoppen uit en keert terug naar de startpagina.
        /// </summary>
        private void BtnUitloggen_Click(object sender, RoutedEventArgs e)
        {
            IngelogdeDokter = null;

            ImgProfielfoto.Source = null;
            TxtDokterNaam.Text = string.Empty;
            PanelDokterInfo.Visibility = Visibility.Collapsed;

            BtnAfspraken.IsEnabled = false;
            BtnPatienten.IsEnabled = false;
            BtnUitloggen.IsEnabled = false;

            MainFrame.Navigate(new StartPage());
        }
    }
}
