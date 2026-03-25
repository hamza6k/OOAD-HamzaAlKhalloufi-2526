using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleKlassenOefenblad.Exercises.Classes
{
    internal class Werknemer
    {
        public int Id { get; set; }
        public string Naam { get; set; }

        private decimal salaris;

        public decimal Salaris
        {
            get { return salaris; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Salaris kan niet negatief zijn");
                salaris = value;

            }
        }

        private DateOnly inDienstSinds;

        public DateOnly InDienstSinds
        {
            get { return inDienstSinds; }
            set
            {
                if (value > DateOnly.FromDateTime(DateTime.Now))
                    throw new ArgumentException("Datum indiensttreding kan niet in de toekomst liggen");

                inDienstSinds = value;

            }
        }
        
        public void GeefOpslag(decimal percentage)
        {
            Salaris += Salaris * percentage / 100;
        }

        public int Ancienniteit
        {
            get
            {
                return DateTime.Now.Year - InDienstSinds.Year;
            }
        }

        public string Seniority
        {
            get
            {
                if (Ancienniteit < 2)
                    return "Junior";
                else if (Ancienniteit < 5)
                    return "Medior";
                else
                    return "Senior";
            }
        }
    }
}
