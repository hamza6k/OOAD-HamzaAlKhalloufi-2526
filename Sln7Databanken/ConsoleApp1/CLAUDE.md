# Project: Dokterspraktijk – WPF applicatie in C# .NET 10

**IDE:** Visual Studio 2022

---

## CODEREGELS – volg deze strikt

- Gebruik **NOOIT**: `var`, LINQ, `async`/`await`, databinding, `DataGrid`, `GridView`, `ListView`, tuples, `dynamic`, expando objecten, `invoke`, structs, type switches, user controls, `out` parameters, case guards
- Gebruik altijd **expliciete types** (bv. `string`, `int`, `List<Patient>`)
- Alle SQL-queries horen **uitsluitend in de Class Library** (`DokterspraktijkLib`), nooit in de WPF-projecten
- **CRUD-methodes horen in de modelklassen zelf**, geen aparte datalaag
- Gebruik `try-catch` voor alle database- en bestandsoperaties; toon fouten in een `TextBlock`, **nooit** in een `MessageBox`
- Gebruik `Frame` en `Page` controls voor navigatie
- Gebruik **SHA256** voor wachtwoordhashing
- Voeg **commentaar** toe aan elke methode en elk niet-triviaal codeblok
- Volg C# naamgevingsconventies: **PascalCase** voor klassen/methodes, **camelCase** voor lokale variabelen

---

## ARCHITECTUUR

De solution bevat 3 projecten:

| Project | Type | Rol |
|---|---|---|
| `DokterspraktijkLib` | Class Library | Modelklassen, CRUD-methodes, alle SQL-queries |
| `WPFDokter` | WPF App | UI voor de dokter |
| `WPFPatient` | WPF App | UI voor de patiënt |

- `DokterspraktijkLib` wordt gerefereerd door zowel `WPFDokter` als `WPFPatient`
- **Database:** `DokterspraktijkDB` op `.\SQLEXPRESS`
