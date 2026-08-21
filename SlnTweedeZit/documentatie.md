# Documentatie — OOAD-project Immokantoor (SlnTweedeZit)

Dit document wordt bijgewerkt naarmate het project vordert (niet pas op het einde).

## 1. Agent-instructiebestand (CLAUDE.md)

Zie [`CLAUDE.md`](./CLAUDE.md) in deze map voor de volledige, letterlijke inhoud van het
instructiebestand dat de agent hanteert tijdens dit project.

## 2. Initiële prompt (letterlijk)

> Je bent mijn coding agent voor mijn OOAD-project in C# / WPF (.NET 10). Lees dit
> volledig en volg het strikt. Documenteer elke stap in documentatie.md (zie onderaan).
>
> ## WERKWIJZE
> Werk in duidelijke fases (zie "VOLGORDE VAN WERKEN" onderaan). Stop na elke fase, geef
> een korte samenvatting van wat je gedaan hebt en welke keuzes je maakte, en wacht op
> mijn "ok, ga verder" voor je aan de volgende fase begint. Bouw/test tussentijds met
> dotnet build.
>
> ## LOCATIE
> Alles moet in de bestaande map SlnTweedeZit terechtkomen. Daar staan al:
> - een sql-bestand (zoek zelf op welk bestand dit is en bekijk de structuur/inhoud
>   vooraleer je je databanklaag ontwerpt)
> - een map met woningfoto's (zoek zelf op hoe deze map heet en welke bestanden erin
>   zitten)
> Gebruik deze bestaande bestanden — genereer GEEN eigen dummy-foto's of eigen
> sql-seedbestand. Baseer je databankschema en je Pand-klasse op wat er werkelijk in het
> sql-bestand en de fotomap staat. Als iets onduidelijk is (bv. kolomnamen, ontbrekende
> foto's voor bepaalde panden), meld dat expliciet aan mij voor je verdergaat.
>
> ## CONTEXT / CONVENTIES (zeer belangrijk, wijk hier niet van af)
> Dit is een schoolopdracht. De code moet aansluiten bij een cursus objectgeoriënteerd
> programmeren met C#/WPF op instapniveau. Volg daarom strikt:
> - GEEN databinding (geen INotifyPropertyChanged, geen ObservableCollection<T> als
>   bindingsbron, geen Binding in XAML). Alles via code-behind: events, rechtstreeks
>   UI-elementen vullen/uitlezen.
> - GEEN LINQ. Gebruik gewone foreach-lussen en if's.
> - GEEN 'var'. Altijd expliciete types.
> - Gebruik de 'field' keyword (C# 12) in properties waar zinvol i.p.v. een aparte
>   backing field te declareren.
> - GEEN DataGrid. Gebruik ItemsControl of een WrapPanel/StackPanel met eigen
>   UserControls voor de woningkaarten.
> - Duidelijke, uitgebreide Nederlandstalige comments in de code (uitleg per
>   klasse/methode).
> - Nette lagenscheiding: alle domeinlogica in de class library (CLImmo), UI-logica in
>   WpfImmo.
>
> ## OPGAVE
> Bouw een eenvoudige toepassing voor een immokantoor:
> - Eigenaars geven hun woning in verkoop.
> - De makelaar kan het aanbod raadplegen, filteren, nieuwe panden registreren en een
>   pand als verkocht markeren.
>
> ## SOLUTION SETUP
> Maak in SlnTweedeZit een solution met 2 projecten:
> 1. **WpfImmo** — WPF App (.NET 10)
> 2. **CLImmo** — Class Library (.NET 10)
> WpfImmo verwijst naar CLImmo.
>
> ## DATABANK
> - Gebruik SQLite via het NuGet-package Microsoft.Data.Sqlite (geen aparte server
>   nodig).
> - Gebruik het bestaande sql-bestand in SlnTweedeZit als basis. Zorg dat het db-bestand
>   meegeleverd wordt met het WPF-project (Copy to Output Directory).
> - Gebruik de bestaande fotomap in SlnTweedeZit, gekoppeld aan WpfImmo (Copy to Output
>   Directory). Voorzie zelf één duidelijke vervangafbeelding (bv. "geen-foto.png") voor
>   panden zonder foto, mocht die nog niet bestaan.
> - BELANGRIJK: de app mag NOOIT crashen op een ontbrekend of ongeldig fotopad — altijd
>   fallback naar de vervangafbeelding.
>
> ## CLASS LIBRARY (CLImmo)
> Maak volgende types aan:
>
> ### enum Energielabel
> Waarden A, B, C, D, E, F, G (gebruikt voor filtering en weergave).
>
> ### klasse Makelaar
> Eenvoudige klasse met minstens: Naam, Telefoon, Email. Wordt gebruikt om aan een Pand
> te koppelen en om op te filteren.
>
> ### abstracte klasse Pand (basisklasse)
> Properties (met gepaste get/set, gebruik 'field' keyword waar relevant):
> - Adres, Prijs, Oppervlakte, Energielabel (enum), Makelaar (Makelaar), FotoPad (string,
>   mag null/leeg zijn), BouwJaar, IsVerkocht (bool)
> Constructor(s) die alle basisgegevens invullen.
> Berekende (read-only) properties:
> - Ouderdom (int) → huidig jaar - BouwJaar
> - PrijsPerVierkanteMeter (double) → Prijs / Oppervlakte
> Methodes:
> - virtual string GeefInfo() → volledige detailtekst voor de detailweergave, toont ook
>   duidelijk "VERKOCHT" als IsVerkocht true is.
> - override string ToString() → korte tekst voor woningkaart/keuzelijsten (bv. "Adres -
>   €Prijs - Energielabel [VERKOCHT]" indien van toepassing).
> - MarkeerAlsVerkocht() → zet IsVerkocht op true.
>
> ### klasse Huis : Pand
> Extra property: Tuinoppervlakte (double).
> Override GeefInfo() → roept base.GeefInfo() op en voegt tuinoppervlakte-info toe.
>
> ### klasse Appartement : Pand
> Extra property: HeeftLift (bool).
> Override GeefInfo() → roept base.GeefInfo() op en voegt lift-info toe.
>
> ### klasse PandValidator
> - Property MinimumAantalTekensAdres (int, bv. default 5).
> - bool IsGeldigAdres(string adres) → niet leeg, minstens MinimumAantalTekensAdres
>   tekens, mag enkel letters, cijfers, spaties en gewone leestekens (. , -) bevatten.
> - bool IsGeldigPrijs(decimal/double prijs) → strikt groter dan 0.
> - Gebruik deze validator verplicht vóór het toevoegen van een nieuw Pand in de UI.
>
> ## WPF APP (WpfImmo)
> Hoofdvenster (MainWindow), layout:
> 1. **Filterbalk (bovenaan)**: dropdown/combobox energielabel, dropdown/combobox
>    makelaar, checkbox "enkel te koop", knop "Nieuw pand toevoegen".
> 2. **Links: galerij** met woningkaarten (foto of vervangafbeelding, adres, prijs,
>    energielabel). Verkochte panden zijn visueel duidelijk anders (bv. rode band
>    "VERKOCHT", grijze overlay of vergelijkbaar). Klikken op een kaart selecteert het
>    pand.
> 3. **Rechts (volledige kolom)**: detailweergave van geselecteerd pand — grote foto,
>    alle info via GeefInfo(), knop "Markeer verkocht" (disabled indien pand al
>    verkocht).
>
> ### Venster "Nieuw pand toevoegen"
> Apart venster (Window), geopend via de knop in de filterbalk. Formulier met:
> - Keuze type (Huis/Appartement), adres, prijs, oppervlakte, bouwjaar, energielabel,
>   makelaar, tuinoppervlakte (indien huis) of lift (indien appartement), foto kiezen
>   (optioneel).
> Bij bevestigen:
> - Valideer via PandValidator, toon duidelijke foutmelding(en) bij ongeldige invoer
>   (geen crash, gewoon MessageBox of tekst in het venster).
> - Maak het juiste object aan (Huis of Appartement).
> - Sla het pand op in de SQLite-databank.
> - Sluit het venster en herlaad het overzicht in het hoofdvenster.
>
> ## DOCUMENTATIE
> Maak een documentatie.md aan met:
> 1. Inhoud van een CLAUDE.md/agent-instructiebestand dat je hanteert (zet dit ook
>    effectief als apart bestand in de repo).
> 2. De initiële prompt (deze volledige prompt, letterlijk).
> 3. Een kort plan van aanpak (stappen die je gaat volgen).
> 4. Welke AI/agent gebruikt is + een bondige samenvatting per gespreksstap/beslissing
>    die genomen is tijdens het bouwen.
> Werk documentatie.md bij naarmate je verder bouwt, niet pas op het einde in één keer.
>
> ## VOLGORDE VAN WERKEN (elke fase = 1 checkpoint, wacht op mijn goedkeuring)
> 1. Bestaand sql-bestand en fotomap in SlnTweedeZit inspecteren en rapporteren wat je
>    erin vindt.
> 2. Solution + projecten opzetten.
> 3. Class library volledig uitwerken (Pand, Huis, Appartement, Energielabel, Makelaar,
>    PandValidator) + korte consolentest of unit test om te bevestigen dat alles werkt.
> 4. SQLite-laag (databank koppelen, CRUD: ophalen/filteren, invoegen, markeren als
>    verkocht).
> 5. WPF hoofdvenster: galerij + filters + detailweergave.
> 6. WPF venster "nieuw pand toevoegen" + validatie + opslaan + herladen.
> 7. Foutafhandeling foto's (fallback-afbeelding) grondig testen.
> 8. documentatie.md afwerken.

## 3. Plan van aanpak

1. **Fase 1 — Inspectie**: bestaand `ImmoDB.sql` en `afbeeldingen immo.zip` bekijken,
   schema en fotonamen documenteren, onduidelijkheden melden.
2. **Fase 2 — Solution setup**: `SlnTweedeZit.sln` aanvullen met de projecten `CLImmo`
   (class library, .NET 10) en `WpfImmo` (WPF-app, .NET 10); projectreferentie leggen.
3. **Fase 3 — Class library**: `Energielabel`, `Makelaar`, `Pand` (abstract), `Huis`,
   `Appartement`, `PandValidator` uitwerken volgens de conventies (geen LINQ, geen var,
   field-keyword, Nederlandstalige comments). Korte consoletest/unit test.
4. **Fase 4 — SQLite-laag**: databankschema uitbreiden (telefoon/email voor makelaars),
   db-bestand meeleveren, repository-klasse(n) voor ophalen/filteren/invoegen/markeren
   als verkocht.
5. **Fase 5 — WPF hoofdvenster**: filterbalk, galerij met woningkaarten (UserControl),
   detailweergave, alles via code-behind.
6. **Fase 6 — WPF "nieuw pand toevoegen"**: apart venster, validatie via
   `PandValidator`, opslaan in DB, hoofdvenster herladen.
7. **Fase 7 — Fouttolerantie foto's**: vervangafbeelding `geen-foto.png` toevoegen,
   grondig testen op ontbrekende/ongeldige fotopaden.
8. **Fase 8 — Documentatie afwerken**: deze `documentatie.md` volledig maken.

## 4. Gebruikte AI / agent + logboek van beslissingen

**Gebruikte agent:** Claude Code (Sonnet 5), werkend volgens [`CLAUDE.md`](./CLAUDE.md)
in deze map.

### Fase 1 — Inspectie (2026-08-21)

- `SlnTweedeZit` bevat: `ImmoDB.sql`, `afbeeldingen immo.zip` (géén uitgepakte map, wél
  een zip), en een lege `SlnTweedeZit.sln` (nog zonder projecten).
- **`ImmoDB.sql`**: tabel `makelaars (id TEXT PK, voornaam, achternaam)` met 5 rijen
  (MK001–MK005), en tabel `panden (id, adres, makelaarId FK, prijs REAL, oppervlakte
  INTEGER, energielabel TEXT, bouwjaar INTEGER, type TEXT ['Huis'/'Appartement'],
  tuinoppervlakte INTEGER null, heeftLift INTEGER null, foto TEXT null, isVerkocht
  INTEGER, datumVerkocht TEXT null)` met 20 testpanden (11 huizen, 9 appartementen). De
  laatste 2 panden hebben bewust `foto = NULL` — goede testcase voor de fallback-foto.
- **`afbeeldingen immo.zip`**: bevat 18 .jpg-bestanden waarvan de namen exact overeenkomen
  met de niet-NULL `foto`-kolom in de SQL-data. Geen ontbrekende foto's voor panden die
  er wel een verwachten.
- **Onduidelijkheid gemeld aan gebruiker**: de opgave vraagt `Telefoon` en `Email` op de
  `Makelaar`-klasse, maar de SQL-tabel `makelaars` heeft die kolommen niet.
  **Beslissing (gebruiker gekozen):** het schema uitbreiden met kolommen `telefoon` en
  `email`, ingevuld met plausibele placeholder-gegevens per makelaar. De bestaande 5
  makelaars (id's/namen) blijven ongewijzigd.
- `CLAUDE.md` (agent-instructiebestand) en dit `documentatie.md` aangemaakt in
  `SlnTweedeZit/`.

### Fase 2 — Solution + projecten opzetten (2026-08-21)

- `CLImmo` (class library, `net10.0`) en `WpfImmo` (WPF-app, `net10.0-windows`) aangemaakt
  via `dotnet new` en toegevoegd aan de bestaande `SlnTweedeZit.sln`.
- Projectreferentie gelegd: `WpfImmo` → `CLImmo`.
- Default gegenereerd bestand `CLImmo/Class1.cs` verwijderd (leeg, niet nodig).
  `WpfImmo` behoudt de standaard WPF-startbestanden (`App.xaml`, `MainWindow.xaml`, ...)
  die in Fase 5/6 verder ingevuld worden.
- Beide csproj's gebruiken standaard `Nullable=enable`, `ImplicitUsings=enable`. Geen
  expliciete `LangVersion` nodig: `net10.0` gebruikt standaard C# 14, waarin de
  `field`-keyword stabiel beschikbaar is (nodig voor Fase 3).
- `dotnet build SlnTweedeZit.sln` slaagt zonder warnings/errors.

### Fase 3 — Class library CLImmo (2026-08-21)

- **`Energielabel.cs`**: enum met waarden A t.e.m. G.
- **`Makelaar.cs`**: `Id, Naam, Telefoon, Email` (allemaal auto-properties, geen
  bijzondere logica nodig), constructor die alles invult, `ToString()` geeft de naam
  terug (voor gebruik in comboboxen).
- **`Pand.cs`** (abstract): `Id, Adres, Prijs, Oppervlakte, Energielabel, Makelaar,
  FotoPad, BouwJaar, IsVerkocht`. De `field`-keyword wordt bewust op 3 plaatsen gebruikt
  waar een custom setter zinvol is zonder een aparte backing field te moeten
  declareren:
  - `Adres`: trimt automatisch spaties vooraan/achteraan.
  - `Prijs` en `Oppervlakte`: klemmen een negatieve waarde af naar 0 (verdedigt het
    domeinmodel tegen ongeldige data, los van de UI-validatie).
  - `IsVerkocht` heeft een `private set`: enkel `MarkeerAlsVerkocht()` kan dit op
    `true` zetten, nooit terug op `false`.
  - Berekende properties `Ouderdom` en `PrijsPerVierkanteMeter`.
  - `GeefInfo()` (virtual), `ToString()` (override), `MarkeerAlsVerkocht()`.
- **`Huis.cs`** / **`Appartement.cs`**: erven over van `Pand`, voegen resp.
  `Tuinoppervlakte` en `HeeftLift` toe, en overriden `GeefInfo()` met
  `base.GeefInfo() + eigen info`.
- **`PandValidator.cs`**: `MinimumAantalTekensAdres` (default 5), `IsGeldigAdres`
  (niet leeg, min. lengte, enkel letters/cijfers/spaties/`. , -`), `IsGeldigPrijs`
  (strikt > 0).
- **Test**: tijdelijk consoleprojectje (buiten de solution, in de scratchpad-map, dus
  geen 3de project toegevoegd aan `SlnTweedeZit.sln`) dat Huis, Appartement, ToString,
  GeefInfo, MarkeerAlsVerkocht, PandValidator en het `field`-keyword-gedrag
  (trimmen/negatieve waarden afklemmen) doorloopt. Alle output klopte; nadien
  opgeruimd.
- `dotnet build CLImmo\CLImmo.csproj` slaagt zonder warnings/errors.

### Fase 4 — SQLite-laag (2026-08-21)

- **Schema uitgebreid**: `SlnTweedeZit/ImmoDB.sql` aangepast conform de eerder gekozen
  optie — kolommen `telefoon` en `email` toegevoegd aan `makelaars`, met plausibele
  placeholder-gegevens per bestaande makelaar (id's/namen/panden-data ongewijzigd). Een
  duidelijke NOTA bovenaan het bestand legt deze wijziging uit.
- **NuGet**: `Microsoft.Data.Sqlite` (v10.0.11) toegevoegd aan `CLImmo.csproj`.
- **Foto's**: `afbeeldingen immo.zip` uitgepakt naar `WpfImmo/Afbeeldingen/` (18 originele
  foto's, ongewijzigd).
- **Vervangafbeelding**: `WpfImmo/Afbeeldingen/geen-foto.png` zelf gegenereerd (grijze
  huis-silhouet-placeholder met tekst "Geen foto beschikbaar"), via een tijdelijk
  hulpprogramma (System.Drawing.Common), nadien opgeruimd.
- **Databankbestand**: `WpfImmo/Data/ImmoDB.db` opgebouwd door het (bijgewerkte)
  `ImmoDB.sql`-script uit te voeren tegen een nieuw SQLite-bestand, via een tijdelijk
  hulpprogramma met Microsoft.Data.Sqlite (nadien opgeruimd). Bevat 5 makelaars + 20
  panden zoals verwacht.
- **`WpfImmo.csproj`**: `Data\ImmoDB.db` en alles in `Afbeeldingen\` krijgen
  `CopyToOutputDirectory = PreserveNewest`. Build-verificatie bevestigt dat beide
  correct in de output directory terechtkomen.
- **`CLImmo/PandRepository.cs`**: nieuwe klasse, enige plaats in het project die
  rechtstreeks SQL uitvoert (UI roept enkel deze methodes aan):
  - `GeefAlleMakelaars()`
  - `GeefAllePanden()` / `GeefGefilterdePanden(Energielabel?, string? makelaarId, bool enkelTeKoop)`
    — WHERE-clausule wordt dynamisch opgebouwd met geparametriseerde SQL (geen
    string-concatenatie van gebruikersinvoer, ter voorkoming van SQL-injectie).
  - `VoegPandToe(Pand pand)` — herkent Huis/Appartement via `is`-pattern matching, vult
    `Id` van het object na insert.
  - `MarkeerAlsVerkocht(Pand pand)` — zet zowel het object (`Pand.MarkeerAlsVerkocht()`)
    als de databankrij (`isVerkocht`, `datumVerkocht`) in één keer bij.
  - Geen LINQ: makelaar-koppeling via een eigen `foreach`-lus (`ZoekMakelaar`), met een
    defensieve "onbekende makelaar" fallback bij inconsistente data (kan in de praktijk
    niet voorkomen met de huidige seeddata, maar voorkomt een crash).
- **Test**: tijdelijk consoleprojectje (buiten de solution) tegen een kopie van de echte
  db bevestigde: alle 5 makelaars + 20 panden correct geladen met juiste
  type/foto/VERKOCHT-status, filter op energielabel A → 5 resultaten, filter "enkel te
  koop" → 17 (20 - 3 verkocht), filter op makelaar MK002 → 4, nieuw pand toevoegen → Id
  21 toegekend en meteen zichtbaar bij herladen, markeren als verkocht → correct
  weerspiegeld na herladen vanuit de databank. Nadien opgeruimd.

### Fase 5 — WPF hoofdvenster (2026-08-21)

- **`FotoHelper.cs`** (WpfImmo): laadt een foto op basis van bestandsnaam uit
  `Afbeeldingen/`, met fallback naar `geen-foto.png` bij null/leeg pad, ontbrekend
  bestand of eender welke laadfout (try/catch, geeft nooit een uitzondering door).
- **`PandKaart.xaml(.cs)`**: UserControl voor één woningkaart (foto, adres, prijs,
  energielabel), volledig via code-behind gevuld (geen Binding). Verkochte panden
  krijgen een grijze overlay + rode "VERKOCHT"-band. Vuurt een `Geklikt`-event af bij
  een muisklik; `ZetGeselecteerd(bool)` toont/verbergt een blauwe selectierand.
- **`MainWindow.xaml(.cs)`**: filterbalk (energielabel-combo, makelaar-combo, "enkel te
  koop"-checkbox, knop "Nieuw pand toevoegen"), galerij in een `WrapPanel` binnen een
  `ScrollViewer`, en een detailkolom rechts (foto, `GeefInfo()`-tekst, knop "Markeer
  verkocht", disabled bij een reeds verkocht pand). Elke filterwijziging herlaadt het
  aanbod via `PandRepository.GeefGefilterdePanden`. Klikken op een kaart selecteert het
  pand en vult de detailweergave. De knop "Nieuw pand toevoegen" toont voorlopig een
  placeholder-MessageBox — het echte venster volgt in Fase 6.
- **Test**: de app effectief opgestart en via een screenshot gecontroleerd: filterbalk,
  gevulde galerij (foto, prijs, energielabel per kaart) en de tekst "Selecteer een
  pand..." rechts (geen detail zichtbaar bij opstart, zoals verwacht) waren correct
  zichtbaar. Geen opstartfouten. Testinstantie nadien afgesloten.
- `dotnet build SlnTweedeZit.sln` slaagt zonder warnings/errors.

### Fase 6 — Venster "Nieuw pand toevoegen" (2026-08-21)

- **`NieuwPandWindow.xaml(.cs)`**: apart venster met keuze Huis/Appartement
  (RadioButtons, tonen/verbergen tuinoppervlakte- resp. liftveld), adres, prijs,
  oppervlakte, bouwjaar, energielabel-combo, makelaar-combo, en een optionele
  foto-kiezer (`Microsoft.Win32.OpenFileDialog`) die de gekozen foto kopieert naar
  `Afbeeldingen/` zodat ze meteen weergegeven kan worden.
- **Validatie**: verplicht via `PandValidator` (adres, prijs); oppervlakte/bouwjaar
  worden met `double.TryParse`/`int.TryParse` gecontroleerd (geen crash bij
  niet-numerieke invoer); alle fouten worden verzameld en samen als rode tekst in het
  venster getoond (geen popup nodig). Bij een mislukte fotokopie (bv. bestand in
  gebruik) valt de app terug op "geen foto" i.p.v. te crashen. Pas bij volledig
  geldige invoer wordt het Huis/Appartement-object aangemaakt.
- Bij bevestigen: `PandRepository.VoegPandToe(...)` slaat het nieuwe pand op, het
  venster sluit met `DialogResult = true`.
- **`MainWindow.NieuwPandButton_Click`**: placeholder vervangen door het echt openen
  van `NieuwPandWindow` (`ShowDialog`); bij `DialogResult == true` worden de
  makelaar-combo en de galerij herladen, zodat het nieuwe pand meteen zichtbaar is.
- `dotnet build SlnTweedeZit.sln` slaagt zonder warnings/errors.
