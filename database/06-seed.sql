-- =====================================================================
--  Demo podaci za lokalni razvoj, testove i snimak aplikacije.
--
--  Nalozi (BCrypt):
--    administrator  admin  / Admin123!   (i: marija)
--    zaposleni      radnik / Radnik123!  (i: jelena, emir, ivana, milica, dario, sanja, petra)
--    kupac          kupac  / Kupac123!   (i: sara, nikola, lejla, marko, ana, dejan, tea)
--
--  Racuni za posljednjih 90 dana prolaze kroz pravu proceduru narucivanje,
--  pa trigeri sami skidaju zalihe, racunaju iznose i pune istoriju zaliha.
-- =====================================================================

SET NAMES utf8mb4;

-- ---------------------------------------------------------------------
--  Adrese, prodavnice, proizvodjaci
-- ---------------------------------------------------------------------
INSERT INTO adresa (idADRESA, Grad, Ulica, Drzava) VALUES
  (1,  'Banja Luka', 'Kralja Petra I 12', 'BiH'),
  (2,  'Sarajevo', 'Ferhadija 5', 'BiH'),
  (3,  'Novi Sad', 'Zmaj Jovina 21', 'Srbija'),
  (4,  'Mostar', 'Rondo 3', 'BiH'),
  (5,  'Banja Luka', 'Bulevar Cara Dušana 2', 'BiH'),
  (6,  'Beograd', 'Knez Mihailova 40', 'Srbija'),
  (7,  'Koprivnica', 'Ante Starčevića 32', 'Hrvatska'),
  (8,  'Parma', 'Via Mantova 166', 'Italija'),
  (9,  'Bremen', 'Langemarckstrasse 4', 'Njemačka'),
  (10, 'Beograd', 'Bulevar Mihajla Pupina 6', 'Srbija'),
  (11, 'Bihać', 'Bihaćkih branitelja 20', 'BiH'),
  (12, 'Bačka Topola', 'Industrijska zona bb', 'Srbija'),
  (13, 'Banja Luka', 'Vidovdanska 8', 'BiH'),
  (14, 'Sarajevo', 'Zmaja od Bosne 44', 'BiH'),
  (15, 'Trebinje', 'Njegoševa 14', 'BiH'),
  (16, 'Novi Sad', 'Bulevar oslobođenja 77', 'Srbija'),
  (17, 'Mostar', 'Maršala Tita 120', 'BiH'),
  (18, 'Doboj', 'Kralja Dragutina 9', 'BiH'),
  (19, 'Tuzla', 'Turalibegova 15', 'BiH'),
  (20, 'Prijedor', 'Nikole Pašića 3', 'BiH');

INSERT INTO prodavnica (idProdavnica, Ime, ADRESA_idADRESA) VALUES
  (1, 'Centar Banja Luka', 1),
  (2, 'Sarajevo Ferhadija', 2),
  (3, 'Novi Sad Promenada', 3),
  (4, 'Mostar Rondo', 4);

INSERT INTO proizvodjac (idPROIZVODJAC, Ime, Email, ADRESA_idADRESA) VALUES
  (1, 'Vitinka', 'info@vitinka.ba', 5),
  (2, 'Bambi', 'kontakt@bambi.rs', 6),
  (3, 'Podravka', 'info@podravka.hr', 7),
  (4, 'Barilla', 'info@barilla.it', 8),
  (5, 'Milka', 'service@milka.de', 9),
  (6, 'Grand Kafa', 'info@grandkafa.rs', 10),
  (7, 'Meggle', 'info@meggle.ba', 11),
  (8, 'Marbo', 'info@marbo.rs', 12);

-- ---------------------------------------------------------------------
--  Katalog: isti proizvodi (isti EAN-13) u svakoj prodavnici,
--  sa cijenom i zalihama koje se razlikuju po prodavnici
-- ---------------------------------------------------------------------
CREATE TEMPORARY TABLE katalog (
  id INT PRIMARY KEY, naziv VARCHAR(45), barkod VARCHAR(45), proizvodjac INT,
  opis VARCHAR(255), cijena DECIMAL(10,2), rok_dana INT, kategorija VARCHAR(45)
);

