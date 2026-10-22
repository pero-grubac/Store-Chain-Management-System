using OnlineKupovinaDB;
using OnlineKupovinaGUI.Ui;

namespace OnlineKupovinaGUI.Views.Stranice
{
    /// <summary>Kontrolna tabla administratora: KPI, prihod po danima i prodavnicama, najprodavaniji, niske zalihe.</summary>
    internal sealed class PregledStranica : Stranica
    {
        private static readonly (string Naziv, int Dana)[] Periodi = [("Posljednjih 7 dana", 7), ("Posljednjih 30 dana", 30), ("Posljednjih 90 dana", 90)];

        private readonly ComboBox period;
        private readonly StatKartica prihod;
        private readonly StatKartica racuni;
        private readonly StatKartica korpa;
        private readonly StatKartica zalihe;
        private readonly Grafikon poDanima;
        private readonly Grafikon poProdavnicama;
        private readonly DataGridView najprodavaniji;
        private readonly DataGridView niskeZalihe;

        public PregledStranica(Kontekst k) : base(k)
        {
            period = Raspored.Izbor(200);
            period.Items.AddRange(Periodi.Select(p => (object)p.Naziv).ToArray());
            period.SelectedIndex = 1;
            period.SelectedIndexChanged += async (_, _) => await UcitajAsync();
            var osvjezi = new Dugme("Osvježi", Ikona.Osvjezi);
            osvjezi.Click += async (_, _) => await UcitajAsync();
            var traka = Raspored.Red(period, osvjezi);
            traka.Dock = DockStyle.Top;
            traka.Padding = new Padding(0, 0, 0, 14);

            prihod = new StatKartica("Prihod", Ikona.Trend, Tema.Akcent) { Dock = DockStyle.Fill };
            racuni = new StatKartica("Računi", Ikona.Racun, Tema.Hex("#0D9488")) { Dock = DockStyle.Fill };
            korpa = new StatKartica("Prosječna korpa", Ikona.Korpa, Tema.Hex("#DB2777")) { Dock = DockStyle.Fill };
            zalihe = new StatKartica("Niske zalihe", Ikona.Upozorenje, Tema.Upozorenje) { Dock = DockStyle.Fill };

            var kpi = Raspored.Mreza(4, 1);
            for (int i = 0; i < 4; i++) kpi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            kpi.Controls.AddRange([prihod, racuni, korpa, zalihe]);
            zalihe.Margin = new Padding(0, 0, 0, 16);

            poDanima = new Grafikon(VrstaGrafikona.Stupci) { Name = "grafikonDani", Format = v => v >= 1000 ? (v / 1000m).ToString("0.#") + "k" : v.ToString("N0") };
            poProdavnicama = new Grafikon(VrstaGrafikona.Trake) { Format = Cijene.KM, Boja = Tema.Hex("#0D9488") };
            var karticaDani = Raspored.UKartici("Prihod po danima", poDanima);
            var karticaProdavnice = Raspored.UKartici("Prihod po prodavnicama", poProdavnicama);
            karticaProdavnice.Margin = new Padding(0, 0, 0, 16);

            najprodavaniji = Tabela.Nova();
            najprodavaniji.Columns.AddRange(
                Tabela.Broj("#", nameof(NajprodavanijiProizvod.Rang), 18),
                Tabela.Kolona("Proizvod", nameof(NajprodavanijiProizvod.Naziv), 120, podebljano: true),
                Tabela.Broj("Prodato", nameof(NajprodavanijiProizvod.Kolicina), 45),
                Tabela.Novac("Prihod (KM)", nameof(NajprodavanijiProizvod.Prihod), 55));
            niskeZalihe = Tabela.Nova();
            niskeZalihe.Columns.AddRange(
                Tabela.Kolona("Proizvod", nameof(Proizvod.Naziv), 110, podebljano: true),
                Tabela.Kolona("Prodavnica", nameof(Proizvod.Prodavnica), 100),
                Tabela.Broj("Zalihe", nameof(Proizvod.Zalihe), 40),
                Tabela.Kolona("Status", nameof(Proizvod.StatusZaliha), 45));
            Tabela.BojaStatusa(niskeZalihe, nameof(Proizvod.StatusZaliha));
            var karticaTop = Raspored.UKartici("Najprodavaniji proizvodi", najprodavaniji);
            var karticaZalihe = Raspored.UKartici("Proizvodi sa niskim zalihama", niskeZalihe);
            karticaZalihe.Margin = new Padding(0);
            karticaTop.Margin = new Padding(0, 0, 16, 0);

            var mreza = Raspored.Mreza(2, 3);
            mreza.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58));
            mreza.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42));
            mreza.RowStyles.Add(new RowStyle(SizeType.Absolute, 132));
            mreza.RowStyles.Add(new RowStyle(SizeType.Percent, 52));
            mreza.RowStyles.Add(new RowStyle(SizeType.Percent, 48));
            mreza.Controls.Add(kpi, 0, 0);
            mreza.SetColumnSpan(kpi, 2);
            mreza.Controls.Add(karticaDani, 0, 1);
            mreza.Controls.Add(karticaProdavnice, 1, 1);
            mreza.Controls.Add(karticaTop, 0, 2);
            mreza.Controls.Add(karticaZalihe, 1, 2);

            Controls.Add(mreza);
            Controls.Add(traka);
        }

        public override string Naslov => "Pregled";
        public override string? Opis => "Poslovanje lanca u izabranom periodu. Storno računi se ne računaju.";

        public override Task UcitajAsync() => RadiAsync(async () =>
        {
            int dana = Periodi[Math.Max(0, period.SelectedIndex)].Dana;
            var od = DateTime.Today.AddDays(-(dana - 1));
            var @do = DateTime.Today;

            var kpiTask = K.Repo.Izvjestaji.KpiAsync(od, @do);
            var daniTask = K.Repo.Izvjestaji.PrihodPoDanimaAsync(od, @do);
            var prodavniceTask = K.Repo.Izvjestaji.PrihodPoProdavnicamaAsync(od, @do);
            var topTask = K.Repo.Izvjestaji.NajprodavanijiAsync(od, @do, 8);
            var zaliheTask = K.Repo.Proizvodi.NiskeZaliheAsync();
            await Task.WhenAll(kpiTask, daniTask, prodavniceTask, topTask, zaliheTask);

            var kpi = kpiTask.Result;
            prihod.Postavi(Cijene.KM(kpi.Prihod), $"{od:dd.MM.} – {@do:dd.MM.yyyy.}");
            racuni.Postavi(kpi.BrojRacuna.ToString("N0"), $"{kpi.BrojKupaca} kupaca");
            korpa.Postavi(Cijene.KM(kpi.ProsjecnaKorpa), "po plaćenom računu");
            zalihe.Postavi(kpi.NiskeZalihe.ToString(), kpi.NiskeZalihe == 0 ? "sve je na stanju" : "treba naručiti");

            poDanima.Postavi(daniTask.Result.Select(d => (d.Dan.ToString(dana > 30 ? "dd.MM." : "d.M."), d.Prihod)));
            poProdavnicama.Postavi(prodavniceTask.Result.Select(p => (p.Prodavnica, p.Prihod)));
            Tabela.Postavi(najprodavaniji, topTask.Result);
            Tabela.Postavi(niskeZalihe, zaliheTask.Result);
        });
    }
}
