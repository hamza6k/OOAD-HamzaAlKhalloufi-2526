using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using DokterspraktijkLib;

namespace WPFPatient
{
    /// <summary>
    /// Pagina waarmee de ingelogde patiënt zijn/haar profielgegevens kan bewerken.
    /// Alle velden worden vooraf ingevuld vanuit de sessie. Wijzigingen worden opgeslagen via patient.Update().
    /// </summary>
    public partial class ProfielBewerkenPage : Page
    {
        /// <summary>
        /// Initialiseert de bewerkingspagina.
        /// </summary>
        public ProfielBewerkenPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Vult alle formuliervelden in vanuit de sessie van de ingelogde patiënt.
        /// Vult de notificatie-ComboBox programmatisch.
        /// </summary>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Notificatie-opties toevoegen aan de ComboBox
            CmbNotificaties.Items.Add("Geen");
            CmbNotificaties.Items.Add("Mail");
            CmbNotificaties.Items.Add("Sms");
            CmbNotificaties.Items.Add("Beide");

            VulFormulierIn();
        }

        /// <summary>
        /// Leest de eigenschappen van IngelogdePatient en zet ze in de invoervelden.
        /// Toont de profielfoto als BitmapImage indien aanwezig.
        /// </summary>
        private void VulFormulierIn()
        {
            Patient patient = MainWindow.IngelogdePatient;

            TxtVoornaam.Text = patient.Voornaam;
            TxtAchternaam.Text = patient.Achternaam;
            TxtGeslacht.Text = patient.Geslacht;
            TxtGsm.Text = patient.Gsm;
            TxtEmail.Text = patient.Email;
            DpGeboortedatum.SelectedDate = patient.Geboortedatum;

            // Notificatie-enum omzetten naar ComboBox-index (0=Geen, 1=Mail, 2=Sms, 3=Beide)
            CmbNotificaties.SelectedIndex = (int)patient.Notificaties;

            // Profielfoto tonen als er data aanwezig is
            if (patient.Profielfotodata != null && patient.Profielfotodata.Length > 0)
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = new MemoryStream(patient.Profielfotodata);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    ImgFotoVoorbeeld.Source = bitmap;
                }
                catch (Exception)
                {
                    ImgFotoVoorbeeld.Source = null;
                }
            }
        }

        /// <summary>
        /// Opent een bestandsdialoog waarmee de patiënt een profielfoto kan kiezen.
        /// De foto wordt ingelezen als byte[] en getoond als voorvertoning.
        /// </summary>
        private void BtnFotoUploaden_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialoog = new OpenFileDialog();
            dialoog.Filter = "Afbeeldingen|*.jpg;*.jpeg;*.png;*.bmp|Alle bestanden|*.*";
            dialoog.Title = "Profielfoto kiezen";

            if (dialoog.ShowDialog() != true)
                return;

            try
            {
                // Foto inlezen als bytes en opslaan in de sessie
                byte[] fotodata = File.ReadAllBytes(dialoog.FileName);
                MainWindow.IngelogdePatient.Profielfotodata = fotodata;

                // Voorvertoning tonen
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = new MemoryStream(fotodata);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                ImgFotoVoorbeeld.Source = bitmap;
            }
            catch (Exception ex)
            {
                TxtFout.Text = "Fout bij het laden van de foto: " + ex.Message;
                TxtFout.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Valideert alle velden, werkt de patiëntgegevens bij via patient.Update()
        /// en navigeert terug naar ProfielInfoPage.
        /// </summary>
        private void BtnOpslaan_Click(object sender, RoutedEventArgs e)
        {
            // Vorige meldingen wissen
            TxtValidatie.Text = string.Empty;
            TxtFout.Text = string.Empty;
            TxtFout.Visibility = Visibility.Collapsed;

            string voornaam = TxtVoornaam.Text.Trim();
            string achternaam = TxtAchternaam.Text.Trim();
            string geslacht = TxtGeslacht.Text.Trim();
            string gsm = TxtGsm.Text.Trim();
            string email = TxtEmail.Text.Trim();
            string wachtwoord = PbxWachtwoord.Password;

            // Verplichte velden controleren
            if (string.IsNullOrEmpty(voornaam))
            {
                TxtValidatie.Text = "Voornaam is verplicht.";
                return;
            }
            if (string.IsNullOrEmpty(achternaam))
            {
                TxtValidatie.Text = "Achternaam is verplicht.";
                return;
            }
            if (string.IsNullOrEmpty(geslacht))
            {
                TxtValidatie.Text = "Geslacht is verplicht.";
                return;
            }
            if (string.IsNullOrEmpty(gsm))
            {
                TxtValidatie.Text = "GSM is verplicht.";
                return;
            }
            if (string.IsNullOrEmpty(email))
            {
                TxtValidatie.Text = "E-mailadres is verplicht.";
                return;
            }
            if (DpGeboortedatum.SelectedDate == null)
            {
                TxtValidatie.Text = "Geboortedatum is verplicht.";
                return;
            }
            if (CmbNotificaties.SelectedIndex < 0)
            {
                TxtValidatie.Text = "Kies een notificatieoptie.";
                return;
            }

            // Gegevens overzetten naar de sessie-patiënt
            Patient patient = MainWindow.IngelogdePatient;
            patient.Voornaam = voornaam;
            patient.Achternaam = achternaam;
            patient.Geslacht = geslacht;
            patient.Gsm = gsm;
            patient.Email = email;
            patient.Geboortedatum = DpGeboortedatum.SelectedDate.Value;
            patient.Notificaties = (Notificaties)CmbNotificaties.SelectedIndex;

            // Nieuw wachtwoord enkel hashen als het ingevuld is
            if (!string.IsNullOrEmpty(wachtwoord))
                patient.Paswoord = Gebruiker.HashWachtwoord(wachtwoord);

            try
            {
                patient.Update();
                NavigationService.Navigate(new ProfielInfoPage());
            }
            catch (Exception ex)
            {
                TxtFout.Text = "Fout bij het opslaan: " + ex.Message;
                TxtFout.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Navigeert terug naar ProfielInfoPage zonder wijzigingen op te slaan.
        /// </summary>
        private void BtnAnnuleren_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProfielInfoPage());
        }
    }
}
