using OnlineKupovina.Data;

namespace OnlineKupovina.Tests.Database
{
    [Collection(DatabaseCollection.Name)]
    [Trait("Category", "Integration")]
    public class FunctionTests(MySqlFixture db)
    {
        [Fact]
        public async Task ActiveDiscount_IsTheBiggestValidOneToday()
        {
            var p = await db.NoviProizvodAsync(cijena: 10m);
            await db.Repo.Akcije.DodajAkcijuAsync(new() { IdProizvoda = p.Id, Popust = 10, Od = DateTime.Today.AddDays(-1), Do = DateTime.Today.AddDays(1) });
            await db.Repo.Akcije.DodajAkcijuAsync(new() { IdProizvoda = p.Id, Popust = 30, Od = DateTime.Today, Do = DateTime.Today });
            await db.Repo.Akcije.DodajAkcijuAsync(new() { IdProizvoda = p.Id, Popust = 50, Od = DateTime.Today.AddDays(2), Do = DateTime.Today.AddDays(3) });

            Assert.Equal(30, db.Scalar<int>("SELECT fn_aktivni_popust(@id, CURDATE())", new { id = p.Id }));
            Assert.Equal(7.00m, db.Scalar<decimal>("SELECT fn_cijena_sa_popustom(@id, CURDATE())", new { id = p.Id }));
            Assert.Equal(10, db.Scalar<int>("SELECT fn_aktivni_popust(@id, CURDATE() + INTERVAL 1 DAY)", new { id = p.Id }));
        }

        [Fact]
        public async Task CouponDiscount_OnlyForLinkedProduct_InValidPeriod()
        {
            var vezan = await db.NoviProizvodAsync();
            var drugi = await db.NoviProizvodAsync();
            string sifra = MySqlFixture.Jedinstveno("F").ToUpperInvariant();
            await db.Repo.Akcije.KreirajKuponAsync(new() { Sifra = sifra, Popust = 12, Od = DateTime.Today, Do = DateTime.Today.AddDays(5) }, vezan.Id);

            Assert.Equal(12, db.Scalar<int>("SELECT fn_kupon_popust(@s, @id, CURDATE())", new { s = sifra.ToLowerInvariant(), id = vezan.Id }));
            Assert.Equal(0, db.Scalar<int>("SELECT fn_kupon_popust(@s, @id, CURDATE())", new { s = sifra, id = drugi.Id }));
            Assert.Equal(0, db.Scalar<int>("SELECT fn_kupon_popust(@s, @id, CURDATE() + INTERVAL 6 DAY)", new { s = sifra, id = vezan.Id }));
        }

        [Theory]
        [InlineData(0, 5, "Nema")]
        [InlineData(5, 5, "Nisko")]
        [InlineData(6, 5, "OK")]
        public void StockStatus(int zalihe, int min, string status)
        {
            Assert.Equal(status, db.Scalar<string>("SELECT fn_status_zaliha(@zalihe, @min)", new { zalihe, min }));
        }

        [Fact]
        public void PeriodStatus()
        {
            Assert.Equal("Najavljen", db.Scalar<string>("SELECT fn_status_perioda(CURDATE() + INTERVAL 1 DAY, NULL)"));
            Assert.Equal("Istekao", db.Scalar<string>("SELECT fn_status_perioda(CURDATE() - INTERVAL 5 DAY, CURDATE() - INTERVAL 1 DAY)"));
            Assert.Equal("Aktivan", db.Scalar<string>("SELECT fn_status_perioda(CURDATE(), NULL)"));
        }
    }

    [Collection(DatabaseCollection.Name)]
    [Trait("Category", "Integration")]
    public class TriggerTests(MySqlFixture db)
    {
        private const string Hash = "$2a$10$JiBMEXrmsh6JFGj1nZO3aeOq2N8wxjQVIqe7XUw8LXPOnsvjwEOEu";