INSERT INTO katalog VALUES
  (1,  'Mineralna voda 1.5l',     '3870000000013', 1, 'Prirodna mineralna voda',       1.20, 400, 'Piće'),
  (2,  'Mineralna voda 0.5l',     '3870000000020', 1, 'Prirodna mineralna voda',       0.80, 400, 'Piće'),
  (3,  'Sok od narandže 1l',      '3870000000037', 1, '100% voćni sok',                2.90, 200, 'Piće'),
  (4,  'Plazma keks 300g',        '8600000000011', 2, 'Keks sa vitaminima',            3.10, 300, 'Slatkiši'),
  (5,  'Plazma mljevena 300g',    '8600000000028', 2, 'Za kolače',                     3.80, 300, 'Slatkiši'),
  (6,  'Vegeta 500g',             '3850000000015', 3, 'Dodatak jelima',                5.40, 700, 'Začini'),
  (7,  'Vegeta 250g',             '3850000000022', 3, 'Dodatak jelima',                3.10, 700, 'Začini'),
  (8,  'Spaghetti n.5 500g',      '8000000000019', 4, 'Tjestenina od durum pšenice',   2.60, 500, 'Tjestenina'),
  (9,  'Penne rigate 500g',       '8000000000026', 4, 'Tjestenina od durum pšenice',   2.70, 500, 'Tjestenina'),
  (10, 'Čokolada mliječna 100g',  '4000000000013', 5, 'Alpsko mlijeko',                2.25, 250, 'Slatkiši'),
  (11, 'Čokolada lješnik 100g',   '4000000000020', 5, 'Sa cijelim lješnicima',         2.60, 250, 'Slatkiši'),
  (12, 'Kafa mljevena 200g',      '8600000000035', 6, 'Pržena mljevena kafa',          4.95, 365, 'Kafa'),
  (13, 'Jogurt 1l',               '3870000000044', 7, 'Tečni jogurt 2.8% m.m.',        1.85, 21,  'Mliječni'),
  (14, 'Čips slani 150g',         '8600000000042', 8, 'Krompir čips',                  2.40, 180, 'Grickalice');

INSERT INTO kategorija (Naziv) SELECT DISTINCT kategorija FROM katalog;

SET @razlog_zaliha = 'Početno stanje';
INSERT INTO proizvod (Naziv, DatumIsteka, BarKod, PROIZVODJAC_idPROIZVODJAC, Opis, Zalihe, MinZalihe, Cijena, PRODVNICA_idProdavnica)
SELECT k.naziv,
       CURDATE() + INTERVAL k.rok_dana DAY,
       k.barkod,
       k.proizvodjac,
       k.opis,
       60 + MOD(k.id * 37 + s.idProdavnica * 23, 140),
       CASE WHEN k.cijena > 4 THEN 8 ELSE 15 END,
       ROUND(k.cijena * (1 + (s.idProdavnica - 1) * 0.03), 2),
       s.idProdavnica
FROM katalog k
CROSS JOIN prodavnica s
ORDER BY s.idProdavnica, k.id;
SET @razlog_zaliha = NULL;

INSERT INTO kategorija_has_proizvod (Kategorija_Naziv, PROIZVOD_idPROIZVOD)
SELECT k.kategorija, p.idPROIZVOD
FROM proizvod p JOIN katalog k ON k.barkod = p.BarKod;

DROP TEMPORARY TABLE katalog;

-- ---------------------------------------------------------------------
--  Korisnici
-- ---------------------------------------------------------------------
SET @hash_admin  = '$2a$10$JiBMEXrmsh6JFGj1nZO3aeOq2N8wxjQVIqe7XUw8LXPOnsvjwEOEu';
SET @hash_radnik = '$2a$10$XtK2bQL04qcTgKanFC0sDutflz10ieRZb/ekCzi8kwuErrP41/8qS';
SET @hash_kupac  = '$2a$10$EJ3hBMm3vXzM7qyMYf7UQul5R6wCTBfHIQqE8tugnMVqRjQfFVrs2';

INSERT INTO administrator (Ime, Prezime, Email, Lozinka, KorisnickoIme) VALUES
  ('Ana', 'Kovačević', 'ana.kovacevic@prodavnica.ba', @hash_admin, 'admin'),
  ('Marija', 'Lukić', 'marija.lukic@prodavnica.ba', @hash_admin, 'marija');

