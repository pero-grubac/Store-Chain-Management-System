-- =====================================================================
--  Trigeri
--
--  proizvod   BI/BU  validacija bar koda, naziva, cijene i roka trajanja
--             AI/AU  upis svake promjene zaliha u zalihe_istorija
--  stavka     BI/BU  provjera kolicine, prodavnice i stanja na zalihama
--             AI/AU/AD  skidanje/vracanje zaliha i odrzavanje racun.Ukupno
--  racun      BU     storniran racun se ne moze vratiti
--             AU     storno vraca robu na zalihe
--  popust, kupon     BI/BU  validacija procenta, perioda i sifre
--  administrator, zaposleni, kupac  BI/BU  email, korisnicko ime, BCrypt lozinka
--
--  Razlog promjene zaliha dolazi iz @razlog_zaliha (postavljaju ga procedure),
--  a korisnik iz @korisnik.
-- =====================================================================

SET NAMES utf8mb4;

DROP TRIGGER IF EXISTS trg_proizvod_bi;
DROP TRIGGER IF EXISTS trg_proizvod_bu;
DROP TRIGGER IF EXISTS trg_proizvod_ai;
DROP TRIGGER IF EXISTS trg_proizvod_au;
DROP TRIGGER IF EXISTS trg_stavka_bi;
DROP TRIGGER IF EXISTS trg_stavka_ai;
DROP TRIGGER IF EXISTS trg_stavka_bu;
DROP TRIGGER IF EXISTS trg_stavka_au;
DROP TRIGGER IF EXISTS trg_stavka_ad;
DROP TRIGGER IF EXISTS trg_racun_bu;
DROP TRIGGER IF EXISTS trg_racun_au;
DROP TRIGGER IF EXISTS trg_popust_bi;
DROP TRIGGER IF EXISTS trg_popust_bu;
DROP TRIGGER IF EXISTS trg_kupon_bi;
DROP TRIGGER IF EXISTS trg_kupon_bu;
DROP TRIGGER IF EXISTS trg_administrator_bi;
DROP TRIGGER IF EXISTS trg_administrator_bu;
DROP TRIGGER IF EXISTS trg_zaposleni_bi;
DROP TRIGGER IF EXISTS trg_zaposleni_bu;
DROP TRIGGER IF EXISTS trg_kupac_bi;
DROP TRIGGER IF EXISTS trg_kupac_bu;

DELIMITER ;;

-- ---------------------------------------------------------------------
--  proizvod
-- ---------------------------------------------------------------------
CREATE TRIGGER trg_proizvod_bi BEFORE INSERT ON proizvod
FOR EACH ROW
BEGIN
    SET NEW.BarKod = TRIM(NEW.BarKod);
    SET NEW.Naziv = TRIM(NEW.Naziv);

    IF NEW.Naziv = '' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Naziv proizvoda je obavezan';
    END IF;
    IF NEW.BarKod = '' OR NEW.BarKod NOT REGEXP '^[0-9A-Za-z-]+$' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Bar kod smije sadržati samo cifre, slova i crticu';
    END IF;
    IF NEW.Cijena IS NULL OR NEW.Cijena < 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Cijena ne može biti negativna';
    END IF;
    IF NEW.Zalihe < 0 OR NEW.MinZalihe < 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Zalihe ne mogu biti negativne';
    END IF;
    IF NEW.DatumIsteka IS NOT NULL AND NEW.DatumIsteka < CURDATE() THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Rok trajanja je već istekao';
    END IF;
END ;;

CREATE TRIGGER trg_proizvod_bu BEFORE UPDATE ON proizvod
FOR EACH ROW
BEGIN
    SET NEW.BarKod = TRIM(NEW.BarKod);
    SET NEW.Naziv = TRIM(NEW.Naziv);

    IF NEW.Naziv = '' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Naziv proizvoda je obavezan';
    END IF;
    IF NEW.BarKod = '' OR NEW.BarKod NOT REGEXP '^[0-9A-Za-z-]+$' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Bar kod smije sadržati samo cifre, slova i crticu';
    END IF;
    IF NEW.Cijena IS NULL OR NEW.Cijena < 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Cijena ne može biti negativna';
    END IF;
    IF NEW.Zalihe < 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Nema dovoljno proizvoda na zalihama';
    END IF;
    IF NEW.MinZalihe < 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Zalihe ne mogu biti negativne';
    END IF;
    IF NOT (NEW.DatumIsteka <=> OLD.DatumIsteka) AND NEW.DatumIsteka < CURDATE() THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Rok trajanja je već istekao';
    END IF;
