-- =====================================================================
--  Procedure: sve operacije koje diraju vise tabela ili nose poslovno pravilo.
--  Greske poslovnih pravila idu kroz SIGNAL SQLSTATE '45000' sa porukom
--  koju aplikacija prikazuje korisniku.
-- =====================================================================

SET NAMES utf8mb4;

DROP PROCEDURE IF EXISTS provjeri_korisnika;
DROP PROCEDURE IF EXISTS dodaj_adresu;
DROP PROCEDURE IF EXISTS azuriraj_adresu;
DROP PROCEDURE IF EXISTS dodaj_prodavnicu;
DROP PROCEDURE IF EXISTS izmijeni_prodavnicu;
DROP PROCEDURE IF EXISTS obrisi_prodavnicu;
DROP PROCEDURE IF EXISTS dodaj_administratora;
DROP PROCEDURE IF EXISTS dodaj_radnika;
DROP PROCEDURE IF EXISTS dodaj_kupca;
DROP PROCEDURE IF EXISTS registruj_kupca;
DROP PROCEDURE IF EXISTS izmijeni_kupca;
DROP PROCEDURE IF EXISTS obrisi_kupca;
DROP PROCEDURE IF EXISTS prijava_korisnika;
DROP PROCEDURE IF EXISTS promijeni_lozinku;
DROP PROCEDURE IF EXISTS dodaj_proizvodjaca;
DROP PROCEDURE IF EXISTS registruj_proizvodjaca;
DROP PROCEDURE IF EXISTS izmijeni_proizvodjaca;
DROP PROCEDURE IF EXISTS obrisi_proizvodjaca;
DROP PROCEDURE IF EXISTS dodaj_proizvod;
DROP PROCEDURE IF EXISTS update_proizvod;
DROP PROCEDURE IF EXISTS obrisi_proizvod;
DROP PROCEDURE IF EXISTS prijem_robe;
DROP PROCEDURE IF EXISTS get_proizvod_by_prodavnica;
DROP PROCEDURE IF EXISTS pronadji_po_barkodu;
DROP PROCEDURE IF EXISTS dodaj_popust;
DROP PROCEDURE IF EXISTS obrisi_popust;
DROP PROCEDURE IF EXISTS kreiraj_kupon;
DROP PROCEDURE IF EXISTS dodaj_proizvod_kuponu;
DROP PROCEDURE IF EXISTS obrisi_kupon;
DROP PROCEDURE IF EXISTS provjeri_kupon;
DROP PROCEDURE IF EXISTS narucivanje;
DROP PROCEDURE IF EXISTS storniraj_racun;
DROP PROCEDURE IF EXISTS detalji_racuna;
DROP PROCEDURE IF EXISTS stavke_racuna;
DROP PROCEDURE IF EXISTS pregled_racuna;
DROP PROCEDURE IF EXISTS izvjestaj_kpi;
DROP PROCEDURE IF EXISTS izvjestaj_prihod_po_prodavnici;
DROP PROCEDURE IF EXISTS izvjestaj_prihod_po_danima;
DROP PROCEDURE IF EXISTS izvjestaj_najprodavaniji;
DROP PROCEDURE IF EXISTS izvjestaj_niske_zalihe;
DROP PROCEDURE IF EXISTS istorija_zaliha;

DELIMITER ;;

-- ---------------------------------------------------------------------
--  Zajednicka validacija korisnika (pozivaju je trigeri na sve tri tabele)
-- ---------------------------------------------------------------------
CREATE PROCEDURE provjeri_korisnika(IN pEmail VARCHAR(100), IN pKorisnickoIme VARCHAR(50), IN pLozinka VARCHAR(255))
BEGIN
    IF pEmail IS NULL OR pEmail NOT REGEXP '^[^@[:space:]]+@[^@[:space:]]+\\.[^@[:space:]]+$' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Email adresa nije ispravna';
    END IF;
    IF pKorisnickoIme IS NULL OR pKorisnickoIme NOT REGEXP '^[a-z0-9._-]{3,50}$' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Korisničko ime: 3-50 znakova, mala slova, cifre, tačka, crtica';
    END IF;
    IF pLozinka IS NULL OR pLozinka NOT LIKE '$2_$%' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Lozinka mora biti sačuvana kao BCrypt hash';
    END IF;
END ;;

-- ---------------------------------------------------------------------
--  Adrese
-- ---------------------------------------------------------------------
CREATE PROCEDURE dodaj_adresu(IN pGrad VARCHAR(45), IN pUlica VARCHAR(100), IN pDrzava VARCHAR(45), OUT pIdAdrese INT)
BEGIN
    IF NULLIF(TRIM(pGrad), '') IS NULL OR NULLIF(TRIM(pUlica), '') IS NULL OR NULLIF(TRIM(pDrzava), '') IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Grad, ulica i država su obavezni';
    END IF;
    INSERT INTO adresa (Grad, Ulica, Drzava) VALUES (TRIM(pGrad), TRIM(pUlica), TRIM(pDrzava));
    SET pIdAdrese = LAST_INSERT_ID();
