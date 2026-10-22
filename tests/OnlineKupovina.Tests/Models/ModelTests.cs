using OnlineKupovinaDB;

namespace OnlineKupovina.Tests.Models
{
    public class BarkodTests
    {
        [Theory]
        [InlineData("3870000000013")]
        [InlineData("4000000000013")]
        [InlineData("96385074")]       // EAN-8
        [InlineData("036000291452")]   // UPC-A
        public void ValidGtin_HasCorrectCheckDigit(string kod)
        {
            Assert.True(Barkod.JeGtin(kod));
            Assert.True(Barkod.ImaIspravnuKontrolnuCifru(kod));
        }

        [Theory]
        [InlineData("3870000000014")]
        [InlineData("96385075")]
        public void WrongCheckDigit_IsDetected(string kod)
        {
            Assert.False(Barkod.ImaIspravnuKontrolnuCifru(kod));
        }

        [Fact]
        public void ScannerInput_IsNormalized()
        {
            Assert.Equal("3870000000013", Barkod.Normalizuj(" 3870 000000013\r\n"));
        }

        [Theory]
        [InlineData("ABC-123", true)]
        [InlineData("12", false)]
        [InlineData("AB C1", true)]
        [InlineData("12#4", false)]
        public void Format_AllowsDigitsLettersAndDash(string kod, bool ocekivano)
        {
            Assert.Equal(ocekivano, Barkod.JeDozvoljenFormat(kod));
        }

        [Fact]
        public void InternalCode_IsValidEan13WithStorePrefix()
        {
            for (int i = 0; i < 50; i++)
            {
                string kod = Barkod.GenerisiInterni();

                Assert.Equal(13, kod.Length);
                Assert.StartsWith("20", kod);
                Assert.True(Barkod.ImaIspravnuKontrolnuCifru(kod));
            }
        }
    }

    public class CijeneTests
    {
        [Theory]
        [InlineData(2.25, 20, 1.80)]
        [InlineData(0.99, 15, 0.84)]
        [InlineData(1.15, 50, 0.58)]   // 0.575 se zaokruzuje navise, kao ROUND u MySQL-u
        [InlineData(10.00, 0, 10.00)]
        public void Discount_RoundsLikeDatabase(decimal cijena, int popust, decimal ocekivano)
        {
            Assert.Equal(ocekivano, Cijene.SaPopustom(cijena, popust));
        }