END ;;

CREATE TRIGGER trg_proizvod_ai AFTER INSERT ON proizvod
FOR EACH ROW
BEGIN
    IF NEW.Zalihe > 0 THEN
        INSERT INTO zalihe_istorija (PROIZVOD_idPROIZVOD, StaraKolicina, NovaKolicina, Razlog, Korisnik)
        VALUES (NEW.idPROIZVOD, 0, NEW.Zalihe, COALESCE(@razlog_zaliha, 'Početno stanje'), @korisnik);
    END IF;
END ;;

CREATE TRIGGER trg_proizvod_au AFTER UPDATE ON proizvod
FOR EACH ROW
BEGIN
    IF NEW.Zalihe <> OLD.Zalihe THEN
        INSERT INTO zalihe_istorija (PROIZVOD_idPROIZVOD, StaraKolicina, NovaKolicina, Razlog, Korisnik)
        VALUES (NEW.idPROIZVOD, OLD.Zalihe, NEW.Zalihe, COALESCE(@razlog_zaliha, 'Izmjena'), @korisnik);
    END IF;
END ;;

-- ---------------------------------------------------------------------
--  stavka
-- ---------------------------------------------------------------------
CREATE TRIGGER trg_stavka_bi BEFORE INSERT ON stavka
FOR EACH ROW
BEGIN
    DECLARE vZalihe INT;
    DECLARE vNaziv VARCHAR(45);
    DECLARE vProdavnicaProizvoda INT;
    DECLARE vProdavnicaRacuna INT;
    DECLARE vStatus VARCHAR(10);
    DECLARE vAktivan TINYINT;

    IF NEW.Kolicina IS NULL OR NEW.Kolicina <= 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Količina mora biti veća od nule';
    END IF;

    SELECT Zalihe, Naziv, PRODVNICA_idProdavnica, Aktivan
      INTO vZalihe, vNaziv, vProdavnicaProizvoda, vAktivan
    FROM proizvod WHERE idPROIZVOD = NEW.PROIZVOD_idPROIZVOD;

    SELECT PRODVNICA_idProdavnica, Status INTO vProdavnicaRacuna, vStatus
    FROM racun WHERE idRACUN = NEW.RACUN_idRACUN;

    IF vStatus = 'Storniran' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Storniran račun se ne može mijenjati';
    END IF;
    IF vProdavnicaProizvoda <> vProdavnicaRacuna OR vAktivan = 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Proizvod ne pripada izabranoj prodavnici';
    END IF;
    IF vZalihe < NEW.Kolicina THEN
        SET @poruka_zalihe = LEFT(CONCAT('Nema dovoljno proizvoda na zalihama: ', vNaziv, ' (na stanju ', vZalihe, ')'), 128);
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = @poruka_zalihe;
    END IF;
END ;;

CREATE TRIGGER trg_stavka_ai AFTER INSERT ON stavka
FOR EACH ROW
BEGIN
    DECLARE vStariRazlog VARCHAR(30) DEFAULT @razlog_zaliha;

    SET @razlog_zaliha = 'Prodaja';
    UPDATE proizvod SET Zalihe = Zalihe - NEW.Kolicina WHERE idPROIZVOD = NEW.PROIZVOD_idPROIZVOD;
    SET @razlog_zaliha = vStariRazlog;

    UPDATE racun SET Ukupno = Ukupno + NEW.Kolicina * NEW.Cijena WHERE idRACUN = NEW.RACUN_idRACUN;
END ;;

CREATE TRIGGER trg_stavka_bu BEFORE UPDATE ON stavka
FOR EACH ROW
BEGIN
    DECLARE vZalihe INT;
    DECLARE vStatus VARCHAR(10);

    SELECT Status INTO vStatus FROM racun WHERE idRACUN = NEW.RACUN_idRACUN;
    IF vStatus = 'Storniran' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Storniran račun se ne može mijenjati';
    END IF;
    IF NEW.PROIZVOD_idPROIZVOD <> OLD.PROIZVOD_idPROIZVOD OR NEW.RACUN_idRACUN <> OLD.RACUN_idRACUN THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Stavka se ne može premjestiti; obrišite je i dodajte novu';
    END IF;
    IF NEW.Kolicina IS NULL OR NEW.Kolicina <= 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Količina mora biti veća od nule';
    END IF;

    SELECT Zalihe INTO vZalihe FROM proizvod WHERE idPROIZVOD = NEW.PROIZVOD_idPROIZVOD;
    IF NEW.Kolicina - OLD.Kolicina > vZalihe THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Nema dovoljno proizvoda na zalihama';
    END IF;
