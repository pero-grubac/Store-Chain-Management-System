-- =====================================================================
--  Store Chain Management System - sema baze `prodavnica`
--  MySQL 8.4 | utf8mb4 | InnoDB
--
--  Redoslijed skripti:
--    01-schema.sql      tabele, kljucevi, CHECK ogranicenja, indeksi
--    02-functions.sql   skalarne funkcije (cijene, popusti, status zaliha)
--    03-procedures.sql  procedure za sve operacije koje mijenjaju vise tabela
--    04-triggers.sql    trigeri: validacija, zalihe, iznos racuna, istorija zaliha
--    05-views.sql       pogledi za aplikaciju i izvjestaje
--    06-seed.sql        demo podaci
-- =====================================================================

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

DROP TABLE IF EXISTS zalihe_istorija;
DROP TABLE IF EXISTS stavka;
DROP TABLE IF EXISTS racun;
DROP TABLE IF EXISTS proizvod_has_kupon;
DROP TABLE IF EXISTS proizvod_has_popust;
DROP TABLE IF EXISTS kategorija_has_proizvod;
DROP TABLE IF EXISTS dodavanjeproizvoda;
DROP TABLE IF EXISTS kupon;
DROP TABLE IF EXISTS popust;
DROP TABLE IF EXISTS kategorija;
DROP TABLE IF EXISTS proizvod;
DROP TABLE IF EXISTS zaposleni;
DROP TABLE IF EXISTS kupac;
DROP TABLE IF EXISTS administrator;
DROP TABLE IF EXISTS proizvodjac;
DROP TABLE IF EXISTS prodavnica;
DROP TABLE IF EXISTS adresa;

SET FOREIGN_KEY_CHECKS = 1;

-- ---------------------------------------------------------------------
--  Adrese i firme
-- ---------------------------------------------------------------------

