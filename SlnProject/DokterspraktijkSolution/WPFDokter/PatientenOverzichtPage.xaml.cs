using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using DokterspraktijkLib;

namespace WPFDokter
{
    /// <summary>
    /// Overzichtspagina van alle patiënten.
    /// Biedt zoekfunctionaliteit en toont elke patiënt als een dynamisch aangemaakt kaartje.
    /// </summary>
    public partial class PatientenOverzichtPage : Page
    {
        /// <summary>
        /// Initialiseert de pagina.
        /// </summary>
        public PatientenOverzichtPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Laadt alle patiënten bij het openen van de pagina.
        /// </summary>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LaadPatienten(string.Empty);
        }

        /// <summary>
        /// Haalt patiënten op uit de database en bouwt de kaartjes opnieuw op.
        /// Roept Patient.GetAll() aan bij lege zoekterm, anders Patient.Zoek(zoekterm).
        /// </summary>
        /// <param name="zoekterm">Vrije tekst om op voornaam of achternaam te filteren. Leeg voor alle patiënten.</param>
        private void LaadPatienten(string zoekterm)
        {
            TxtFout.Visibility = Visibility.Collapsed;
            TxtFout.Text = string.Empty;
            PatientenPanel.Children.Clear();

            try
            {
                List<Patient> patienten;

                if (string.IsNullOrEmpty(zoekterm))
                {
                    patienten = Patient.GetAll();
                }
                else
                {
                    patienten = Patient.Zoek(zoekterm);
                }

                if (patienten.Count == 0)
                {
                    TextBlock geen = new TextBlock();
                    geen.Text = "Geen patiënten gevonden.";
                    geen.Foreground = new SolidColorBrush(Color.FromRgb(127, 140, 141));
                    geen.FontSize = 14;
                    geen.Margin = new Thickness(8, 16, 0, 0);
                    PatientenPanel.Children.Add(geen);
                    return;
                }

                foreach (Patient patient in patienten)
                {
                    Border kaartje = MaakPatientKaartje(patient);
                    PatientenPanel.Children.Add(kaartje);
                }
            }
            catch (Exception ex)
            {
                TxtFout.Text = "Fout bij het laden van patiënten: " + ex.Message;
                TxtFout.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Bouwt één patiëntenkaartje op als Border-element met foto, tekstinfo en actieknoppen.
        /// Alle controls worden programmatisch aangemaakt; er wordt geen databinding gebruikt.
        /// </summary>
        /// <param name="patient">De patiënt waarvoor het kaartje wordt aangemaakt.</param>
        /// <returns>Een Border die het volledige kaartje bevat.</returns>
        private Border MaakPatientKaartje(Patient patient)
        {
            // Buitenste kaart-container
            Border kaartje = new Border();
            kaartje.Width = 210;
            kaartje.Background = new SolidColorBrush(Colors.White);
            kaartje.BorderBrush = new SolidColorBrush(Color.FromRgb(220, 220, 220));
            kaartje.BorderThickness = new Thickness(1);
            kaartje.CornerRadius = new CornerRadius(8);
            kaartje.Margin = new Thickness(8);
            kaartje.Padding = new Thickness(14, 16, 14, 14);

            DropShadowEffect schaduw = new DropShadowEffect();
            schaduw.BlurRadius = 8;
            schaduw.ShadowDepth = 2;
            schaduw.Opacity = 0.10;
            schaduw.Color = Colors.Black;
            kaartje.Effect = schaduw;

            StackPanel inhoud = new StackPanel();
            inhoud.HorizontalAlignment = HorizontalAlignment.Center;

            // --- Profielfoto ---
            Image foto = new Image();
            foto.Width = 60;
            foto.Height = 60;
            foto.Stretch = Stretch.UniformToFill;
            foto.HorizontalAlignment = HorizontalAlignment.Center;
            foto.Margin = new Thickness(0, 0, 0, 10);

            // Afgeronde clip (cirkel)
            EllipseGeometry clip = new EllipseGeometry();
            clip.Center = new System.Windows.Point(30, 30);
            clip.RadiusX = 30;
            clip.RadiusY = 30;
            foto.Clip = clip;

            if (patient.Profielfotodata != null && patient.Profielfotodata.Length > 0)
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    MemoryStream stream = new MemoryStream(patient.Profielfotodata);
                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    foto.Source = bitmap;
                }
                catch
                {
                    // Geen foto weergeven bij corrupte data
                }
            }

            inhoud.Children.Add(foto);

            // --- Volledige naam ---
            TextBlock naam = new TextBlock();
            naam.Text = patient.Voornaam + " " + patient.Achternaam;
            naam.FontSize = 13;
            naam.FontWeight = FontWeights.SemiBold;
            naam.Foreground = new SolidColorBrush(Color.FromRgb(44, 62, 80));
            naam.TextAlignment = TextAlignment.Center;
            naam.TextWrapping = TextWrapping.Wrap;
            naam.Margin = new Thickness(0, 0, 0, 4);
            inhoud.Children.Add(naam);

            // --- E-mailadres ---
            TextBlock email = new TextBlock();
            email.Text = patient.Email;
            email.FontSize = 11;
            email.Foreground = new SolidColorBrush(Color.FromRgb(127, 140, 141));
            email.TextAlignment = TextAlignment.Center;
            email.TextWrapping = TextWrapping.Wrap;
            email.Margin = new Thickness(0, 0, 0, 2);
            inhoud.Children.Add(email);

            // --- GSM-nummer ---
            TextBlock gsm = new TextBlock();
            gsm.Text = patient.Gsm;
            gsm.FontSize = 11;
            gsm.Foreground = new SolidColorBrush(Color.FromRgb(127, 140, 141));
            gsm.TextAlignment = TextAlignment.Center;
            inhoud.Children.Add(gsm);

            // --- Actieknoppen ---
            StackPanel knoppen = new StackPanel();
            knoppen.Orientation = Orientation.Horizontal;
            knoppen.HorizontalAlignment = HorizontalAlignment.Center;
            knoppen.Margin = new Thickness(0, 14, 0, 0);

            Button btnDetails = new Button();
            btnDetails.Content = "Details";
            btnDetails.Padding = new Thickness(8, 5, 8, 5);
            btnDetails.Margin = new Thickness(2);
            btnDetails.FontSize = 11;
            btnDetails.Background = new SolidColorBrush(Color.FromRgb(52, 152, 219));
            btnDetails.Foreground = new SolidColorBrush(Colors.White);
            btnDetails.BorderThickness = new Thickness(0);
            btnDetails.Tag = patient;
            btnDetails.Click += BtnDetails_Click;
            knoppen.Children.Add(btnDetails);

            Button btnWijzigen = new Button();
            btnWijzigen.Content = "Wijzigen";
            btnWijzigen.Padding = new Thickness(8, 5, 8, 5);
            btnWijzigen.Margin = new Thickness(2);
            btnWijzigen.FontSize = 11;
            btnWijzigen.Background = new SolidColorBrush(Color.FromRgb(243, 156, 18));
            btnWijzigen.Foreground = new SolidColorBrush(Colors.White);
            btnWijzigen.BorderThickness = new Thickness(0);
            btnWijzigen.Tag = patient;
            btnWijzigen.Click += BtnWijzigen_Click;
            knoppen.Children.Add(btnWijzigen);

            Button btnVerwijderen = new Button();
            btnVerwijderen.Content = "Verwijderen";
            btnVerwijderen.Padding = new Thickness(8, 5, 8, 5);
            btnVerwijderen.Margin = new Thickness(2);
            btnVerwijderen.FontSize = 11;
            btnVerwijderen.Background = new SolidColorBrush(Color.FromRgb(192, 57, 43));
            btnVerwijderen.Foreground = new SolidColorBrush(Colors.White);
            btnVerwijderen.BorderThickness = new Thickness(0);
            btnVerwijderen.Tag = patient;
            btnVerwijderen.Click += BtnVerwijderen_Click;
            knoppen.Children.Add(btnVerwijderen);

            inhoud.Children.Add(knoppen);
            kaartje.Child = inhoud;

            return kaartje;
        }

