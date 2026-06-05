namespace CLHelpdesk;

/// <summary>
/// Stelt een ticket voor dat betrekking heeft op een softwareprobleem.
/// </summary>
public class SoftwareTicket : Ticket
{
    /// <summary>Naam van de betrokken applicatie.</summary>
    public string Applicatie { get; set; }

    /// <summary>
    /// Maakt een nieuwe instantie van SoftwareTicket aan.
    /// </summary>
    public SoftwareTicket() { }

    /// <summary>
    /// Geeft een gedetailleerde tekstweergave terug, inclusief de applicatie.
    /// </summary>
    /// <returns>Een string met alle gegevens van het ticket en de applicatie.</returns>
    public override string GeefInfo()
    {
        return $"{base.GeefInfo()}\nApplicatie: {Applicatie}";
    }

    /// <summary>
    /// Geeft een korte weergave van het ticket terug voor gebruik in een ListBox.
    /// </summary>
    /// <returns>Een string in het formaat [Prioriteit] Titel - Voornaam Achternaam (Applicatie).</returns>
    public override string ToString()
    {
        return $"[{Prioriteit}] {Titel} - {Melder.Voornaam} {Melder.Achternaam} ({Applicatie})";
    }
}
