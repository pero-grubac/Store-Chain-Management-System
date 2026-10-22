-- =====================================================================
--  Funkcije: jedno mjesto za pravila o cijenama, popustima i zalihama.
--  Koriste ih procedure, pogledi i trigeri, pa aplikacija nikad ne racuna
--  konacnu cijenu sama.
-- =====================================================================

SET NAMES utf8mb4;

DROP FUNCTION IF EXISTS fn_aktivni_popust;
DROP FUNCTION IF EXISTS fn_cijena_sa_popustom;
DROP FUNCTION IF EXISTS fn_kupon_popust;
DROP FUNCTION IF EXISTS fn_status_zaliha;
DROP FUNCTION IF EXISTS fn_status_perioda;

DELIMITER ;;

-- Najveci popust (akcija) koji vazi za proizvod na dati dan, 0 ako nema
CREATE FUNCTION fn_aktivni_popust(pIdProizvoda INT, pDatum DATE)
RETURNS INT
READS SQL DATA
BEGIN
    DECLARE vPopust INT;

    SELECT COALESCE(MAX(p.Popust), 0) INTO vPopust
    FROM proizvod_has_popust pp
    JOIN popust p ON p.idPopust = pp.Popust_idPopust
    WHERE pp.PROIZVOD_idPROIZVOD = pIdProizvoda
      AND pDatum BETWEEN p.Start AND p.End;

    RETURN vPopust;
END ;;

-- Cijena proizvoda nakon akcije, zaokruzena na 2 decimale
CREATE FUNCTION fn_cijena_sa_popustom(pIdProizvoda INT, pDatum DATE)
RETURNS DECIMAL(10,2)
READS SQL DATA
BEGIN
    DECLARE vCijena DECIMAL(10,2);

    SELECT Cijena INTO vCijena FROM proizvod WHERE idPROIZVOD = pIdProizvoda;

    RETURN ROUND(vCijena * (100 - fn_aktivni_popust(pIdProizvoda, pDatum)) / 100, 2);
END ;;

-- Popust kupona za proizvod: 0 ako kupon ne postoji, nije vazeci tog dana
-- ili nije vezan za taj proizvod
CREATE FUNCTION fn_kupon_popust(pSifra VARCHAR(20), pIdProizvoda INT, pDatum DATE)
RETURNS INT
READS SQL DATA
BEGIN
    DECLARE vPopust INT;

    SELECT COALESCE(MAX(k.popust), 0) INTO vPopust
    FROM kupon k
    JOIN proizvod_has_kupon pk ON pk.Kupon_idKupon = k.idKupon
    WHERE k.Sifra = UPPER(TRIM(pSifra))
      AND pk.PROIZVOD_idPROIZVOD = pIdProizvoda
      AND pDatum >= k.Start
      AND (k.end IS NULL OR pDatum <= k.end);

    RETURN vPopust;
END ;;

CREATE FUNCTION fn_status_zaliha(pZalihe INT, pMinZalihe INT)
RETURNS VARCHAR(10)
DETERMINISTIC
NO SQL
BEGIN
    RETURN CASE
        WHEN pZalihe <= 0 THEN 'Nema'
        WHEN pZalihe <= pMinZalihe THEN 'Nisko'
        ELSE 'OK'
    END;
END ;;

-- Status akcije ili kupona u odnosu na danasnji dan
CREATE FUNCTION fn_status_perioda(pStart DATE, pEnd DATE)
RETURNS VARCHAR(10)
NOT DETERMINISTIC
NO SQL
BEGIN
    RETURN CASE
        WHEN CURDATE() < pStart THEN 'Najavljen'
        WHEN pEnd IS NOT NULL AND CURDATE() > pEnd THEN 'Istekao'
        ELSE 'Aktivan'
    END;
END ;;

DELIMITER ;
