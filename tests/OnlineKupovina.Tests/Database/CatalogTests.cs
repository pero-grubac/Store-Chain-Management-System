using OnlineKupovina.Data;
using OnlineKupovinaDB;

namespace OnlineKupovina.Tests.Database
{
    [Collection(DatabaseCollection.Name)]
    [Trait("Category", "Integration")]
    public class AuthTests(MySqlFixture db)
    {
        [Theory]
        [InlineData(Uloga.Administrator, "admin", "Admin123!", "Ana Kovačević")]
        [InlineData(Uloga.Zaposleni, "radnik", "Radnik123!", "Marko Jović")]
        [InlineData(Uloga.Kupac, "kupac", "Kupac123!", "kupac")]
        public async Task DemoAccounts_SignIn(Uloga uloga, string korisnik, string lozinka, string punoIme)
        {
            var sesija = await db.Repo.Auth.PrijavaAsync(uloga, " " + korisnik.ToUpperInvariant() + " ", lozinka);

            Assert.NotNull(sesija);
            Assert.Equal(punoIme, sesija.PunoIme);
            Assert.Equal(uloga, sesija.Uloga);
        }

        [Fact]
        public async Task Employee_SessionCarriesStore()
        {
            var sesija = await db.Repo.Auth.PrijavaAsync(Uloga.Zaposleni, "radnik", "Radnik123!");

            Assert.Equal(1, sesija!.IdProdavnice);
            Assert.Equal("Centar Banja Luka", sesija.ImeProdavnice);
        }

        [Theory]
        [InlineData(Uloga.Administrator, "admin", "pogresna")]
        [InlineData(Uloga.Kupac, "admin", "Admin123!")]       // ispravna lozinka, pogresna uloga
        [InlineData(Uloga.Kupac, "nepostoji", "Kupac123!")]
        [InlineData(Uloga.Kupac, "", "")]
        public async Task WrongCredentials_ReturnNull(Uloga uloga, string korisnik, string lozinka)
        {
            Assert.Null(await db.Repo.Auth.PrijavaAsync(uloga, korisnik, lozinka));
        }

        [Fact]
        public async Task ChangePassword_RequiresCurrentPassword_AndRules()
        {
            string ime = MySqlFixture.Jedinstveno("lozinka");
            await db.Repo.Korisnici.DodajKupcaAsync(new Kupac { KorisnickoIme = ime, Email = ime + "@mail.com", Grad = "Doboj", Ulica = "Glavna 1", Drzava = "BiH" }, "Pocetna1");
            var sesija = (await db.Repo.Auth.PrijavaAsync(Uloga.Kupac, ime, "Pocetna1"))!;

            await Assert.ThrowsAsync<PoslovnaGreskaException>(() => db.Repo.Auth.PromijeniLozinkuAsync(sesija, "Pogresna1", "NovaLozinka2", "NovaLozinka2"));
            await Assert.ThrowsAsync<PoslovnaGreskaException>(() => db.Repo.Auth.PromijeniLozinkuAsync(sesija, "Pocetna1", "kratka", "kratka"));
            await Assert.ThrowsAsync<PoslovnaGreskaException>(() => db.Repo.Auth.PromijeniLozinkuAsync(sesija, "Pocetna1", "NovaLozinka2", "Drugacija2"));

            await db.Repo.Auth.PromijeniLozinkuAsync(sesija, "Pocetna1", "NovaLozinka2", "NovaLozinka2");

            Assert.Null(await db.Repo.Auth.PrijavaAsync(Uloga.Kupac, ime, "Pocetna1"));
            Assert.NotNull(await db.Repo.Auth.PrijavaAsync(Uloga.Kupac, ime, "NovaLozinka2"));
        }
    }