END ;;

CREATE TRIGGER trg_stavka_au AFTER UPDATE ON stavka
FOR EACH ROW
BEGIN
    DECLARE vStariRazlog VARCHAR(30) DEFAULT @razlog_zaliha;

    IF NEW.Kolicina <> OLD.Kolicina THEN
        SET @razlog_zaliha = 'Izmjena računa';
        UPDATE proizvod SET Zalihe = Zalihe - (NEW.Kolicina - OLD.Kolicina) WHERE idPROIZVOD = NEW.PROIZVOD_idPROIZVOD;
        SET @razlog_zaliha = vStariRazlog;
    END IF;

    UPDATE racun
    SET Ukupno = Ukupno - OLD.Kolicina * OLD.Cijena + NEW.Kolicina * NEW.Cijena
    WHERE idRACUN = NEW.RACUN_idRACUN;
END ;;

CREATE TRIGGER trg_stavka_ad AFTER DELETE ON stavka
FOR EACH ROW
BEGIN
    DECLARE vStariRazlog VARCHAR(30) DEFAULT @razlog_zaliha;
    DECLARE vStatus VARCHAR(10);

    SELECT Status INTO vStatus FROM racun WHERE idRACUN = OLD.RACUN_idRACUN;

    IF vStatus = 'Placen' THEN
        SET @razlog_zaliha = 'Brisanje stavke';
        UPDATE proizvod SET Zalihe = Zalihe + OLD.Kolicina WHERE idPROIZVOD = OLD.PROIZVOD_idPROIZVOD;
        SET @razlog_zaliha = vStariRazlog;

        UPDATE racun SET Ukupno = Ukupno - OLD.Kolicina * OLD.Cijena WHERE idRACUN = OLD.RACUN_idRACUN;
    END IF;
END ;;

-- ---------------------------------------------------------------------
--  racun
-- ---------------------------------------------------------------------
CREATE TRIGGER trg_racun_bu BEFORE UPDATE ON racun
FOR EACH ROW
BEGIN
    IF OLD.Status = 'Storniran' AND NEW.Status <> 'Storniran' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Storniran račun se ne može vratiti';
    END IF;
    IF NEW.KUPAC_idKUPAC <> OLD.KUPAC_idKUPAC OR NEW.PRODVNICA_idProdavnica <> OLD.PRODVNICA_idProdavnica THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Kupac i prodavnica na računu se ne mijenjaju';
    END IF;
END ;;

CREATE TRIGGER trg_racun_au AFTER UPDATE ON racun
FOR EACH ROW
BEGIN
    DECLARE vStariRazlog VARCHAR(30) DEFAULT @razlog_zaliha;

    IF OLD.Status = 'Placen' AND NEW.Status = 'Storniran' THEN
        SET @razlog_zaliha = 'Storno';
        UPDATE proizvod p
        JOIN stavka s ON s.PROIZVOD_idPROIZVOD = p.idPROIZVOD
        SET p.Zalihe = p.Zalihe + s.Kolicina
        WHERE s.RACUN_idRACUN = NEW.idRACUN;
        SET @razlog_zaliha = vStariRazlog;
    END IF;
END ;;

-- ---------------------------------------------------------------------
--  popust i kupon
-- ---------------------------------------------------------------------
CREATE TRIGGER trg_popust_bi BEFORE INSERT ON popust
FOR EACH ROW
BEGIN
    IF NEW.Popust IS NULL OR NEW.Popust NOT BETWEEN 1 AND 90 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Popust mora biti između 1 i 90 %';
    END IF;
    IF NEW.End < NEW.Start THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Akcija ne može završiti prije početka';
    END IF;
END ;;

CREATE TRIGGER trg_popust_bu BEFORE UPDATE ON popust
FOR EACH ROW
BEGIN
    IF NEW.Popust IS NULL OR NEW.Popust NOT BETWEEN 1 AND 90 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Popust mora biti između 1 i 90 %';
    END IF;
    IF NEW.End < NEW.Start THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Akcija ne može završiti prije početka';
    END IF;
