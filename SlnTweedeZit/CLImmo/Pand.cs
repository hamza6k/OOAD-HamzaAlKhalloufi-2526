namespace CLImmo;

/// <summary>
/// Abstracte basisklasse voor een onroerend goed dat via het immokantoor te koop
/// wordt aangeboden. Huis en Appartement erven hiervan over en voegen elk hun
/// eigen, specifieke kenmerken toe (tuinoppervlakte, lift, ...).
/// </summary>
public abstract class Pand
{
    // Id van het pand in de databank. -1 zolang het pand nog niet is opgeslagen
    // (bv. een nieuw pand dat net in de UI is aangemaakt).
    public int Id { get; set; } = -1;

    // Adres van het pand. Via de 'field'-keyword knippen we onbedoelde spaties
    // vooraan/achteraan automatisch weg, zonder een aparte backing field nodig
    // te hebben.
    public string Adres
    {
        get;
        set => field = value == null ? string.Empty : value.Trim();
    } = string.Empty;

    // Vraagprijs in euro. Via de 'field'-keyword zorgen we ervoor dat een
    // negatieve prijs (bv. door een programmeerfout elders) nooit in het domein
    // terechtkomt: een pand kan nu eenmaal geen negatieve waarde hebben.
    public double Prijs
    {
        get;
        set => field = value < 0 ? 0 : value;
    }

    // Bewoonbare oppervlakte in vierkante meter. Zelfde redenering als Prijs:
    // nooit negatief.
    public double Oppervlakte
    {
        get;
        set => field = value < 0 ? 0 : value;
    }

    public Energielabel Energielabel { get; set; }

    public Makelaar Makelaar { get; set; }

    // Bestandsnaam (of relatief pad) van de foto van het pand. Mag null of leeg
    // zijn: dan toont de UI de vervangafbeelding (geen-foto.png).
    public string? FotoPad { get; set; }

    public int BouwJaar { get; set; }

    // IsVerkocht kan van buitenaf enkel gelezen worden. De enige manier om een
    // pand als verkocht te markeren, is via de methode MarkeerAlsVerkocht():
    // zo kan een verkocht pand nooit per ongeluk terug "te koop" gezet worden.
    public bool IsVerkocht { get; private set; }

    /// <summary>
    /// Basisconstructor die door Huis en Appartement wordt aangeroepen via
    /// base(...). Vult alle gegevens in die elk pand gemeen heeft.
    /// </summary>
    protected Pand(string adres, double prijs, double oppervlakte, Energielabel energielabel,
        Makelaar makelaar, int bouwJaar, string? fotoPad, bool isVerkocht)
    {
        Adres = adres;
        Prijs = prijs;
        Oppervlakte = oppervlakte;
        Energielabel = energielabel;
        Makelaar = makelaar;
        BouwJaar = bouwJaar;
        FotoPad = fotoPad;
        IsVerkocht = isVerkocht;
    }

    /// <summary>
    /// Ouderdom van het pand in jaren, berekend t.o.v. het huidige jaar.
    /// </summary>
    public int Ouderdom
    {
        get { return DateTime.Now.Year - BouwJaar; }
    }

    /// <summary>
    /// Prijs per vierkante meter, gebruikt als extra vergelijkingsinfo.
    /// </summary>
    public double PrijsPerVierkanteMeter
    {
        get { return Prijs / Oppervlakte; }
    }

    /// <summary>
    /// Geeft de volledige detailtekst van het pand terug, zoals getoond in de
    /// detailweergave van het hoofdvenster. Huis en Appartement roepen deze
    /// methode op via base.GeefInfo() en voegen er hun eigen specifieke info
    /// aan toe.
    /// </summary>
    public virtual string GeefInfo()
    {
        string tekst = "Adres: " + Adres + Environment.NewLine;
        tekst += "Prijs: €" + Prijs.ToString("N0") + Environment.NewLine;
        tekst += "Oppervlakte: " + Oppervlakte.ToString("N0") + " m²" + Environment.NewLine;
        tekst += "Prijs per m²: €" + PrijsPerVierkanteMeter.ToString("N0") + Environment.NewLine;
        tekst += "Bouwjaar: " + BouwJaar + " (ouderdom: " + Ouderdom + " jaar)" + Environment.NewLine;
        tekst += "Energielabel: " + Energielabel + Environment.NewLine;
        tekst += "Makelaar: " + Makelaar.Naam + " (" + Makelaar.Telefoon + ", " + Makelaar.Email + ")";

        if (IsVerkocht)
        {
            tekst += Environment.NewLine + Environment.NewLine + "*** DIT PAND IS VERKOCHT ***";
        }

        return tekst;
    }

    /// <summary>
    /// Korte weergave van het pand, gebruikt op de woningkaart en in
    /// keuzelijsten.
    /// </summary>
    public override string ToString()
    {
        string tekst = Adres + " - €" + Prijs.ToString("N0") + " - " + Energielabel;

        if (IsVerkocht)
        {
            tekst += " [VERKOCHT]";
        }

        return tekst;
    }

    /// <summary>
    /// Markeert dit pand als verkocht. Eenmaal verkocht, kan een pand niet meer
    /// terug op "te koop" gezet worden.
    /// </summary>
    public void MarkeerAlsVerkocht()
    {
        IsVerkocht = true;
    }
}
