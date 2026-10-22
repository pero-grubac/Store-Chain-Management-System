-- =====================================================================
--  Pogledi: aplikacija cita podatke kroz njih, imena kolona odgovaraju
--  svojstvima klasa u OnlineKupovinaDB.
-- =====================================================================

SET NAMES utf8mb4;

CREATE OR REPLACE VIEW v_prodavnice AS
SELECT pr.idProdavnica AS Id, pr.Ime AS Naziv,
       a.idADRESA AS IdAdrese, a.Grad, a.Ulica, a.Drzava,
       (SELECT COUNT(*) FROM zaposleni z WHERE z.Prodavnica_idProdavnica = pr.idProdavnica) AS BrojZaposlenih,
       (SELECT COUNT(*) FROM proizvod p WHERE p.PRODVNICA_idProdavnica = pr.idProdavnica AND p.Aktivan = 1) AS BrojProizvoda
FROM prodavnica pr
JOIN adresa a ON a.idADRESA = pr.ADRESA_idADRESA;

CREATE OR REPLACE VIEW v_proizvodjaci AS
SELECT pj.idPROIZVODJAC AS Id, pj.Ime AS Naziv, pj.Email,
       a.idADRESA AS IdAdrese, a.Grad, a.Ulica, a.Drzava,
       (SELECT COUNT(*) FROM proizvod p WHERE p.PROIZVODJAC_idPROIZVODJAC = pj.idPROIZVODJAC AND p.Aktivan = 1) AS BrojProizvoda
FROM proizvodjac pj
JOIN adresa a ON a.idADRESA = pj.ADRESA_idADRESA;

CREATE OR REPLACE VIEW v_proizvodi AS
SELECT p.idPROIZVOD AS Id,
       p.PRODVNICA_idProdavnica AS IdProdavnice,
       pr.Ime AS Prodavnica,
       p.Naziv, p.BarKod, p.Opis, p.Cijena,
       fn_aktivni_popust(p.idPROIZVOD, CURDATE()) AS Popust,
       fn_cijena_sa_popustom(p.idPROIZVOD, CURDATE()) AS CijenaSaPopustom,
       p.Zalihe, p.MinZalihe,
       fn_status_zaliha(p.Zalihe, p.MinZalihe) AS StatusZaliha,
       p.DatumIsteka,
       pj.idPROIZVODJAC AS IdProizvodjaca,
       pj.Ime AS Proizvodjac,
       a.Drzava AS DrzavaPorijekla,
       p.Aktivan
FROM proizvod p
JOIN prodavnica pr ON pr.idProdavnica = p.PRODVNICA_idProdavnica
JOIN proizvodjac pj ON pj.idPROIZVODJAC = p.PROIZVODJAC_idPROIZVODJAC
JOIN adresa a ON a.idADRESA = pj.ADRESA_idADRESA;

CREATE OR REPLACE VIEW v_niske_zalihe AS
SELECT * FROM v_proizvodi
WHERE Aktivan = 1 AND StatusZaliha <> 'OK';

CREATE OR REPLACE VIEW v_zaposleni AS
SELECT z.idZAPOSLENI AS Id, z.Ime, z.Prezime, z.KorisnickoIme, z.Email, z.BrojTelefona,
       z.Plata, z.Prodavnica_idProdavnica AS IdProdavnice, pr.Ime AS Prodavnica
FROM zaposleni z
JOIN prodavnica pr ON pr.idProdavnica = z.Prodavnica_idProdavnica;

CREATE OR REPLACE VIEW v_kupci AS
SELECT k.idKUPAC AS Id, k.KorisnickoIme, k.Email,
       a.idADRESA AS IdAdrese, a.Grad, a.Ulica, a.Drzava,
       COUNT(r.idRACUN) AS BrojRacuna,
       COALESCE(SUM(r.Ukupno), 0) AS Potroseno
FROM kupac k
JOIN adresa a ON a.idADRESA = k.ADRESA_idADRESA
LEFT JOIN racun r ON r.KUPAC_idKUPAC = k.idKUPAC AND r.Status = 'Placen'
GROUP BY k.idKUPAC, k.KorisnickoIme, k.Email, a.idADRESA, a.Grad, a.Ulica, a.Drzava;

CREATE OR REPLACE VIEW v_administratori AS
SELECT idAdministrator AS Id, Ime, Prezime, KorisnickoIme, Email
FROM administrator;

CREATE OR REPLACE VIEW v_racuni AS
SELECT r.idRACUN AS Id, r.DatumKupovine AS Datum,
       r.KUPAC_idKUPAC AS IdKupca, k.KorisnickoIme AS Kupac,
       r.PRODVNICA_idProdavnica AS IdProdavnice, pr.Ime AS Prodavnica,
       ku.Sifra AS Kupon,
       (SELECT COALESCE(SUM(s.Kolicina), 0) FROM stavka s WHERE s.RACUN_idRACUN = r.idRACUN) AS BrojArtikala,
       r.Ukupno, r.Status
FROM racun r
JOIN kupac k ON k.idKUPAC = r.KUPAC_idKUPAC
JOIN prodavnica pr ON pr.idProdavnica = r.PRODVNICA_idProdavnica
LEFT JOIN kupon ku ON ku.idKupon = r.KUPON_idKupon;

-- Prodate stavke sa placenih racuna: osnova za izvjestaje
CREATE OR REPLACE VIEW v_prodaja AS
SELECT r.idRACUN AS IdRacuna, r.DatumKupovine AS Datum,
       r.PRODVNICA_idProdavnica AS IdProdavnice,
       p.idPROIZVOD AS IdProizvoda, p.Naziv, p.BarKod,
       s.Kolicina, s.Cijena, s.Kolicina * s.Cijena AS Iznos
FROM stavka s
JOIN racun r ON r.idRACUN = s.RACUN_idRACUN AND r.Status = 'Placen'
JOIN proizvod p ON p.idPROIZVOD = s.PROIZVOD_idPROIZVOD;

CREATE OR REPLACE VIEW v_popusti AS
SELECT po.idPopust AS Id, p.idPROIZVOD AS IdProizvoda, p.Naziv AS Proizvod, pr.Ime AS Prodavnica,
       po.Popust, po.Start AS Od, po.End AS Do, fn_status_perioda(po.Start, po.End) AS Status
FROM popust po
JOIN proizvod_has_popust pp ON pp.Popust_idPopust = po.idPopust
JOIN proizvod p ON p.idPROIZVOD = pp.PROIZVOD_idPROIZVOD
JOIN prodavnica pr ON pr.idProdavnica = p.PRODVNICA_idProdavnica;

CREATE OR REPLACE VIEW v_kuponi AS
SELECT k.idKupon AS Id, k.Sifra, k.popust AS Popust, k.Start AS Od, k.end AS Do,
       fn_status_perioda(k.Start, k.end) AS Status,
       COUNT(pk.PROIZVOD_idPROIZVOD) AS BrojProizvoda,
       COALESCE(GROUP_CONCAT(DISTINCT p.Naziv ORDER BY p.Naziv SEPARATOR ', '), '') AS Proizvodi
FROM kupon k
LEFT JOIN proizvod_has_kupon pk ON pk.Kupon_idKupon = k.idKupon
LEFT JOIN proizvod p ON p.idPROIZVOD = pk.PROIZVOD_idPROIZVOD
GROUP BY k.idKupon, k.Sifra, k.popust, k.Start, k.end;
