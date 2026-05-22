using System.Windows;
using System.Windows.Controls;

namespace WPFDokter
{
    /// <summary>
    /// Startpagina van de doktersapplicatie.
    /// Toont een welkomstboodschap met uitleg en een knop om naar de loginpagina te gaan.
    /// </summary>
    public partial class StartPage : Page
    {
        /// <summary>
        /// Initialiseert de startpagina.
        /// </summary>
        public StartPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Navigeert naar de loginpagina wanneer de gebruiker op "Inloggen" klikt.
        /// </summary>
        private void BtnNaarLogin_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new LoginPage());
        }
    }
}
