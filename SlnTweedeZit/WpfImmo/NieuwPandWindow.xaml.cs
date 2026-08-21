using System.IO;
using System.Windows;
using Microsoft.Win32;
using CLImmo;

namespace WpfImmo;

/// <summary>
/// Venster waarmee de makelaar een nieuw pand (Huis of Appartement) kan
/// registreren. Valideert de invoer via PandValidator, maakt het juiste
/// object aan, slaat het op in de databank en sluit zichzelf. MainWindow
/// herlaadt daarna het aanbod (zie MainWindow.NieuwPandButton_Click).
/// </summary>
public partial class NieuwPandWindow : Window
{
    private readonly PandRepository repository;
    private readonly PandValidator validator;

    // Bestandsnaam van de gekozen foto (enkel de naam, zoals opgeslagen in de
    // databank), of null als er geen foto gekozen is.
    private string? gekozenFotoNaam;

    public NieuwPandWindow(PandRepository repository)
    {
        InitializeComponent();
        this.repository = repository;
        validator = new PandValidator();

        VulEnergielabelComboIn();
        VulMakelaarComboIn();
    }

    private void VulEnergielabelComboIn()
    {
        foreach (Energielabel label in Enum.GetValues<Energielabel>())
        {
            EnergielabelCombo.Items.Add(label);
        }

        EnergielabelCombo.SelectedIndex = 0;
    }

    private void VulMakelaarComboIn()
    {
        List<Makelaar> alleMakelaars = repository.GeefAlleMakelaars();

        foreach (Makelaar makelaar in alleMakelaars)
        {
            MakelaarCombo.Items.Add(makelaar);
        }

        if (MakelaarCombo.Items.Count > 0)
        {
            MakelaarCombo.SelectedIndex = 0;
        }
    }

    /// <summary>
    /// Toont het tuinoppervlakte-veld bij "Huis" en het lift-veld bij
    /// "Appartement".
    /// </summary>
    private void TypeRadio_Checked(object sender, RoutedEventArgs e)
    {
        if (HuisRadio.IsChecked == true)
        {
            TuinPanel.Visibility = Visibility.Visible;
            LiftPanel.Visibility = Visibility.Collapsed;
        }
        else
        {
            TuinPanel.Visibility = Visibility.Collapsed;
            LiftPanel.Visibility = Visibility.Visible;
        }
    }

    /// <summary>
    /// Laat de gebruiker een foto kiezen via een standaard bestandsdialoog.
    /// De gekozen foto wordt meteen gekopieerd naar de map "Afbeeldingen" van
    /// de applicatie, zodat ze na het opslaan meteen getoond kan worden.
    /// </summary>
    private void FotoKiezenButton_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog dialoog = new OpenFileDialog();
        dialoog.Filter = "Afbeeldingen (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp";
        dialoog.Title = "Kies een foto voor dit pand";

        bool? resultaat = dialoog.ShowDialog(this);

        if (resultaat == true)
        {
            try
            {
                string afbeeldingenMap = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Afbeeldingen");
                Directory.CreateDirectory(afbeeldingenMap);

                string bestandsnaam = Path.GetFileName(dialoog.FileName);
                string doelPad = Path.Combine(afbeeldingenMap, bestandsnaam);

                File.Copy(dialoog.FileName, doelPad, true);

                gekozenFotoNaam = bestandsnaam;
                FotoNaamTekst.Text = bestandsnaam;
            }
            catch (IOException)
            {
                // Kopiëren mislukt (bv. bestand in gebruik, schijf vol, ...):
                // we gaan gewoon verder zonder foto in plaats van te crashen.
                gekozenFotoNaam = null;
                FotoNaamTekst.Text = "(kon foto niet kopiëren, geen foto gebruikt)";
            }
        }
    }

    /// <summary>
    /// Valideert alle invoer, maakt het juiste Pand-object aan, slaat het op
    /// in de databank en sluit het venster. Toont een duidelijke foutmelding
    /// (zonder te crashen) als de invoer ongeldig is.
    /// </summary>
    private void BevestigenButton_Click(object sender, RoutedEventArgs e)
    {
        List<string> fouten = new List<string>();

        string adres = AdresTextBox.Text.Trim();
        if (!validator.IsGeldigAdres(adres))
        {
            fouten.Add("Het adres is ongeldig (minstens " + validator.MinimumAantalTekensAdres
                + " tekens, enkel letters, cijfers, spaties en . , -).");
        }

        double prijs = 0;
        bool prijsIsGetal = double.TryParse(PrijsTextBox.Text.Trim(), out prijs);
        if (!prijsIsGetal || !validator.IsGeldigPrijs(prijs))
        {
            fouten.Add("De prijs moet een getal groter dan 0 zijn.");
        }

        double oppervlakte = 0;
        bool oppervlakteIsGetal = double.TryParse(OppervlakteTextBox.Text.Trim(), out oppervlakte);
        if (!oppervlakteIsGetal || oppervlakte <= 0)
        {
            fouten.Add("De oppervlakte moet een getal groter dan 0 zijn.");
        }

        int bouwJaar = 0;
        bool bouwJaarIsGetal = int.TryParse(BouwJaarTextBox.Text.Trim(), out bouwJaar);
        if (!bouwJaarIsGetal || bouwJaar < 1800 || bouwJaar > DateTime.Now.Year)
        {
            fouten.Add("Het bouwjaar moet een geldig jaartal zijn (tussen 1800 en " + DateTime.Now.Year + ").");
        }

        if (MakelaarCombo.SelectedItem is not Makelaar gekozenMakelaar)
        {
            fouten.Add("Er is geen makelaar beschikbaar om te koppelen aan dit pand.");
            gekozenMakelaar = null!;
        }

        double tuinoppervlakte = 0;
        if (HuisRadio.IsChecked == true)
        {
            bool tuinIsGetal = double.TryParse(TuinTextBox.Text.Trim(), out tuinoppervlakte);
            if (!tuinIsGetal || tuinoppervlakte < 0)
            {
                fouten.Add("De tuinoppervlakte moet een getal zijn van 0 of meer.");
            }
        }

        if (fouten.Count > 0)
        {
            FoutTekst.Text = string.Join(Environment.NewLine, fouten);
            FoutTekst.Visibility = Visibility.Visible;
            return;
        }

        FoutTekst.Visibility = Visibility.Collapsed;

        Energielabel gekozenLabel = (Energielabel)EnergielabelCombo.SelectedItem;

        Pand nieuwPand;
        if (HuisRadio.IsChecked == true)
        {
            nieuwPand = new Huis(adres, prijs, oppervlakte, gekozenLabel, gekozenMakelaar,
                bouwJaar, tuinoppervlakte, gekozenFotoNaam);
        }
        else
        {
            bool heeftLift = LiftCheckBox.IsChecked == true;
            nieuwPand = new Appartement(adres, prijs, oppervlakte, gekozenLabel, gekozenMakelaar,
                bouwJaar, heeftLift, gekozenFotoNaam);
        }

        repository.VoegPandToe(nieuwPand);

        DialogResult = true;
        Close();
    }

    private void AnnulerenButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
