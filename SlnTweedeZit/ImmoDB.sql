-- ImmoDB.sql
-- SQLite-schema en testdata voor het immokantoor
-- Uitvoeren via: DB Browser for SQLite > Open Database > Execute SQL
-- Of via de CLI: sqlite3 immo.db < ImmoDB.sql
--
-- NOTA (OOAD-project, fase 4): de kolommen 'telefoon' en 'email' zijn t.o.v. het
-- oorspronkelijke bestand toegevoegd omdat de opgave een Makelaar-klasse met
-- Naam/Telefoon/Email vereist. De 5 bestaande makelaars (id's/namen) en alle
-- panden-data zijn ongewijzigd gebleven; enkel plausibele placeholder-waarden voor
-- telefoon/email zijn toegevoegd.

CREATE TABLE makelaars (
    id TEXT PRIMARY KEY,
    voornaam TEXT NOT NULL,
    achternaam TEXT NOT NULL,
    telefoon TEXT NOT NULL,
    email TEXT NOT NULL
);

CREATE TABLE panden (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    adres TEXT NOT NULL,
    makelaarId TEXT NOT NULL REFERENCES makelaars(id),
    prijs REAL NOT NULL,
    oppervlakte INTEGER NOT NULL,
    energielabel TEXT NOT NULL DEFAULT 'C',
    bouwjaar INTEGER NOT NULL,
    type TEXT NOT NULL,
    tuinoppervlakte INTEGER,
    heeftLift INTEGER,
    foto TEXT,
    isVerkocht INTEGER NOT NULL DEFAULT 0,
    datumVerkocht TEXT
);

INSERT INTO makelaars (id, voornaam, achternaam, telefoon, email) VALUES
    ('MK001', 'Sofie', 'Janssens', '0470 11 22 33', 'sofie.janssens@immokantoor.be'),
    ('MK002', 'Karel', 'Vermeulen', '0471 22 33 44', 'karel.vermeulen@immokantoor.be'),
    ('MK003', 'Nadia', 'El Amrani', '0472 33 44 55', 'nadia.elamrani@immokantoor.be'),
    ('MK004', 'Pieter', 'Declercq', '0473 44 55 66', 'pieter.declercq@immokantoor.be'),
    ('MK005', 'Lien', 'Maes', '0474 55 66 77', 'lien.maes@immokantoor.be');

-- 20 panden: 11 huizen, 9 appartementen; de laatste 2 hebben geen foto (foto = NULL)
INSERT INTO panden
    (adres, makelaarId, prijs, oppervlakte, energielabel, bouwjaar, type,
     tuinoppervlakte, heeftLift, foto, isVerkocht, datumVerkocht)
VALUES
    ('Parklaan 23, Gent', 'MK002', 549000, 210, 'A', 2020, 'Huis', 280, NULL, 'huis-modern.jpg', 0, NULL),
    ('Eikenlaan 8, Leuven', 'MK005', 495000, 190, 'B', 2017, 'Huis', 240, NULL, 'huis-modern2.jpg', 0, NULL),
    ('Dorpsstraat 41, Oudenaarde', 'MK003', 365000, 175, 'C', 1995, 'Huis', 600, NULL, 'huis-landelijk.jpg', 0, NULL),
    ('Veldweg 12, Geel', 'MK004', 339000, 165, 'C', 1990, 'Huis', 540, NULL, 'huis-landelijk2.jpg', 1, '2026-05-28T11:00:00'),
    ('Begijnhofstraat 5, Mechelen', 'MK001', 279000, 150, 'E', 1962, 'Huis', 110, NULL, 'huis-oud.jpg', 0, NULL),
    ('Statiestraat 77, Aalst', 'MK004', 189000, 95, 'F', 1948, 'Huis', 60, NULL, 'huis-arbeider.jpg', 0, NULL),
    ('Hoekstraat 2, Kortrijk', 'MK002', 315000, 160, 'C', 2001, 'Huis', 130, NULL, 'huis-hoek.jpg', 0, NULL),
    ('Lindendreef 19, Hasselt', 'MK005', 298000, 145, 'D', 1988, 'Huis', 120, NULL, 'huis-hoek2.jpg', 1, '2026-06-03T15:30:00'),
    ('Bosrand 7, Brasschaat', 'MK003', 695000, 260, 'A', 2019, 'Huis', 850, NULL, 'huis-vrijstaand.jpg', 0, NULL),
    ('Zeedijk 14, Knokke', 'MK001', 720000, 200, 'B', 2015, 'Huis', 150, NULL, 'huis-wit.jpg', 0, NULL),
    ('Stationsplein 30, Gent', 'MK002', 245000, 85, 'B', 2008, 'Appartement', NULL, 1, 'appartement-blok.jpg', 0, NULL),
    ('Kasteelstraat 11, Antwerpen', 'MK003', 329000, 110, 'D', 1925, 'Appartement', NULL, 0, 'appartement-herenhuis.jpg', 0, NULL),
    ('Nauwstraat 4, Leuven', 'MK005', 169000, 48, 'C', 1979, 'Appartement', NULL, 0, 'appartement-klein.jpg', 1, '2026-06-12T09:45:00'),
    ('Zeelaan 88, Oostende', 'MK001', 385000, 92, 'B', 2012, 'Appartement', NULL, 1, 'appartement-kust.jpg', 0, NULL),
    ('Albert I-promenade 5, Oostende', 'MK001', 419000, 96, 'A', 2018, 'Appartement', NULL, 1, 'appartement-kust2.jpg', 0, NULL),
    ('Meir 120, Antwerpen', 'MK003', 575000, 130, 'A', 2021, 'Appartement', NULL, 1, 'appartement-luxe.jpg', 0, NULL),
    ('Nieuwstraat 60, Mechelen', 'MK004', 289000, 78, 'A', 2023, 'Appartement', NULL, 1, 'appartement-nieuwbouw.jpg', 0, NULL),
    ('Ringlaan 45, Vilvoorde', 'MK004', 219000, 80, 'C', 2006, 'Appartement', NULL, 1, 'appartement-randstad.jpg', 0, NULL),
    ('Smidsestraat 9, Ninove', 'MK002', 235000, 130, 'D', 1985, 'Huis', 140, NULL, NULL, 0, NULL),
    ('Marktstraat 18, Sint-Niklaas', 'MK005', 199000, 72, 'C', 2003, 'Appartement', NULL, 0, NULL, 0, NULL);