    [Collection(DatabaseCollection.Name)]
    [Trait("Category", "Integration")]
    public class AccountRepositoryTests(MySqlFixture db)
    {
        [Fact]
        public async Task Store_CanBeAddedEditedAndDeleted_WithItsAddress()
        {
            string naziv = MySqlFixture.Jedinstveno("Prodavnica ");
            int id = await db.Repo.Prodavnice.DodajAsync(new Prodavnica { Naziv = naziv, Grad = "Doboj", Ulica = "Glavna 1", Drzava = "BiH" });

            await db.Repo.Prodavnice.IzmijeniAsync(new Prodavnica { Id = id, Naziv = naziv, Grad = "Tuzla", Ulica = "Nova 2", Drzava = "BiH" });
            var p = Assert.Single(await db.Repo.Prodavnice.SveAsync(), x => x.Id == id);
            Assert.Equal("Tuzla", p.Grad);
            int adresa = p.IdAdrese;

            await db.Repo.Prodavnice.ObrisiAsync(id);

            Assert.DoesNotContain(await db.Repo.Prodavnice.SveAsync(), x => x.Id == id);
            Assert.Equal(0L, db.Scalar<long>("SELECT COUNT(*) FROM adresa WHERE idADRESA = @adresa", new { adresa }));
        }

        [Fact]
        public async Task StoreWithEmployees_CannotBeDeleted()
        {
            var e = await Assert.ThrowsAsync<PoslovnaGreskaException>(() => db.Repo.Prodavnice.ObrisiAsync(1));
            Assert.Contains("ne može se obrisati", e.Message);
        }

        [Fact]
        public async Task DuplicateStoreName_GivesFriendlyMessage()
        {
            var e = await Assert.ThrowsAsync<PoslovnaGreskaException>(() =>
                db.Repo.Prodavnice.DodajAsync(new Prodavnica { Naziv = "Centar Banja Luka", Grad = "A", Ulica = "B", Drzava = "C" }));
            Assert.Equal("Prodavnica sa tim nazivom već postoji.", e.Message);
        }

        [Fact]
        public async Task InvalidInput_IsRejectedBeforeDatabase_WithAllMessages()
        {
            var e = await Assert.ThrowsAsync<PoslovnaGreskaException>(() =>
                db.Repo.Korisnici.DodajZaposlenogAsync(new Zaposleni { Ime = "", Prezime = "", KorisnickoIme = "x", Email = "y", Plata = 0 }, "slaba"));
            Assert.True(e.Message.Split(Environment.NewLine).Length >= 5);
        }

        [Fact]
        public async Task Employee_FullLifecycle()
        {
            string ime = MySqlFixture.Jedinstveno("radnik");
            var z = new Zaposleni { Ime = "Novi", Prezime = "Radnik", KorisnickoIme = ime, Email = ime + "@prodavnica.ba", BrojTelefona = "065 000 000", Plata = 1200m, IdProdavnice = 2 };

            await db.Repo.Korisnici.DodajZaposlenogAsync(z, "Radnik123!");
            var sacuvan = Assert.Single(await db.Repo.Korisnici.ZaposleniAsync(2), x => x.KorisnickoIme == ime);
            Assert.Equal(1200m, sacuvan.Plata);
            Assert.NotNull(await db.Repo.Auth.PrijavaAsync(Uloga.Zaposleni, ime, "Radnik123!"));

            sacuvan.Plata = 1350.50m;
            sacuvan.IdProdavnice = 3;
            await db.Repo.Korisnici.IzmijeniZaposlenogAsync(sacuvan);
            Assert.Equal(1350.50m, Assert.Single(await db.Repo.Korisnici.ZaposleniAsync(3), x => x.KorisnickoIme == ime).Plata);

            await db.Repo.Auth.PostaviLozinkuAsync(Uloga.Zaposleni, ime, "Resetovana9");
            Assert.NotNull(await db.Repo.Auth.PrijavaAsync(Uloga.Zaposleni, ime, "Resetovana9"));

            await db.Repo.Korisnici.ObrisiZaposlenogAsync(sacuvan.Id);
            Assert.DoesNotContain(await db.Repo.Korisnici.ZaposleniAsync(), x => x.KorisnickoIme == ime);
        }