END ;;

CREATE PROCEDURE azuriraj_adresu(IN pIdAdrese INT, IN pGrad VARCHAR(45), IN pUlica VARCHAR(100), IN pDrzava VARCHAR(45))
BEGIN
    IF NULLIF(TRIM(pGrad), '') IS NULL OR NULLIF(TRIM(pUlica), '') IS NULL OR NULLIF(TRIM(pDrzava), '') IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Grad, ulica i država su obavezni';
    END IF;
    UPDATE adresa SET Grad = TRIM(pGrad), Ulica = TRIM(pUlica), Drzava = TRIM(pDrzava) WHERE idADRESA = pIdAdrese;
END ;;

-- ---------------------------------------------------------------------
--  Prodavnice
-- ---------------------------------------------------------------------
CREATE PROCEDURE dodaj_prodavnicu(IN pIme VARCHAR(45), IN pGrad VARCHAR(45), IN pUlica VARCHAR(100), IN pDrzava VARCHAR(45), OUT pIdProdavnice INT)
BEGIN
    DECLARE vIdAdrese INT;
    DECLARE EXIT HANDLER FOR SQLEXCEPTION BEGIN ROLLBACK; RESIGNAL; END;

    START TRANSACTION;
    CALL dodaj_adresu(pGrad, pUlica, pDrzava, vIdAdrese);
    INSERT INTO prodavnica (Ime, ADRESA_idADRESA) VALUES (TRIM(pIme), vIdAdrese);
    SET pIdProdavnice = LAST_INSERT_ID();
    COMMIT;
END ;;

CREATE PROCEDURE izmijeni_prodavnicu(IN pIdProdavnice INT, IN pIme VARCHAR(45), IN pGrad VARCHAR(45), IN pUlica VARCHAR(100), IN pDrzava VARCHAR(45))
BEGIN
    DECLARE vIdAdrese INT;
    DECLARE EXIT HANDLER FOR SQLEXCEPTION BEGIN ROLLBACK; RESIGNAL; END;

    SELECT ADRESA_idADRESA INTO vIdAdrese FROM prodavnica WHERE idProdavnica = pIdProdavnice;
    START TRANSACTION;
    UPDATE prodavnica SET Ime = TRIM(pIme) WHERE idProdavnica = pIdProdavnice;
    CALL azuriraj_adresu(vIdAdrese, pGrad, pUlica, pDrzava);
    COMMIT;
END ;;

CREATE PROCEDURE obrisi_prodavnicu(IN pIdProdavnice INT)
BEGIN
    DECLARE vIdAdrese INT;

    IF EXISTS (SELECT 1 FROM zaposleni WHERE Prodavnica_idProdavnica = pIdProdavnice)
       OR EXISTS (SELECT 1 FROM proizvod WHERE PRODVNICA_idProdavnica = pIdProdavnice)
       OR EXISTS (SELECT 1 FROM racun WHERE PRODVNICA_idProdavnica = pIdProdavnice) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Prodavnica ima zaposlene, proizvode ili račune i ne može se obrisati';
    END IF;

    SELECT ADRESA_idADRESA INTO vIdAdrese FROM prodavnica WHERE idProdavnica = pIdProdavnice;
    DELETE FROM prodavnica WHERE idProdavnica = pIdProdavnice;
    DELETE FROM adresa
    WHERE idADRESA = vIdAdrese
      AND NOT EXISTS (SELECT 1 FROM kupac WHERE ADRESA_idADRESA = vIdAdrese)
      AND NOT EXISTS (SELECT 1 FROM proizvodjac WHERE ADRESA_idADRESA = vIdAdrese)
      AND NOT EXISTS (SELECT 1 FROM prodavnica WHERE ADRESA_idADRESA = vIdAdrese);
END ;;

-- ---------------------------------------------------------------------
--  Korisnici
-- ---------------------------------------------------------------------
CREATE PROCEDURE dodaj_administratora(
    IN pIme VARCHAR(50), IN pPrezime VARCHAR(50), IN pEmail VARCHAR(100),
    IN pLozinka VARCHAR(255), IN pKorisnickoIme VARCHAR(50))
BEGIN
    INSERT INTO administrator (Ime, Prezime, Email, Lozinka, KorisnickoIme)
    VALUES (TRIM(pIme), TRIM(pPrezime), pEmail, pLozinka, pKorisnickoIme);
END ;;

CREATE PROCEDURE dodaj_radnika(
    IN pIme VARCHAR(50), IN pPrezime VARCHAR(50), IN pEmail VARCHAR(100), IN pBrojTelefona VARCHAR(20),
    IN pIdProdavnice INT, IN pPlata DECIMAL(10,2), IN pLozinka VARCHAR(255), IN pKorisnickoIme VARCHAR(50))
