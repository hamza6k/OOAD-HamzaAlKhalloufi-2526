using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using DokterspraktijkLib;

namespace WPFPatient
{
    /// <summary>
    /// Hoofdvenster van de patiëntenapplicatie.
    /// Beheert de navigatie tussen pagina's en de staat van de ingelogde patiënt.
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Patient _ingelogdePatient;

        /// <summary>
        /// De momenteel ingelogde patiënt. Null als niemand is ingelogd.
        /// Statisch zodat alle pagina's de sessie kunnen raadplegen.
        /// </summary>
        public static Patient IngelogdePatient
        {
            get { return _ingelogdePatient; }
            set { _ingelogdePatient = value; }
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
        /// Toont de naam en profielfoto van de ingelogde patiënt rechtsboven
        /// en schakelt de navigatieknoppen in.
        /// Wordt aangeroepen vanuit LoginPage na een succesvolle login.
        /// </summary>
        public void ToonIngelogdePatient()
        {
            TxtPatientNaam.Text = _ingelogdePatient.Voornaam + " " + _ingelogdePatient.Achternaam;

            // Profielfoto tonen als er binaire data aanwezig is
            if (_ingelogdePatient.Profielfotodata != null && _ingelogdePatient.Profielfotodata.Length > 0)
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = new MemoryStream(_ingelogdePatient.Profielfotodata);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    ImgProfielfoto.Source = bitmap;
                }
                catch (Exception)
                {
                    ImgProfielfoto.Source = null;
                }
            }

            PanelPatientInfo.Visibility = Visibility.Visible;
            BtnAfspraken.IsEnabled = true;
            BtnProfiel.IsEnabled = true;
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
        /// Navigeert naar de afsprakenoverzichtpagina.
        /// Alleen beschikbaar na login.
        /// </summary>
        private void BtnAfspraken_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AfsprakenOverzichtPage());
        }

        /// <summary>
        /// Navigeert naar de profielpagina van de ingelogde patiënt.
        /// Alleen beschikbaar na login.
        /// </summary>
        private void BtnProfiel_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProfielInfoPage());
        }

        /// <summary>
        /// Logt de patiënt uit: wist de sessie, verbergt de profielbalk,
        /// schakelt beveiligde knoppen uit en keert terug naar de startpagina.
        /// </summary>
        private void BtnUitloggen_Click(object sender, RoutedEventArgs e)
        {
            IngelogdePatient = null;

            ImgProfielfoto.Source = null;
            TxtPatientNaam.Text = string.Empty;
            PanelPatientInfo.Visibility = Visibility.Collapsed;

            BtnAfspraken.IsEnabled = false;
            BtnProfiel.IsEnabled = false;
            BtnUitloggen.IsEnabled = false;

            MainFrame.Navigate(new StartPage());
        }
    }
}
