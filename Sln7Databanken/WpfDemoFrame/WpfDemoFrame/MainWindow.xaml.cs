using System.Windows;
using WpfDemoFrame.Pages;

namespace WpfDemoFrame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected const string Username = "Stan";
        public MainWindow()
        {
            InitializeComponent();
            frmMain.Content = new HomePage(Username);
        }

        private void BtnHome_Click(object sender, RoutedEventArgs e)
        {
            frmMain.Content = new HomePage(Username);
        }

        private void BtnProducts_Click(object sender, RoutedEventArgs e)
        {
            frmMain.Content = new ProductsPage(Username);
        }

        private void BtnAbout_Click(object sender, RoutedEventArgs e)
        {
            frmMain.Content = new AboutPage(Username);
        }
    }
}