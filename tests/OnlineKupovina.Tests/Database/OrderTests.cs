using OnlineKupovina.Data;
using OnlineKupovinaDB;

namespace OnlineKupovina.Tests.Database
{
    /// <summary>
    /// Procedura narucivanje kroz repozitorij: racun, stavke i zalihe se mijenjaju
    /// zajedno ili nikako, a cijene i popuste odredjuje baza.
    /// </summary>
    [Collection(DatabaseCollection.Name)]
    [Trait("Category", "Integration")]
    public class OrderTests(MySqlFixture db)
    {
        private const int Kupac = 2;

        private long BrojRacuna() => db.Scalar<long>("SELECT COUNT(*) FROM racun");

        [Fact]
        public async Task Order_CreatesBill_ReducesStock_AndRecordsUser()
        {
            var a = await db.NoviProizvodAsync(zalihe: 10, cijena: 2.50m);
            var b = await db.NoviProizvodAsync(zalihe: 5, cijena: 4m);
            var korpa = new Korpa();
            korpa.Dodaj(a, 3);
            korpa.Dodaj(b, 5);

            int racun = await db.Repo.Prodaja.NaruciAsync(Kupac, korpa, "sara");

            var stavke = await db.Repo.Prodaja.StavkeAsync(racun);
            Assert.Equal(2, stavke.Count);
            Assert.Equal(27.50m, stavke.Sum(s => s.Ukupno));
            var zaglavlje = Assert.Single(await db.Repo.Prodaja.RacuniKupcaAsync(Kupac), r => r.Id == racun);
            Assert.Equal(27.50m, zaglavlje.Ukupno);
            Assert.Equal(8, zaglavlje.BrojArtikala);
            Assert.Equal(Racun.Placen, zaglavlje.Status);
            Assert.Equal(7, db.Zalihe(a.Id));
            Assert.Equal(0, db.Zalihe(b.Id));
            Assert.Equal("sara", db.Scalar<string>(
                "SELECT Korisnik FROM zalihe_istorija WHERE PROIZVOD_idPROIZVOD = @id AND Razlog = 'Prodaja'", new { id = a.Id }));
        }

        [Fact]
        public async Task DatabasePrice_MatchesCartPrice_WithActionAndCoupon()
        {
            var akcija = await db.NoviProizvodAsync(cijena: 2.25m);
            var kuponom = await db.NoviProizvodAsync(cijena: 3.10m);
            await db.Repo.Akcije.DodajAkcijuAsync(new() { IdProizvoda = akcija.Id, Popust = 20, Od = DateTime.Today, Do = DateTime.Today });
            string sifra = MySqlFixture.Jedinstveno("K").ToUpperInvariant();
            await db.Repo.Akcije.KreirajKuponAsync(new() { Sifra = sifra, Popust = 15, Od = DateTime.Today }, kuponom.Id);

            var korpa = new Korpa();
            korpa.Dodaj((await db.Repo.Proizvodi.PoBarkoduAsync(1, akcija.BarKod))!, 2);
            korpa.Dodaj(kuponom, 3);
            korpa.PrimijeniKupon(sifra, await db.Repo.Prodaja.ProvjeriKuponAsync(sifra, 1));

            int racun = await db.Repo.Prodaja.NaruciAsync(Kupac, korpa, "test");

            var stavke = await db.Repo.Prodaja.StavkeAsync(racun);
            Assert.Equal(korpa.Ukupno, stavke.Sum(s => s.Ukupno));
            Assert.Equal(1.80m, stavke.Single(s => s.IdProizvoda == akcija.Id).Cijena);
            Assert.Equal(15, stavke.Single(s => s.IdProizvoda == kuponom.Id).Popust);
            Assert.Equal(sifra, (await db.Repo.Prodaja.RacuniKupcaAsync(Kupac)).Single(r => r.Id == racun).Kupon);
        }

        [Fact]
        public async Task InvalidCoupon_RejectsOrder()
        {
            var p = await db.NoviProizvodAsync();
            var korpa = new Korpa();
            korpa.Dodaj(p, 1);
            korpa.PrimijeniKupon("NEPOSTOJI", new Dictionary<int, int>());
            long prije = BrojRacuna();

            var e = await Assert.ThrowsAsync<PoslovnaGreskaException>(() => db.Repo.Prodaja.NaruciAsync(Kupac, korpa, "test"));

            Assert.Contains("Kupon nije važeći", e.Message);
            Assert.Equal(prije, BrojRacuna());
        }

