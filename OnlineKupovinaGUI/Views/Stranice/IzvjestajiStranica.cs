using OnlineKupovinaDB;
using OnlineKupovinaGUI.Ui;
using System.Globalization;
using System.Text;

namespace OnlineKupovinaGUI.Views.Stranice
{
    /// <summary>Izvjestaji za period sa izvozom u CSV.</summary>
    internal sealed class IzvjestajiStranica : Stranica
    {
        private enum Prikaz { Prihod, Najprodavaniji, Zalihe }

        private readonly DateTimePicker od;
        private readonly DateTimePicker @do;
        private readonly Dictionary<Prikaz, Dugme> prekidaci = new();
        private readonly Panel sadrzaj;
        private readonly Grafikon grafikon;
        private readonly Grafikon poDanima;
        private readonly DataGridView prihod;
        private readonly DataGridView najprodavaniji;
        private readonly DataGridView istorija;
        private readonly Control prikazPrihoda;
        private readonly Kartica karticaNajprodavanijih;
        private readonly Kartica karticaIstorije;
        private Prikaz aktivni = Prikaz.Prihod;

        public IzvjestajiStranica(Kontekst k) : base(k)
        {
            od = Raspored.Datum(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-2));
            @do = Raspored.Datum(DateTime.Today);
            var prikazi = new Dugme("Prikaži", Ikona.Pretraga, Varijanta.Primarno);
            prikazi.Click += async (_, _) => await UcitajAsync();
            var izvoz = new Dugme("Izvoz u CSV", Ikona.Izvoz);
            izvoz.Click += (_, _) => IzveziCsv();

            var traka = Raspored.Red(Raspored.Oznaka("Od"), od, Raspored.Oznaka("Do"), @do, prikazi, izvoz);
            foreach (Label l in traka.Controls.OfType<Label>()) l.Margin = new Padding(0, 12, 6, 0);

            var tabovi = Raspored.Red();
            foreach (var (prikaz, tekst, ikona) in new[]
                     {
                         (Prikaz.Prihod, "Prihod po prodavnicama", Ikona.Prodavnica),
                         (Prikaz.Najprodavaniji, "Najprodavaniji proizvodi", Ikona.Trend),
                         (Prikaz.Zalihe, "Istorija zaliha", Ikona.Istorija)
                     })
            {
                var d = new Dugme(tekst, ikona);
                d.Click += (_, _) => Prebaci(prikaz);
                prekidaci[prikaz] = d;
                tabovi.Controls.Add(d);
            }

            var vrh = new TableLayoutPanel { Dock = DockStyle.Top, Height = 56, ColumnCount = 2, BackColor = Tema.Pozadina, Padding = new Padding(0, 0, 0, 14) };
            vrh.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            vrh.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            vrh.Controls.Add(traka, 0, 0);
            vrh.Controls.Add(tabovi, 1, 0);

