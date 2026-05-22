using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using DokterspraktijkLib;

namespace WPFPatient
{
    /// <summary>
    /// Toont de profielgegevens van de ingelogde patiënt.
    /// Profielfoto, naam, geslacht, geboortedatum, gsm, e-mail en notificaties worden weergegeven.
    /// </summary>
    public partial class ProfielInfoPage : Page
    {
        /// <summary>
        /// Initialiseert de profielpagina.
        /// </summary>
        public ProfielInfoPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Vult alle velden in vanuit de sessie van de ingelogde patiënt bij het laden van de pagina.
        /// </summary>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            VulGegevensIn();
        }

        /// <summary>
        /// Leest de eigenschappen van IngelogdePatient en toont ze in de TextBlocks.
        /// Laadt de profielfoto als BitmapImage via StreamSource.
        /// </summary>
        private void VulGegevensIn()
        {
            Patient patient = MainWindow.IngelogdePatient;

            TxtNaam.Text = patient.Voornaam + " " + patient.Achternaam;
            TxtGeslacht.Text = patient.Geslacht;
            TxtGeboortedatum.Text = patient.Geboortedatum.ToString("dd/MM/yyyy");
            TxtGsm.Text = patient.Gsm;
            TxtEmail.Text = patient.Email;
            TxtNotificaties.Text = patient.Notificaties.ToString();

            // Profielfoto laden als byte[] aanwezig is
            if (patient.Profielfotodata != null && patient.Profielfotodata.Length > 0)
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = new MemoryStream(patient.Profielfotodata);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    ImgProfielfoto.Source = bitmap;
                }
                catch (Exception)
                {
                    ImgProfielfoto.Source = null;
                }
            }
            else
            {
                ImgProfielfoto.Source = null;
            }
        }

        /// <summary>
        /// Navigeert naar de bewerkingspagina van het profiel.
        /// </summary>
        private void BtnBewerken_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProfielBewerkenPage());
        }
    }
}