INSERT INTO zaposleni (Ime, Prezime, Email, BrojTelefona, Prodavnica_idProdavnica, Plata, Lozinka, KorisnickoIme) VALUES
  ('Marko', 'Jović', 'marko.jovic@prodavnica.ba', '065 111 222', 1, 1450.00, @hash_radnik, 'radnik'),
  ('Jelena', 'Savić', 'jelena.savic@prodavnica.ba', '065 333 444', 1, 1300.00, @hash_radnik, 'jelena'),
  ('Emir', 'Hodžić', 'emir.hodzic@prodavnica.ba', '061 555 666', 2, 1350.00, @hash_radnik, 'emir'),
  ('Ivana', 'Perić', 'ivana.peric@prodavnica.ba', '062 121 343', 2, 1280.00, @hash_radnik, 'ivana'),
  ('Milica', 'Popov', 'milica.popov@prodavnica.rs', '063 777 888', 3, 1400.00, @hash_radnik, 'milica'),
  ('Dario', 'Knežević', 'dario.knezevic@prodavnica.rs', '064 232 454', 3, 1250.00, @hash_radnik, 'dario'),
  ('Sanja', 'Marić', 'sanja.maric@prodavnica.ba', '063 909 101', 4, 1320.00, @hash_radnik, 'sanja'),
  ('Petra', 'Zovko', 'petra.zovko@prodavnica.ba', '063 404 505', 4, 1240.00, @hash_radnik, 'petra');

INSERT INTO kupac (KorisnickoIme, Lozinka, Email, ADRESA_idADRESA) VALUES
  ('kupac', @hash_kupac, 'petar@mail.com', 13),
  ('sara', @hash_kupac, 'sara@mail.com', 14),
  ('nikola', @hash_kupac, 'nikola@mail.com', 15),
  ('lejla', @hash_kupac, 'lejla@mail.com', 17),
  ('marko', @hash_kupac, 'marko@mail.com', 16),
  ('ana', @hash_kupac, 'ana@mail.com', 18),
  ('dejan', @hash_kupac, 'dejan@mail.com', 20),
  ('tea', @hash_kupac, 'tea@mail.com', 19);

INSERT INTO dodavanjeproizvoda (PROIZVOD_idPROIZVOD, ZAPOSLENI_idZAPOSLENI, Datum)
SELECT p.idPROIZVOD, z.idZAPOSLENI, CURDATE() - INTERVAL 95 DAY
FROM proizvod p
JOIN zaposleni z ON z.Prodavnica_idProdavnica = p.PRODVNICA_idProdavnica
WHERE z.idZAPOSLENI IN (1, 3, 5, 7);

-- ---------------------------------------------------------------------
--  Akcije i kuponi: aktivni, najavljeni i istekli
-- ---------------------------------------------------------------------
INSERT INTO popust (idPopust, Popust, Start, End) VALUES
  (1, 20, CURDATE() - INTERVAL 20 DAY, CURDATE() + INTERVAL 10 DAY),
  (2, 15, CURDATE() - INTERVAL 5 DAY,  CURDATE() + INTERVAL 25 DAY),
  (3, 10, CURDATE() + INTERVAL 7 DAY,  CURDATE() + INTERVAL 21 DAY),
  (4, 25, CURDATE() - INTERVAL 70 DAY, CURDATE() - INTERVAL 50 DAY);

INSERT INTO proizvod_has_popust (PROIZVOD_idPROIZVOD, Popust_idPopust)
SELECT idPROIZVOD, 1 FROM proizvod WHERE BarKod = '4000000000013'
UNION ALL SELECT idPROIZVOD, 2 FROM proizvod WHERE BarKod = '8000000000019' AND PRODVNICA_idProdavnica IN (1, 2)
UNION ALL SELECT idPROIZVOD, 3 FROM proizvod WHERE BarKod = '8600000000035' AND PRODVNICA_idProdavnica = 1
UNION ALL SELECT idPROIZVOD, 4 FROM proizvod WHERE BarKod = '8600000000011';

INSERT INTO kupon (idKupon, Sifra, Start, end, popust) VALUES
  (1, 'JESEN15', CURDATE() - INTERVAL 25 DAY, CURDATE() + INTERVAL 35 DAY, 15),
  (2, 'KAFA10',  CURDATE() - INTERVAL 10 DAY, NULL, 10),
  (3, 'LJETO30', CURDATE() - INTERVAL 120 DAY, CURDATE() - INTERVAL 60 DAY, 30);

INSERT INTO proizvod_has_kupon (PROIZVOD_idPROIZVOD, Kupon_idKupon)
SELECT idPROIZVOD, 1 FROM proizvod WHERE BarKod IN ('8600000000011', '3870000000037', '8600000000042')
UNION ALL SELECT idPROIZVOD, 2 FROM proizvod WHERE BarKod = '8600000000035'
UNION ALL SELECT idPROIZVOD, 3 FROM proizvod WHERE BarKod = '3870000000013';

