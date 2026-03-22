using ConsoleKaartspel.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleKaartspel.Classes;

public class Deck
{
    public List<Kaart> Kaarten { get; set; }

    public Deck()
    {
        Kaarten = new List<Kaart>();
        char[] kleuren = { 'C', 'S', 'H', 'D' };

        for (int k = 0; k < kleuren.Length; k++)
        {
            for (int nummer = 1; nummer <= 13; nummer++)
            {
                Kaarten.Add(new Kaart(nummer, kleuren[k]));
            }
        }
    }

    public void Schudden()
    {
        Random rng = new Random();
        for (int i = Kaarten.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            Kaart temp = Kaarten[i];
            Kaarten[i] = Kaarten[j];
            Kaarten[j] = temp;
        }
    }

    public Kaart NeemKaart()
    {
        Kaart kaart = Kaarten[0];
        Kaarten.RemoveAt(0);
        return kaart;
    }
}