        [Theory]
        [InlineData("Cijena", "-1", "Cijena ne može biti negativna")]
        [InlineData("BarKod", "'12#4'", "Bar kod smije sadržati samo cifre")]
        [InlineData("Naziv", "'   '", "Naziv proizvoda je obavezan")]
        [InlineData("DatumIsteka", "CURDATE() - INTERVAL 1 DAY", "Rok trajanja je već istekao")]
        public void Product_IsValidatedOnInsert(string kolona, string vrijednost, string poruka)
        {
            var vrijednosti = new Dictionary<string, string>
            {
                ["Naziv"] = "'Ispravan'", ["BarKod"] = $"'{MySqlFixture.NoviBarkod()}'", ["Cijena"] = "1", ["DatumIsteka"] = "NULL"
            };
            vrijednosti[kolona] = vrijednost;

            string greska = db.Fails(
                $@"INSERT INTO proizvod (Naziv, BarKod, Cijena, DatumIsteka, PROIZVODJAC_idPROIZVODJAC, PRODVNICA_idProdavnica)
                   VALUES ({vrijednosti["Naziv"]}, {vrijednosti["BarKod"]}, {vrijednosti["Cijena"]}, {vrijednosti["DatumIsteka"]}, 1, 1)");

            Assert.Contains(poruka, greska);
        }

        [Fact]
        public async Task Product_BarcodeIsTrimmed()
        {
            string kod = MySqlFixture.NoviBarkod();
            db.Execute(@"INSERT INTO proizvod (Naziv, BarKod, Cijena, PROIZVODJAC_idPROIZVODJAC, PRODVNICA_idProdavnica)
                         VALUES ('  Razmaci  ', @kod, 1, 1, 2)", new { kod = "  " + kod + " " });

            var p = await db.Repo.Proizvodi.PoBarkoduAsync(2, kod);
            Assert.NotNull(p);
            Assert.Equal("Razmaci", p.Naziv);
        }

        [Fact]
        public async Task StockChanges_AreWrittenToHistory_WithReasonAndUser()
        {
            var p = await db.NoviProizvodAsync(zalihe: 5);

            await db.Repo.Proizvodi.PrijemRobeAsync(p.Id, 7, "magacioner");

            var istorija = (await db.Repo.Izvjestaji.IstorijaZalihaAsync(DateTime.Today, DateTime.Today, 1)).Where(h => h.BarKod == p.BarKod).ToList();
            var prijem = Assert.Single(istorija, h => h.Razlog == "Prijem robe");
            Assert.Equal(5, prijem.StaraKolicina);
            Assert.Equal(12, prijem.NovaKolicina);
            Assert.Equal(7, prijem.Promjena);
            Assert.Equal("magacioner", prijem.Korisnik);
            Assert.Contains(istorija, h => h.Razlog == "Početno stanje" && h.NovaKolicina == 5);
        }

        [Fact]
        public async Task ManualStockEdit_IsLoggedAsCorrection()
        {
            var p = await db.NoviProizvodAsync(zalihe: 5);
            p.Zalihe = 2;

            await db.Repo.Proizvodi.IzmijeniAsync(p);

            Assert.Equal("Korekcija", db.Scalar<string>(
                "SELECT Razlog FROM zalihe_istorija WHERE PROIZVOD_idPROIZVOD = @id ORDER BY idIstorija DESC LIMIT 1", new { id = p.Id }));
        }

        [Fact]
        public async Task BillLineTriggers_KeepStockAndTotalInSync()
        {
            var p = await db.NoviProizvodAsync(zalihe: 10, cijena: 3m);
            var korpa = new OnlineKupovinaDB.Korpa();
            korpa.Dodaj(p, 2);
            int racun = await db.Repo.Prodaja.NaruciAsync(1, korpa, "test");

            db.Execute("UPDATE stavka SET Kolicina = 5 WHERE RACUN_idRACUN = @r", new { r = racun });
            Assert.Equal(5, db.Zalihe(p.Id));
            Assert.Equal(15m, db.Scalar<decimal>("SELECT Ukupno FROM racun WHERE idRACUN = @r", new { r = racun }));

            db.Execute("DELETE FROM stavka WHERE RACUN_idRACUN = @r", new { r = racun });
            Assert.Equal(10, db.Zalihe(p.Id));
            Assert.Equal(0m, db.Scalar<decimal>("SELECT Ukupno FROM racun WHERE idRACUN = @r", new { r = racun }));
        }

        [Fact]
        public async Task Storno_ReturnsStock_AndCannotBeUndone()
        {
            var p = await db.NoviProizvodAsync(zalihe: 8);
            var korpa = new OnlineKupovinaDB.Korpa();
            korpa.Dodaj(p, 3);
            int racun = await db.Repo.Prodaja.NaruciAsync(1, korpa, "test");
            Assert.Equal(5, db.Zalihe(p.Id));

            await db.Repo.Prodaja.StornirajAsync(racun, "admin");

            Assert.Equal(8, db.Zalihe(p.Id));
            Assert.Contains("ne može vratiti", db.Fails("UPDATE racun SET Status = 'Placen' WHERE idRACUN = @r", new { r = racun }));
            Assert.Contains("ne može mijenjati", db.Fails("UPDATE stavka SET Kolicina = 1 WHERE RACUN_idRACUN = @r", new { r = racun }));
            var e = await Assert.ThrowsAsync<PoslovnaGreskaException>(() => db.Repo.Prodaja.StornirajAsync(racun, "admin"));
            Assert.Contains("već storniran", e.Message);
        }

        [Fact]
        public void PlainTextPassword_IsRejected()
        {
            Assert.Contains("BCrypt", db.Fails(
                "INSERT INTO kupac (KorisnickoIme, Lozinka, Email, ADRESA_idADRESA) VALUES ('plain1', 'lozinka123', 'plain1@mail.com', 1)"));
        }

        [Fact]
        public void UserEmailAndUsername_AreNormalized()
        {
            string ime = MySqlFixture.Jedinstveno("Veliko");
            db.Execute("INSERT INTO administrator (Ime, Prezime, Email, Lozinka, KorisnickoIme) VALUES ('A', 'B', @email, @hash, @ime)",
                new { email = "  " + ime.ToUpperInvariant() + "@MAIL.COM ", hash = Hash, ime = ime.ToUpperInvariant() });

            Assert.Equal(ime.ToLowerInvariant() + "@mail.com",
                db.Scalar<string>("SELECT Email FROM administrator WHERE KorisnickoIme = @ime", new { ime = ime.ToLowerInvariant() }));
        }

        [Fact]
        public void InvalidEmail_IsRejected()
        {
            Assert.Contains("Email adresa nije ispravna", db.Fails(
                "INSERT INTO zaposleni (Ime, Prezime, Email, Prodavnica_idProdavnica, Plata, Lozinka, KorisnickoIme) VALUES ('A', 'B', 'bez-majmuna', 1, 100, @hash, 'zz_email')",
                new { hash = Hash }));
        }

        [Theory]
        [InlineData(0, 0, "između 1 i 90")]
        [InlineData(20, -1, "ne može završiti prije početka")]
        public void Discount_IsValidated(int popust, int krajDana, string poruka)
        {
            Assert.Contains(poruka, db.Fails("INSERT INTO popust (Popust, Start, End) VALUES (@popust, CURDATE(), CURDATE() + INTERVAL @kraj DAY)",
                new { popust, kraj = krajDana }));
        }

        [Fact]
        public void CouponCode_IsUppercased_AndValidated()
        {
            string sifra = MySqlFixture.Jedinstveno("kod");
            db.Execute("INSERT INTO kupon (Sifra, Start, popust) VALUES (@s, CURDATE(), 5)", new { s = " " + sifra + " " });

            Assert.Equal(1L, db.Scalar<long>("SELECT COUNT(*) FROM kupon WHERE BINARY Sifra = @s", new { s = sifra.ToUpperInvariant() }));
            Assert.Contains("Šifra kupona", db.Fails("INSERT INTO kupon (Sifra, Start, popust) VALUES ('a b', CURDATE(), 5)"));
        }
    }
}