        /// <summary>
        /// Zoekt patiënten op basis van de ingevoerde zoektekst en herlaadt de kaartjes.
        /// </summary>
        private void BtnZoeken_Click(object sender, RoutedEventArgs e)
        {
            LaadPatienten(TxtZoeken.Text.Trim());
        }

        /// <summary>
        /// Navigeert naar PatiëntWijzigenPage met een lege Patient voor het aanmaken van een nieuw dossier.
        /// </summary>
        private void BtnNieuwePatient_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PatientWijzigenPage(new Patient()));
        }

        /// <summary>
        /// Navigeert naar PatiëntDetailsPage voor de patiënt die bij de gekliktekknop hoort.
        /// </summary>
        private void BtnDetails_Click(object sender, RoutedEventArgs e)
        {
            Button knop = (Button)sender;
            Patient patient = (Patient)knop.Tag;
            NavigationService.Navigate(new PatientDetailsPage(patient));
        }

        /// <summary>
        /// Navigeert naar PatiëntWijzigenPage voor de patiënt die bij de gekliktekknop hoort.
        /// </summary>
        private void BtnWijzigen_Click(object sender, RoutedEventArgs e)
        {
            Button knop = (Button)sender;
            Patient patient = (Patient)knop.Tag;
            NavigationService.Navigate(new PatientWijzigenPage(patient));
        }

        /// <summary>
        /// Navigeert naar PatiëntVerwijderenPage voor de patiënt die bij de gekliktekknop hoort.
        /// </summary>
        private void BtnVerwijderen_Click(object sender, RoutedEventArgs e)
        {
            Button knop = (Button)sender;
            Patient patient = (Patient)knop.Tag;
            NavigationService.Navigate(new PatientVerwijderenPage(patient));
        }
    }
}
