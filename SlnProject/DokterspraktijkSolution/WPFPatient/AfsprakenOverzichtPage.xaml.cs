using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DokterspraktijkLib;

namespace WPFPatient
{
    /// <summary>
    /// Overzichtspagina van alle afspraken van de ingelogde patiënt.
    /// Toont afspraken als "dd/MM/yyyy HH:mm – Dr. Voornaam Achternaam".
    /// Annuleren is enkel mogelijk voor toekomstige afspraken.
    /// </summary>
    public partial class AfsprakenOverzichtPage : Page
    {
        private List<Afspraak> _afspraken;

        /// <summary>
        /// Initialiseert de pagina en de interne afsprakenlijst.
        /// </summary>
        public AfsprakenOverzichtPage()
        {
            InitializeComponent();
            _afspraken = new List<Afspraak>();
        }

        /// <summary>
        /// Laadt alle afspraken bij het openen van de pagina.
        /// </summary>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LaadAfspraken();
        }

        /// <summary>
        /// Haalt alle afspraken van de ingelogde patiënt op en vult de ListBox.
        /// Per afspraak wordt de doktornaam opgezocht via Dokter.GetById.
        /// </summary>
        private void LaadAfspraken()
        {
            LstAfspraken.Items.Clear();
            _afspraken.Clear();
            PanelKlacht.Visibility = Visibility.Collapsed;
            PanelBevestiging.Visibility = Visibility.Collapsed;
            BtnAnnuleren.IsEnabled = false;
            TxtFout.Visibility = Visibility.Collapsed;

            try
            {
                List<Afspraak> lijst = Afspraak.GetByPatient(MainWindow.IngelogdePatient.Id);

                foreach (Afspraak afspraak in lijst)
                {
                    Dokter dokter = Dokter.GetById(afspraak.DokterId);
                    string dokterNaam = dokter != null
                        ? "Dr. " + dokter.Voornaam + " " + dokter.Achternaam
                        : "Onbekende dokter";

                    string regel = afspraak.Moment.ToString("dd/MM/yyyy HH:mm") + " – " + dokterNaam;
                    LstAfspraken.Items.Add(regel);
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
        /// Toont de klacht van de geselecteerde afspraak.
        /// Activeert BtnAnnuleren enkel als de afspraak in de toekomst ligt.
        /// </summary>
        private void LstAfspraken_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PanelBevestiging.Visibility = Visibility.Collapsed;
            BtnAnnuleren.IsEnabled = false;
            PanelKlacht.Visibility = Visibility.Collapsed;

            int index = LstAfspraken.SelectedIndex;
            if (index < 0 || index >= _afspraken.Count)
                return;

            Afspraak afspraak = _afspraken[index];

            // Klacht tonen
            TxtKlacht.Text = string.IsNullOrEmpty(afspraak.Klacht)
                ? "(geen klacht opgegeven)"
                : afspraak.Klacht;
            PanelKlacht.Visibility = Visibility.Visible;

            // Annuleren enkel mogelijk voor toekomstige afspraken
            if (afspraak.Moment > DateTime.Now)
                BtnAnnuleren.IsEnabled = true;
        }

        /// <summary>
        /// Toont het bevestigingspaneel met de datum/tijd van de te annuleren afspraak.
        /// </summary>
        private void BtnAnnuleren_Click(object sender, RoutedEventArgs e)
        {
            int index = LstAfspraken.SelectedIndex;
            if (index < 0 || index >= _afspraken.Count)
                return;

            Afspraak afspraak = _afspraken[index];
            TxtBevestigingVraag.Text = "Weet u zeker dat u de afspraak op "
                + afspraak.Moment.ToString("dd/MM/yyyy") + " om "
                + afspraak.Moment.ToString("HH:mm")
                + " wilt annuleren?";

            PanelBevestiging.Visibility = Visibility.Visible;
            BtnAnnuleren.IsEnabled = false;
        }

        /// <summary>
        /// Verwijdert de geselecteerde afspraak en herlaadt de lijst.
        /// </summary>
        private void BtnJa_Click(object sender, RoutedEventArgs e)
        {
            int index = LstAfspraken.SelectedIndex;
            if (index < 0 || index >= _afspraken.Count)
                return;

            Afspraak afspraak = _afspraken[index];

            try
            {
                afspraak.Delete();
                LaadAfspraken();
            }
            catch (Exception ex)
            {
                PanelBevestiging.Visibility = Visibility.Collapsed;
                TxtFout.Text = "Fout bij het annuleren: " + ex.Message;
                TxtFout.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Verbergt het bevestigingspaneel zonder de afspraak te verwijderen.
        /// </summary>
        private void BtnNee_Click(object sender, RoutedEventArgs e)
        {
            PanelBevestiging.Visibility = Visibility.Collapsed;

            int index = LstAfspraken.SelectedIndex;
            if (index >= 0 && index < _afspraken.Count && _afspraken[index].Moment > DateTime.Now)
                BtnAnnuleren.IsEnabled = true;
        }

        /// <summary>
        /// Navigeert naar NieuweAfspraakPage.
        /// </summary>
        private void BtnNieuweAfspraak_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new NieuweAfspraakPage());
        }
    }
}
