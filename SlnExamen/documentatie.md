# Documentatie WpfHelpdesk

## Initiële prompt
Maak een nieuwe solution aan genaamd SlnExamen met twee projecten:
1. Een WPF App (.NET10) genaamd WpfHelpdesk
2. Een Class Library (.NET10) genaamd CLHelpdesk
Voeg een referentie toe van WpfHelpdesk naar CLHelpdesk.

## Plan van aanpak

- **Stap 1 — Project setup en solution structuur**
  Solution SlnExamen aanmaken via de .NET CLI, twee projecten toevoegen (WpfHelpdesk en CLHelpdesk) en een projectreferentie instellen van WpfHelpdesk naar CLHelpdesk.

- **Stap 2 — Class library aanmaken (enum, klassen, overerving)**
  In CLHelpdesk de enum `TicketPrioriteit` definiëren, de klasse `Medewerker` aanmaken met automatische properties, de abstracte basisklasse `Ticket` uitwerken met `GeefInfo()` en `ToString()`, en twee subklassen `HardwareTicket` en `SoftwareTicket` aanmaken die overerven van `Ticket` en `base.GeefInfo()` hergebruiken (DRY).

- **Stap 3 — CSV inlezen en opslaan**
  De klasse `TicketRepository` aanmaken met drie statische methodes: `LaadTickets` (StreamReader, kolomvalidatie, ParseExact, Enum.TryParse), `SlaTicketOp` (StreamWriter append) en `SlaAlleTicketsOp` (StreamWriter overschrijven voor updates). Een private hulpmethode `BouwCsvRegel` vermijdt herhaling.

- **Stap 4 — XAML venster bouwen**
  MainWindow.xaml opbouwen met een tweekoloms Grid. Linkerkolom: GroupBox Filters (cmbPrioriteit, cmbMelder, chkAlleenOpen) en GroupBox Tickets (lbxTickets). Rechterkolom: GroupBox Details (tbkDetails), GroupBox Nieuw ticket (txbTitel, cmbMelderNieuw, cmbPrioriteitNieuw, cmbType, txbExtraInfo, btnToevoegen) en btnAfsluiten.

- **Stap 5 — Code-behind logica**
  MainWindow.xaml.cs uitwerken: CSV laden via TicketRepository, comboboxen vullen met unieke melders, VulListBox() met drie filters via foreach, UpdateButtonStates(), eventhandlers voor selectie, filters, toevoegen en afsluiten. App.config toegevoegd voor het CSV-pad via ConfigurationManager.

- **Stap 6 — Validatie, commentaar en button states**
  Kolomvalidatie toegevoegd in LaadTickets (11 kolommen), btnToevoegen disabled als txbTitel leeg is (TextChanged event), btnAfsluiten disabled als geen ticket geselecteerd of al afgesloten, afzonderlijke MessageBox per ongeldig veld, XML-documentatiecommentaar op alle klassen en methodes in beide projecten.

## Gebruikte agents
- Claude (Anthropic) via VSCode extensie: voor alle codegeneratie

## Gespreksverloop

### Gesprek 1 — Project setup en class library
Solution SlnExamen aangemaakt met `dotnet new sln`, WpfHelpdesk (WPF App .NET 10) en CLHelpdesk (Class Library .NET 10) toegevoegd via `dotnet new wpf` en `dotnet new classlib`, projectreferentie ingesteld met `dotnet add reference`. Vervolgens in CLHelpdesk de enum `TicketPrioriteit` en klasse `Medewerker` aangemaakt in Medewerker.cs, de abstracte klasse `Ticket` met `GeefInfo()` en `ToString()` in Ticket.cs, en de subklassen `HardwareTicket` en `SoftwareTicket` elk in een apart bestand met override van beide methodes via `base.GeefInfo()`.

### Gesprek 2 — CSV logica
`TicketRepository` aangemaakt met `LaadTickets` (StreamReader, header overslaan, Split op puntkomma, DateTime.ParseExact met formaat `yyyy-MM-dd HHmm`, Enum.TryParse, object initializer voor Medewerker, try/catch met FileNotFoundException → IOException → Exception), `SlaTicketOp` (StreamWriter append) en `SlaAlleTicketsOp` (StreamWriter overschrijven, schrijft header + alle tickets). Gemeenschappelijke schrijflogica geëxtraheerd naar de private methode `BouwCsvRegel`.

### Gesprek 3 — XAML en code-behind
MainWindow.xaml opgebouwd met tweekoloms Grid, GroupBoxen, alle controls met x:Name, event-handlers gekoppeld in XAML en IsEnabled="False" op btnAfsluiten. NuGet-pakket `System.Configuration.ConfigurationManager` toegevoegd aan WpfHelpdesk, App.config aangemaakt met sleutel `CsvPad`. Code-behind geschreven: CSV laden in constructor, `VulMelderComboBoxes()` bouwt unieke melderlijst op zonder LINQ, `VulListBox()` past drie filters toe via foreach met continue, `UpdateButtonStates()` beheert beide knoppen, eventhandlers voor alle controls.

### Gesprek 4 — Afwerking en validatie
Kolomvalidatie toegevoegd in `LaadTickets` (regels met ≠ 11 kolommen worden overgeslagen). `UpdateButtonStates()` uitgebreid met beheer van `btnToevoegen` (disabled als txbTitel leeg is), `txbTitel_TextChanged` handler toegevoegd in XAML en code-behind. Validatie in `btnToevoegen_Click` toont per ongeldig veld een aparte MessageBox. XML-documentatiecommentaar (`/// <summary>`, `/// <param>`, `/// <returns>`) toegevoegd op alle klassen en methodes in CLHelpdesk en WpfHelpdesk. Inline commentaar toegevoegd bij complexe logica zoals de kolomcheck, datumparse en filterlogica.
