using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleKaartspel.Classes;

public class Speler
{
    public string Naam { get; set; }
    public List<Kaart> Kaarten { get; set; }
    public bool HeeftNogKaarten { get { return Kaarten.Count > 0; } }

    public Speler(string naam)
    {
        Naam = naam;
        Kaarten = new List<Kaart>();
    }

    public Speler(string naam, List<Kaart> kaarten)
        : this(naam)
    {
        Kaarten = kaarten;
    }

    public Kaart LegKaart()
    {
        Random rng = new Random();
        int index = rng.Next(Kaarten.Count);
        Kaart kaart = Kaarten[index];
        Kaarten.RemoveAt(index);
        return kaart;
    }
}