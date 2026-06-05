namespace CLHelpdesk;

/// <summary>
/// Stelt een ticket voor dat betrekking heeft op een hardwareprobleem.
/// </summary>
public class HardwareTicket : Ticket
{
    /// <summary>Naam of omschrijving van het betrokken toestel.</summary>
    public string Toestel { get; set; }

    /// <summary>
    /// Maakt een nieuwe instantie van HardwareTicket aan.
    /// </summary>
    public HardwareTicket() { }

    /// <summary>
    /// Geeft een gedetailleerde tekstweergave terug, inclusief het toestel.
    /// </summary>
    /// <returns>Een string met alle gegevens van het ticket en het toestel.</returns>
    public override string GeefInfo()
    {
        return $"{base.GeefInfo()}\nToestel: {Toestel}";
    }

    /// <summary>
    /// Geeft een korte weergave van het ticket terug voor gebruik in een ListBox.
    /// </summary>
    /// <returns>Een string in het formaat [Prioriteit] Titel - Voornaam Achternaam (Toestel).</returns>
    public override string ToString()
    {
        return $"[{Prioriteit}] {Titel} - {Melder.Voornaam} {Melder.Achternaam} ({Toestel})";
    }
}
