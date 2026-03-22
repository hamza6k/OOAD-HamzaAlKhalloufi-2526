using ConsoleKlassenOefenblad.Exercises.Classes;

namespace ConsoleKlassenOefenblad.Exercises;

internal class Ex01LegeClass
{
    public static void Run()
    {
        Console.WriteLine("Oefening 1: lege klasse");
        Console.WriteLine("-------------");
        // 1. maak in de map "Exercises/Classes" een lege klasse "Knikker", zonder properties of constructors

        // 2. maak een List van knikkers met de naam "potje"
        // ...
        List<Knikker> potje = new List<Knikker>();

        // 3. voeg met een lus 10 knikkers toe aan de lijst
        // ...
        for (int i = 0; i < 10; i++)
        {
            potje.Add(new Knikker());
        }

        // 4. toon hoeveel knikkers er in het potje zitten
        // ...
        Console.WriteLine($"er zitten {potje.Count} knikkers in het potje.");
    }
}
