using ConsoleStaticEnumOefenblad.Exercises.Classes;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace ConsoleStaticEnumOefenblad.Exercises;

internal class Ex02StaticMethode
{
    public static void Run()
    {
        Console.WriteLine("\nOefening 2: couponcodes controleren");
        Console.WriteLine("-------------");

        // 1. Maak in "Exercises/Classes" een klasse "CouponCode":
        //   - private static string _couponRegex = @"^[A-Z]{3}\d{2}-[A-Z]{2}$";
        //   - property "Code" van type string.
        //   - property "IsGeldig" van type bool met alleen getter:
        //     gebruik Regex.IsMatch(...) om te controleren of Code geldig is
        //   - constructor met één parameter
        //
        // 2. Voeg vervolgens statische methode IsGeldig(string code) toe, die toelaat een gegeven code te controleren
        //
        // 3. Voeg tenslotte nog een statische methode Beschrijf(string code) toe: 
        //   - als de code geldig is, geef je een tekst terug in dit formaat:
        //     "Prefix=ABC, Nummer=12, Regio=DE"
        //   - als de code ongeldig is, geef je "ongeldige code" terug.

        // Testcode (haal uit commentaar):

        //string[] codes = { "ABC12-DE", "AB12-DE", "XYZ99-BE" };
        //Console.WriteLine("\ntesten IsGeldig() methode:\n");
        //foreach (string code in codes)
        //{
        //    Console.WriteLine($"Code {code} is {(CouponCode.ControleerCode(code) ? "geldig" : "ongeldig")}");
        //}
        //Console.WriteLine("\ntesten Beschrijf() methode:\n");
        //foreach (string code in codes) 
        //{
        //    Console.WriteLine($"Code {code}: {CouponCode.Beschrijf(code)}");
        //}

 class CouponCode
 {
            
    private static string _couponRegex = @"^[A-Z]{3}\d{2}-[A-Z]{2}$";
    public string Code { get; set; }
    public bool IsGeldig 
    { 
        get { return Regex.IsMatch(Code, _couponRegex);  }
    }

    public CouponCode(string code)
    {
        Code = code;
    }
        public static bool ControleerCode(string code)
        {
            CouponCode cc = new CouponCode(code);
            return cc.IsGeldig;
        }

        public static string Beschrijf(string code)
        {
            Match match = Regex.Match(code, @"^[A-Z]{3}\d{2}-[A-Z]{2}$");
            if (!match.Success)
            {
                return "ongeldige code";
            }
            string prefix = match.Groups[1].Value;
            string nummer = match.Groups[2].Value;
            string regio = match.Groups[3].Value;
            return $"Prefix={prefix}, Nummer={nummer}, Regio={regio}";
        }

 }

    



}


abstract class Voertuig
{
    public string Merk { get; set; }
    public decimal DagPrijs { get; set; }
    public abstract int MaxPassagiers { get; }

    public Voertuig(string merk, decimal dagPrijs)
    {
        Merk = merk;
        DagPrijs = dagPrijs;
    }

    public decimal BerekenHuurprijs(int dagen)
    {
        return DagPrijs + dagen;
    }

    public override string ToString()
    {
        return $"{Merk} (max {MaxPassagiers} passagier(s)) - €{DagPrijs:F2}/dag";
    }
}

class Auto : Voertuig
{
    public int AantalDeuren { get; set; }
    public override int MaxPassagiers => 5;
    public Auto(string merk, decimal dagPrijs, int aantalDeuren) : base(merk, dagPrijs)
    {
        AantalDeuren = aantalDeuren;
    }
    public override string ToString()
    {
        return $"{{base.ToString()}} — {{AantalDeuren}} deuren";
    }
}

class Bestelwagen : Voertuig
{
    public double LaadruimteM3 { get; set; }
    public override int MaxPassagiers => 2;

    public Bestelwagen(string merk, decimal dagPrijs, double laadruimteM3) : base(merk, dagPrijs)
    {
        LaadruimteM3 = laadruimteM3;
    }
    public override string ToString()
    {
        return $"{base.ToString()} — {LaadruimteM3:F1} m³ laadruimte";
    }
}