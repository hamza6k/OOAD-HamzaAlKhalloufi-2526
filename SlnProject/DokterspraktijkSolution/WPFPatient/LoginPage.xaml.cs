using System;
using System.Windows;
using System.Windows.Controls;
using DokterspraktijkLib;

namespace WPFPatient
{
    /// <summary>
    /// Loginpagina voor de patiëntenapplicatie.
    /// Valideert het formulier, roept Patient.Login aan en navigeert
    /// bij succes naar de afsprakenoverzichtpagina.
    /// </summary>
    public partial class LoginPage : Page
    {
        /// <summary>
        /// Initialiseert de loginpagina en vult het wachtwoordveld vooraf in.
        /// </summary>
        public LoginPage()
        {
            InitializeComponent();

            // PasswordBox ondersteunt geen Text-binding in XAML; vooraf invullen via code
            PbxWachtwoord.Password = "klepketoe";
        }

        /// <summary>
        /// Verwerkt de loginklik: valideert invoer, roept de database aan
        /// en navigeert bij succes naar AfsprakenOverzichtPage.
        /// Fouten worden getoond in een TextBlock, nooit in een MessageBox.
        /// </summary>
        private void BtnInloggen_Click(object sender, RoutedEventArgs e)
        {
            // Vorige foutmeldingen wissen
            TxtValidatie.Text = string.Empty;
            TxtFout.Text = string.Empty;
            TxtFout.Visibility = Visibility.Collapsed;

            string email = TxtEmail.Text.Trim();
            string wachtwoord = PbxWachtwoord.Password;

            // Formuliervalidatie: verplichte velden controleren
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(wachtwoord))
            {
                TxtValidatie.Text = "Vul uw e-mailadres en wachtwoord in.";
                return;
            }

            try
            {
                Patient patient = Patient.Login(email, wachtwoord);

                if (patient == null)
                {
                    // Ongeldige combinatie
                    TxtFout.Text = "E-mailadres of wachtwoord is incorrect.";
                    TxtFout.Visibility = Visibility.Visible;
                    return;
                }

                // Sessie instellen in het hoofdvenster
                MainWindow.IngelogdePatient = patient;

                // Naam en profielfoto tonen in de navigatiebalk
                MainWindow hoofdvenster = (MainWindow)Application.Current.MainWindow;
                hoofdvenster.ToonIngelogdePatient();

                // Navigeer naar de afsprakenoverzichtpagina
                NavigationService.Navigate(new AfsprakenOverzichtPage());
            }
            catch (Exception ex)
            {
                // Databasefouten tonen in het fout-TextBlock
                TxtFout.Text = "Fout bij het aanmelden: " + ex.Message;
                TxtFout.Visibility = Visibility.Visible;
            }
        }
    }
}
