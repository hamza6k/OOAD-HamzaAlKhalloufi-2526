namespace CLImmo;

/// <summary>
/// Een appartement. Voegt aan Pand toe of er een lift aanwezig is in het
/// gebouw, wat voor een huis niet van toepassing is.
/// </summary>
public class Appartement : Pand
{
    public bool HeeftLift { get; set; }

    public Appartement(string adres, double prijs, double oppervlakte, Energielabel energielabel,
        Makelaar makelaar, int bouwJaar, bool heeftLift, string? fotoPad = null, bool isVerkocht = false)
        : base(adres, prijs, oppervlakte, energielabel, makelaar, bouwJaar, fotoPad, isVerkocht)
    {
        HeeftLift = heeftLift;
    }

    /// <summary>
    /// Vult de algemene pand-info aan met de aan-/afwezigheid van een lift.
    /// </summary>
    public override string GeefInfo()
    {
        string tekst = base.GeefInfo();

        if (HeeftLift)
        {
            tekst += Environment.NewLine + "Er is een lift aanwezig.";
        }
        else
        {
            tekst += Environment.NewLine + "Er is geen lift aanwezig.";
        }

        return tekst;
    }
}