        [Fact]
        public async Task DuplicateUsername_GivesFriendlyMessage()
        {
            var e = await Assert.ThrowsAsync<PoslovnaGreskaException>(() => db.Repo.Korisnici.DodajKupcaAsync(
                new Kupac { KorisnickoIme = "sara", Email = MySqlFixture.Jedinstveno("s") + "@mail.com", Grad = "A", Ulica = "B", Drzava = "C" }, "Lozinka123"));
            Assert.Equal("Korisničko ime je već zauzeto.", e.Message);
        }

        [Fact]
        public async Task CustomerWithBills_CannotBeDeleted_NewOneCan()
        {
            await Assert.ThrowsAsync<PoslovnaGreskaException>(() => db.Repo.Korisnici.ObrisiKupcaAsync(1));

            string ime = MySqlFixture.Jedinstveno("brisi");
            int id = await db.Repo.Korisnici.DodajKupcaAsync(new Kupac { KorisnickoIme = ime, Email = ime + "@mail.com", Grad = "A", Ulica = "B", Drzava = "C" }, "Lozinka123");
            var kupac = Assert.Single(await db.Repo.Korisnici.KupciAsync(), k => k.Id == id);
            Assert.Equal(0, kupac.BrojRacuna);

            await db.Repo.Korisnici.ObrisiKupcaAsync(id);
            Assert.DoesNotContain(await db.Repo.Korisnici.KupciAsync(), k => k.Id == id);
        }

        [Fact]
        public async Task Admin_CannotDeleteThemselves()
        {
            var e = await Assert.ThrowsAsync<PoslovnaGreskaException>(() => db.Repo.Korisnici.ObrisiAdministratoraAsync(1, 1));
            Assert.Contains("prijavljeni", e.Message);
        }
    }

    [Collection(DatabaseCollection.Name)]
    [Trait("Category", "Integration")]
    public class ProductRepositoryTests(MySqlFixture db)
    {
        [Fact]
        public async Task AddProduct_RecordsEmployee_AndCanBeFoundByScannedBarcode()
        {
            string kod = MySqlFixture.NoviBarkod();
            var p = new Proizvod { Naziv = "Sok od jabuke 1l", BarKod = kod, Cijena = 2.90m, Zalihe = 30, MinZalihe = 5, IdProizvodjaca = 1, IdProdavnice = 3, DatumIsteka = DateTime.Today.AddMonths(6) };

            int id = await db.Repo.Proizvodi.DodajAsync(p, idZaposlenog: 5);

            var pronadjen = await db.Repo.Proizvodi.PoBarkoduAsync(3, " " + kod + "\r\n");
            Assert.NotNull(pronadjen);
            Assert.Equal(id, pronadjen.Id);
            Assert.Equal(2.90m, pronadjen.CijenaSaPopustom);
            Assert.Equal("Vitinka", pronadjen.Proizvodjac);
            Assert.Null(await db.Repo.Proizvodi.PoBarkoduAsync(1, kod));
            Assert.Equal(1L, db.Scalar<long>("SELECT COUNT(*) FROM dodavanjeproizvoda WHERE PROIZVOD_idPROIZVOD = @id AND ZAPOSLENI_idZAPOSLENI = 5", new { id }));
            Assert.Equal("milica", db.Scalar<string>(
                "SELECT Korisnik FROM zalihe_istorija WHERE PROIZVOD_idPROIZVOD = @id AND Razlog = 'Početno stanje'", new { id }));
        }

        [Fact]
        public async Task SameBarcodeTwiceInStore_IsRejected()
        {
            var postojeci = await db.NoviProizvodAsync();
            var e = await Assert.ThrowsAsync<PoslovnaGreskaException>(() => db.Repo.Proizvodi.DodajAsync(
                new Proizvod { Naziv = "Duplikat", BarKod = postojeci.BarKod, Cijena = 1, IdProizvodjaca = 1, IdProdavnice = 1 }, null));
            Assert.Contains("već postoji", e.Message);
        }