BEGIN
    INSERT INTO zaposleni (Ime, Prezime, Email, BrojTelefona, Prodavnica_idProdavnica, Plata, Lozinka, KorisnickoIme)
    VALUES (TRIM(pIme), TRIM(pPrezime), pEmail, NULLIF(TRIM(pBrojTelefona), ''), pIdProdavnice, pPlata, pLozinka, pKorisnickoIme);
END ;;

CREATE PROCEDURE dodaj_kupca(IN pKorisnickoIme VARCHAR(50), IN pLozinka VARCHAR(255), IN pEmail VARCHAR(100), IN pIdAdresa INT)
BEGIN
    INSERT INTO kupac (KorisnickoIme, Lozinka, Email, ADRESA_idADRESA)
    VALUES (pKorisnickoIme, pLozinka, pEmail, pIdAdresa);
END ;;

-- Kupac zajedno sa adresom, u jednoj transakciji
CREATE PROCEDURE registruj_kupca(
    IN pKorisnickoIme VARCHAR(50), IN pLozinka VARCHAR(255), IN pEmail VARCHAR(100),
    IN pGrad VARCHAR(45), IN pUlica VARCHAR(100), IN pDrzava VARCHAR(45), OUT pIdKupca INT)
BEGIN
    DECLARE vIdAdrese INT;
    DECLARE EXIT HANDLER FOR SQLEXCEPTION BEGIN ROLLBACK; RESIGNAL; END;

    START TRANSACTION;
    CALL dodaj_adresu(pGrad, pUlica, pDrzava, vIdAdrese);
    CALL dodaj_kupca(pKorisnickoIme, pLozinka, pEmail, vIdAdrese);
    SET pIdKupca = LAST_INSERT_ID();
    COMMIT;
END ;;

CREATE PROCEDURE izmijeni_kupca(
    IN pIdKupca INT, IN pKorisnickoIme VARCHAR(50), IN pEmail VARCHAR(100),
    IN pGrad VARCHAR(45), IN pUlica VARCHAR(100), IN pDrzava VARCHAR(45))
BEGIN
    DECLARE vIdAdrese INT;
    DECLARE EXIT HANDLER FOR SQLEXCEPTION BEGIN ROLLBACK; RESIGNAL; END;

    SELECT ADRESA_idADRESA INTO vIdAdrese FROM kupac WHERE idKUPAC = pIdKupca;
    START TRANSACTION;
    UPDATE kupac SET KorisnickoIme = pKorisnickoIme, Email = pEmail WHERE idKUPAC = pIdKupca;
    CALL azuriraj_adresu(vIdAdrese, pGrad, pUlica, pDrzava);
    COMMIT;
END ;;

CREATE PROCEDURE obrisi_kupca(IN pIdKupca INT)
BEGIN
    DECLARE vIdAdrese INT;

    IF EXISTS (SELECT 1 FROM racun WHERE KUPAC_idKUPAC = pIdKupca) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Kupac ima račune i ne može se obrisati';
    END IF;

    SELECT ADRESA_idADRESA INTO vIdAdrese FROM kupac WHERE idKUPAC = pIdKupca;
    DELETE FROM kupac WHERE idKUPAC = pIdKupca;
    DELETE FROM adresa
    WHERE idADRESA = vIdAdrese
      AND NOT EXISTS (SELECT 1 FROM kupac WHERE ADRESA_idADRESA = vIdAdrese)
      AND NOT EXISTS (SELECT 1 FROM proizvodjac WHERE ADRESA_idADRESA = vIdAdrese)
      AND NOT EXISTS (SELECT 1 FROM prodavnica WHERE ADRESA_idADRESA = vIdAdrese);
END ;;

-- Podaci za prijavu. Lozinku (hash) provjerava aplikacija kroz BCrypt.
CREATE PROCEDURE prijava_korisnika(IN pUloga VARCHAR(20), IN pKorisnickoIme VARCHAR(50))
BEGIN
    CASE pUloga
        WHEN 'administrator' THEN
            SELECT idAdministrator AS Id, KorisnickoIme, CONCAT(Ime, ' ', Prezime) AS PunoIme, Lozinka,
                   NULL AS IdProdavnice, NULL AS ImeProdavnice
            FROM administrator WHERE KorisnickoIme = pKorisnickoIme;
        WHEN 'zaposleni' THEN
            SELECT z.idZAPOSLENI AS Id, z.KorisnickoIme, CONCAT(z.Ime, ' ', z.Prezime) AS PunoIme, z.Lozinka,
                   p.idProdavnica AS IdProdavnice, p.Ime AS ImeProdavnice
            FROM zaposleni z JOIN prodavnica p ON p.idProdavnica = z.Prodavnica_idProdavnica
            WHERE z.KorisnickoIme = pKorisnickoIme;
        WHEN 'kupac' THEN
            SELECT idKUPAC AS Id, KorisnickoIme, KorisnickoIme AS PunoIme, Lozinka,
                   NULL AS IdProdavnice, NULL AS ImeProdavnice
            FROM kupac WHERE KorisnickoIme = pKorisnickoIme;
        ELSE
            SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Nepoznata uloga';
    END CASE;