CREATE TABLE adresa (
  idADRESA  INT          NOT NULL AUTO_INCREMENT,
  Grad      VARCHAR(45)  NOT NULL,
  Ulica     VARCHAR(100) NOT NULL,
  Drzava    VARCHAR(45)  NOT NULL,
  PRIMARY KEY (idADRESA)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE prodavnica (
  idProdavnica     INT         NOT NULL AUTO_INCREMENT,
  Ime              VARCHAR(45) NOT NULL,
  ADRESA_idADRESA  INT         NOT NULL,
  PRIMARY KEY (idProdavnica),
  UNIQUE KEY uq_prodavnica_ime (Ime),
  KEY fk_prodavnica_adresa_idx (ADRESA_idADRESA),
  CONSTRAINT fk_prodavnica_adresa FOREIGN KEY (ADRESA_idADRESA) REFERENCES adresa (idADRESA)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE proizvodjac (
  idPROIZVODJAC    INT          NOT NULL AUTO_INCREMENT,
  Ime              VARCHAR(45)  NOT NULL,
  Email            VARCHAR(100) NOT NULL,
  ADRESA_idADRESA  INT          NOT NULL,
  PRIMARY KEY (idPROIZVODJAC),
  UNIQUE KEY uq_proizvodjac_ime (Ime),
  KEY fk_proizvodjac_adresa_idx (ADRESA_idADRESA),
  CONSTRAINT fk_proizvodjac_adresa FOREIGN KEY (ADRESA_idADRESA) REFERENCES adresa (idADRESA)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- ---------------------------------------------------------------------
--  Korisnici: tri uloge, svaka sa svojom tabelom.
--  Lozinke su iskljucivo BCrypt hash (provjerava trigger).
-- ---------------------------------------------------------------------

CREATE TABLE administrator (
  idAdministrator  INT          NOT NULL AUTO_INCREMENT,
  Ime              VARCHAR(50)  NOT NULL,
  Prezime          VARCHAR(50)  NOT NULL,
  Email            VARCHAR(100) NOT NULL,
  Lozinka          VARCHAR(255) NOT NULL,
  KorisnickoIme    VARCHAR(50)  NOT NULL,
  PRIMARY KEY (idAdministrator),
  UNIQUE KEY uq_administrator_email (Email),
  UNIQUE KEY uq_administrator_korisnicko_ime (KorisnickoIme)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE zaposleni (
  idZAPOSLENI              INT           NOT NULL AUTO_INCREMENT,
  Ime                      VARCHAR(50)   NOT NULL,
  Prezime                  VARCHAR(50)   NOT NULL,
  Email                    VARCHAR(100)  NOT NULL,
  BrojTelefona             VARCHAR(20)   DEFAULT NULL,
  Prodavnica_idProdavnica  INT           NOT NULL,
  Plata                    DECIMAL(10,2) NOT NULL,
  Lozinka                  VARCHAR(255)  NOT NULL,
  KorisnickoIme            VARCHAR(50)   NOT NULL,
  PRIMARY KEY (idZAPOSLENI),
  UNIQUE KEY uq_zaposleni_email (Email),
  UNIQUE KEY uq_zaposleni_korisnicko_ime (KorisnickoIme),
  KEY fk_zaposleni_prodavnica_idx (Prodavnica_idProdavnica),
  CONSTRAINT fk_zaposleni_prodavnica FOREIGN KEY (Prodavnica_idProdavnica) REFERENCES prodavnica (idProdavnica),
  CONSTRAINT chk_zaposleni_plata CHECK (Plata > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE kupac (
  idKUPAC          INT          NOT NULL AUTO_INCREMENT,
  Lozinka          VARCHAR(255) NOT NULL,
  Email            VARCHAR(100) NOT NULL,
  ADRESA_idADRESA  INT          NOT NULL,
  KorisnickoIme    VARCHAR(50)  NOT NULL,
  PRIMARY KEY (idKUPAC),
  UNIQUE KEY uq_kupac_email (Email),
  UNIQUE KEY uq_kupac_korisnicko_ime (KorisnickoIme),
  KEY fk_kupac_adresa_idx (ADRESA_idADRESA),
  CONSTRAINT fk_kupac_adresa FOREIGN KEY (ADRESA_idADRESA) REFERENCES adresa (idADRESA)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- ---------------------------------------------------------------------
--  Proizvodi: svaka prodavnica ima svoj red za proizvod (svoje zalihe i cijenu),
--  pa je bar kod jedinstven unutar prodavnice, a ne u cijelom lancu.
-- ---------------------------------------------------------------------

CREATE TABLE proizvod (
  idPROIZVOD                 INT           NOT NULL AUTO_INCREMENT,
  Naziv                      VARCHAR(45)   NOT NULL,
  DatumIsteka                DATE          DEFAULT NULL,
  BarKod                     VARCHAR(45)   NOT NULL,
  PROIZVODJAC_idPROIZVODJAC  INT           NOT NULL,
  Opis                       VARCHAR(255)  DEFAULT NULL,
  Zalihe                     INT           NOT NULL DEFAULT 0,
  MinZalihe                  INT           NOT NULL DEFAULT 10,
  Cijena                     DECIMAL(10,2) NOT NULL,
  PRODVNICA_idProdavnica     INT           NOT NULL,
  Aktivan                    TINYINT(1)    NOT NULL DEFAULT 1,
  PRIMARY KEY (idPROIZVOD),
  UNIQUE KEY uq_proizvod_barkod_prodavnica (PRODVNICA_idProdavnica, BarKod),
  KEY fk_proizvod_proizvodjac_idx (PROIZVODJAC_idPROIZVODJAC),
  KEY ix_proizvod_barkod (BarKod),
  CONSTRAINT fk_proizvod_proizvodjac FOREIGN KEY (PROIZVODJAC_idPROIZVODJAC) REFERENCES proizvodjac (idPROIZVODJAC),
  CONSTRAINT fk_proizvod_prodavnica FOREIGN KEY (PRODVNICA_idProdavnica) REFERENCES prodavnica (idProdavnica),
  CONSTRAINT chk_proizvod_cijena CHECK (Cijena >= 0),
  CONSTRAINT chk_proizvod_zalihe CHECK (Zalihe >= 0),
  CONSTRAINT chk_proizvod_min_zalihe CHECK (MinZalihe >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE kategorija (
  Naziv VARCHAR(45) NOT NULL,
  PRIMARY KEY (Naziv)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE kategorija_has_proizvod (
  Kategorija_Naziv     VARCHAR(45) NOT NULL,
  PROIZVOD_idPROIZVOD  INT         NOT NULL,
  PRIMARY KEY (Kategorija_Naziv, PROIZVOD_idPROIZVOD),
  KEY fk_khp_proizvod_idx (PROIZVOD_idPROIZVOD),
  CONSTRAINT fk_khp_kategorija FOREIGN KEY (Kategorija_Naziv) REFERENCES kategorija (Naziv) ON UPDATE CASCADE,
  CONSTRAINT fk_khp_proizvod FOREIGN KEY (PROIZVOD_idPROIZVOD) REFERENCES proizvod (idPROIZVOD) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Ko je i kada dodao proizvod u katalog prodavnice
CREATE TABLE dodavanjeproizvoda (
  PROIZVOD_idPROIZVOD    INT  NOT NULL,
  ZAPOSLENI_idZAPOSLENI  INT  NOT NULL,
  Datum                  DATE NOT NULL,
  PRIMARY KEY (PROIZVOD_idPROIZVOD, ZAPOSLENI_idZAPOSLENI),
  KEY fk_dp_zaposleni_idx (ZAPOSLENI_idZAPOSLENI),
  CONSTRAINT fk_dp_zaposleni FOREIGN KEY (ZAPOSLENI_idZAPOSLENI) REFERENCES zaposleni (idZAPOSLENI) ON DELETE CASCADE,
  CONSTRAINT fk_dp_proizvod FOREIGN KEY (PROIZVOD_idPROIZVOD) REFERENCES proizvod (idPROIZVOD) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- ---------------------------------------------------------------------
--  Akcije (popust na proizvod u periodu) i kuponi (sifra koju kupac upise)
-- ---------------------------------------------------------------------

CREATE TABLE popust (
  idPopust  INT  NOT NULL AUTO_INCREMENT,
  Popust    INT  NOT NULL,
  Start     DATE NOT NULL,
  End       DATE NOT NULL,
  PRIMARY KEY (idPopust),
  CONSTRAINT chk_popust_procenat CHECK (Popust BETWEEN 1 AND 90),
  CONSTRAINT chk_popust_period CHECK (End >= Start)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE proizvod_has_popust (
  PROIZVOD_idPROIZVOD  INT NOT NULL,
  Popust_idPopust      INT NOT NULL,
  PRIMARY KEY (PROIZVOD_idPROIZVOD, Popust_idPopust),
  KEY fk_php_popust_idx (Popust_idPopust),
  CONSTRAINT fk_php_popust FOREIGN KEY (Popust_idPopust) REFERENCES popust (idPopust) ON DELETE CASCADE,
  CONSTRAINT fk_php_proizvod FOREIGN KEY (PROIZVOD_idPROIZVOD) REFERENCES proizvod (idPROIZVOD) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE kupon (
  idKupon  INT         NOT NULL AUTO_INCREMENT,
  Sifra    VARCHAR(20) NOT NULL,
  Start    DATE        NOT NULL,
  end      DATE        DEFAULT NULL,
  popust   INT         NOT NULL,
  PRIMARY KEY (idKupon),
  UNIQUE KEY uq_kupon_sifra (Sifra),
  CONSTRAINT chk_kupon_procenat CHECK (popust BETWEEN 1 AND 90),
  CONSTRAINT chk_kupon_period CHECK (end IS NULL OR end >= Start)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE proizvod_has_kupon (
  PROIZVOD_idPROIZVOD  INT NOT NULL,
  Kupon_idKupon        INT NOT NULL,
  PRIMARY KEY (PROIZVOD_idPROIZVOD, Kupon_idKupon),
  KEY fk_phk_kupon_idx (Kupon_idKupon),
  CONSTRAINT fk_phk_kupon FOREIGN KEY (Kupon_idKupon) REFERENCES kupon (idKupon) ON DELETE CASCADE,
  CONSTRAINT fk_phk_proizvod FOREIGN KEY (PROIZVOD_idPROIZVOD) REFERENCES proizvod (idPROIZVOD) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- ---------------------------------------------------------------------
--  Racuni. Ukupno odrzavaju trigeri na tabeli stavka;
--  storno vraca robu na zalihe (trigger na racunu).
-- ---------------------------------------------------------------------

CREATE TABLE racun (
  idRACUN                 INT            NOT NULL AUTO_INCREMENT,
  DatumKupovine           DATETIME       NOT NULL DEFAULT CURRENT_TIMESTAMP,
  KUPAC_idKUPAC           INT            NOT NULL,
  PRODVNICA_idProdavnica  INT            NOT NULL,
  KUPON_idKupon           INT            DEFAULT NULL,
  Ukupno                  DECIMAL(12,2)  NOT NULL DEFAULT 0,
  Status                  ENUM('Placen','Storniran') NOT NULL DEFAULT 'Placen',
  PRIMARY KEY (idRACUN),
  KEY fk_racun_kupac_idx (KUPAC_idKUPAC),
  KEY fk_racun_prodavnica_idx (PRODVNICA_idProdavnica),
  KEY fk_racun_kupon_idx (KUPON_idKupon),
  KEY ix_racun_datum (DatumKupovine),
  CONSTRAINT fk_racun_kupac FOREIGN KEY (KUPAC_idKUPAC) REFERENCES kupac (idKUPAC),
  CONSTRAINT fk_racun_prodavnica FOREIGN KEY (PRODVNICA_idProdavnica) REFERENCES prodavnica (idProdavnica),
  CONSTRAINT fk_racun_kupon FOREIGN KEY (KUPON_idKupon) REFERENCES kupon (idKupon) ON DELETE SET NULL,
  CONSTRAINT chk_racun_ukupno CHECK (Ukupno >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Cijena je konacna jedinicna cijena nakon popusta; PunaCijena i Popust cuvaju kako je nastala
CREATE TABLE stavka (
  RACUN_idRACUN        INT           NOT NULL,
  PROIZVOD_idPROIZVOD  INT           NOT NULL,
  Kolicina             INT           NOT NULL,
  PunaCijena           DECIMAL(10,2) NOT NULL,
  Popust               INT           NOT NULL DEFAULT 0,
  Cijena               DECIMAL(10,2) NOT NULL,
  PRIMARY KEY (RACUN_idRACUN, PROIZVOD_idPROIZVOD),
  KEY fk_stavka_proizvod_idx (PROIZVOD_idPROIZVOD),
  CONSTRAINT fk_stavka_racun FOREIGN KEY (RACUN_idRACUN) REFERENCES racun (idRACUN) ON DELETE CASCADE,
  CONSTRAINT fk_stavka_proizvod FOREIGN KEY (PROIZVOD_idPROIZVOD) REFERENCES proizvod (idPROIZVOD),
  CONSTRAINT chk_stavka_kolicina CHECK (Kolicina > 0),
  CONSTRAINT chk_stavka_cijena CHECK (Cijena >= 0 AND PunaCijena >= 0),
  CONSTRAINT chk_stavka_popust CHECK (Popust BETWEEN 0 AND 90)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- ---------------------------------------------------------------------
--  Istorija zaliha: svaku promjenu kolicine upisuje trigger na proizvodu.
--  Razlog i korisnika postavljaju procedure kroz @razlog_zaliha i @korisnik.
-- ---------------------------------------------------------------------

CREATE TABLE zalihe_istorija (
  idIstorija           INT          NOT NULL AUTO_INCREMENT,
  PROIZVOD_idPROIZVOD  INT          NOT NULL,
  StaraKolicina        INT          NOT NULL,
  NovaKolicina         INT          NOT NULL,
  Promjena             INT          AS (NovaKolicina - StaraKolicina) STORED,
  Razlog               VARCHAR(30)  NOT NULL,
  Korisnik             VARCHAR(50)  DEFAULT NULL,
  Vrijeme              DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (idIstorija),
  KEY ix_zi_proizvod_vrijeme (PROIZVOD_idPROIZVOD, Vrijeme),
  CONSTRAINT fk_zi_proizvod FOREIGN KEY (PROIZVOD_idPROIZVOD) REFERENCES proizvod (idPROIZVOD) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