-- ---------------------------------------------------------------------
--  Racuni za posljednjih 90 dana kroz proceduru narucivanje.
--  SET TIMESTAMP pomjera NOW()/CURDATE() sesije, pa racun, cijene sa akcijom
--  i istorija zaliha dobiju datum kupovine.
-- ---------------------------------------------------------------------
DROP PROCEDURE IF EXISTS seed_racuni;

DELIMITER ;;
CREATE PROCEDURE seed_racuni()
BEGIN
    DECLARE i INT DEFAULT 1;
    DECLARE j INT;
    DECLARE vBroj INT DEFAULT 160;
    DECLARE vDanas DATE DEFAULT CURDATE();
    DECLARE vDana INT;
    DECLARE vKupac INT;
    DECLARE vProdavnica INT;
    DECLARE vStavki INT;
    DECLARE vProizvod INT;
    DECLARE vKolicina INT;
    DECLARE vStavke JSON;
    DECLARE vKupon VARCHAR(20);
    DECLARE vRacun INT;

    WHILE i <= vBroj DO
        SET vDana = 89 - FLOOR((i - 1) * 90 / vBroj);
        SET vKupac = 1 + MOD(i * 7, 8);
        SET vProdavnica = CASE
            WHEN MOD(i, 10) < 4 THEN 1
            WHEN MOD(i, 10) < 7 THEN 2
            WHEN MOD(i, 10) < 9 THEN 3
            ELSE 4 END;
        SET vStavki = 1 + MOD(i * 5, 3);
        SET vStavke = JSON_ARRAY();
        SET j = 0;

        WHILE j < vStavki DO
            -- popularniji proizvodi (manji indeks) se biraju cesce
            SET @offset_seed = MOD(i * 3 + j * 5 + FLOOR(i / 7), 14);
            SET @offset_seed = IF(MOD(i + j, 3) = 0, MOD(@offset_seed, 5), @offset_seed);
            SELECT idPROIZVOD INTO vProizvod
            FROM (SELECT idPROIZVOD, ROW_NUMBER() OVER (ORDER BY idPROIZVOD) - 1 AS rn
                  FROM proizvod
                  WHERE PRODVNICA_idProdavnica = vProdavnica AND Zalihe >= 10) t
            WHERE t.rn = MOD(@offset_seed, (SELECT COUNT(*) FROM proizvod
                                            WHERE PRODVNICA_idProdavnica = vProdavnica AND Zalihe >= 10));

            SET vKolicina = 1 + MOD(i + j * 2, 3);
            SET vStavke = JSON_ARRAY_APPEND(vStavke, '$', JSON_OBJECT('id', vProizvod, 'kolicina', vKolicina));
            SET j = j + 1;
        END WHILE;

        SET vKupon = IF(vDana <= 20 AND MOD(i, 4) = 0, 'JESEN15', NULL);

        SET TIMESTAMP = UNIX_TIMESTAMP(vDanas - INTERVAL vDana DAY
                                       + INTERVAL (8 + MOD(i * 13, 12)) HOUR
                                       + INTERVAL MOD(i * 17, 60) MINUTE);
        SET @korisnik = 'seed';
        CALL narucivanje(vKupac, vProdavnica, vStavke, vKupon, vRacun);
        SET i = i + 1;
    END WHILE;

    SET TIMESTAMP = DEFAULT;
    SET @korisnik = NULL;
END ;;
DELIMITER ;

CALL seed_racuni();
DROP PROCEDURE seed_racuni;

-- Jedan racun storniran (trigger vraca robu na zalihe)
SET @storno = (SELECT MAX(idRACUN) FROM racun WHERE PRODVNICA_idProdavnica = 3);
CALL storniraj_racun(@storno, 'admin');

-- Inventura: nekoliko proizvoda sa niskim ili nultim zalihama za upozorenja
SET @razlog_zaliha = 'Inventura';
SET @korisnik = 'radnik';
UPDATE proizvod SET Zalihe = 6 WHERE PRODVNICA_idProdavnica = 1 AND BarKod = '8600000000035';
UPDATE proizvod SET Zalihe = 3 WHERE PRODVNICA_idProdavnica = 1 AND BarKod = '3870000000044';
UPDATE proizvod SET Zalihe = 0 WHERE PRODVNICA_idProdavnica = 2 AND BarKod = '4000000000020';
UPDATE proizvod SET Zalihe = 9 WHERE PRODVNICA_idProdavnica = 3 AND BarKod = '3850000000015';
UPDATE proizvod SET Zalihe = 4 WHERE PRODVNICA_idProdavnica = 4 AND BarKod = '8600000000042';
SET @razlog_zaliha = NULL;
SET @korisnik = NULL;