END ;;

-- Dinamicki SQL: ime tabele dolazi iz bijele liste, vrijednosti idu kao parametri
CREATE PROCEDURE promijeni_lozinku(IN pUloga VARCHAR(20), IN pKorisnickoIme VARCHAR(50), IN pLozinka VARCHAR(255))
BEGIN
    DECLARE vTabela VARCHAR(20);

    SET vTabela = CASE pUloga
        WHEN 'administrator' THEN 'administrator'
        WHEN 'zaposleni' THEN 'zaposleni'
        WHEN 'kupac' THEN 'kupac'
    END;
    IF vTabela IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Nepoznata uloga';
    END IF;

    SET @sql_lozinka = CONCAT('UPDATE ', vTabela, ' SET Lozinka = ? WHERE KorisnickoIme = ?');
    SET @p_lozinka = pLozinka;
    SET @p_korisnik = pKorisnickoIme;
    PREPARE stmt FROM @sql_lozinka;
    EXECUTE stmt USING @p_lozinka, @p_korisnik;
    DEALLOCATE PREPARE stmt;
    SET @p_lozinka = NULL;
END ;;

-- ---------------------------------------------------------------------
--  Proizvodjaci
-- ---------------------------------------------------------------------
CREATE PROCEDURE dodaj_proizvodjaca(IN pIme VARCHAR(45), IN pEmail VARCHAR(100), IN pIdAdresa INT)
BEGIN
    INSERT INTO proizvodjac (Ime, Email, ADRESA_idADRESA) VALUES (TRIM(pIme), LOWER(TRIM(pEmail)), pIdAdresa);
END ;;

CREATE PROCEDURE registruj_proizvodjaca(
    IN pIme VARCHAR(45), IN pEmail VARCHAR(100),
    IN pGrad VARCHAR(45), IN pUlica VARCHAR(100), IN pDrzava VARCHAR(45), OUT pIdProizvodjaca INT)
BEGIN
    DECLARE vIdAdrese INT;
    DECLARE EXIT HANDLER FOR SQLEXCEPTION BEGIN ROLLBACK; RESIGNAL; END;

    START TRANSACTION;
    CALL dodaj_adresu(pGrad, pUlica, pDrzava, vIdAdrese);
    CALL dodaj_proizvodjaca(pIme, pEmail, vIdAdrese);
    SET pIdProizvodjaca = LAST_INSERT_ID();
    COMMIT;
END ;;

CREATE PROCEDURE izmijeni_proizvodjaca(
    IN pIdProizvodjaca INT, IN pIme VARCHAR(45), IN pEmail VARCHAR(100),
    IN pGrad VARCHAR(45), IN pUlica VARCHAR(100), IN pDrzava VARCHAR(45))
BEGIN
    DECLARE vIdAdrese INT;
    DECLARE EXIT HANDLER FOR SQLEXCEPTION BEGIN ROLLBACK; RESIGNAL; END;

    SELECT ADRESA_idADRESA INTO vIdAdrese FROM proizvodjac WHERE idPROIZVODJAC = pIdProizvodjaca;
    START TRANSACTION;
    UPDATE proizvodjac SET Ime = TRIM(pIme), Email = LOWER(TRIM(pEmail)) WHERE idPROIZVODJAC = pIdProizvodjaca;
    CALL azuriraj_adresu(vIdAdrese, pGrad, pUlica, pDrzava);
    COMMIT;
END ;;

CREATE PROCEDURE obrisi_proizvodjaca(IN pIdProizvodjaca INT)
BEGIN
    DECLARE vIdAdrese INT;

    IF EXISTS (SELECT 1 FROM proizvod WHERE PROIZVODJAC_idPROIZVODJAC = pIdProizvodjaca) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Proizvođač ima proizvode i ne može se obrisati';
    END IF;

    SELECT ADRESA_idADRESA INTO vIdAdrese FROM proizvodjac WHERE idPROIZVODJAC = pIdProizvodjaca;
    DELETE FROM proizvodjac WHERE idPROIZVODJAC = pIdProizvodjaca;
    DELETE FROM adresa
    WHERE idADRESA = vIdAdrese
      AND NOT EXISTS (SELECT 1 FROM kupac WHERE ADRESA_idADRESA = vIdAdrese)
      AND NOT EXISTS (SELECT 1 FROM proizvodjac WHERE ADRESA_idADRESA = vIdAdrese)
      AND NOT EXISTS (SELECT 1 FROM prodavnica WHERE ADRESA_idADRESA = vIdAdrese);
