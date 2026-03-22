using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleKaartspel.Classes;

public class Kaart
{
    private int _nummer;
    private char _kleur;

    public int Nummer
    {
        get { return _nummer; }
        set
        {
            if (value < 1 || value > 13)
                throw new ArgumentException("Nummer moet tussen 1 en 13 liggen.");
            _nummer = value;
        }
    }

    public char Kleur
    {
        get { return _kleur; }
        set
        {
            if (value != 'C' && value != 'S' && value != 'H' && value != 'D')
                throw new ArgumentException("Kleur moet C, S, H of D zijn.");
            _kleur = value;
        }
    }

    public Kaart(int nummer, char kleur)
    {
        Nummer = nummer;
        Kleur = kleur;
    }
}