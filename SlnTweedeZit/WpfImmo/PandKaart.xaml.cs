using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CLImmo;

namespace WpfImmo;

/// <summary>
/// UserControl die één pand voorstelt als "woningkaart" in de galerij van het
/// hoofdvenster. Toont de foto (of vervangafbeelding), het adres, de prijs en
/// het energielabel. Verkochte panden krijgen een grijze overlay en een rode
/// "VERKOCHT"-band. Een klik op de kaart vuurt het event Geklikt af, zodat
/// MainWindow het pand kan selecteren en de detailweergave kan vullen.
/// </summary>
public partial class PandKaart : UserControl
{
    // Het pand dat deze kaart voorstelt.
    public Pand Pand { get; private set; }

    // Event dat MainWindow gebruikt om te weten wanneer deze kaart is aangeklikt.
    public event EventHandler<Pand>? Geklikt;

    public PandKaart(Pand pand)
    {
        InitializeComponent();
        Pand = pand;
        VulIn();
    }

    /// <summary>
    /// Vult alle UI-elementen van de kaart met de gegevens van Pand. Er wordt
    /// bewust geen Binding gebruikt: elk element wordt hier rechtstreeks
    /// vanuit code-behind ingesteld.
    /// </summary>
    private void VulIn()
    {
        FotoAfbeelding.Source = FotoHelper.LaadAfbeelding(Pand.FotoPad);
        AdresTekst.Text = Pand.Adres;
        PrijsTekst.Text = "€" + Pand.Prijs.ToString("N0");
        EnergielabelTekst.Text = "Energielabel: " + Pand.Energielabel;

        if (Pand.IsVerkocht)
        {
            GrijzeOverlay.Visibility = Visibility.Visible;
            VerkochtBand.Visibility = Visibility.Visible;
        }
        else
        {
            GrijzeOverlay.Visibility = Visibility.Collapsed;
            VerkochtBand.Visibility = Visibility.Collapsed;
        }
    }

    /// <summary>
    /// Toont of verbergt een blauwe selectierand rond de kaart. Wordt door
    /// MainWindow gebruikt om de huidig geselecteerde kaart visueel te
    /// markeren.
    /// </summary>
    public void ZetGeselecteerd(bool geselecteerd)
    {
        if (geselecteerd)
        {
            KaartBorder.BorderBrush = Brushes.DodgerBlue;
            KaartBorder.BorderThickness = new Thickness(3);
        }
        else
        {
            KaartBorder.BorderBrush = Brushes.LightGray;
            KaartBorder.BorderThickness = new Thickness(1);
        }
    }

    private void Kaart_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (Geklikt != null)
        {
            Geklikt(this, Pand);
        }
    }
}