END ;;

-- ---------------------------------------------------------------------
--  Proizvodi
-- ---------------------------------------------------------------------
CREATE PROCEDURE dodaj_proizvod(
    IN pNaziv VARCHAR(45), IN pCijena DECIMAL(10,2), IN pDatumIsteka DATE, IN pBarKod VARCHAR(45),
    IN pIdProizvodjaca INT, IN pOpis VARCHAR(255), IN pZalihe INT, IN pIdProdavnice INT,
    IN pMinZalihe INT, IN pIdZaposlenog INT, OUT pIdProizvoda INT)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION BEGIN ROLLBACK; SET @razlog_zaliha = NULL; SET @korisnik = NULL; RESIGNAL; END;

    IF EXISTS (SELECT 1 FROM proizvod WHERE PRODVNICA_idProdavnica = pIdProdavnice AND BarKod = TRIM(pBarKod)) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Proizvod sa ovim bar kodom već postoji u prodavnici';
    END IF;

    START TRANSACTION;
    SET @razlog_zaliha = 'Početno stanje';
    SET @korisnik = COALESCE((SELECT KorisnickoIme FROM zaposleni WHERE idZAPOSLENI = pIdZaposlenog), @korisnik);
    INSERT INTO proizvod (Naziv, DatumIsteka, BarKod, PROIZVODJAC_idPROIZVODJAC, Opis, Zalihe, MinZalihe, Cijena, PRODVNICA_idProdavnica)
    VALUES (pNaziv, pDatumIsteka, pBarKod, pIdProizvodjaca, NULLIF(TRIM(pOpis), ''), COALESCE(pZalihe, 0), COALESCE(pMinZalihe, 10), pCijena, pIdProdavnice);
    SET pIdProizvoda = LAST_INSERT_ID();

    IF pIdZaposlenog IS NOT NULL THEN
        INSERT INTO dodavanjeproizvoda (PROIZVOD_idPROIZVOD, ZAPOSLENI_idZAPOSLENI, Datum)
        VALUES (pIdProizvoda, pIdZaposlenog, CURDATE());
    END IF;
    COMMIT;
    SET @razlog_zaliha = NULL;
    SET @korisnik = NULL;
END ;;

CREATE PROCEDURE update_proizvod(
    IN pIdProizvoda INT, IN pNaziv VARCHAR(45), IN pDatumIsteka DATE, IN pBarKod VARCHAR(45),
    IN pIdProizvodjaca INT, IN pOpis VARCHAR(255), IN pZalihe INT, IN pCijena DECIMAL(10,2), IN pMinZalihe INT)
BEGIN
    SET @razlog_zaliha = 'Korekcija';
    UPDATE proizvod
    SET Naziv = pNaziv, DatumIsteka = pDatumIsteka, BarKod = pBarKod, PROIZVODJAC_idPROIZVODJAC = pIdProizvodjaca,
        Opis = NULLIF(TRIM(pOpis), ''), Zalihe = pZalihe, Cijena = pCijena, MinZalihe = COALESCE(pMinZalihe, MinZalihe)
    WHERE idPROIZVOD = pIdProizvoda;
    SET @razlog_zaliha = NULL;
END ;;

-- Proizvod koji je prodavan ostaje u bazi zbog racuna, samo se povlaci iz prodaje
CREATE PROCEDURE obrisi_proizvod(IN pIdProizvoda INT)
BEGIN
    IF EXISTS (SELECT 1 FROM stavka WHERE PROIZVOD_idPROIZVOD = pIdProizvoda) THEN
        UPDATE proizvod SET Aktivan = 0 WHERE idPROIZVOD = pIdProizvoda;
    ELSE
        DELETE FROM proizvod WHERE idPROIZVOD = pIdProizvoda;
    END IF;
END ;;

CREATE PROCEDURE prijem_robe(IN pIdProizvoda INT, IN pKolicina INT, IN pKorisnik VARCHAR(50))
BEGIN
    IF pKolicina IS NULL OR pKolicina <= 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Količina prijema mora biti veća od nule';
    END IF;
    IF NOT EXISTS (SELECT 1 FROM proizvod WHERE idPROIZVOD = pIdProizvoda AND Aktivan = 1) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Proizvod ne postoji ili je povučen iz prodaje';
    END IF;

    SET @razlog_zaliha = 'Prijem robe';
    SET @korisnik = pKorisnik;
    UPDATE proizvod SET Zalihe = Zalihe + pKolicina WHERE idPROIZVOD = pIdProizvoda;
    SET @razlog_zaliha = NULL;
    SET @korisnik = NULL;
END ;;

CREATE PROCEDURE get_proizvod_by_prodavnica(IN pIdProdavnica INT)
BEGIN
    SELECT * FROM v_proizvodi
    WHERE IdProdavnice = pIdProdavnica AND Aktivan = 1
    ORDER BY Naziv;