            grafikon = new Grafikon(VrstaGrafikona.Trake) { Format = Cijene.KM };
            prihod = Tabela.Nova();
            prihod.Columns.AddRange(
                Tabela.Kolona("Prodavnica", nameof(PrihodProdavnice.Prodavnica), 150, podebljano: true),
                Tabela.Broj("Računa", nameof(PrihodProdavnice.BrojRacuna), 60),
                Tabela.Novac("Prihod (KM)", nameof(PrihodProdavnice.Prihod), 70, podebljano: true));
            poDanima = new Grafikon(VrstaGrafikona.Stupci) { Format = v => v >= 1000 ? (v / 1000m).ToString("0.#") + "k" : v.ToString("N0") };
            var mreza = Raspored.Mreza(2, 2);
            mreza.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62));
            mreza.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38));
            mreza.RowStyles.Add(new RowStyle(SizeType.Percent, 42));
            mreza.RowStyles.Add(new RowStyle(SizeType.Percent, 58));
            var kg = Raspored.UKartici("Prihod po prodavnicama", grafikon);
            var kd = Raspored.UKartici("Prihod po danima", poDanima);
            kd.Margin = new Padding(0, 0, 16, 0);
            var kt = Raspored.UKartici("Pregled po prodavnicama", prihod);
            kt.Margin = new Padding(0);
            mreza.Controls.Add(kg, 0, 0);
            mreza.Controls.Add(kd, 0, 1);
            mreza.Controls.Add(kt, 1, 0);
            mreza.SetRowSpan(kt, 2);
            prikazPrihoda = mreza;

            najprodavaniji = Tabela.Nova();
            najprodavaniji.Columns.AddRange(
                Tabela.Broj("Rang", nameof(NajprodavanijiProizvod.Rang), 30),
                Tabela.Kolona("Proizvod", nameof(NajprodavanijiProizvod.Naziv), 160, podebljano: true),
                Tabela.Kolona("Bar kod", nameof(NajprodavanijiProizvod.BarKod), 90),
                Tabela.Broj("Prodato kom.", nameof(NajprodavanijiProizvod.Kolicina), 60),
                Tabela.Novac("Prihod (KM)", nameof(NajprodavanijiProizvod.Prihod), 70, podebljano: true),
                Tabela.Broj("Prodavnica", nameof(NajprodavanijiProizvod.BrojProdavnica), 50));

            istorija = Tabela.Nova();
            istorija.Columns.AddRange(
                Tabela.Datum("Vrijeme", nameof(PromjenaZaliha.Vrijeme), 80, vrijeme: true),
                Tabela.Kolona("Proizvod", nameof(PromjenaZaliha.Naziv), 120, podebljano: true),
                Tabela.Kolona("Prodavnica", nameof(PromjenaZaliha.Prodavnica), 100),
                Tabela.Broj("Staro", nameof(PromjenaZaliha.StaraKolicina), 40),
                Tabela.Broj("Novo", nameof(PromjenaZaliha.NovaKolicina), 40),
                Tabela.Kolona("Promjena", nameof(PromjenaZaliha.Promjena), 45, "+0;-0;0", DataGridViewContentAlignment.MiddleRight, true),
                Tabela.Kolona("Razlog", nameof(PromjenaZaliha.Razlog), 70),
                Tabela.Kolona("Korisnik", nameof(PromjenaZaliha.Korisnik), 60));
            istorija.CellFormatting += (_, e) =>
            {
                if (e.ColumnIndex >= 0 && istorija.Columns[e.ColumnIndex].Name == nameof(PromjenaZaliha.Promjena) && e.Value is int v)
                {
                    e.CellStyle!.ForeColor = v < 0 ? Tema.Opasnost : Tema.Uspjeh;
                }
            };

            karticaNajprodavanijih = Raspored.UKartici("Najprodavaniji proizvodi", najprodavaniji);
            karticaNajprodavanijih.Margin = new Padding(0);
            karticaIstorije = Raspored.UKartici("Istorija zaliha", istorija);
            karticaIstorije.Margin = new Padding(0);

            sadrzaj = new Panel { Dock = DockStyle.Fill, BackColor = Tema.Pozadina };
            Controls.Add(sadrzaj);
            Controls.Add(vrh);
            Prebaci(Prikaz.Prihod);
        }

        public override string Naslov => "Izvještaji";
        public override string? Opis => "Prihod, najprodavaniji proizvodi (RANK) i istorija zaliha koju pune trigeri.";

        private void Prebaci(Prikaz prikaz)
        {
            aktivni = prikaz;
            foreach (var (p, d) in prekidaci) d.Varijanta = p == prikaz ? Varijanta.Primarno : Varijanta.Sekundarno;
            foreach (var d in prekidaci.Values) d.Invalidate();
            sadrzaj.Controls.Clear();
            sadrzaj.Controls.Add(prikaz switch
            {
                Prikaz.Prihod => prikazPrihoda,
                Prikaz.Najprodavaniji => karticaNajprodavanijih,
                _ => karticaIstorije
            });
        }

        public override Task UcitajAsync() => RadiAsync(async () =>
        {
            if (od.Value.Date > @do.Value.Date) throw new ArgumentException("Datum „od” je poslije datuma „do”.");
            if ((@do.Value.Date - od.Value.Date).TotalDays > 900) throw new ArgumentException("Period može biti najviše 900 dana.");
            var p = K.Repo.Izvjestaji.PrihodPoProdavnicamaAsync(od.Value, @do.Value);
            var n = K.Repo.Izvjestaji.NajprodavanijiAsync(od.Value, @do.Value, 25);
            var z = K.Repo.Izvjestaji.IstorijaZalihaAsync(od.Value, @do.Value);
            var d = K.Repo.Izvjestaji.PrihodPoDanimaAsync(od.Value, @do.Value);
            await Task.WhenAll(p, n, z, d);
            poDanima.Postavi(d.Result.Select(x => (x.Dan.ToString("d.M."), x.Prihod)));
            Tabela.Postavi(prihod, p.Result);
            grafikon.Postavi(p.Result.Select(x => (x.Prodavnica, x.Prihod)));
            Tabela.Postavi(najprodavaniji, n.Result);
            Tabela.Postavi(istorija, z.Result);
        });

        private void IzveziCsv()
        {
            var tabela = aktivni switch { Prikaz.Prihod => prihod, Prikaz.Najprodavaniji => najprodavaniji, _ => istorija };
            using var dijalog = new SaveFileDialog
            {
                Filter = "CSV (*.csv)|*.csv",
                FileName = $"izvjestaj-{aktivni.ToString().ToLowerInvariant()}-{od.Value:yyyyMMdd}-{@do.Value:yyyyMMdd}.csv"
            };
            if (dijalog.ShowDialog(Vlasnik) != DialogResult.OK) return;

            File.WriteAllText(dijalog.FileName, UCsv(tabela), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
            K.Shell.Obavijesti($"Sačuvano: {Path.GetFileName(dijalog.FileName)}", VrstaObavjestenja.Uspjeh);
        }

        /// <summary>CSV sa tacka-zarezom (Excel u lokalnim postavkama), vrijednosti kako su prikazane.</summary>
        internal static string UCsv(DataGridView tabela)
        {
            static string Polje(string? s) => s != null && s.IndexOfAny([';', '"', '\n']) >= 0 ? "\"" + s.Replace("\"", "\"\"") + "\"" : s ?? string.Empty;

            var kolone = tabela.Columns.Cast<DataGridViewColumn>().Where(c => c.Visible).OrderBy(c => c.DisplayIndex).ToList();
            var sb = new StringBuilder();
            sb.AppendLine(string.Join(";", kolone.Select(c => Polje(c.HeaderText))));
            foreach (DataGridViewRow red in tabela.Rows)
            {
                sb.AppendLine(string.Join(";", kolone.Select(c => Polje(Convert.ToString(red.Cells[c.Index].FormattedValue, CultureInfo.CurrentCulture)))));
            }
            return sb.ToString();
        }
    }
}
