# Dokterspraktijk – WPF Applicatie

**Project:** Dokterspraktijk – WPF applicatie in C# .NET 10  
**IDE:** Visual Studio 2022

---

## Coderegels – volg deze strikt

### VERBODEN – gebruik NOOIT:
- `var`
- LINQ
- `async`/`await`
- Databinding
- `DataGrid`, `GridView`, `ListView`
- Tuples
- `dynamic`
- Expando objecten
- `invoke`
- Structs
- Type switches
- User controls
- `out` parameters
- Case guards

### VERPLICHT:
- Gebruik altijd expliciete types (bv. `string`, `int`, `List<Patient>`)
- Alle SQL-queries horen uitsluitend in de Class Library (`DokterspraktijkLib`), nooit in de WPF-projecten
- CRUD-methodes horen in de modelklassen zelf, geen aparte datalaag
- Gebruik `try-catch` voor alle database- en bestandsoperaties in de WPF-projecten
- Toon fouten altijd in een `TextBlock`, nooit in een `MessageBox`
- Gebruik `Frame` en `Page` controls voor navigatie
- Gebruik SHA256 voor wachtwoordhashing
- Gebruik voor elke property een private variabele met een publieke property (geen auto-properties)
- Voeg commentaar toe aan elke methode en elk niet-triviaal codeblok
- Volg C# naamgevingsconventies: PascalCase voor klassen/methodes, camelCase voor lokale variabelen

---

## Architectuur

De solution bevat 3 projecten:

| Project | Type | Beschrijving |
|---|---|---|
| `DokterspraktijkLib` | Class Library | Alle modelklassen, SQL-queries, business logica |
| `WPFDokter` | WPF App | Interface voor de dokter |
| `WPFPatient` | WPF App | Interface voor de patiënt |

- `DokterspraktijkLib` wordt gerefereerd door zowel `WPFDokter` als `WPFPatient`
- **Database:** `DokterspraktijkDB` op `.\SQLEXPRESS`

---

## Database kolomnamen – gebruik deze exact in alle SQL

### Tabel `Dokter`
| Kolom | Beschrijving |
|---|---|
| `id` | Primary key |
| `voornaam` | Voornaam |
| `achternaam` | Achternaam |
| `gsm` | GSM-nummer |
| `email` | E-mailadres |
| `paswoord` | Gehashed wachtwoord (SHA256) |
| `profielfotodata` | Profielfoto als binaire data |
| `rizivnummer` | RIZIV-nummer |
| `isgeconventioneerd` | Boolean: geconventioneerd of niet |

### Tabel `Patient`
| Kolom | Beschrijving |
|---|---|
| `id` | Primary key |
| `voornaam` | Voornaam |
| `achternaam` | Achternaam |
| `geslacht` | Geslacht |
| `gsm` | GSM-nummer |
| `email` | E-mailadres |
| `paswoord` | Gehashed wachtwoord (SHA256) |
| `geboortedatum` | Geboortedatum |
| `profielfotodata` | Profielfoto als binaire data |
| `notificaties` | Notificatie-instellingen |

### Tabel `Afspraak`
| Kolom | Beschrijving |
|---|---|
| `id` | Primary key |
| `moment` | Datum en tijdstip van de afspraak |
| `klacht` | Beschrijving van de klacht |
| `patient_id` | Foreign key naar `Patient` |
| `dokter_id` | Foreign key naar `Dokter` |
