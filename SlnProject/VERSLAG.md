# Verslag – Dokterspraktijk WPF Applicatie

**Student:** Hamza AlKhalloufi  
**Datum:**  22/05/2026
**Vak:**  OOAD

---
## 1. Agent Instruction File (CLAUDE.md)

De volgende instructies werden opgeslagen in CLAUDE.md in de root van het project:

- Verboden technieken: var, LINQ, async/await, databinding, DataGrid, ListView, tuples, dynamic, MessageBox
- Expliciete types verplicht
- Alle SQL uitsluitend in DokterspraktijkLib
- CRUD-methodes in de klassen zelf
- Try-catch in WPF-projecten, fouten in TextBlock
- Frame en Page voor navigatie
- SHA256 voor wachtwoordhashing
- Private variabelen met publieke getter/setter
- Commentaar op elke methode
- Database: DokterspraktijkDB op .\HRMDDB
---

## 2. Initiële Prompt

Ik studeer eerste jaar Toegepaste Informatica aan Odisee en werk aan een project voor het vak OOAD. Ik moet een WPF-applicatie maken in C# .NET 10 voor een dokterspraktijk. De applicatie bestaat uit drie projecten: een class library (DokterspraktijkLib), een WPF-app voor dokters (WPFDokter) en een WPF-app voor patiënten (WPFPatient).

De database DokterspraktijkDB draait al op .\hrmddb met drie tabellen: Dokter, Patient en Afspraak. De kolomnamen zijn exact vastgelegd.

Verboden technieken zijn: var, LINQ, async/await, databinding, DataGrid, ListView, tuples, dynamic en MessageBox voor fouten. Alle SQL hoort uitsluitend in de class library. Navigatie via Frame en Page controls. Wachtwoorden worden gehasht via SHA256. Fouten worden getoond in een TextBlock. Patiëntenkaartjes worden dynamisch aangemaakt in code-behind via een WrapPanel.

---

## 3. Plan van Aanpak

De AI-agent werd gevraagd om eerst een plan van aanpak op te stellen voordat er code geschreven werd.

### Fase 1 – Voorbereiding
- Agent instruction file (CLAUDE.md) aanmaken
- Solution aanmaken met drie projecten
- NuGet-pakketten installeren
- App.config configureren met connection string

### Fase 2 – Class Library
- Superklasse Gebruiker aanmaken met SHA256 hashing
- Klasse Dokter aanmaken met CRUD-methodes
- Enum Notificaties aanmaken (Geen, Mail, Sms, Beide)
- Klasse Patient aanmaken met volledige CRUD
- Klasse Afspraak aanmaken
- XML-commentaar toevoegen aan alle klassen

### Fase 3 – WPFDokter
- MainWindow met navigatiepanel en Frame
- StartPage en LoginPage met SHA256 login
- AfsprakenPage met Calendar en ListBox
- PatiëntenOverzichtPage met dynamische kaartjes
- CRUD-pagina's: Details, Wijzigen, Verwijderen

### Fase 4 – WPFPatient
- MainWindow, StartPage en LoginPage
- AfsprakenOverzichtPage en NieuweAfspraakPage
- ProfielInfoPage en ProfielBewerkenPage

### Fase 5 – Afwerking
- Try-catch controleren
- Verboden technieken controleren
- Commentaar toevoegen
- Testen en debuggen

---



## 4. Best Practices

**Gebruikte AI tools**
Voor dit project heb ik twee AI tools gebruikt:

1. **Claude Code** – rechtstreeks geïntegreerd in Visual Studio via de Claude Code Extension. Dit is een echte AI-agent die code kan lezen, schrijven en aanpassen in het project. Claude Code werd gebruikt voor alle code-gerelateerde taken.

2. **ChatGPT** – enkel gebruikt voor taal en spelling. Mijn prompts werden eerst door ChatGPT gehaald om ze duidelijker en professioneler te formuleren voordat ik ze naar Claude Code stuurde. ChatGPT heeft op geen enkel moment code gegenereerd of het project beïnvloed.

---

