namespace CLHelpdesk;

/// <summary>
/// Stelt een medewerker voor die tickets kan indienen.
/// </summary>
public enum TicketPrioriteit { Laag, Normaal, Hoog }

/// <summary>
/// Stelt een medewerker voor die tickets kan indienen.
/// </summary>
public class Medewerker
{
    /// <summary>Unieke identifier van de medewerker.</summary>
    public int Id { get; set; }

    /// <summary>Voornaam van de medewerker.</summary>
    public string Voornaam { get; set; }

    /// <summary>Achternaam van de medewerker.</summary>
    public string Achternaam { get; set; }

    /// <summary>Lijst van tickets ingediend door deze medewerker.</summary>
    public List<Ticket> Tickets { get; set; } = new List<Ticket>();

    /// <summary>
    /// Maakt een nieuwe instantie van Medewerker aan.
    /// </summary>
    public Medewerker() { }
}
