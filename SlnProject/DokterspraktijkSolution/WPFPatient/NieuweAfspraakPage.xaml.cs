using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DokterspraktijkLib;

namespace WPFPatient
{
    /// <summary>
    /// Pagina voor het aanmaken van een nieuwe afspraak.
    /// De patiënt kiest een dokter, een datum, een tijdslot en geeft een klacht op.
    /// </summary>
    public partial class NieuweAfspraakPage : Page
    {
        private List<Dokter> _dokters;

        /// <summary>
        /// Initialiseert de pagina en de interne dokterlijst.
        /// </summary>
        public NieuweAfspraakPage()
        {
            InitializeComponent();
            _dokters = new List<Dokter>();
        }

        /// <summary>
        /// Laadt de lijst van dokters en de tijdsloten bij het openen van de pagina.
        /// Stelt de minimale datum in op vandaag.
        /// </summary>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CalendarDatum.BlackoutDates.AddDatesInPast();
            CalendarDatum.SelectedDate = DateTime.Today;

            LaadTijdsloten();
            LaadDokters();
        }

        /// <summary>
        /// Vult de tijdslot-ComboBox met tijden van 08:00 tot 17:30 met stappen van 30 minuten.
        /// </summary>
        private void LaadTijdsloten()
        {
            CmbTijdslot.Items.Clear();

            DateTime tijdstip = new DateTime(2000, 1, 1, 8, 0, 0);
            DateTime eindtijd = new DateTime(2000, 1, 1, 17, 30, 0);

            while (tijdstip <= eindtijd)
            {
                CmbTijdslot.Items.Add(tijdstip.ToString("HH:mm"));
                tijdstip = tijdstip.AddMinutes(30);
            }
        }

        /// <summary>
        /// Haalt alle dokters op uit de database en vult de ListBox.
        /// </summary>
        private void LaadDokters()
        {
            LstDokters.Items.Clear();
            _dokters.Clear();
            TxtFout.Visibility = Visibility.Collapsed;

            try
            {
                List<Dokter> lijst = Dokter.GetAll();

                foreach (Dokter dokter in lijst)
                {
                    LstDokters.Items.Add(dokter.Voornaam + " " + dokter.Achternaam);
                    _dokters.Add(dokter);
                }
            }
            catch (Exception ex)
            {
                TxtFout.Text = "Fout bij het laden van dokters: " + ex.Message;
                TxtFout.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Toont de gegevens en profielfoto van de geselecteerde dokter.
        /// </summary>
        private void LstDokters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = LstDokters.SelectedIndex;
            if (index < 0 || index >= _dokters.Count)
            {
                PanelDokterInfo.Visibility = Visibility.Collapsed;
                return;
            }

            Dokter dokter = _dokters[index];

            TxtDokterNaam.Text = "Dr. " + dokter.Voornaam + " " + dokter.Achternaam;
            TxtRizivnummer.Text = "RIZIV: " + dokter.Rizivnummer;
            TxtGeconventioneerd.Text = dokter.IsGeconventioneerd
                ? "Geconventioneerd"
                : "Niet geconventioneerd";

            // Profielfoto laden
            if (dokter.Profielfotodata != null && dokter.Profielfotodata.Length > 0)
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = new MemoryStream(dokter.Profielfotodata);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    ImgDokterFoto.Source = bitmap;
                }
                catch (Exception)
                {
                    ImgDokterFoto.Source = null;
                }
            }
            else
            {
                ImgDokterFoto.Source = null;
            }

            PanelDokterInfo.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Valideert het formulier, maakt een nieuwe Afspraak aan en slaat op.
        /// Navigeert bij succes naar AfsprakenOverzichtPage.
        /// </summary>
        private void BtnBevestigen_Click(object sender, RoutedEventArgs e)
        {
            TxtValidatie.Text = string.Empty;
            TxtFout.Text = string.Empty;
            TxtFout.Visibility = Visibility.Collapsed;

            // Validatie: dokter
            if (LstDokters.SelectedIndex < 0)
            {
                TxtValidatie.Text = "Kies een dokter.";
                return;
            }

            // Validatie: datum
            if (CalendarDatum.SelectedDate == null)
            {
                TxtValidatie.Text = "Kies een datum.";
                return;
            }

            // Validatie: tijdslot
            if (CmbTijdslot.SelectedIndex < 0)
            {
                TxtValidatie.Text = "Kies een tijdslot.";
                return;
            }

            // Validatie: klacht
            string klacht = TxtKlacht.Text.Trim();
            if (string.IsNullOrEmpty(klacht))
            {
                TxtValidatie.Text = "Geef een klacht of opmerking op.";
                return;
            }

            // Moment samenstellen uit datum + tijdslot
            DateTime gekozenDatum = CalendarDatum.SelectedDate.Value.Date;
            string tijdTekst = CmbTijdslot.SelectedItem.ToString();
            string[] delen = tijdTekst.Split(':');
            int uur = int.Parse(delen[0]);
            int minuten = int.Parse(delen[1]);
            DateTime moment = new DateTime(gekozenDatum.Year, gekozenDatum.Month, gekozenDatum.Day, uur, minuten, 0);

            // Moment moet in de toekomst liggen
            if (moment <= DateTime.Now)
            {
                TxtValidatie.Text = "Kies een datum en tijdstip in de toekomst.";
                return;
            }

            Dokter gekozenDokter = _dokters[LstDokters.SelectedIndex];

            Afspraak nieuweAfspraak = new Afspraak();
            nieuweAfspraak.DokterId = gekozenDokter.Id;
            nieuweAfspraak.PatientId = MainWindow.IngelogdePatient.Id;
            nieuweAfspraak.Moment = moment;
            nieuweAfspraak.Klacht = klacht;

            try
            {
                nieuweAfspraak.Insert();
                NavigationService.Navigate(new AfsprakenOverzichtPage());
            }
            catch (Exception ex)
            {
                TxtFout.Text = "Fout bij het opslaan van de afspraak: " + ex.Message;
                TxtFout.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Navigeert terug naar het afsprakenoverzicht zonder op te slaan.
        /// </summary>
        private void BtnAnnuleren_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AfsprakenOverzichtPage());
        }
    }
}
