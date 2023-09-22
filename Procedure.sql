-- MySQL dump 10.13  Distrib 8.0.32, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: prodavnica
-- ------------------------------------------------------
-- Server version	8.0.32

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Dumping routines for database 'prodavnica'
--
/*!50003 DROP PROCEDURE IF EXISTS `detalji_racuna` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`pero`@`localhost` PROCEDURE `detalji_racuna`(IN pIdKupca INT)
BEGIN
    SELECT
        r.idRACUN AS 'ID racuna',
        r.DatumKupovine AS 'Datum kupovine',
        s.Kolicina AS 'Kolicina',
        s.Cijena AS 'Cijena',
        pd.Ime AS 'Ime prodavnice',
        pz.Naziv AS 'Naziv proizvoda'
    FROM
        stavka s
    INNER JOIN
        racun r ON s.RACUN_idRACUN = r.idRACUN
    INNER JOIN
        prodavnica pd ON r.PRODVNICA_idProdavnica = pd.idProdavnica
    INNER JOIN
        proizvod pz ON s.PROIZVOD_idPROIZVOD = pz.idPROIZVOD
    WHERE
        r.KUPAC_idKUPAC = pIdKupca;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `dodaj_administratora` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`pero`@`localhost` PROCEDURE `dodaj_administratora`(
    IN pIme VARCHAR(50),
    IN pPrezime VARCHAR(50),
    IN pEmail VARCHAR(100),
    IN pLozinka varchar(255),
    IN pKorisnickoIme varchar(50)
)
BEGIN
    INSERT INTO Prodavnica.administrator (Ime, Prezime, Email, Lozinka, KorisnickoIme)
    VALUES (pIme, pPrezime, pEmail, pLozinka ,pKorisnickoIme);
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `dodaj_kupca` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`pero`@`localhost` PROCEDURE `dodaj_kupca`(
    IN pKorisnickoIme VARCHAR(50),
    IN pLozinka VARCHAR(255),
    IN pEmail VARCHAR(45),
    IN pIdAdresa INT
)
BEGIN
    INSERT INTO Prodavnica.KUPAC (KorisnickoIme, Lozinka, Email, ADRESA_idADRESA)
    VALUES (pKorisnickoIme, pLozinka, pEmail, pIdAdresa);
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `dodaj_popust` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`pero`@`localhost` PROCEDURE `dodaj_popust`(
    IN pIdProizvoda INT,
    IN pPopust INT,
    IN pStartDate DATE,
    IN pEndDate DATE
)
BEGIN
    INSERT INTO Prodavnica.Popust (Popust, Start, End)
    VALUES (pPopust, pStartDate, pEndDate);
    
    INSERT INTO Prodavnica.PROIZVOD_has_Popust (PROIZVOD_idPROIZVOD, Popust_idPopust)
    VALUES (pIdProizvoda, LAST_INSERT_ID());
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `dodaj_proizvod` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`pero`@`localhost` PROCEDURE `dodaj_proizvod`(
    IN pNaziv VARCHAR(45),
    IN pCijena double,
    IN pDatumIsteka DATETIME,
    IN pBarKod VARCHAR(45),
    IN pIdProizvodjaca INT,
    IN pOpis VARCHAR(255),
    IN pZalihe INT,
    IN pIdProdavnice INT
)
BEGIN
    INSERT INTO Prodavnica.PROIZVOD (Naziv, DatumIsteka, BarKod, PROIZVODJAC_idPROIZVODJAC, Opis, Zalihe,Cijena,PRODVNICA_idProdavnica)
    VALUES (pNaziv, pDatumIsteka, pBarKod, pIdProizvodjaca, pOpis, pZalihe,pCijena,pIdProdavnice);
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `dodaj_proizvodjaca` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`pero`@`localhost` PROCEDURE `dodaj_proizvodjaca`(
    IN pIme VARCHAR(45),
    IN pEmail VARCHAR(45),
	IN pIdAdresa INT
)
BEGIN
    INSERT INTO proizvodjac (Ime, Email, ADRESA_idADRESA)
    VALUES (pIme, pEmail, pIdAdresa);
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `dodaj_radnika` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`pero`@`localhost` PROCEDURE `dodaj_radnika`(
    IN pIme VARCHAR(15),
    IN pPrezime VARCHAR(20),
    IN pEmail VARCHAR(20),
    IN pBrojTelefona VARCHAR(20),
    IN pIdProdavnice INT,
    IN pPlata DOUBLE,
    IN pLozinka varchar(255),
    IN pKorisnickoIme varchar(50)
)
BEGIN
    INSERT INTO Prodavnica.ZAPOSLENI (Ime, Prezime, Email, BrojTelefona, Prodavnica_idProdavnica, Plata,Lozinka,KorisnickoIme)
    VALUES (pIme, pPrezime, pEmail, pBrojTelefona, pIdProdavnice, pPlata,pLozinka,pKorisnickoIme);
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `get_proizvod_by_prodavnica` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`pero`@`localhost` PROCEDURE `get_proizvod_by_prodavnica`(IN pIdProdavnica INT)
BEGIN
    SELECT
        p.idPROIZVOD AS 'Id proizvoda',
        p.Naziv AS 'Naziv',
        p.Cijena AS 'Cijena',
        p.DatumIsteka AS 'Rok trajanja',
        p.BarKod AS 'Bar kod',
        p.Opis AS 'Opis',
        p.zalihe AS 'Zalihe',
        pro.idPROIZVODJAC as 'Id proizvodjaca',
        pro.Ime AS 'Ime proizvodjaca',
        pro.Email AS 'Kontak email',
        a.Drzava AS 'Drzava porijekla'
    FROM proizvod p
    LEFT JOIN proizvodjac pro ON pro.idPROIZVODJAC = p.PROIZVODJAC_idPROIZVODJAC
    LEFT JOIN adresa a ON a.idADRESA = pro.ADRESA_idADRESA
    WHERE p.PRODVNICA_idProdavnica=pIdProdavnica;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `kreiraj_kupon` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`pero`@`localhost` PROCEDURE `kreiraj_kupon`(
    IN pIdProizvoda INT,
    IN pPopust INT,
    IN pSifra INT,
    IN pStart DATE,
    IN pEnd DATE
)
BEGIN
    INSERT INTO Prodavnica.Kupon (Sifra, Start, end,popust)
    VALUES (pPopust, pStart, pEnd,pPopust);
    
    INSERT INTO Prodavnica.PROIZVOD_has_Popust (PROIZVOD_idPROIZVOD, Kupon_idKupon)
    VALUES (pIdProizvoda, LAST_INSERT_ID());
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `narucivanje` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`pero`@`localhost` PROCEDURE `narucivanje`(
	IN pIdKupca INT,
    IN pIdProizvoda JSON,
    IN pIdProdavnice INT,
    IN pKolicina JSON,
    IN pCijena JSON,
    IN pDatumKupovine DATE
)
BEGIN
	DECLARE productIndex INT;
    DECLARE productCount INT;
    DECLARE currentRacunId INT;
    
    INSERT INTO Prodavnica.RACUN (KUPAC_idKUPAC, DatumKupovine, PRODVNICA_idProdavnica)
    VALUES (pIdKupca, pDatumKupovine, pIdProdavnice);
    
    SET currentRacunId = LAST_INSERT_ID();
	SET productCount = JSON_LENGTH(pKolicina);
    SET productIndex = 0;
    
    WHILE productIndex < productCount DO
        INSERT INTO Prodavnica.stavka (RACUN_idRACUN, PROIZVOD_idPROIZVOD, Kolicina, Cijena)
        VALUES (currentRacunId,JSON_EXTRACT(pIdProizvoda, CONCAT('$[', productIndex, ']')), JSON_EXTRACT(pKolicina, CONCAT('$[', productIndex, ']')),
                JSON_EXTRACT(pCijena, CONCAT('$[', productIndex, ']')));

        SET productIndex = productIndex + 1;
    END WHILE;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `obrisi_kupon` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`pero`@`localhost` PROCEDURE `obrisi_kupon`(
    IN pIdKupona INT
)
BEGIN
    DELETE FROM Prodavnica.Kupon
    WHERE idKupon = pIdKupona;
    
    DELETE FROM Prodavnica.Proizvod_has_kupon
    WHERE idKupon = pIdKupona;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `obrisi_popust` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`pero`@`localhost` PROCEDURE `obrisi_popust`(
    IN pIdPopusta INT
)
BEGIN
    DELETE FROM Prodavnica.Popust
    WHERE idPopust=pIdPopusta;
    
    DELETE FROM Prodavnica.Proizvod_has_popust
    WHERE idPopust=pIdPopusta;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `obrisi_proizvod` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`pero`@`localhost` PROCEDURE `obrisi_proizvod`(
    IN pIdProizvoda INT
)
BEGIN
    DELETE FROM Prodavnica.PROIZVOD
    WHERE idPROIZVOD = pIdProizvoda;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `update_proizvod` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`pero`@`localhost` PROCEDURE `update_proizvod`(
	IN pIdProizvoda INT,
   IN pNaziv VARCHAR(45),
    IN pDatumIsteka DATETIME,
    IN pBarKod VARCHAR(45),
    IN pIdProizvodjaca INT,
    IN pOpis VARCHAR(255),
    IN pZalihe INT,
    IN pCijena double
)
BEGIN
    UPDATE Prodavnica.PROIZVOD
    SET Naziv = pNaziv, DatumIsteka = pDatumIsteka, BarKod = pBarKod, PROIZVODJAC_idPROIZVODJAC = pIdProizvodjaca, Opis = pOpis, Zalihe = pZalihe, Cijena=pCijena
    WHERE idPROIZVOD = pIdProizvoda;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2023-08-22 10:39:12
