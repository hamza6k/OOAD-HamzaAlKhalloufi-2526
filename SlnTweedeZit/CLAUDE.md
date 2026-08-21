# CLAUDE.md — Instructies voor de coding agent (OOAD-project Immokantoor)

Dit bestand bevat de vaste spelregels waaraan de agent (Claude Code) zich houdt bij het
bouwen van dit schoolproject. Het is het "agent-instructiebestand" waarnaar
`documentatie.md` verwijst.

## Projectcontext
- Schoolopdracht objectgeoriënteerd programmeren (OOAD), instapniveau C# / WPF (.NET 10).
- Alles wordt gebouwd in de bestaande map `SlnTweedeZit`.
- Bestaande bronbestanden (NIET vervangen door eigen dummy-data):
  - `SlnTweedeZit/ImmoDB.sql` — SQLite-schema + 20 testpanden + 5 makelaars.
  - `SlnTweedeZit/afbeeldingen immo.zip` — 18 woningfoto's die overeenkomen met de
    `foto`-kolom in de SQL-seeddata.

## Verplichte conventies (niet van afwijken)
- **Geen databinding**: geen `INotifyPropertyChanged`, geen `ObservableCollection<T>` als
  bindingsbron, geen `Binding` in XAML. Alles via code-behind (events, UI-elementen
  rechtstreeks vullen/uitlezen).
- **Geen LINQ**: gewone `foreach`-lussen en `if`-statements.
- **Geen `var`**: altijd expliciete types.
- Gebruik de `field`-keyword (C# 12) in properties waar zinvol, i.p.v. een aparte
  backing field.
- **Geen DataGrid**: gebruik `ItemsControl` of een `WrapPanel`/`StackPanel` met eigen
  `UserControl`s voor woningkaarten.
- Uitgebreide **Nederlandstalige comments** per klasse/methode.
- Lagenscheiding: domeinlogica in `CLImmo` (class library), UI-logica in `WpfImmo`.
- De app mag **nooit crashen** op een ontbrekend/ongeldig fotopad — altijd fallback naar
  een vervangafbeelding (`geen-foto.png`).

## Werkwijze
- Werken in fases (zie `documentatie.md` § Plan van aanpak).
- Na elke fase: korte samenvatting + gemaakte keuzes, dan wachten op expliciete
  goedkeuring ("ok, ga verder") vooraleer aan de volgende fase te beginnen.
- Tussentijds bouwen/testen met `dotnet build`.
- Elke fase en elke belangrijke beslissing wordt gedocumenteerd in `documentatie.md`,
  bijgewerkt naarmate het project vordert (niet pas op het einde).
- Bij onduidelijkheden (ontbrekende kolommen, ontbrekende data, ...) expliciet aan de
  gebruiker melden vóór verdergegaan wordt, in plaats van zelf te veronderstellen.
