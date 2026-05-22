using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using DokterspraktijkLib;

namespace WPFDokter
{
    /// <summary>
    /// Pagina voor het bekijken en beheren van afspraken van de ingelogde dokter.
    /// Toont een kalender, een lijst met afspraken per dag en laat toe afspraken te annuleren.
    /// </summary>
    public partial class AfsprakenPage : Page
    {
        /// <summary>
        /// De afspraken die momenteel in de ListBox getoond worden.
        /// Index correspondeert 1-op-1 met LstAfspraken.Items.
        /// </summary>
        private List<Afspraak> _afspraken;

        /// <summary>
        /// Initialiseert de pagina, laadt de profielfoto en selecteert de huidige datum.
        /// </summary>
        public AfsprakenPage()
        {
            InitializeComponent();
            _afspraken = new List<Afspraak>();
            ToonProfielfoto();

            // Vandaag als standaardselectie instellen – triggert ook SelectedDatesChanged
            CalendarDatum.SelectedDate = DateTime.Today;
        }

        /// <summary>
        /// Toont de profielfoto van de ingelogde dokter rechtsboven.
        /// Heeft de dokter geen foto, blijft de Image leeg.
        /// </summary>
        private void ToonProfielfoto()
        {
            if (MainWindow.IngelogdeDokter == null) return;
            if (MainWindow.IngelogdeDokter.Profielfotodata == null) return;
            if (MainWindow.IngelogdeDokter.Profielfotodata.Length == 0) return;

            BitmapImage bitmap = new BitmapImage();
            MemoryStream stream = new MemoryStream(MainWindow.IngelogdeDokter.Profielfotodata);
            bitmap.BeginInit();
            bitmap.StreamSource = stream;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            ImgProfielfoto.Source = bitmap;
        }

        /// <summary>
        /// Laadt de afspraken van de ingelogde dokter voor de opgegeven datum
        /// en vult LstAfspraken met "HH:mm – Voornaam Achternaam".
        /// </summary>
        /// <param name="datum">De datum waarvoor afspraken worden opgehaald.</param>
        private void LaadAfspraken(DateTime datum)
        {
            TxtFout.Visibility = Visibility.Collapsed;
            TxtFout.Text = string.Empty;
            LstAfspraken.Items.Clear();
            _afspraken.Clear();
            TxtKlacht.Text = string.Empty;
            BtnAnnuleren.IsEnabled = false;
            PanelBevestiging.Visibility = Visibility.Collapsed;

            try
            {
                List<Afspraak> opgehaald = Afspraak.GetByDokterEnDatum(
                    MainWindow.IngelogdeDokter.Id, datum);

                foreach (Afspraak afspraak in opgehaald)
                {
                    // Patiëntgegevens ophalen om naam te kunnen tonen
                    Patient patient = Patient.GetById(afspraak.PatientId);
                    string patientNaam = patient != null
                        ? patient.Voornaam + " " + patient.Achternaam
                        : "(onbekende patiënt)";

                    string rij = afspraak.Moment.ToString("HH:mm") + " – " + patientNaam;
                    LstAfspraken.Items.Add(rij);
                    _afspraken.Add(afspraak);
                }
            }
            catch (Exception ex)
            {
                TxtFout.Text = "Fout bij het laden van afspraken: " + ex.Message;
                TxtFout.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Wordt aangeroepen wanneer de gebruiker een andere datum selecteert in de kalender.
        /// Herlaadt de afsprakenlijst voor de geselecteerde datum.
        /// </summary>
        private void CalendarDatum_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CalendarDatum.SelectedDate == null) return;
            if (MainWindow.IngelogdeDokter == null) return;

            DateTime gekozenDatum = CalendarDatum.SelectedDate.Value;
            LaadAfspraken(gekozenDatum);
        }

        /// <summary>
        /// Wordt aangeroepen wanneer de gebruiker een afspraak in de lijst selecteert.
        /// Toont de bijbehorende klacht en schakelt de knop Annuleren in.
        /// </summary>
        private void LstAfspraken_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = LstAfspraken.SelectedIndex;

            if (index < 0)
            {
                TxtKlacht.Text = string.Empty;
                BtnAnnuleren.IsEnabled = false;
                PanelBevestiging.Visibility = Visibility.Collapsed;
                return;
            }

            Afspraak geselecteerd = _afspraken[index];
            TxtKlacht.Text = geselecteerd.Klacht;
            BtnAnnuleren.IsEnabled = true;

            // Bevestigingspaneel verbergen bij nieuwe selectie
            PanelBevestiging.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Toont het bevestigingspaneel met "Ja" en "Nee" knoppen.
        /// De eigenlijke verwijdering gebeurt pas na bevestiging via BtnJa_Click.
        /// </summary>
        private void BtnAnnuleren_Click(object sender, RoutedEventArgs e)
        {
            int index = LstAfspraken.SelectedIndex;
            if (index < 0) return;

            Afspraak geselecteerd = _afspraken[index];

            // Bevestigingstekst bijwerken met de specifieke afspraakdetails
            TxtBevestiging.Text = "Weet u zeker dat u de afspraak van "
                + geselecteerd.Moment.ToString("HH:mm")
                + " op " + geselecteerd.Moment.ToString("dd/MM/yyyy")
                + " wilt annuleren?";

            PanelBevestiging.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Verwijdert de geselecteerde afspraak na bevestiging en herlaadt de lijst.
        /// </summary>
        private void BtnJa_Click(object sender, RoutedEventArgs e)
        {
            int index = LstAfspraken.SelectedIndex;
            if (index < 0)
            {
                PanelBevestiging.Visibility = Visibility.Collapsed;
                return;
            }

            Afspraak teVerwijderen = _afspraken[index];

            try
            {
                teVerwijderen.Delete();
                PanelBevestiging.Visibility = Visibility.Collapsed;

                // Lijst herladen na verwijdering
                if (CalendarDatum.SelectedDate != null)
                {
                    LaadAfspraken(CalendarDatum.SelectedDate.Value);
                }
            }
            catch (Exception ex)
            {
                PanelBevestiging.Visibility = Visibility.Collapsed;
                TxtFout.Text = "Fout bij het annuleren van de afspraak: " + ex.Message;
                TxtFout.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Verbergt het bevestigingspaneel zonder de afspraak te verwijderen.
        /// </summary>
        private void BtnNee_Click(object sender, RoutedEventArgs e)
        {
            PanelBevestiging.Visibility = Visibility.Collapsed;
        }
    }
}