END ;;

CREATE PROCEDURE pronadji_po_barkodu(IN pIdProdavnice INT, IN pBarKod VARCHAR(45))
BEGIN
    SELECT * FROM v_proizvodi
    WHERE IdProdavnice = pIdProdavnice AND BarKod = TRIM(pBarKod) AND Aktivan = 1;
END ;;

-- ---------------------------------------------------------------------
--  Akcije i kuponi
-- ---------------------------------------------------------------------
CREATE PROCEDURE dodaj_popust(IN pIdProizvoda INT, IN pPopust INT, IN pStartDate DATE, IN pEndDate DATE)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION BEGIN ROLLBACK; RESIGNAL; END;

    START TRANSACTION;
    INSERT INTO popust (Popust, Start, End) VALUES (pPopust, pStartDate, pEndDate);
    INSERT INTO proizvod_has_popust (PROIZVOD_idPROIZVOD, Popust_idPopust) VALUES (pIdProizvoda, LAST_INSERT_ID());
    COMMIT;
END ;;

CREATE PROCEDURE obrisi_popust(IN pIdPopusta INT)
BEGIN
    DELETE FROM proizvod_has_popust WHERE Popust_idPopust = pIdPopusta;
    DELETE FROM popust WHERE idPopust = pIdPopusta;
END ;;

CREATE PROCEDURE kreiraj_kupon(IN pIdProizvoda INT, IN pPopust INT, IN pSifra VARCHAR(20), IN pStart DATE, IN pEnd DATE)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION BEGIN ROLLBACK; RESIGNAL; END;

    START TRANSACTION;
    INSERT INTO kupon (Sifra, Start, end, popust) VALUES (pSifra, pStart, pEnd, pPopust);
    INSERT INTO proizvod_has_kupon (PROIZVOD_idPROIZVOD, Kupon_idKupon) VALUES (pIdProizvoda, LAST_INSERT_ID());
    COMMIT;
END ;;

CREATE PROCEDURE dodaj_proizvod_kuponu(IN pIdKupona INT, IN pIdProizvoda INT)
BEGIN
    INSERT IGNORE INTO proizvod_has_kupon (PROIZVOD_idPROIZVOD, Kupon_idKupon) VALUES (pIdProizvoda, pIdKupona);
END ;;

CREATE PROCEDURE obrisi_kupon(IN pIdKupona INT)
BEGIN
    DELETE FROM proizvod_has_kupon WHERE Kupon_idKupon = pIdKupona;
    DELETE FROM kupon WHERE idKupon = pIdKupona;
END ;;

-- Proizvodi prodavnice na koje kupon danas daje popust
CREATE PROCEDURE provjeri_kupon(IN pSifra VARCHAR(20), IN pIdProdavnice INT)
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM kupon
        WHERE Sifra = UPPER(TRIM(pSifra)) AND CURDATE() >= Start AND (end IS NULL OR CURDATE() <= end)) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Kupon nije važeći ili je istekao';
    END IF;

    SELECT p.idPROIZVOD AS IdProizvoda, fn_kupon_popust(pSifra, p.idPROIZVOD, CURDATE()) AS Popust
    FROM proizvod p
    WHERE p.PRODVNICA_idProdavnica = pIdProdavnice
      AND p.Aktivan = 1
      AND fn_kupon_popust(pSifra, p.idPROIZVOD, CURDATE()) > 0;
END ;;

-- ---------------------------------------------------------------------
--  Narudzba
--  pStavke: JSON niz [{"id": 1, "kolicina": 2}, ...]
--  Cijene i popuste racuna baza, ne klijent. Zalihe skida trigger na stavki,
--  a sve se desava u jednoj transakciji: ili cijela narudzba ili nista.
-- ---------------------------------------------------------------------
CREATE PROCEDURE narucivanje(
    IN pIdKupca INT, IN pIdProdavnice INT, IN pStavke JSON, IN pSifraKupona VARCHAR(20), OUT pIdRacuna INT)