END ;;

CREATE TRIGGER trg_kupon_bi BEFORE INSERT ON kupon
FOR EACH ROW
BEGIN
    SET NEW.Sifra = UPPER(TRIM(NEW.Sifra));
    IF NEW.Sifra NOT REGEXP '^[A-Z0-9_-]{3,20}$' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Šifra kupona: 3-20 slova, cifara, crtica ili donjih crta';
    END IF;
    IF NEW.popust IS NULL OR NEW.popust NOT BETWEEN 1 AND 90 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Popust mora biti između 1 i 90 %';
    END IF;
    IF NEW.end IS NOT NULL AND NEW.end < NEW.Start THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Kupon ne može isteći prije početka';
    END IF;
END ;;

CREATE TRIGGER trg_kupon_bu BEFORE UPDATE ON kupon
FOR EACH ROW
BEGIN
    SET NEW.Sifra = UPPER(TRIM(NEW.Sifra));
    IF NEW.Sifra NOT REGEXP '^[A-Z0-9_-]{3,20}$' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Šifra kupona: 3-20 slova, cifara, crtica ili donjih crta';
    END IF;
    IF NEW.popust IS NULL OR NEW.popust NOT BETWEEN 1 AND 90 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Popust mora biti između 1 i 90 %';
    END IF;
    IF NEW.end IS NOT NULL AND NEW.end < NEW.Start THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Kupon ne može isteći prije početka';
    END IF;
END ;;

-- ---------------------------------------------------------------------
--  korisnici: normalizacija i zajednicka provjera
-- ---------------------------------------------------------------------
CREATE TRIGGER trg_administrator_bi BEFORE INSERT ON administrator
FOR EACH ROW
BEGIN
    SET NEW.Email = LOWER(TRIM(NEW.Email));
    SET NEW.KorisnickoIme = LOWER(TRIM(NEW.KorisnickoIme));
    CALL provjeri_korisnika(NEW.Email, NEW.KorisnickoIme, NEW.Lozinka);
END ;;

CREATE TRIGGER trg_administrator_bu BEFORE UPDATE ON administrator
FOR EACH ROW
BEGIN
    SET NEW.Email = LOWER(TRIM(NEW.Email));
    SET NEW.KorisnickoIme = LOWER(TRIM(NEW.KorisnickoIme));
    CALL provjeri_korisnika(NEW.Email, NEW.KorisnickoIme, NEW.Lozinka);
END ;;

CREATE TRIGGER trg_zaposleni_bi BEFORE INSERT ON zaposleni
FOR EACH ROW
BEGIN
    SET NEW.Email = LOWER(TRIM(NEW.Email));
    SET NEW.KorisnickoIme = LOWER(TRIM(NEW.KorisnickoIme));
    CALL provjeri_korisnika(NEW.Email, NEW.KorisnickoIme, NEW.Lozinka);
    IF NEW.Plata IS NULL OR NEW.Plata <= 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Plata mora biti veća od nule';
    END IF;
END ;;

CREATE TRIGGER trg_zaposleni_bu BEFORE UPDATE ON zaposleni
FOR EACH ROW
BEGIN
    SET NEW.Email = LOWER(TRIM(NEW.Email));
    SET NEW.KorisnickoIme = LOWER(TRIM(NEW.KorisnickoIme));
    CALL provjeri_korisnika(NEW.Email, NEW.KorisnickoIme, NEW.Lozinka);
    IF NEW.Plata IS NULL OR NEW.Plata <= 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Plata mora biti veća od nule';
    END IF;
END ;;

CREATE TRIGGER trg_kupac_bi BEFORE INSERT ON kupac
FOR EACH ROW
BEGIN
    SET NEW.Email = LOWER(TRIM(NEW.Email));
    SET NEW.KorisnickoIme = LOWER(TRIM(NEW.KorisnickoIme));
    CALL provjeri_korisnika(NEW.Email, NEW.KorisnickoIme, NEW.Lozinka);
END ;;

CREATE TRIGGER trg_kupac_bu BEFORE UPDATE ON kupac
FOR EACH ROW
BEGIN
    SET NEW.Email = LOWER(TRIM(NEW.Email));
    SET NEW.KorisnickoIme = LOWER(TRIM(NEW.KorisnickoIme));
    CALL provjeri_korisnika(NEW.Email, NEW.KorisnickoIme, NEW.Lozinka);
END ;;

DELIMITER ;