        [Fact]
        public async Task SoldProduct_IsDeactivated_UnsoldIsDeleted()
        {
            var prodavan = await db.NoviProizvodAsync();
            var neprodavan = await db.NoviProizvodAsync();
            var korpa = new Korpa();
            korpa.Dodaj(prodavan, 1);
            await db.Repo.Prodaja.NaruciAsync(1, korpa, "test");

            await db.Repo.Proizvodi.ObrisiAsync(prodavan.Id);
            await db.Repo.Proizvodi.ObrisiAsync(neprodavan.Id);

            Assert.Equal(0, db.Scalar<int>("SELECT Aktivan FROM proizvod WHERE idPROIZVOD = @id", new { id = prodavan.Id }));
            Assert.Equal(0L, db.Scalar<long>("SELECT COUNT(*) FROM proizvod WHERE idPROIZVOD = @id", new { id = neprodavan.Id }));
            Assert.DoesNotContain(await db.Repo.Proizvodi.PoProdavniciAsync(1), x => x.Id == prodavan.Id);
        }

        [Fact]
        public async Task Receiving_RejectsNonPositiveQuantity()
        {
            var p = await db.NoviProizvodAsync();
            await Assert.ThrowsAsync<PoslovnaGreskaException>(() => db.Repo.Proizvodi.PrijemRobeAsync(p.Id, 0, "x"));
        }

        [Fact]
        public async Task LowStock_ListsOnlyLowOrEmptyProducts()
        {
            var nisko = await db.NoviProizvodAsync(idProdavnice: 4, zalihe: 2, minZalihe: 5);

            var lista = await db.Repo.Proizvodi.NiskeZaliheAsync(4);

            Assert.Contains(lista, p => p.Id == nisko.Id && p.StatusZaliha == "Nisko");
            Assert.All(lista, p => Assert.NotEqual("OK", p.StatusZaliha));
            Assert.All(lista, p => Assert.Equal(4, p.IdProdavnice));
        }

        [Fact]
        public async Task Manufacturer_Lifecycle_AndProtection()
        {
            string naziv = MySqlFixture.Jedinstveno("Firma ");
            int id = await db.Repo.Proizvodi.DodajProizvodjacaAsync(new Proizvodjac { Naziv = naziv, Email = "Info@Firma.ba", Grad = "Zenica", Ulica = "Glavna 3", Drzava = "BiH" });
            var p = Assert.Single(await db.Repo.Proizvodi.ProizvodjaciAsync(), x => x.Id == id);
            Assert.Equal("info@firma.ba", p.Email);

            p.Grad = "Bihać";
            await db.Repo.Proizvodi.IzmijeniProizvodjacaAsync(p);
            Assert.Equal("Bihać", Assert.Single(await db.Repo.Proizvodi.ProizvodjaciAsync(), x => x.Id == id).Grad);

            await db.Repo.Proizvodi.ObrisiProizvodjacaAsync(id);
            var e = await Assert.ThrowsAsync<PoslovnaGreskaException>(() => db.Repo.Proizvodi.ObrisiProizvodjacaAsync(1));
            Assert.Contains("ima proizvode", e.Message);
        }

