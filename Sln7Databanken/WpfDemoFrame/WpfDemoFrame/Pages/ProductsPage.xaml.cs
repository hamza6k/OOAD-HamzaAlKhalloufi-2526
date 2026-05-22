using System.Windows;
using System.Windows.Controls;

namespace WpfDemoFrame.Pages
{
    /// <summary>
    /// Interaction logic for ProductsPage.xaml
    /// </summary>
    public partial class ProductsPage : Page
    {
        private string userName;
        public ProductsPage(string userName)
        {
            InitializeComponent();
            this.userName = userName;
            txtContent.Text = $"Hallo {userName}! Dit is de product pagina.";
        }
        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new HomePage(userName));
        }
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new AboutPage(userName));
        }
    }
}