BEGIN
    DECLARE vGotovo BOOLEAN DEFAULT FALSE;
    DECLARE vIdProizvoda INT;
    DECLARE vKolicina INT;
    DECLARE vIdKupona INT DEFAULT NULL;
    DECLARE vSifra VARCHAR(20) DEFAULT NULLIF(UPPER(TRIM(pSifraKupona)), '');
    DECLARE vPunaCijena DECIMAL(10,2);
    DECLARE vPopust INT;
    DECLARE vDanas DATE DEFAULT CURDATE();

    DECLARE cStavke CURSOR FOR
        SELECT j.id, SUM(j.kolicina)
        FROM JSON_TABLE(pStavke, '$[*]' COLUMNS (
            id INT PATH '$.id',
            kolicina INT PATH '$.kolicina')) j
        GROUP BY j.id;
    DECLARE CONTINUE HANDLER FOR NOT FOUND SET vGotovo = TRUE;
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        SET @razlog_zaliha = NULL;
        RESIGNAL;
    END;

    IF pStavke IS NULL OR JSON_TYPE(pStavke) <> 'ARRAY' OR JSON_LENGTH(pStavke) = 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Korpa je prazna';
    END IF;

    IF EXISTS (
        SELECT 1 FROM JSON_TABLE(pStavke, '$[*]' COLUMNS (id INT PATH '$.id', kolicina INT PATH '$.kolicina')) j
        WHERE j.id IS NULL OR j.kolicina IS NULL OR j.kolicina <= 0) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Količina mora biti veća od nule';
    END IF;

    IF EXISTS (
        SELECT 1 FROM JSON_TABLE(pStavke, '$[*]' COLUMNS (id INT PATH '$.id')) j
        LEFT JOIN proizvod p
               ON p.idPROIZVOD = j.id AND p.PRODVNICA_idProdavnica = pIdProdavnice AND p.Aktivan = 1
        WHERE p.idPROIZVOD IS NULL) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Proizvod ne pripada izabranoj prodavnici';
    END IF;

    IF vSifra IS NOT NULL THEN
        SELECT idKupon INTO vIdKupona FROM kupon
        WHERE Sifra = vSifra AND vDanas >= Start AND (end IS NULL OR vDanas <= end);
        IF vIdKupona IS NULL THEN
            SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Kupon nije važeći ili je istekao';
        END IF;
    END IF;

    START TRANSACTION;

    INSERT INTO racun (KUPAC_idKUPAC, PRODVNICA_idProdavnica, KUPON_idKupon)
    VALUES (pIdKupca, pIdProdavnice, vIdKupona);
    SET pIdRacuna = LAST_INSERT_ID();

    SET vGotovo = FALSE;
    OPEN cStavke;
    citanje: LOOP
        FETCH cStavke INTO vIdProizvoda, vKolicina;
        IF vGotovo THEN
            LEAVE citanje;
        END IF;

        SELECT Cijena INTO vPunaCijena FROM proizvod WHERE idPROIZVOD = vIdProizvoda;
        SET vPopust = GREATEST(
            fn_aktivni_popust(vIdProizvoda, vDanas),
            IF(vIdKupona IS NULL, 0, fn_kupon_popust(vSifra, vIdProizvoda, vDanas)));

        INSERT INTO stavka (RACUN_idRACUN, PROIZVOD_idPROIZVOD, Kolicina, PunaCijena, Popust, Cijena)
        VALUES (pIdRacuna, vIdProizvoda, vKolicina, vPunaCijena, vPopust,
                ROUND(vPunaCijena * (100 - vPopust) / 100, 2));
    END LOOP;
    CLOSE cStavke;

    COMMIT;
END ;;

-- Storno: trigger na racunu vraca robu na zalihe
CREATE PROCEDURE storniraj_racun(IN pIdRacuna INT, IN pKorisnik VARCHAR(50))
BEGIN
    IF NOT EXISTS (SELECT 1 FROM racun WHERE idRACUN = pIdRacuna) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Račun ne postoji';
    END IF;
    IF EXISTS (SELECT 1 FROM racun WHERE idRACUN = pIdRacuna AND Status = 'Storniran') THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Račun je već storniran';
    END IF;

    SET @korisnik = pKorisnik;
    UPDATE racun SET Status = 'Storniran' WHERE idRACUN = pIdRacuna;
    SET @korisnik = NULL;
END ;;

-- ---------------------------------------------------------------------
--  Racuni
-- ---------------------------------------------------------------------
CREATE PROCEDURE detalji_racuna(IN pIdKupca INT)
BEGIN
    SELECT * FROM v_racuni WHERE IdKupca = pIdKupca ORDER BY Datum DESC, Id DESC;
END ;;

CREATE PROCEDURE stavke_racuna(IN pIdRacuna INT)
BEGIN
    SELECT s.RACUN_idRACUN AS IdRacuna, s.PROIZVOD_idPROIZVOD AS IdProizvoda, p.Naziv, p.BarKod,
           s.Kolicina, s.PunaCijena, s.Popust, s.Cijena, s.Kolicina * s.Cijena AS Ukupno
    FROM stavka s
    JOIN proizvod p ON p.idPROIZVOD = s.PROIZVOD_idPROIZVOD
    WHERE s.RACUN_idRACUN = pIdRacuna
    ORDER BY p.Naziv;
END ;;

CREATE PROCEDURE pregled_racuna(IN pOd DATE, IN pDo DATE, IN pIdProdavnice INT)
BEGIN
    SELECT * FROM v_racuni
    WHERE Datum >= pOd AND Datum < pDo + INTERVAL 1 DAY
      AND (pIdProdavnice IS NULL OR IdProdavnice = pIdProdavnice)
    ORDER BY Datum DESC, Id DESC;
