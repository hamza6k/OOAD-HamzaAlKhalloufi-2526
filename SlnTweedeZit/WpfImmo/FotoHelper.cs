using System.IO;
using System.Windows.Media.Imaging;

namespace WpfImmo;

/// <summary>
/// Hulpklasse die woningfoto's laadt vanuit de map "Afbeeldingen" naast de
/// uitvoerbare applicatie. Dit is een puur technisch/UI-detail (bestandspaden,
/// WPF-afbeeldingen), en hoort daarom in WpfImmo thuis, niet in CLImmo.
///
/// BELANGRIJK: deze klasse mag NOOIT een uitzondering laten ontsnappen. Als een
/// fotopad ontbreekt, leeg is, niet bestaat, of het bestand corrupt/ongeldig is,
/// wordt altijd teruggevallen op de vervangafbeelding "geen-foto.png".
/// </summary>
public static class FotoHelper
{
    private const string VervangAfbeeldingBestandsnaam = "geen-foto.png";

    // Map waarin alle woningfoto's (en de vervangafbeelding) terechtkomen na
    // het builden/publiceren van WpfImmo (zie WpfImmo.csproj: CopyToOutputDirectory).
    private static string AfbeeldingenMap
    {
        get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Afbeeldingen"); }
    }

    /// <summary>
    /// Laadt de foto voor het opgegeven fotopad (enkel de bestandsnaam, zoals
    /// opgeslagen in de databank). Bij eender welk probleem (null/leeg pad,
    /// ontbrekend bestand, ongeldige/corrupte afbeelding) wordt de
    /// vervangafbeelding teruggegeven in plaats van een uitzondering te gooien.
    /// </summary>
    public static BitmapImage LaadAfbeelding(string? fotoPad)
    {
        string volledigPad = Path.Combine(AfbeeldingenMap, VervangAfbeeldingBestandsnaam);

        if (!string.IsNullOrWhiteSpace(fotoPad))
        {
            string kandidaatPad = Path.Combine(AfbeeldingenMap, fotoPad);

            if (File.Exists(kandidaatPad))
            {
                volledigPad = kandidaatPad;
            }
        }

        try
        {
            BitmapImage afbeelding = new BitmapImage();
            afbeelding.BeginInit();
            afbeelding.CacheOption = BitmapCacheOption.OnLoad;
            afbeelding.UriSource = new Uri(volledigPad, UriKind.Absolute);
            afbeelding.EndInit();
            return afbeelding;
        }
        catch
        {
            // Zelfs de vervangafbeelding kon niet geladen worden (bv. bestand
            // ontbreekt of is beschadigd). Geef een leeg beeld terug zodat de
            // applicatie zeker nooit crasht op een fotoprobleem.
            return new BitmapImage();
        }
    }
}
