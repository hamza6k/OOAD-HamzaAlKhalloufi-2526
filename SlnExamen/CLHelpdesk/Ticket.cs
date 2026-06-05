namespace CLHelpdesk;

/// <summary>
/// Abstracte basisklasse voor alle soorten helpdesk-tickets.
/// </summary>
public abstract class Ticket
{
    /// <summary>Unieke identifier van het ticket.</summary>
    public int Id { get; set; }

    /// <summary>Korte omschrijving van het probleem.</summary>
    public string Titel { get; set; }

    /// <summary>De medewerker die het ticket heeft ingediend.</summary>
    public Medewerker Melder { get; set; }

    /// <summary>Prioriteit van het ticket.</summary>
    public TicketPrioriteit Prioriteit { get; set; }

    /// <summary>Geeft aan of het ticket afgesloten is.</summary>
    public bool IsAfgesloten { get; set; }

    /// <summary>Datum en tijdstip waarop het ticket aangemaakt werd.</summary>
    public DateTime DatumAangemaakt { get; set; }

    /// <summary>Datum en tijdstip waarop het ticket afgesloten werd, of null als het nog open is.</summary>
    public DateTime? DatumAfgesloten { get; set; }

    /// <summary>
    /// Maakt een nieuwe instantie van Ticket aan.
    /// </summary>
    public Ticket() { }

    /// <summary>
    /// Geeft een gedetailleerde tekstweergave van het ticket terug.
    /// </summary>
    /// <returns>Een string met alle gegevens van het ticket.</returns>
    public virtual string GeefInfo()
    {
        return $"Id: {Id}\nTitel: {Titel}\nMelder: {Melder.Voornaam} {Melder.Achternaam}\nPrioriteit: {Prioriteit}\nAfgesloten: {IsAfgesloten}\nAangemaakt: {DatumAangemaakt:dd/MM/yyyy}\nAfgesloten op: {(DatumAfgesloten.HasValue ? DatumAfgesloten.Value.ToString("dd/MM/yyyy") : "/")}";
    }

    /// <summary>
    /// Geeft een korte weergave van het ticket terug voor gebruik in een ListBox.
    /// </summary>
    /// <returns>Een string in het formaat [Prioriteit] Titel - Voornaam Achternaam.</returns>
    public override string ToString()
    {
        return $"[{Prioriteit}] {Titel} - {Melder.Voornaam} {Melder.Achternaam}";
    }
}
