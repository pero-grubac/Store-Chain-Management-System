using OnlineKupovinaDB;
using OnlineKupovinaGUI.Ui;

namespace OnlineKupovinaGUI.Views.Stranice
{
    /// <summary>Svi racuni lanca sa filterom i stornom (trigger vraca robu na zalihe).</summary>
    internal sealed class RacuniStranica : Stranica
    {
        private readonly DateTimePicker od;
        private readonly DateTimePicker @do;
        private readonly ComboBox prodavnica;
        private readonly DataGridView racuni;
        private readonly DataGridView stavke;
        private readonly Dugme storno;
        private readonly Label zbir;
        private readonly Kartica karticaStavki;
        private bool prodavniceUcitane;

        public RacuniStranica(Kontekst k) : base(k)
        {
            od = Raspored.Datum(DateTime.Today.AddDays(-29));
            @do = Raspored.Datum(DateTime.Today);
            prodavnica = Raspored.Izbor(200);
            prodavnica.Items.Add("Sve prodavnice");
            prodavnica.SelectedIndex = 0;
            var prikazi = new Dugme("Prikaži", Ikona.Pretraga, Varijanta.Primarno);
            prikazi.Click += async (_, _) => await UcitajAsync();
            storno = new Dugme("Storniraj račun", Ikona.Zatvori, Varijanta.Opasno) { Enabled = false };
            storno.Click += async (_, _) => await StornirajAsync();
            zbir = new Label { AutoSize = true, Font = Tema.Polu, ForeColor = Tema.TekstSlab, Margin = new Padding(16, 12, 0, 0) };

            var traka = Raspored.Red(Raspored.Oznaka("Od"), od, Raspored.Oznaka("Do"), @do, prodavnica, prikazi, storno, zbir);
            foreach (Label l in traka.Controls.OfType<Label>().Where(l => l != zbir)) l.Margin = new Padding(0, 12, 6, 0);
            traka.Dock = DockStyle.Top;
            traka.Padding = new Padding(0, 0, 0, 14);

            racuni = Tabela.Nova();
            racuni.Columns.AddRange(
                Tabela.Kolona("Broj", nameof(Racun.Broj), 60, podebljano: true),
                Tabela.Datum("Datum", nameof(Racun.Datum), 85, vrijeme: true),
                Tabela.Kolona("Kupac", nameof(Racun.Kupac), 70),
                Tabela.Kolona("Prodavnica", nameof(Racun.Prodavnica), 100),
                Tabela.Broj("Artikala", nameof(Racun.BrojArtikala), 45),
                Tabela.Kolona("Kupon", nameof(Racun.Kupon), 55),
                Tabela.Novac("Ukupno (KM)", nameof(Racun.Ukupno), 60, podebljano: true),
                Tabela.Kolona("Status", nameof(Racun.Status), 55));
            Tabela.BojaStatusa(racuni, nameof(Racun.Status));
            racuni.SelectionChanged += async (_, _) => await PrikaziStavkeAsync();

            stavke = StavkeRacuna.NovaTabela();
            karticaStavki = Raspored.UKartici("Stavke računa", stavke);
            karticaStavki.Margin = new Padding(0);

            var mreza = Raspored.Mreza(1, 2);
            mreza.RowStyles.Add(new RowStyle(SizeType.Percent, 62));
            mreza.RowStyles.Add(new RowStyle(SizeType.Percent, 38));
            var gornja = Raspored.UKartici(null, racuni);
            gornja.Margin = new Padding(0, 0, 0, 16);
            gornja.Padding = new Padding(2, 8, 2, 8);
            mreza.Controls.Add(gornja, 0, 0);
            mreza.Controls.Add(karticaStavki, 0, 1);

            Controls.Add(mreza);
            Controls.Add(traka);
        }

        public override string Naslov => "Računi";
        public override string? Opis => "Storno vraća robu na zalihe (trigger na tabeli racun) i izbacuje račun iz prihoda.";

        public override Task UcitajAsync() => RadiAsync(async () =>
        {
            if (!prodavniceUcitane)
            {
                prodavnica.Items.AddRange((await K.Repo.Prodavnice.SveAsync()).ToArray<object>());
                prodavniceUcitane = true;
            }
            var lista = await K.Repo.Prodaja.RacuniAsync(od.Value, @do.Value, (prodavnica.SelectedItem as Prodavnica)?.Id);
            Tabela.Postavi(racuni, lista);
            var placeni = lista.Where(r => !r.JeStorniran).ToList();
            zbir.Text = $"{lista.Count} računa · {Cijene.KM(placeni.Sum(r => r.Ukupno))}";
            await PrikaziStavkeAsync();
        });

        private async Task PrikaziStavkeAsync()
        {
            var racun = Tabela.Odabrani<Racun>(racuni);
            storno.Enabled = racun is { JeStorniran: false };
            karticaStavki.Naslov = racun == null ? "Stavke računa" : $"Stavke računa {racun.Broj}";
            if (racun == null)
            {
                Tabela.Postavi(stavke, Array.Empty<Stavka>());
                return;
            }
            await RadiAsync(async () => Tabela.Postavi(stavke, await K.Repo.Prodaja.StavkeAsync(racun.Id)));
        }

        private async Task StornirajAsync()
        {
            if (Tabela.Odabrani<Racun>(racuni) is not { } r ||
                !Dijalog.Potvrdi(Vlasnik, "Storno računa", $"Stornirati račun {r.Broj} ({Cijene.KM(r.Ukupno)}) kupca {r.Kupac}? Roba se vraća na zalihe.", "Storniraj")) return;
            if (await RadiAsync(() => K.Repo.Prodaja.StornirajAsync(r.Id, K.Sesija.KorisnickoIme), $"Račun {r.Broj} je storniran.")) await UcitajAsync();
        }
    }

    internal static class StavkeRacuna
    {
        public static DataGridView NovaTabela()
        {
            var t = Tabela.Nova();
            t.Columns.AddRange(
                Tabela.Kolona("Proizvod", nameof(Stavka.Naziv), 140, podebljano: true),
                Tabela.Kolona("Bar kod", nameof(Stavka.BarKod), 80),
                Tabela.Broj("Količina", nameof(Stavka.Kolicina), 45),
                Tabela.Novac("Puna cijena", nameof(Stavka.PunaCijena), 55),
                Tabela.Kolona("Popust", nameof(Stavka.Popust), 40, "0'%';;'–'", DataGridViewContentAlignment.MiddleRight),
                Tabela.Novac("Cijena", nameof(Stavka.Cijena), 50),
                Tabela.Novac("Ukupno", nameof(Stavka.Ukupno), 55, podebljano: true));
            return t;
        }
    }
}