        [Fact]
        public async Task ActionsAndCoupons_ListWithStatus_AndCanBeRemoved()
        {
            var p = await db.NoviProizvodAsync();
            var drugi = await db.NoviProizvodAsync();
            await db.Repo.Akcije.DodajAkcijuAsync(new Akcija { IdProizvoda = p.Id, Popust = 30, Od = DateTime.Today.AddDays(3), Do = DateTime.Today.AddDays(9) });
            string sifra = MySqlFixture.Jedinstveno("A").ToUpperInvariant();
            await db.Repo.Akcije.KreirajKuponAsync(new Kupon { Sifra = sifra, Popust = 10, Od = DateTime.Today }, p.Id);
            var kupon = Assert.Single(await db.Repo.Akcije.KuponiAsync(), k => k.Sifra == sifra);
            await db.Repo.Akcije.DodajProizvodKuponuAsync(kupon.Id, drugi.Id);

            var akcija = Assert.Single(await db.Repo.Akcije.AkcijeAsync(), a => a.IdProizvoda == p.Id);
            Assert.Equal("Najavljen", akcija.Status);
            kupon = Assert.Single(await db.Repo.Akcije.KuponiAsync(), k => k.Sifra == sifra);
            Assert.Equal(2, kupon.BrojProizvoda);
            Assert.Equal("Aktivan", kupon.Status);

            await db.Repo.Akcije.ObrisiAkcijuAsync(akcija.Id);
            await db.Repo.Akcije.ObrisiKuponAsync(kupon.Id);
            Assert.DoesNotContain(await db.Repo.Akcije.AkcijeAsync(), a => a.Id == akcija.Id);
            Assert.DoesNotContain(await db.Repo.Akcije.KuponiAsync(), k => k.Id == kupon.Id);
        }
    }

    [Collection(DatabaseCollection.Name)]
    [Trait("Category", "Integration")]
    public class ReportTests(MySqlFixture db)
    {
        private static readonly DateTime Od = DateTime.Today.AddDays(-89);

        [Fact]
        public async Task Kpi_SummarizesPaidBills()
        {
            var kpi = await db.Repo.Izvjestaji.KpiAsync(Od, DateTime.Today);

            Assert.True(kpi.BrojRacuna >= 100);
            Assert.True(kpi.Prihod > 0);
            Assert.Equal(Math.Round(kpi.Prihod / kpi.BrojRacuna, 2), kpi.ProsjecnaKorpa, 1);
            Assert.True(kpi.NiskeZalihe >= 1);
        }

        [Fact]
        public async Task RevenuePerStore_IncludesEveryStore_SortedByRevenue()
        {
            var prihod = await db.Repo.Izvjestaji.PrihodPoProdavnicamaAsync(Od, DateTime.Today);

            Assert.True(prihod.Count >= 4);
            Assert.Equal("Centar Banja Luka", prihod[0].Prodavnica);
            Assert.True(prihod.SequenceEqual(prihod.OrderByDescending(p => p.Prihod)));
        }

        [Fact]
        public async Task RevenuePerDay_HasRowForEveryDay()
        {
            var dani = await db.Repo.Izvjestaji.PrihodPoDanimaAsync(DateTime.Today.AddDays(-29), DateTime.Today);

            Assert.Equal(30, dani.Count);
            Assert.Equal(DateTime.Today.AddDays(-29), dani[0].Dan);
            Assert.Equal(DateTime.Today, dani[^1].Dan);
        }

        [Fact]
        public async Task BestSellers_AreRanked()
        {
            var top = await db.Repo.Izvjestaji.NajprodavanijiAsync(Od, DateTime.Today, 5);

            Assert.InRange(top.Count, 5, 7);
            Assert.Equal(1, top[0].Rang);
            Assert.True(top.SequenceEqual(top.OrderByDescending(t => t.Kolicina)));
        }

        [Fact]
        public async Task StornoBill_IsExcludedFromRevenue()
        {
            var p = await db.NoviProizvodAsync(idProdavnice: 4, zalihe: 5, cijena: 100m);
            var prije = (await db.Repo.Izvjestaji.KpiAsync(DateTime.Today, DateTime.Today)).Prihod;
            var korpa = new Korpa();
            korpa.Dodaj(p, 1);
            int racun = await db.Repo.Prodaja.NaruciAsync(3, korpa, "test");
            Assert.Equal(prije + 100m, (await db.Repo.Izvjestaji.KpiAsync(DateTime.Today, DateTime.Today)).Prihod);

            await db.Repo.Prodaja.StornirajAsync(racun, "admin");

            Assert.Equal(prije, (await db.Repo.Izvjestaji.KpiAsync(DateTime.Today, DateTime.Today)).Prihod);
        }
    }
}
