using OnlineKupovina.Data;
using OnlineKupovinaDB;
using OnlineKupovinaGUI.Ui;
using OnlineKupovinaGUI.Views;
using OnlineKupovinaGUI.Views.Stranice;
using System.Runtime.ExceptionServices;

namespace OnlineKupovinaGUI.Tests
{
    /// <summary>WinForms kontrole se prave na STA niti, kao u aplikaciji.</summary>
    internal static class Sta
    {
        public static void Pokreni(Action akcija)
        {
            Exception? greska = null;
            var nit = new Thread(() =>
            {
                try { akcija(); }
                catch (Exception e) { greska = e; }
            });
            nit.SetApartmentState(ApartmentState.STA);
            nit.CurrentCulture = nit.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("bs-Latn-BA");
            nit.Start();
            nit.Join();
            if (greska != null) ExceptionDispatchInfo.Throw(greska);
        }

        // Konekcija se nikad ne otvara: testovi samo prave kontrole
        public static Repozitoriji LaznaBaza() => new("Server=127.0.0.1;Port=1;Database=nema;User Id=nema;Password=nema");
    }

    public class PageTests
    {
        public static TheoryData<Uloga> Uloge => [Uloga.Administrator, Uloga.Zaposleni, Uloga.Kupac];

        [Fact]
        public void EveryPage_CanBeBuilt_WithTitle()
        {
            Sta.Pokreni(() =>
            {
                var sesija = new Sesija(1, "radnik", "Marko Jović", Uloga.Zaposleni, 1, "Centar Banja Luka");
                using var shell = new ShellView(Sta.LaznaBaza(), sesija);
                var k = new Kontekst(Sta.LaznaBaza(), sesija, shell);

                Stranica[] stranice =
                [
                    new PregledStranica(k), new ProdavniceStranica(k), new ZaposleniStranica(k), new KupciStranica(k),
                    new AdministratoriStranica(k), new AkcijeStranica(k), new RacuniStranica(k), new IzvjestajiStranica(k),
                    new PodesavanjaStranica(k), new ProizvodiStranica(k), new ZaliheStranica(k), new ProizvodjaciStranica(k),
                    new KupovinaStranica(k), new MojiRacuniStranica(k)
                ];

                Assert.All(stranice, s => Assert.False(string.IsNullOrWhiteSpace(s.Naslov)));
                Assert.Equal(stranice.Length, stranice.Select(s => s.Naslov).Distinct().Count());
                foreach (var s in stranice) s.Dispose();
            });
        }

        [Theory]
        [MemberData(nameof(Uloge))]
        public void Menu_DependsOnRole(Uloga uloga)
        {
            Sta.Pokreni(() =>
            {
                using var shell = new ShellView(Sta.LaznaBaza(), new Sesija(1, "x", "Test Korisnik", uloga, uloga == Uloga.Zaposleni ? 1 : null));
                var meni = Svi(shell).OfType<Dugme>().Select(d => d.Text).ToList();

                Assert.Contains("Podešavanja", meni);
                Assert.Contains("Odjava", meni);
                Assert.Equal(uloga == Uloga.Administrator, meni.Contains("Administratori"));
                Assert.Equal(uloga == Uloga.Zaposleni, meni.Contains("Proizvodi"));
                Assert.Equal(uloga == Uloga.Kupac, meni.Contains("Kupovina"));
            });
        }

        [Fact]
        public void MainWindow_StartsWithSignIn_AndTogglesFullScreen()
        {
            Sta.Pokreni(() =>
            {
                using var forma = new GlavnaForma(Sta.LaznaBaza(), cijeliEkran: false);

                Assert.IsType<PrijavaView>(Assert.Single(forma.Controls.Cast<Control>()));
                Assert.False(forma.JeCijeliEkran);
                forma.PrebaciCijeliEkran();
                Assert.True(forma.JeCijeliEkran);
                forma.PrebaciCijeliEkran();
                Assert.False(forma.JeCijeliEkran);
            });
        }

        private static IEnumerable<Control> Svi(Control c) => c.Controls.Cast<Control>().SelectMany(x => Svi(x).Prepend(x));
    }

