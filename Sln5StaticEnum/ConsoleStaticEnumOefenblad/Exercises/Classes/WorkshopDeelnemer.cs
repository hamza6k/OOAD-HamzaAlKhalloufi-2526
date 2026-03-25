namespace ConsoleStaticEnumOefenblad.Exercises.Classes;

internal class WorkshopDeelnemer
{
    public string Naam { get; set; }
    public bool IsAanwezig { get; private set; }

    public WorkshopDeelnemer(string naam, bool isAanwezig)
    {
        Naam = naam;
        IsAanwezig = isAanwezig;
    }
}