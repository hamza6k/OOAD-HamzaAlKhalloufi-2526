using System.IO;
using System.Windows;
using CLImmo;

namespace WpfImmo;

/// <summary>
/// Hoofdvenster van de applicatie. Bevat de filterbalk, de galerij met
/// woningkaarten en de detailweergave van het geselecteerde pand. Alle
/// UI-logica gebeurt hier via code-behind: er wordt nergens Binding gebruikt.
/// </summary>
public partial class MainWindow : Window
{
    // Enige toegangspoort tot de databank; wordt ook doorgegeven aan het
    // venster "Nieuw pand toevoegen" zodat er maar één repository-object nodig is.
    private readonly PandRepository repository;

    // De lijst van panden die momenteel (na filtering) in de galerij getoond wordt.
    private List<Pand> huidigeLijst;

    // Het pand dat momenteel geselecteerd is in de galerij (null = geen selectie).
    private Pand? geselecteerdPand;

    // De woningkaart die met het geselecteerde pand overeenkomt, zodat we de
    // selectiekleur ervan weer kunnen uitzetten bij een nieuwe selectie.
    private PandKaart? geselecteerdeKaart;

    public MainWindow()
    {
        InitializeComponent();

        string dbPad = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "ImmoDB.db");
        repository = new PandRepository(dbPad);
        huidigeLijst = new List<Pand>();
    }

    /// <summary>
    /// Bij het laden van het venster: de filter-comboboxen vullen en het
    /// volledige aanbod ophalen en tonen.
    /// </summary>
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        VulEnergielabelComboIn();
        VulMakelaarComboIn();
        HerladenAanbod();
    }

    /// <summary>
    /// Vult de energielabel-combobox met "Alle energielabels" gevolgd door
    /// elke waarde van de enum Energielabel.
    /// </summary>
    private void VulEnergielabelComboIn()
    {
        EnergielabelCombo.Items.Clear();
        EnergielabelCombo.Items.Add("Alle energielabels");

        foreach (Energielabel label in Enum.GetValues<Energielabel>())
        {
            EnergielabelCombo.Items.Add(label);
        }

        EnergielabelCombo.SelectedIndex = 0;
    }

    /// <summary>
    /// Vult de makelaar-combobox met "Alle makelaars" gevolgd door alle
    /// makelaars uit de databank (elke Makelaar toont zichzelf via ToString()).
    /// </summary>
    private void VulMakelaarComboIn()
    {
        MakelaarCombo.Items.Clear();
        MakelaarCombo.Items.Add("Alle makelaars");

        List<Makelaar> alleMakelaars = repository.GeefAlleMakelaars();
        foreach (Makelaar makelaar in alleMakelaars)
        {
            MakelaarCombo.Items.Add(makelaar);
        }

        MakelaarCombo.SelectedIndex = 0;
    }

    /// <summary>
    /// Wordt aangeroepen telkens een van de filters (energielabel, makelaar,
    /// enkel te koop) wijzigt. Herlaadt gewoon het volledige aanbod met de
    /// nieuwe filterwaarden.
    /// </summary>
    private void Filter_Gewijzigd(object sender, RoutedEventArgs e)
    {
        HerladenAanbod();
    }

    /// <summary>
    /// Haalt de panden op die aan de huidige filters voldoen, en herbouwt de
    /// galerij. Wordt zowel bij het opstarten, bij het wijzigen van een
    /// filter, als na het toevoegen/verkopen van een pand aangeroepen.
    /// </summary>
    private void HerladenAanbod()
    {
        Energielabel? gekozenLabel = null;
        if (EnergielabelCombo.SelectedItem is Energielabel label)
        {
            gekozenLabel = label;
        }

        string? gekozenMakelaarId = null;
        if (MakelaarCombo.SelectedItem is Makelaar makelaar)
        {
            gekozenMakelaarId = makelaar.Id;
        }

        bool enkelTeKoop = EnkelTeKoopCheck.IsChecked == true;

        huidigeLijst = repository.GeefGefilterdePanden(gekozenLabel, gekozenMakelaarId, enkelTeKoop);

        GalerijPanel.Children.Clear();
        geselecteerdeKaart = null;

        foreach (Pand pand in huidigeLijst)
        {
            PandKaart kaart = new PandKaart(pand);
            kaart.Geklikt += Kaart_Geklikt;
            GalerijPanel.Children.Add(kaart);
        }

        // Als het pand dat voordien geselecteerd was er door het filter niet
        // meer bij staat, maken we de detailweergave leeg.
        if (geselecteerdPand != null)
        {
            bool nogAanwezig = false;
            foreach (Pand pand in huidigeLijst)
            {
                if (pand.Id == geselecteerdPand.Id)
                {
                    nogAanwezig = true;
                    break;
                }
            }

            if (!nogAanwezig)
            {
                geselecteerdPand = null;
            }
        }

        ToonDetail(geselecteerdPand);
    }

    /// <summary>
    /// Wordt aangeroepen wanneer de gebruiker op een woningkaart klikt.
    /// Werkt de selectiekleur bij en toont de details van het gekozen pand.
    /// </summary>
    private void Kaart_Geklikt(object? sender, Pand pand)
    {
        if (geselecteerdeKaart != null)
        {
            geselecteerdeKaart.ZetGeselecteerd(false);
        }

        geselecteerdeKaart = sender as PandKaart;
        geselecteerdeKaart?.ZetGeselecteerd(true);

        geselecteerdPand = pand;
        ToonDetail(pand);
    }

    /// <summary>
    /// Vult de detailweergave met de gegevens van het opgegeven pand (foto,
    /// GeefInfo()-tekst en de status van de knop "Markeer verkocht"). Als
    /// pand null is, wordt de detailweergave verborgen.
    /// </summary>
    private void ToonDetail(Pand? pand)
    {
        if (pand == null)
        {
            DetailPanel.Visibility = Visibility.Collapsed;
            GeenSelectieTekst.Visibility = Visibility.Visible;
            return;
        }

        GeenSelectieTekst.Visibility = Visibility.Collapsed;
        DetailPanel.Visibility = Visibility.Visible;

        DetailFoto.Source = FotoHelper.LaadAfbeelding(pand.FotoPad);
        DetailInfoTekst.Text = pand.GeefInfo();
        MarkeerVerkochtButton.IsEnabled = !pand.IsVerkocht;
    }

    /// <summary>
    /// Markeert het geselecteerde pand als verkocht (zowel in de databank als
    /// op het object), herlaadt de galerij, en selecteert het pand opnieuw
    /// zodat de detailweergave meteen "VERKOCHT" toont.
    /// </summary>
    private void MarkeerVerkochtButton_Click(object sender, RoutedEventArgs e)
    {
        if (geselecteerdPand == null)
        {
            return;
        }

        int idOmOpnieuwTeSelecteren = geselecteerdPand.Id;

        repository.MarkeerAlsVerkocht(geselecteerdPand);
        HerladenAanbod();

        foreach (object element in GalerijPanel.Children)
        {
            if (element is PandKaart kaart && kaart.Pand.Id == idOmOpnieuwTeSelecteren)
            {
                geselecteerdPand = kaart.Pand;
                geselecteerdeKaart = kaart;
                kaart.ZetGeselecteerd(true);
                ToonDetail(kaart.Pand);
                break;
            }
        }
    }

    /// <summary>
    /// Opent het venster "Nieuw pand toevoegen". Als de gebruiker daar een
    /// pand succesvol toevoegt, wordt het aanbod hier herladen.
    /// </summary>
    private void NieuwPandButton_Click(object sender, RoutedEventArgs e)
    {
        NieuwPandWindow venster = new NieuwPandWindow(repository);
        venster.Owner = this;
        bool? resultaat = venster.ShowDialog();

        if (resultaat == true)
        {
            VulMakelaarComboIn();
            HerladenAanbod();
        }
    }
}
