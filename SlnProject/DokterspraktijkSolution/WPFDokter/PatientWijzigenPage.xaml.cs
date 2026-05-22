using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using DokterspraktijkLib;

namespace WPFDokter
{
    /// <summary>
    /// Formulierpagina voor het aanmaken of bewerken van een patiëntdossier.
    /// Bij Id == 0 wordt een nieuw dossier aangemaakt via Insert(),
    /// bij Id > 0 worden bestaande gegevens bijgewerkt via Update().
    /// </summary>
    public partial class PatientWijzigenPage : Page
    {
        private Patient _patient;

        /// <summary>
        /// Initialiseert het formulier.
        /// In bewerkmodus worden alle velden vooraf ingevuld met de huidige patiëntdata.
        /// </summary>
        /// <param name="patient">Lege Patient voor nieuw dossier, bestaande Patient voor bewerken.</param>
        public PatientWijzigenPage(Patient patient)
        {
            InitializeComponent();
            _patient = patient;

            if (_patient.Id > 0)
            {
                // Bewerkmodus: velden voorinvullen
                TxtTitel.Text = "Patiënt wijzigen";
                TxtVoornaam.Text = _patient.Voornaam;
                TxtAchternaam.Text = _patient.Achternaam;
                TxtGeslacht.Text = _patient.Geslacht;
                TxtGsm.Text = _patient.Gsm;
                TxtEmail.Text = _patient.Email;
                DpGeboortedatum.SelectedDate = _patient.Geboortedatum;
                CmbNotificaties.SelectedIndex = (int)_patient.Notificaties;

                // Hint tonen dat wachtwoord optioneel is bij bewerken
                TxtWachtwoordHint.Visibility = Visibility.Visible;
                LblWachtwoord.Text = "Wachtwoord";

                // Bestaande profielfoto tonen als die aanwezig is
                if (_patient.Profielfotodata != null && _patient.Profielfotodata.Length > 0)
                {
                    try
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = new MemoryStream(_patient.Profielfotodata);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        ImgFotoPreview.Source = bitmap;
                    }
                    catch (Exception)
                    {
                        ImgFotoPreview.Source = null;
                    }
                }
            }
            else
            {
                // Nieuw dossier: lege pagina
                TxtTitel.Text = "Nieuwe patiënt";
            }
        }

        /// <summary>
        /// Opent een bestandsdialog waarmee de gebruiker een afbeelding kan kiezen.
        /// De gekozen afbeelding wordt als byte[] opgeslagen en getoond als preview.
        /// </summary>
        private void BtnFotoUploaden_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Kies een profielfoto";
            dialog.Filter = "Afbeeldingen|*.jpg;*.jpeg;*.png;*.bmp;*.gif";

            if (dialog.ShowDialog() != true) return;

            try
            {
                // Afbeelding inlezen als bytes en als preview tonen
                _patient.Profielfotodata = File.ReadAllBytes(dialog.FileName);

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(dialog.FileName);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                ImgFotoPreview.Source = bitmap;
            }
            catch (Exception ex)
            {
                TxtValidatie.Text = "Fout bij het laden van de foto: " + ex.Message;
            }
        }

        /// <summary>
        /// Valideert het formulier en slaat de patiënt op via Insert() of Update().
        /// Toont validatiefouten in TxtValidatie. Navigeert bij succes terug naar het overzicht.
        /// </summary>
        private void BtnOpslaan_Click(object sender, RoutedEventArgs e)
        {
            TxtValidatie.Text = string.Empty;

            // Verplichte velden controleren
            if (string.IsNullOrWhiteSpace(TxtVoornaam.Text))
            {
                TxtValidatie.Text = "Voornaam is verplicht.";
                return;
            }
            if (string.IsNullOrWhiteSpace(TxtAchternaam.Text))
            {
                TxtValidatie.Text = "Achternaam is verplicht.";
                return;
            }
            if (string.IsNullOrWhiteSpace(TxtGeslacht.Text))
            {
                TxtValidatie.Text = "Geslacht is verplicht.";
                return;
            }
            if (string.IsNullOrWhiteSpace(TxtGsm.Text))
            {
                TxtValidatie.Text = "GSM-nummer is verplicht.";
                return;
            }
            if (string.IsNullOrWhiteSpace(TxtEmail.Text))
            {
                TxtValidatie.Text = "E-mailadres is verplicht.";
                return;
            }
            if (DpGeboortedatum.SelectedDate == null)
            {
                TxtValidatie.Text = "Geboortedatum is verplicht.";
                return;
            }

            // Wachtwoord: verplicht bij nieuw dossier, optioneel bij bewerken
            string wachtwoord = PbxWachtwoord.Password;
            if (_patient.Id == 0 && string.IsNullOrEmpty(wachtwoord))
            {
                TxtValidatie.Text = "Wachtwoord is verplicht bij een nieuw patiëntdossier.";
                return;
            }

            // Patiëntobject bijwerken met formulierwaarden
            _patient.Voornaam = TxtVoornaam.Text.Trim();
            _patient.Achternaam = TxtAchternaam.Text.Trim();
            _patient.Geslacht = TxtGeslacht.Text.Trim();
            _patient.Gsm = TxtGsm.Text.Trim();
            _patient.Email = TxtEmail.Text.Trim();
            _patient.Geboortedatum = DpGeboortedatum.SelectedDate.Value;
            _patient.Notificaties = (Notificaties)CmbNotificaties.SelectedIndex;

            // Wachtwoord hashen: altijd bij nieuw dossier, enkel als ingevuld bij bewerken
            if (!string.IsNullOrEmpty(wachtwoord))
            {
                _patient.Paswoord = Patient.HashWachtwoord(wachtwoord);
            }

            try
            {
                if (_patient.Id == 0)
                {
                    _patient.Insert();
                }
                else
                {
                    _patient.Update();
                }

                NavigationService.Navigate(new PatientenOverzichtPage());
            }
            catch (Exception ex)
            {
                TxtValidatie.Text = "Fout bij het opslaan: " + ex.Message;
            }
        }

        /// <summary>
        /// Navigeert terug naar het patiëntenoverzicht zonder op te slaan.
        /// </summary>
        private void BtnAnnuleren_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PatientenOverzichtPage());
        }
    }
}
