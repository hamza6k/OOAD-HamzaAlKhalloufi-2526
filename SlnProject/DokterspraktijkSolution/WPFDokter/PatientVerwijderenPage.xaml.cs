using System;
using System.Windows;
using System.Windows.Controls;
using DokterspraktijkLib;

namespace WPFDokter
{
    /// <summary>
    /// Bevestigingspagina voor het permanent verwijderen van een patiëntdossier.
    /// Patient.Delete() verwijdert ook alle gekoppelde afspraken via een transactie.
    /// </summary>
    public partial class PatientVerwijderenPage : Page
    {
        private Patient _patient;

        /// <summary>
        /// Initialiseert de bevestigingspagina en toont de naam van de te verwijderen patiënt.
        /// </summary>
        /// <param name="patient">De patiënt die verwijderd zal worden.</param>
        public PatientVerwijderenPage(Patient patient)
        {
            InitializeComponent();
            _patient = patient;
            TxtPatientNaam.Text = _patient.Voornaam + " " + _patient.Achternaam;
        }

        /// <summary>
        /// Verwijdert de patiënt en alle gekoppelde afspraken via patient.Delete().
        /// Toont een foutmelding in TxtFout bij een databasefout.
        /// Navigeert bij succes terug naar het patiëntenoverzicht.
        /// </summary>
        private void BtnVerwijderen_Click(object sender, RoutedEventArgs e)
        {
            TxtFout.Visibility = Visibility.Collapsed;
            TxtFout.Text = string.Empty;

            // Knoppen uitschakelen om dubbele klik te vermijden
            BtnVerwijderen.IsEnabled = false;
            BtnAnnuleren.IsEnabled = false;

            try
            {
                // Delete() verwijdert eerst afspraken, dan de patiënt (transactie in de library)
                _patient.Delete();
                NavigationService.Navigate(new PatientenOverzichtPage());
            }
            catch (Exception ex)
            {
                TxtFout.Text = "Fout bij het verwijderen: " + ex.Message;
                TxtFout.Visibility = Visibility.Visible;

                // Knoppen terug inschakelen zodat de gebruiker opnieuw kan proberen
                BtnVerwijderen.IsEnabled = true;
                BtnAnnuleren.IsEnabled = true;
            }
        }

        /// <summary>
        /// Navigeert terug naar het patiëntenoverzicht zonder te verwijderen.
        /// </summary>
        private void BtnAnnuleren_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PatientenOverzichtPage());
        }
    }
}
