using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using DokterspraktijkLib;

namespace WPFDokter
{
    /// <summary>
    /// Detailpagina van een patiënt.
    /// Toont alle opgeslagen gegevens van het geselecteerde dossier.
    /// </summary>
    public partial class PatientDetailsPage : Page
    {
        private Patient _patient;

        /// <summary>
        /// Initialiseert de pagina en vult alle controls met de gegevens van de patiënt.
        /// </summary>
        /// <param name="patient">De patiënt waarvan de details worden getoond.</param>
        public PatientDetailsPage(Patient patient)
        {
            InitializeComponent();
            _patient = patient;
            VulGegevensIn();
        }

        /// <summary>
        /// Vult alle TextBlocks en de Image met de gegevens van <see cref="_patient"/>.
        /// </summary>
        private void VulGegevensIn()
        {
            TxtNaam.Text = _patient.Voornaam + " " + _patient.Achternaam;
            TxtGeslacht.Text = _patient.Geslacht;
            TxtGsm.Text = _patient.Gsm;
            TxtEmail.Text = _patient.Email;
            TxtGeboortedatum.Text = _patient.Geboortedatum.ToString("dd/MM/yyyy");
            TxtNotificaties.Text = _patient.Notificaties.ToString();

            // Profielfoto laden als binaire data aanwezig is
            if (_patient.Profielfotodata != null && _patient.Profielfotodata.Length > 0)
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = new MemoryStream(_patient.Profielfotodata);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    ImgFoto.Source = bitmap;
                }
                catch (Exception)
                {
                    ImgFoto.Source = null;
                }
            }
        }

        /// <summary>
        /// Navigeert terug naar het patiëntenoverzicht.
        /// </summary>
        private void BtnTerug_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PatientenOverzichtPage());
        }
    }
}