    public class FieldTests
    {
        [Fact]
        public void TextField_TrimsValue()
        {
            Sta.Pokreni(() =>
            {
                var polje = new TekstPolje("Naziv", "  Prodavnica  ");
                Assert.Equal("Prodavnica", polje.Vrijednost);
                Assert.Equal("  Prodavnica  ", polje.Sirovo);
            });
        }

        [Fact]
        public void BarcodeField_NormalizesScannerInput()
        {
            Sta.Pokreni(() =>
            {
                var polje = new BarkodPolje("Bar kod", " 3870 000000013 ");
                Assert.Equal("3870000000013", polje.Vrijednost);
            });
        }

        [Fact]
        public void NumberField_ClampsToRange()
        {
            Sta.Pokreni(() =>
            {
                var polje = new BrojPolje("Popust", 150, 1, 90);
                Assert.Equal(90, polje.Cijeli);
            });
        }

        [Fact]
        public void OptionalDate_IsNullWhenUnchecked()
        {
            Sta.Pokreni(() =>
            {
                Assert.Null(new DatumPolje("Rok", null, opciono: true).Vrijednost);
                Assert.Equal(new DateTime(2027, 1, 5), new DatumPolje("Rok", new DateTime(2027, 1, 5, 13, 0, 0), opciono: true).Vrijednost);
            });
        }

        [Fact]
        public void ChoiceField_PreselectsMatchingItem()
        {
            Sta.Pokreni(() =>
            {
                var prodavnice = new[] { new Prodavnica { Id = 1, Naziv = "A" }, new Prodavnica { Id = 2, Naziv = "B" } };
                var polje = new IzborPolje<Prodavnica>("Prodavnica", prodavnice, p => p.Naziv, p => p.Id == 2);
                Assert.Equal(2, polje.Vrijednost!.Id);
                Assert.Null(new IzborPolje<Prodavnica>("Prodavnica", prodavnice, p => p.Naziv).Vrijednost);
            });
        }
    }

    public class ThemeAndExportTests
    {
        [Fact]
        public void BackgroundBehindTransparentPanel_IsFirstOpaqueParent()
        {
            Sta.Pokreni(() =>
            {
                using var kartica = new Panel { BackColor = Color.White };
                var red = Raspored.Red();
                var dugme = new Dugme("Test");
                red.Controls.Add(dugme);
                kartica.Controls.Add(red);

                Assert.Equal(Color.White.ToArgb(), Tema.PozadinaIza(dugme).ToArgb());
            });
        }

        [Fact]
        public void RoundedPath_StaysInsideRectangle()
        {
            using var path = Tema.Zaobljeno(new RectangleF(10, 10, 100, 40), 8);
            var granice = path.GetBounds();

            Assert.Equal(10, granice.X, 3);
            Assert.Equal(110, granice.Right, 3);
            Assert.Equal(50, granice.Bottom, 3);
        }

        [Fact]
        public void Csv_UsesSemicolons_FormattedValues_AndQuoting()
        {
            Sta.Pokreni(() =>
            {
                using var forma = new Form();
                var tabela = Tabela.Nova();
                tabela.Columns.AddRange(Tabela.Kolona("Prodavnica", nameof(PrihodProdavnice.Prodavnica)), Tabela.Novac("Prihod", nameof(PrihodProdavnice.Prihod)));
                forma.Controls.Add(tabela);
                forma.CreateControl();
                Tabela.Postavi(tabela, new[]
                {
                    new PrihodProdavnice { Prodavnica = "Centar; Banja Luka", Prihod = 1234.5m },
                    new PrihodProdavnice { Prodavnica = "Mostar \"Rondo\"", Prihod = 0 }
                });

                string[] redovi = IzvjestajiStranica.UCsv(tabela).TrimEnd().Split(Environment.NewLine);

                Assert.Equal("Prodavnica;Prihod", redovi[0]);
                Assert.Equal("\"Centar; Banja Luka\";1.234,50", redovi[1]);
                Assert.Equal("\"Mostar \"\"Rondo\"\"\";0,00", redovi[2]);
            });
        }
    }
}
