namespace OnlineKupovina.Tests.Database
{
    [Collection(DatabaseCollection.Name)]
    [Trait("Category", "Integration")]
    public class SchemaTests(MySqlFixture db)
    {
        private long Count(string sql, string name) => db.Scalar<long>(sql, new { name });

        [Theory]
        [InlineData("administrator")]
        [InlineData("adresa")]
        [InlineData("dodavanjeproizvoda")]
        [InlineData("kategorija")]
        [InlineData("kategorija_has_proizvod")]
        [InlineData("kupac")]
        [InlineData("kupon")]
        [InlineData("popust")]
        [InlineData("prodavnica")]
        [InlineData("proizvod")]
        [InlineData("proizvod_has_kupon")]
        [InlineData("proizvod_has_popust")]
        [InlineData("proizvodjac")]
        [InlineData("racun")]
        [InlineData("stavka")]
        [InlineData("zalihe_istorija")]
        [InlineData("zaposleni")]
        public void Table_Exists(string table)
        {
            Assert.Equal(1, Count(
                "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = DATABASE() AND table_type = 'BASE TABLE' AND table_name = @name;",
                table));
        }

        [Theory]
        [InlineData("v_prodavnice")]
        [InlineData("v_proizvodjaci")]
        [InlineData("v_proizvodi")]
        [InlineData("v_niske_zalihe")]
        [InlineData("v_zaposleni")]
        [InlineData("v_kupci")]
        [InlineData("v_administratori")]
        [InlineData("v_racuni")]
        [InlineData("v_prodaja")]
        [InlineData("v_popusti")]
        [InlineData("v_kuponi")]
        public void View_ExistsAndIsQueryable(string view)
        {
            Assert.Equal(1, Count("SELECT COUNT(*) FROM information_schema.views WHERE table_schema = DATABASE() AND table_name = @name;", view));
            db.Execute($"SELECT * FROM {view} LIMIT 1");
        }

        [Theory]
        [InlineData("fn_aktivni_popust")]
        [InlineData("fn_cijena_sa_popustom")]
        [InlineData("fn_kupon_popust")]
        [InlineData("fn_status_zaliha")]
        [InlineData("fn_status_perioda")]
        public void Function_Exists(string function)
        {
            Assert.Equal(1, Count(
                "SELECT COUNT(*) FROM information_schema.routines WHERE routine_schema = DATABASE() AND routine_type = 'FUNCTION' AND routine_name = @name;",
                function));
        }

        [Fact]
        public void AllProceduresExist_IncludingOriginalOnes()
        {
            string[] ocekivane =
            [
                "detalji_racuna", "dodaj_administratora", "dodaj_kupca", "dodaj_popust", "dodaj_proizvod", "dodaj_proizvodjaca",
                "dodaj_radnika", "get_proizvod_by_prodavnica", "kreiraj_kupon", "narucivanje", "obrisi_kupon", "obrisi_popust",
                "obrisi_proizvod", "update_proizvod",
                "provjeri_korisnika", "dodaj_adresu", "azuriraj_adresu", "dodaj_prodavnicu", "izmijeni_prodavnicu", "obrisi_prodavnicu",
                "registruj_kupca", "izmijeni_kupca", "obrisi_kupca", "prijava_korisnika", "promijeni_lozinku", "registruj_proizvodjaca",
                "izmijeni_proizvodjaca", "obrisi_proizvodjaca", "prijem_robe", "pronadji_po_barkodu", "dodaj_proizvod_kuponu",
                "provjeri_kupon", "storniraj_racun", "stavke_racuna", "pregled_racuna", "izvjestaj_kpi", "izvjestaj_prihod_po_prodavnici",
                "izvjestaj_prihod_po_danima", "izvjestaj_najprodavaniji", "izvjestaj_niske_zalihe", "istorija_zaliha"
            ];

            using var k = db.Open();
            var postojece = Dapper.SqlMapper.Query<string>(k,
                "SELECT routine_name FROM information_schema.routines WHERE routine_schema = DATABASE() AND routine_type = 'PROCEDURE'").ToHashSet();

            Assert.Empty(ocekivane.Where(p => !postojece.Contains(p)));
        }

        [Fact]
        public void AllTriggersExist()
        {
            Assert.Equal(21, db.Scalar<long>("SELECT COUNT(*) FROM information_schema.triggers WHERE trigger_schema = DATABASE();"));
        }

        [Fact]
        public void MoneyColumns_AreDecimal()
        {
            using var k = db.Open();
            var tipovi = Dapper.SqlMapper.Query<(string Tabela, string Kolona, string Tip)>(k,
                @"SELECT table_name, column_name, data_type FROM information_schema.columns
                  WHERE table_schema = DATABASE() AND column_name IN ('Cijena', 'PunaCijena', 'Plata', 'Ukupno')
                    AND table_name IN ('proizvod', 'stavka', 'zaposleni', 'racun')").ToList();

            Assert.Equal(5, tipovi.Count);
            Assert.All(tipovi, t => Assert.Equal("decimal", t.Tip));
        }

        [Fact]
        public void Barcode_IsUniquePerStore_NotAcrossChain()
        {
            long prodavnica = db.Scalar<long>("SELECT COUNT(DISTINCT PRODVNICA_idProdavnica) FROM proizvod WHERE BarKod = '3870000000013'");
            Assert.True(prodavnica > 1);

            db.Fails("INSERT INTO proizvod (Naziv, BarKod, PROIZVODJAC_idPROIZVODJAC, Cijena, PRODVNICA_idProdavnica) VALUES ('Dup', '3870000000013', 1, 1, 1)");
        }

        [Fact]
        public void SeedBills_TotalsMatchTheirLines()
        {
            Assert.True(db.Scalar<long>("SELECT COUNT(*) FROM racun") >= 100);
            Assert.Equal(0L, db.Scalar<long>(
                @"SELECT COUNT(*) FROM racun r
                  WHERE r.Ukupno <> (SELECT COALESCE(SUM(s.Kolicina * s.Cijena), 0) FROM stavka s WHERE s.RACUN_idRACUN = r.idRACUN)"));
        }
    }
}
