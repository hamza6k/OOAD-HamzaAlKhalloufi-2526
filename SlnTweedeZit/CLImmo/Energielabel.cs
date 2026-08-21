namespace CLImmo;

/// <summary>
/// Het energielabel van een pand, van A (zeer energiezuinig) tot G (weinig
/// energiezuinig). Dit label wordt gebruikt bij het weergeven van een pand
/// (GeefInfo/ToString) en bij het filteren van het aanbod in de UI.
/// </summary>
public enum Energielabel
{
    A,
    B,
    C,
    D,
    E,
    F,
    G
}