        [Fact]
        public async Task ExpiredCoupon_IsRejectedOnCheck()
        {
            var e = await Assert.ThrowsAsync<PoslovnaGreskaException>(() => db.Repo.Prodaja.ProvjeriKuponAsync("LJETO30", 1));
            Assert.Contains("istekao", e.Message);
        }

        [Fact]
        public async Task InsufficientStock_RollsBackEverything()
        {
            var dovoljno = await db.NoviProizvodAsync(zalihe: 10);
            var malo = await db.NoviProizvodAsync(zalihe: 3);
            var korpa = new Korpa();
            korpa.Dodaj(dovoljno, 2);
            korpa.Dodaj(malo, 3);
            db.Execute("UPDATE proizvod SET Zalihe = 1 WHERE idPROIZVOD = @id", new { id = malo.Id });   // neko je kupio u medjuvremenu
            long prije = BrojRacuna();

            var e = await Assert.ThrowsAsync<PoslovnaGreskaException>(() => db.Repo.Prodaja.NaruciAsync(Kupac, korpa, "test"));

            Assert.Contains("Nema dovoljno proizvoda na zalihama", e.Message);
            Assert.Contains(malo.Naziv, e.Message);
            Assert.Equal(prije, BrojRacuna());
            Assert.Equal(10, db.Zalihe(dovoljno.Id));
        }

        [Fact]
        public async Task ProductFromAnotherStore_IsRejected()
        {
            var drugaProdavnica = await db.NoviProizvodAsync(idProdavnice: 2);
            long prije = BrojRacuna();

            var e = Assert.Throws<PoslovnaGreskaException>(() =>
            {
                using var k = db.Open();
                try
                {
                    Dapper.SqlMapper.Execute(k, "CALL narucivanje(@kupac, 1, @stavke, NULL, @izlaz)",
                        new { kupac = Kupac, stavke = $"[{{\"id\":{drugaProdavnica.Id},\"kolicina\":1}}]" });
                }
                catch (MySqlConnector.MySqlException ex)
                {
                    throw Greske.Prevedi(ex);
                }
            });

            Assert.Contains("ne pripada izabranoj prodavnici", e.Message);
            Assert.Equal(prije, BrojRacuna());
        }

        [Theory]
        [InlineData("[]", "Korpa je prazna")]
        [InlineData("[{\"id\":1,\"kolicina\":0}]", "Količina mora biti veća od nule")]
        [InlineData("[{\"id\":1,\"kolicina\":-3}]", "Količina mora biti veća od nule")]
        [InlineData("{\"id\":1}", "Korpa je prazna")]
        public void BadOrderInput_IsRejected(string stavke, string poruka)
        {
            long prije = BrojRacuna();

            Assert.Contains(poruka, db.Fails("CALL narucivanje(1, 1, @stavke, NULL, @izlaz)", new { stavke }));
            Assert.Equal(prije, BrojRacuna());
        }

        [Fact]
        public async Task SameProductTwiceInJson_IsMergedIntoOneLine()
        {
            var p = await db.NoviProizvodAsync(zalihe: 10);
            db.Execute("CALL narucivanje(1, 1, @stavke, NULL, @izlaz)",
                new { stavke = $"[{{\"id\":{p.Id},\"kolicina\":2}},{{\"id\":{p.Id},\"kolicina\":3}}]" });

            Assert.Equal(5, db.Scalar<int>("SELECT Kolicina FROM stavka WHERE PROIZVOD_idPROIZVOD = @id", new { id = p.Id }));
            Assert.Equal(5, db.Zalihe(p.Id));
        }

        [Fact]
        public async Task EmptyCart_IsRejectedBeforeCallingDatabase()
        {
            await Assert.ThrowsAsync<PoslovnaGreskaException>(() => db.Repo.Prodaja.NaruciAsync(Kupac, new Korpa(), "test"));
        }

        [Fact]
        public async Task BillOverview_FiltersByStoreAndPeriod()
        {
            var racuni = await db.Repo.Prodaja.RacuniAsync(DateTime.Today.AddDays(-89), DateTime.Today, 4);

            Assert.NotEmpty(racuni);
            Assert.All(racuni, r => Assert.Equal("Mostar Rondo", r.Prodavnica));
            Assert.True(racuni.SequenceEqual(racuni.OrderByDescending(r => r.Datum).ThenByDescending(r => r.Id)));
        }
    }
}
