namespace CLImmo;

/// <summary>
/// Een vrijstaand of half-open/gesloten huis. Voegt aan Pand de
/// tuinoppervlakte toe, wat voor een appartement niet van toepassing is.
/// </summary>
public class Huis : Pand
{
    // Oppervlakte van de tuin in vierkante meter (0 als er geen tuin is).
    public double Tuinoppervlakte { get; set; }

    public Huis(string adres, double prijs, double oppervlakte, Energielabel energielabel,
        Makelaar makelaar, int bouwJaar, double tuinoppervlakte, string? fotoPad = null, bool isVerkocht = false)
        : base(adres, prijs, oppervlakte, energielabel, makelaar, bouwJaar, fotoPad, isVerkocht)
    {
        Tuinoppervlakte = tuinoppervlakte;
    }

    /// <summary>
    /// Vult de algemene pand-info aan met de tuinoppervlakte.
    /// </summary>
    public override string GeefInfo()
    {
        string tekst = base.GeefInfo();
        tekst += Environment.NewLine + "Tuinoppervlakte: " + Tuinoppervlakte.ToString("N0") + " m²";
        return tekst;
    }
}
