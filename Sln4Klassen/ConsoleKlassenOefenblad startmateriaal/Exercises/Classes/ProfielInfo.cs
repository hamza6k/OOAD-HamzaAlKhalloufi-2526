using System.ComponentModel;

namespace ConsoleKlassenOefenblad.Exercises.Classes
{
    public class ProfielInfo
    {
        // Properties (verplichte info)
        public int Id { get; set; }
        public string Gebruikersnaam { get; set; }
        public string Email { get; set; }
        public DateTime AanmaakDatum { get; private set; }

        // Properties (optionele info)
        public string Voornaam { get; set; } = "";
        public string Achternaam { get; set; } = "";
        public string Biografie { get; set; } = "";
        public string Website { get; set; } = "";
        public bool IsPubliek { get; set; } = true;

        // Berekende properties
        public bool IsVolledig
        {
            get
            {
                return !string.IsNullOrEmpty(Voornaam)
                    && !string.IsNullOrEmpty(Achternaam)
                    && !string.IsNullOrEmpty(Biografie)
                    && !string.IsNullOrEmpty(Website);
            }
        }

        // Verplichte constructor — minimale gegevens om een geldig profiel te maken
        // ...
        public ProfielInfo(int id, string gebruikersnaam, string email)
        {
            Id = id;
            Gebruikersnaam = gebruikersnaam;
            Email = email;
            AanmaakDatum = DateTime.Now;
        }

        // Uitgebreide constructor — verplichte én optionele gegevens in één keer
        // ...
        public ProfielInfo(int id, string gebruikersnaam, string email, string voornaam, string achternaam, string biografie, string website, bool isPubliek)
            : this(id, gebruikersnaam, email)
        {
            Voornaam = voornaam;
            Achternaam = achternaam;
            Biografie = biografie;
            Website = website;
            IsPubliek = isPubliek;
        }

        // ToString override
        public override string ToString()
        {
            return $"{Gebruikersnaam} — {(IsPubliek ? "publiek" : "privé")}"; 
        }
    }
}













class Werknemer
{
    // 1. Voeg properties toe: Id, Naam, Salaris, InDienstSinds
    public int Id { get; set; }
    public string Naam { get; set; }

    private decimal _salaris;
    public decimal Salaris
    {
        get { return _salaris; }
        set
        {
            if (value < 0)
                throw new ArgumentException("Salaris kan niet negatief zijn");
            _salaris = value;
        }
    }

    private DateOnly _inDienstSinds;
    public DateOnly InDienstSinds
    {
        get { return _inDienstSinds; }
        set
        {
            if (value > DateOnly.FromDateTime(DateTime.Now))
                throw new ArgumentException("Datum Indiensttreding kan niet in de toekomst liggen");

        }
    }


}



class Nummer
{
    public string Titel { get; set; } = "onbekend nummer";
    private int _duurInSeconden;
    public int DuurInSeconden
    {
        get { return _duurInSeconden; }
        set
        {
            if (value <= 0)
                throw new ArgumentException("Duur moet groter zijn dan nul");
            _duurInSeconden = value;
        }
    }
    public string DuurAlsTekst
    {
        get
        {
            int minuten = DuurInSeconden / 60;
            int seconden = DuurInSeconden % 60;
            return $"{minuten} : {seconden:D2}";
        }
    }

    public Nummer(int duurInSeconden)
    {
        DuurInSeconden = duurInSeconden;
    }


    public Nummer(string titel, int minuten, int seconden) : this(minuten * 60 + seconden)
    {
        Titel = titel;
    }

    public override string ToString()
    {
        return $"{Titel} ({DuurAlsTekst})";
    }
}

class Album
{
    public string Titel { get; set; } = "onbekend album";
    public int Jaar { get; set; }
    public List<Nummer> Nummers { get; set; } = new List<Nummer>();

    public Album(string titel, int jaar)
    {
        Titel = titel;
        Jaar = jaar;
    }

    public bool IsEp
    {
        get { return Nummers.Count <= 4; }
    }

    public void VoegNummerToe(Nummer n)
    {
        Nummers.Add(n);
    }

    public override string ToString()
    {
        string type = IsEp ? "EP" : "LP";
        return $"{Titel} ({Jaar}) | {Nummers.Count} nummer(s) | {type}";
    }
}







