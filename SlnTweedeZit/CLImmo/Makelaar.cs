namespace CLImmo;

/// <summary>
/// Een makelaar van het immokantoor. Elke Pand wordt beheerd door precies één
/// makelaar. De UI gebruikt deze klasse ook om het aanbod op makelaar te filteren.
/// </summary>
public class Makelaar
{
    // De Id komt overeen met de primaire sleutel in de databank (bv. "MK001").
    // Zo kunnen we een pand dat we uit de databank lezen, koppelen aan het
    // juiste Makelaar-object zonder de naam als sleutel te moeten gebruiken.
    public string Id { get; set; }

    public string Naam { get; set; }

    public string Telefoon { get; set; }

    public string Email { get; set; }

    public Makelaar(string id, string naam, string telefoon, string email)
    {
        Id = id;
        Naam = naam;
        Telefoon = telefoon;
        Email = email;
    }

    /// <summary>
    /// Korte weergave van de makelaar, gebruikt in comboboxen/keuzelijsten.
    /// </summary>
    public override string ToString()
    {
        return Naam;
    }
}
