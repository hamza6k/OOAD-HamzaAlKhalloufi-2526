namespace CLImmo;

/// <summary>
/// Controleert of de gegevens die een gebruiker invoert voor een nieuw pand
/// geldig zijn, vóór er een Huis/Appartement-object van gemaakt wordt. Wordt
/// verplicht gebruikt in de UI vooraleer een nieuw pand wordt toegevoegd.
/// </summary>
public class PandValidator
{
    // Minimum aantal tekens dat een geldig adres moet bevatten.
    public int MinimumAantalTekensAdres { get; set; } = 5;

    /// <summary>
    /// Een adres is geldig als het niet leeg is, minstens
    /// MinimumAantalTekensAdres tekens bevat, en enkel bestaat uit letters,
    /// cijfers, spaties en de gewone leestekens punt, komma en koppelteken.
    /// </summary>
    public bool IsGeldigAdres(string adres)
    {
        if (string.IsNullOrWhiteSpace(adres))
        {
            return false;
        }

        string getrimdAdres = adres.Trim();

        if (getrimdAdres.Length < MinimumAantalTekensAdres)
        {
            return false;
        }

        foreach (char teken in getrimdAdres)
        {
            bool isGeldigTeken = char.IsLetterOrDigit(teken)
                || teken == ' '
                || teken == '.'
                || teken == ','
                || teken == '-';

            if (!isGeldigTeken)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Een prijs is enkel geldig als ze strikt groter is dan 0.
    /// </summary>
    public bool IsGeldigPrijs(double prijs)
    {
        return prijs > 0;
    }
}
