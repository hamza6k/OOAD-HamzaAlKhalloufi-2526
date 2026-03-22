using ConsoleKlassenOefenblad.Exercises.Classes;

namespace ConsoleKlassenOefenblad.Exercises;

internal class Ex02Properties
{
    public static void Run()
    {
        Console.WriteLine("\nOefening 2: properties, standaardwaarden, object initializer syntax");
        Console.WriteLine("-------------");
        // 1. maak in "Exercises/Classes" een klasse "Recept" met volgende properties:
        //   - Titel
        //   - Rating (int)
        //   - IsVegetarisch (standaardwaarde is false)
        //   - Ingredienten (List van strings, standaard lege lijst)

        // 2. maak volgend recept aan met de lege constructor (... = new Recept()) en stel dan één voor één de properties in:
        //   - Pasta Carbonara (Rating 4, Ingrediënten: Pasta, Eieren, Spek, Parmezaanse kaas)
        // ...

        Recept pasta = new Recept();
        pasta.Titel = "Pasta Carbonara";
        pasta.Rating = 4;
        pasta.Ingredienten.Add("Pasta");
        pasta.Ingredienten.Add("Eieren");
        pasta.Ingredienten.Add("Spek");
        pasta.Ingredienten.Add("Parmezaanse kaas");

        // 3. maak volgende recepten aan met de object initializer syntax:
        //   - Lasagne (Rating 5, IsVegetarisch false, Ingrediënten: Lasagnebladen, Tomatensaus, Courgette, Aubergine, Mozzarella)
        //   - Salade Niçoise (Rating 4, Ingrediënten: Sla, Tonijn, Eieren, Pindakaas, Olijven, Tomaten)
        // ...

        Recept Lasagne = new Recept
        {
            Titel = "Lasagne",
            Rating = 5,
            IsVegetarisch = false,
            Ingredienten = new List<string>
            {
                "Lasagnebladen",
                "Tomatensaus",
                "courgette",
                "Aubergine",
                "Mozzarella",

            }


        };

        Recept salade = new Recept
        {
            Titel = "Salade Niçoise",
            Rating = 4,
            Ingredienten = new List<string>
            {
                "sla",
                "Tonijnen",
                "Eieren",
                "Pindakaas",
                "Olijven",
                "Tomaten"
            }
        };

        // 4. pas het recept van de salade niçoise aan:
        //  - verwijder de pindakaas
        //  - zet IsVegetarisch op false
        // ...

        salade.Ingredienten.Remove("Pindakaas");

        salade.IsVegetarisch = false;

        // 5. maak een lijst "kookboek" aan en voeg de drie recepten toe
        // ...

        List<Recept> kookboek = new List<Recept>();
        kookboek.Add(pasta);
        kookboek.Add(Lasagne);
        kookboek.Add(salade);

        // 6. toon het aantal vegetarische recepten (zie screenshot) en de gemiddelde rating
        // ...
        
        int aantalVeggie = kookboek.Count(r => r.IsVegetarisch);
        double gemiddeldeRating = kookboek.Average(r => r.Rating);

        Console.WriteLine($"Aantal vegetarische recepten: {aantalVeggie}");
        Console.WriteLine($"Gemiddelde rating: {gemiddeldeRating}");




    }
}