        [Fact]
        public void InvalidDiscount_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Cijene.SaPopustom(1, 101));
        }

        [Fact]
        public void KM_UsesLocalFormat()
        {
            Assert.Equal("1.234,50 KM", Cijene.KM(1234.5m));
        }
    }

    public class KorpaTests
    {
        private static Proizvod P(int id, decimal cijena, int zalihe = 10, int popust = 0, int prodavnica = 1) => new()
        {
            Id = id, Naziv = "P" + id, BarKod = "10" + id, Cijena = cijena, Zalihe = zalihe, Popust = popust, IdProdavnice = prodavnica
        };

        [Fact]
        public void AddingSameProduct_MergesQuantity()
        {
            var korpa = new Korpa();

            korpa.Dodaj(P(1, 2m), 2);
            korpa.Dodaj(P(1, 2m), 3);

            var stavka = Assert.Single(korpa.Stavke);
            Assert.Equal(5, stavka.Kolicina);
            Assert.Equal(10m, korpa.Ukupno);
            Assert.Equal(1, korpa.IdProdavnice);
        }

        [Fact]
        public void CannotExceedStock()
        {
            var korpa = new Korpa();
            korpa.Dodaj(P(1, 2m, zalihe: 3), 2);

            var e = Assert.Throws<InvalidOperationException>(() => korpa.Dodaj(P(1, 2m, zalihe: 3), 2));
            Assert.Contains("3", e.Message);
            Assert.Equal(2, korpa.Stavke[0].Kolicina);
        }

        [Fact]
        public void ProductsFromAnotherStore_AreRejected()
        {
            var korpa = new Korpa();
            korpa.Dodaj(P(1, 2m, prodavnica: 1));

            Assert.Throws<InvalidOperationException>(() => korpa.Dodaj(P(2, 2m, prodavnica: 2)));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void NonPositiveQuantity_IsRejected(int kolicina)
        {
            Assert.Throws<ArgumentException>(() => new Korpa().Dodaj(P(1, 2m), kolicina));
        }

        [Fact]
        public void Totals_UseActionDiscount()
        {
            var korpa = new Korpa();

            korpa.Dodaj(P(1, 2.25m, popust: 20), 2);   // 1.80 x 2
            korpa.Dodaj(P(2, 3.10m), 1);

            Assert.Equal(7.60m, korpa.Medjuzbir);
            Assert.Equal(6.70m, korpa.Ukupno);
            Assert.Equal(0.90m, korpa.Usteda);
        }

        [Fact]
        public void Coupon_AppliesOnlyToLinkedProducts_AndBiggerDiscountWins()
        {
            var korpa = new Korpa();
            korpa.Dodaj(P(1, 10m, popust: 20), 1);
            korpa.Dodaj(P(2, 10m, popust: 5), 1);
            korpa.Dodaj(P(3, 10m), 1);

            korpa.PrimijeniKupon(" jesen15 ", new Dictionary<int, int> { [1] = 15, [2] = 15 });

            Assert.Equal("JESEN15", korpa.Kupon);
            Assert.Equal(20, korpa.Stavke[0].Popust);
            Assert.Equal(15, korpa.Stavke[1].Popust);
            Assert.Equal(0, korpa.Stavke[2].Popust);
            Assert.Equal(8m + 8.5m + 10m, korpa.Ukupno);
        }

        [Fact]
        public void CouponAppliesToProductsAddedLater_AndCanBeRemoved()
        {
            var korpa = new Korpa();
            korpa.PrimijeniKupon("KAFA10", new Dictionary<int, int> { [7] = 10 });

            korpa.Dodaj(P(7, 5m), 1);
            Assert.Equal(4.5m, korpa.Ukupno);

            korpa.UkloniKupon();
            Assert.Null(korpa.Kupon);
            Assert.Equal(5m, korpa.Ukupno);
        }

        [Fact]
        public void ChangingQuantityToZero_RemovesItem_AndFreesStore()
        {
            var korpa = new Korpa();
            korpa.Dodaj(P(1, 2m), 1);

            korpa.PromijeniKolicinu(1, 0);

            Assert.True(korpa.JePrazna);
            Assert.Null(korpa.IdProdavnice);
            korpa.Dodaj(P(2, 2m, prodavnica: 2));
        }

        [Fact]
        public void Json_MatchesProcedureFormat()
        {
            var korpa = new Korpa();
            korpa.Dodaj(P(4, 1m), 2);
            korpa.Dodaj(P(9, 1m), 1);

            Assert.Equal("""[{"id":4,"kolicina":2},{"id":9,"kolicina":1}]""", korpa.UJson());
        }
    }

    public class ValidacijaTests
    {
        [Theory]
        [InlineData("kratka1", false)]
        [InlineData("bezcifara", false)]
        [InlineData("12345678", false)]
        [InlineData("Lozinka1", true)]
        public void Password_NeedsLengthLetterAndDigit(string lozinka, bool ispravna)
        {
            Assert.Equal(ispravna, Validacija.Lozinka(lozinka).Count == 0);
        }

        [Fact]
        public void Password_ConfirmationMustMatch()
        {
            Assert.Contains("Lozinke se ne poklapaju.", Validacija.Lozinka("Lozinka1", "Lozinka2"));
        }

        [Fact]
        public void Product_ReportsEveryProblemAtOnce()
        {
            var greske = new Proizvod { Naziv = "", BarKod = "3870000000014", Cijena = 0, Zalihe = -1, IdProizvodjaca = 0 }
                .Validiraj(noviProizvod: true);

            Assert.Equal(5, greske.Count);
            Assert.Contains(greske, g => g.Contains("kontrolnu cifru"));
        }

        [Fact]
        public void NewProduct_CannotBeExpired()
        {
            var p = new Proizvod { Naziv = "Mlijeko", BarKod = "3870000000013", Cijena = 1, IdProizvodjaca = 1, DatumIsteka = DateTime.Today.AddDays(-1) };

            Assert.Contains("Rok trajanja je već istekao.", p.Validiraj(noviProizvod: true));
            Assert.Empty(p.Validiraj(noviProizvod: false));
        }

        [Fact]
        public void Employee_ChecksEmailUsernamePhoneAndSalary()
        {
            var z = new Zaposleni { Ime = "Ana", Prezime = "Anić", KorisnickoIme = "a", Email = "nije-email", BrojTelefona = "abc", Plata = 0, IdProdavnice = 1 };

            Assert.Equal(4, z.Validiraj().Count);
        }

        [Fact]
        public void Store_RequiresNameAndAddress()
        {
            Assert.Equal(4, new Prodavnica().Validiraj().Count);
            Assert.Empty(new Prodavnica { Naziv = "Nova", Grad = "Doboj", Ulica = "Glavna 1", Drzava = "BiH" }.Validiraj());
        }

        [Fact]
        public void Coupon_CodeAndPeriod()
        {
            var k = new Kupon { Sifra = "a", Popust = 95, Od = DateTime.Today, Do = DateTime.Today.AddDays(-1) };

            Assert.Equal(3, k.Validiraj().Count);
            Assert.Equal("JESEN15", Kupon.NormalizujSifru(" jesen15 "));
        }

        [Fact]
        public void Roles_MapToProcedureValues()
        {
            Assert.Equal("administrator", Uloga.Administrator.UBazi());
            Assert.Equal("zaposleni", Uloga.Zaposleni.UBazi());
            Assert.Equal("kupac", Uloga.Kupac.UBazi());
            Assert.Equal("Radnik", Uloga.Zaposleni.Naziv());
        }

        [Theory]
        [InlineData(0, 5, "Nema")]
        [InlineData(5, 5, "Nisko")]
        [InlineData(6, 5, "OK")]
        public void StockStatus_MatchesDatabaseFunction(int zalihe, int min, string status)
        {
            Assert.Equal(status, Proizvod.Status(zalihe, min));
        }

        [Fact]
        public void BillLine_TotalsAndSavings()
        {
            var s = new Stavka { Kolicina = 3, PunaCijena = 2.25m, Popust = 20, Cijena = 1.80m };

            Assert.Equal(5.40m, s.Ukupno);
            Assert.Equal(1.35m, s.Usteda);
        }
    }
}