END ;;

-- ---------------------------------------------------------------------
--  Izvjestaji (racunaju samo placene racune)
-- ---------------------------------------------------------------------
CREATE PROCEDURE izvjestaj_kpi(IN pOd DATE, IN pDo DATE)
BEGIN
    SELECT
        COALESCE(SUM(r.Ukupno), 0)                       AS Prihod,
        COUNT(*)                                         AS BrojRacuna,
        COALESCE(ROUND(AVG(r.Ukupno), 2), 0)             AS ProsjecnaKorpa,
        COUNT(DISTINCT r.KUPAC_idKUPAC)                  AS BrojKupaca,
        (SELECT COUNT(*) FROM v_niske_zalihe)            AS NiskeZalihe
    FROM racun r
    WHERE r.Status = 'Placen'
      AND r.DatumKupovine >= pOd AND r.DatumKupovine < pDo + INTERVAL 1 DAY;
END ;;

CREATE PROCEDURE izvjestaj_prihod_po_prodavnici(IN pOd DATE, IN pDo DATE)
BEGIN
    SELECT pr.idProdavnica AS IdProdavnice, pr.Ime AS Prodavnica,
           COUNT(r.idRACUN) AS BrojRacuna,
           COALESCE(SUM(r.Ukupno), 0) AS Prihod
    FROM prodavnica pr
    LEFT JOIN racun r
           ON r.PRODVNICA_idProdavnica = pr.idProdavnica
          AND r.Status = 'Placen'
          AND r.DatumKupovine >= pOd AND r.DatumKupovine < pDo + INTERVAL 1 DAY
    GROUP BY pr.idProdavnica, pr.Ime
    ORDER BY Prihod DESC;
END ;;

-- Rekurzivni CTE pravi sve dane u periodu, pa i dani bez prodaje imaju red
CREATE PROCEDURE izvjestaj_prihod_po_danima(IN pOd DATE, IN pDo DATE)
BEGIN
    WITH RECURSIVE dani (Dan) AS (
        SELECT pOd
        UNION ALL
        SELECT Dan + INTERVAL 1 DAY FROM dani WHERE Dan < pDo
    )
    SELECT d.Dan, COALESCE(SUM(r.Ukupno), 0) AS Prihod, COUNT(r.idRACUN) AS BrojRacuna
    FROM dani d
    LEFT JOIN racun r ON DATE(r.DatumKupovine) = d.Dan AND r.Status = 'Placen'
    GROUP BY d.Dan
    ORDER BY d.Dan;
END ;;

-- Window funkcija RANK() za poredak proizvoda po prodatoj kolicini
CREATE PROCEDURE izvjestaj_najprodavaniji(IN pOd DATE, IN pDo DATE, IN pLimit INT)
BEGIN
    SELECT * FROM (
        SELECT RANK() OVER (ORDER BY SUM(v.Kolicina) DESC) AS Rang,
               v.Naziv, v.BarKod,
               SUM(v.Kolicina) AS Kolicina,
               SUM(v.Iznos) AS Prihod,
               COUNT(DISTINCT v.IdProdavnice) AS BrojProdavnica
        FROM v_prodaja v
        WHERE v.Datum >= pOd AND v.Datum < pDo + INTERVAL 1 DAY
        GROUP BY v.Naziv, v.BarKod
    ) t
    WHERE t.Rang <= COALESCE(pLimit, 10)
    ORDER BY t.Rang, t.Naziv;
END ;;

CREATE PROCEDURE izvjestaj_niske_zalihe(IN pIdProdavnice INT)
BEGIN
    SELECT * FROM v_niske_zalihe
    WHERE pIdProdavnice IS NULL OR IdProdavnice = pIdProdavnice
    ORDER BY Zalihe, Naziv;
END ;;

CREATE PROCEDURE istorija_zaliha(IN pOd DATE, IN pDo DATE, IN pIdProdavnice INT)
BEGIN
    SELECT zi.idIstorija AS Id, zi.Vrijeme, p.Naziv, p.BarKod, pr.Ime AS Prodavnica,
           zi.StaraKolicina, zi.NovaKolicina, zi.Promjena, zi.Razlog, zi.Korisnik
    FROM zalihe_istorija zi
    JOIN proizvod p ON p.idPROIZVOD = zi.PROIZVOD_idPROIZVOD
    JOIN prodavnica pr ON pr.idProdavnica = p.PRODVNICA_idProdavnica
    WHERE zi.Vrijeme >= pOd AND zi.Vrijeme < pDo + INTERVAL 1 DAY
      AND (pIdProdavnice IS NULL OR p.PRODVNICA_idProdavnica = pIdProdavnice)
    ORDER BY zi.Vrijeme DESC, zi.idIstorija DESC;
END ;;

DELIMITER ;
