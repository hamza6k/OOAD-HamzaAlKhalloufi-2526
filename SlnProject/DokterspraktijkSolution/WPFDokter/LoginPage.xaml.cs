using System;
using System.Windows;
using System.Windows.Controls;
using DokterspraktijkLib;

namespace WPFDokter
{
    /// <summary>
    /// Loginpagina voor de doktersapplicatie.
    /// Valideert het formulier, roept Dokter.Login aan en navigeert
    /// bij succes naar de afsprakenpagina.
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
            PbxWachtwoord.Password = "t9ZmRrAbSfCv";
        }

        /// <summary>
        /// Verwerkt de loginklik: valideert invoer, roept de database aan
        /// en navigeert bij succes naar AfsprakenPage.
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
                Dokter dokter = Dokter.Login(email, wachtwoord);

                if (dokter == null)
                {
                    // Ongeldige combinatie
                    TxtFout.Text = "E-mailadres of wachtwoord is incorrect.";
                    TxtFout.Visibility = Visibility.Visible;
                    return;
                }

                // Sessie instellen in het hoofdvenster
                MainWindow.IngelogdeDokter = dokter;

                // Naam en profielfoto tonen in de navigatiebalk
                MainWindow hoofdvenster = Application.Current.MainWindow as MainWindow;
                hoofdvenster.ToonIngelogdeDokter();

                // Navigeer naar de afsprakenpagina
                NavigationService.Navigate(new AfsprakenPage());
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
