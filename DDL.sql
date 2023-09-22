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
-- Table structure for table `administrator`
--

DROP TABLE IF EXISTS `administrator`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `administrator` (
  `idAdministrator` int NOT NULL AUTO_INCREMENT,
  `Ime` varchar(50) NOT NULL,
  `Prezime` varchar(50) NOT NULL,
  `Email` varchar(100) NOT NULL,
  `Lozinka` varchar(255) NOT NULL,
  `KorisnickoIme` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`idAdministrator`),
  UNIQUE KEY `Email` (`Email`),
  UNIQUE KEY `KorisnickoIme` (`KorisnickoIme`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `adresa`
--

DROP TABLE IF EXISTS `adresa`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `adresa` (
  `idADRESA` int NOT NULL AUTO_INCREMENT,
  `Grad` varchar(45) NOT NULL,
  `Ulica` varchar(45) NOT NULL,
  `Drzava` varchar(45) NOT NULL,
  PRIMARY KEY (`idADRESA`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `dodavanjeproizvoda`
--

DROP TABLE IF EXISTS `dodavanjeproizvoda`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `dodavanjeproizvoda` (
  `PROIZVOD_idPROIZVOD` int NOT NULL,
  `ZAPOSLENI_idZAPOSLENI` int NOT NULL,
  `Datum` date NOT NULL,
  PRIMARY KEY (`PROIZVOD_idPROIZVOD`,`ZAPOSLENI_idZAPOSLENI`),
  KEY `fk_PROIZVOD_has_ZAPOSLENI_ZAPOSLENI1_idx` (`ZAPOSLENI_idZAPOSLENI`),
  KEY `fk_PROIZVOD_has_ZAPOSLENI_PROIZVOD1_idx` (`PROIZVOD_idPROIZVOD`),
  CONSTRAINT `dodavanjeproizvoda_ibfk_1` FOREIGN KEY (`ZAPOSLENI_idZAPOSLENI`) REFERENCES `zaposleni` (`idZAPOSLENI`) ON DELETE CASCADE,
  CONSTRAINT `fk_PROIZVOD_has_ZAPOSLENI_PROIZVOD1` FOREIGN KEY (`PROIZVOD_idPROIZVOD`) REFERENCES `proizvod` (`idPROIZVOD`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `kategorija`
--

DROP TABLE IF EXISTS `kategorija`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `kategorija` (
  `Naziv` varchar(45) NOT NULL,
  PRIMARY KEY (`Naziv`),
  UNIQUE KEY `Naziv_UNIQUE` (`Naziv`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `kategorija_has_proizvod`
--

DROP TABLE IF EXISTS `kategorija_has_proizvod`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `kategorija_has_proizvod` (
  `Kategorija_Naziv` varchar(45) NOT NULL,
  `PROIZVOD_idPROIZVOD` int NOT NULL,
  PRIMARY KEY (`Kategorija_Naziv`,`PROIZVOD_idPROIZVOD`),
  KEY `fk_Kategorija_has_PROIZVOD_PROIZVOD1_idx` (`PROIZVOD_idPROIZVOD`),
  KEY `fk_Kategorija_has_PROIZVOD_Kategorija1_idx` (`Kategorija_Naziv`),
  CONSTRAINT `fk_Kategorija_has_PROIZVOD_Kategorija1` FOREIGN KEY (`Kategorija_Naziv`) REFERENCES `kategorija` (`Naziv`),
  CONSTRAINT `fk_Kategorija_has_PROIZVOD_PROIZVOD1` FOREIGN KEY (`PROIZVOD_idPROIZVOD`) REFERENCES `proizvod` (`idPROIZVOD`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `kupac`
--

DROP TABLE IF EXISTS `kupac`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `kupac` (
  `idKUPAC` int NOT NULL AUTO_INCREMENT,
  `Lozinka` varchar(255) DEFAULT NULL,
  `Email` varchar(45) NOT NULL,
  `ADRESA_idADRESA` int NOT NULL,
  `KorisnickoIme` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`idKUPAC`),
  UNIQUE KEY `Email_UNIQUE` (`Email`),
  UNIQUE KEY `KorisnickoIme` (`KorisnickoIme`),
  KEY `fk_KUPAC_ADRESA1_idx` (`ADRESA_idADRESA`),
  CONSTRAINT `fk_KUPAC_ADRESA1` FOREIGN KEY (`ADRESA_idADRESA`) REFERENCES `adresa` (`idADRESA`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `kupon`
--

DROP TABLE IF EXISTS `kupon`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `kupon` (
  `idKupon` int NOT NULL AUTO_INCREMENT,
  `Sifra` int NOT NULL,
  `Start` date NOT NULL,
  `end` date DEFAULT NULL,
  `popust` int NOT NULL,
  PRIMARY KEY (`idKupon`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `popust`
--

DROP TABLE IF EXISTS `popust`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `popust` (
  `idPopust` int NOT NULL AUTO_INCREMENT,
  `Popust` int NOT NULL,
  `Start` date NOT NULL,
  `End` date NOT NULL,
  PRIMARY KEY (`idPopust`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `prodavnica`
--

DROP TABLE IF EXISTS `prodavnica`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `prodavnica` (
  `idProdavnica` int NOT NULL AUTO_INCREMENT,
  `Ime` varchar(45) DEFAULT NULL,
  `ADRESA_idADRESA` int NOT NULL,
  PRIMARY KEY (`idProdavnica`),
  KEY `fk_Prodavnica_ADRESA1_idx` (`ADRESA_idADRESA`),
  CONSTRAINT `fk_Prodavnica_ADRESA1` FOREIGN KEY (`ADRESA_idADRESA`) REFERENCES `adresa` (`idADRESA`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `proizvod`
--

DROP TABLE IF EXISTS `proizvod`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `proizvod` (
  `idPROIZVOD` int NOT NULL AUTO_INCREMENT,
  `Naziv` varchar(45) NOT NULL,
  `DatumIsteka` date DEFAULT NULL,
  `BarKod` varchar(45) NOT NULL,
  `PROIZVODJAC_idPROIZVODJAC` int NOT NULL,
  `Opis` varchar(255) DEFAULT NULL,
  `Zalihe` int DEFAULT NULL,
  `Cijena` double DEFAULT NULL,
  `PRODVNICA_idProdavnica` int DEFAULT NULL,
  PRIMARY KEY (`idPROIZVOD`),
  UNIQUE KEY `BarKod_UNIQUE` (`BarKod`),
  KEY `fk_PROIZVOD_PROIZVODJAC1_idx` (`PROIZVODJAC_idPROIZVODJAC`),
  KEY `PRODVNICA_idProdavnica` (`PRODVNICA_idProdavnica`),
  CONSTRAINT `fk_PROIZVOD_PROIZVODJAC1` FOREIGN KEY (`PROIZVODJAC_idPROIZVODJAC`) REFERENCES `proizvodjac` (`idPROIZVODJAC`),
  CONSTRAINT `proizvod_ibfk_1` FOREIGN KEY (`PRODVNICA_idProdavnica`) REFERENCES `prodavnica` (`idProdavnica`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `proizvod_has_kupon`
--

DROP TABLE IF EXISTS `proizvod_has_kupon`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `proizvod_has_kupon` (
  `PROIZVOD_idPROIZVOD` int NOT NULL,
  `Kupon_idKupon` int NOT NULL,
  PRIMARY KEY (`PROIZVOD_idPROIZVOD`,`Kupon_idKupon`),
  KEY `fk_PROIZVOD_has_Kupon_Kupon1_idx` (`Kupon_idKupon`),
  KEY `fk_PROIZVOD_has_Kupon_PROIZVOD1_idx` (`PROIZVOD_idPROIZVOD`),
  CONSTRAINT `fk_PROIZVOD_has_Kupon_Kupon1` FOREIGN KEY (`Kupon_idKupon`) REFERENCES `kupon` (`idKupon`),
  CONSTRAINT `fk_PROIZVOD_has_Kupon_PROIZVOD1` FOREIGN KEY (`PROIZVOD_idPROIZVOD`) REFERENCES `proizvod` (`idPROIZVOD`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `proizvod_has_popust`
--

DROP TABLE IF EXISTS `proizvod_has_popust`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `proizvod_has_popust` (
  `PROIZVOD_idPROIZVOD` int NOT NULL,
  `Popust_idPopust` int NOT NULL,
  PRIMARY KEY (`PROIZVOD_idPROIZVOD`,`Popust_idPopust`),
  KEY `fk_PROIZVOD_has_Popust_Popust1_idx` (`Popust_idPopust`),
  KEY `fk_PROIZVOD_has_Popust_PROIZVOD1_idx` (`PROIZVOD_idPROIZVOD`),
  CONSTRAINT `fk_PROIZVOD_has_Popust_Popust1` FOREIGN KEY (`Popust_idPopust`) REFERENCES `popust` (`idPopust`),
  CONSTRAINT `fk_PROIZVOD_has_Popust_PROIZVOD1` FOREIGN KEY (`PROIZVOD_idPROIZVOD`) REFERENCES `proizvod` (`idPROIZVOD`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `proizvodjac`
--

DROP TABLE IF EXISTS `proizvodjac`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `proizvodjac` (
  `idPROIZVODJAC` int NOT NULL AUTO_INCREMENT,
  `Ime` varchar(45) NOT NULL,
  `Email` varchar(45) NOT NULL,
  `ADRESA_idADRESA` int NOT NULL,
  PRIMARY KEY (`idPROIZVODJAC`),
  KEY `fk_PROIZVODJAC_ADRESA1_idx` (`ADRESA_idADRESA`),
  CONSTRAINT `fk_PROIZVODJAC_ADRESA1` FOREIGN KEY (`ADRESA_idADRESA`) REFERENCES `adresa` (`idADRESA`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `racun`
--

DROP TABLE IF EXISTS `racun`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `racun` (
  `idRACUN` int NOT NULL AUTO_INCREMENT,
  `DatumKupovine` date NOT NULL,
  `KUPAC_idKUPAC` int NOT NULL,
  `PRODVNICA_idProdavnica` int NOT NULL,
  PRIMARY KEY (`idRACUN`),
  KEY `fk_RACUN_KUPAC1_idx` (`KUPAC_idKUPAC`),
  KEY `fk_RACUN_PRODVNICA1_idx` (`PRODVNICA_idProdavnica`),
  CONSTRAINT `fk_RACUN_KUPAC1` FOREIGN KEY (`KUPAC_idKUPAC`) REFERENCES `kupac` (`idKUPAC`),
  CONSTRAINT `fk_RACUN_PRODVNICA1` FOREIGN KEY (`PRODVNICA_idProdavnica`) REFERENCES `prodavnica` (`idProdavnica`)
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `stavka`
--

DROP TABLE IF EXISTS `stavka`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `stavka` (
  `RACUN_idRACUN` int NOT NULL,
  `PROIZVOD_idPROIZVOD` int NOT NULL,
  `Kolicina` int DEFAULT NULL,
  `Cijena` double NOT NULL,
  PRIMARY KEY (`RACUN_idRACUN`,`PROIZVOD_idPROIZVOD`),
  KEY `fk_RACUN_has_PROIZVOD_PROIZVOD1_idx` (`PROIZVOD_idPROIZVOD`),
  KEY `fk_RACUN_has_PROIZVOD_RACUN1_idx` (`RACUN_idRACUN`),
  CONSTRAINT `fk_RACUN_has_PROIZVOD_PROIZVOD1` FOREIGN KEY (`PROIZVOD_idPROIZVOD`) REFERENCES `proizvod` (`idPROIZVOD`),
  CONSTRAINT `fk_RACUN_has_PROIZVOD_RACUN1` FOREIGN KEY (`RACUN_idRACUN`) REFERENCES `racun` (`idRACUN`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `zaposleni`
--

DROP TABLE IF EXISTS `zaposleni`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `zaposleni` (
  `idZAPOSLENI` int NOT NULL AUTO_INCREMENT,
  `Ime` varchar(15) NOT NULL,
  `Prezime` varchar(20) NOT NULL,
  `Email` varchar(20) NOT NULL,
  `BrojTelefona` varchar(20) DEFAULT NULL,
  `Prodavnica_idProdavnica` int NOT NULL,
  `Plata` double NOT NULL,
  `Lozinka` varchar(255) DEFAULT NULL,
  `KorisnickoIme` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`idZAPOSLENI`),
  UNIQUE KEY `KorisnickoIme` (`KorisnickoIme`),
  KEY `fk_ZAPOSLENI_Prodavnica1_idx` (`Prodavnica_idProdavnica`),
  CONSTRAINT `fk_ZAPOSLENI_Prodavnica1` FOREIGN KEY (`Prodavnica_idProdavnica`) REFERENCES `prodavnica` (`idProdavnica`)
) ENGINE=InnoDB AUTO_INCREMENT=36 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2023-08-22 10:38:17
