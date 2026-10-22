using OnlineKupovinaDB;
using OnlineKupovinaGUI.Ui;

namespace OnlineKupovinaGUI.Views.Stranice
{
    /// <summary>
    /// Proizvodi prodavnice u kojoj radnik radi. Polje za skeniranje prima unos sa citaca bar koda
    /// (citac kuca kod i Enter): poznat proizvod otvara prijem robe, nepoznat formu za novi proizvod.
    /// </summary>
    internal sealed class ProizvodiStranica : TabelaStranica<Proizvod>
    {
        private readonly Unos skener;
        private List<Proizvodjac> proizvodjaci = new();

        public ProizvodiStranica(Kontekst k) : base(k)
        {
            DodajAlat("Novi proizvod", Ikona.Dodaj, () => NoviAsync(null), Varijanta.Primarno);
            DodajAlat("Izmijeni", Ikona.Izmijeni, IzmijeniAsync, trebaOdabir: true);
            DodajAlat("Prijem robe", Ikona.Prijem, () => PrijemAsync(Odabrani), trebaOdabir: true);
            DodajAlat("Povuci iz prodaje", Ikona.Obrisi, ObrisiAsync, trebaOdabir: true);

            Tabela.BojaStatusa(Lista, nameof(Proizvod.StatusZaliha));
            Tabela.BojaZaliha(Lista, o => ((Proizvod)o).StatusZaliha);
            Lista.CellFormatting += (_, e) =>
            {
                if (e.ColumnIndex < 0 || e.RowIndex < 0) return;
                string kolona = Lista.Columns[e.ColumnIndex].Name;
                if (kolona == nameof(Proizvod.CijenaSaPopustom) && Lista.Rows[e.RowIndex].DataBoundItem is Proizvod { NaAkciji: true })
                {
                    e.CellStyle!.ForeColor = Tema.Akcent;
                    e.CellStyle.Font = Tema.Polu;
                }
            };

            var karticaSkenera = new Kartica { Dock = DockStyle.Top, Height = 92, Margin = new Padding(0), Padding = new Padding(18, 18, 18, 18) };
            skener = new Unos("Skenirajte bar kod ili ga upišite i pritisnite Enter", Ikona.Skener) { Dock = DockStyle.Left, Width = 520, Height = 48, Name = "skener" };
            skener.Polje.Font = new Font("Segoe UI", 13f);
            skener.Polje.KeyDown += async (_, e) =>
            {
                if (e.KeyCode != Keys.Enter) return;
                e.SuppressKeyPress = true;
                await SkeniranoAsync();
            };
            var opis = new Label
            {
                Text = $"Prodavnica: {k.Sesija.ImeProdavnice}\nPoznat bar kod otvara prijem robe, nepoznat formu za novi proizvod.",
                ForeColor = Tema.TekstSlab, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(20, 0, 0, 0)
            };
            karticaSkenera.Controls.Add(opis);
            karticaSkenera.Controls.Add(skener);
            var razmak = new Panel { Dock = DockStyle.Top, Height = 16, BackColor = Tema.Pozadina };
            Controls.Add(razmak);
            Controls.Add(karticaSkenera);
        }

        public override string Naslov => "Proizvodi";
        public override string? Opis => "Katalog vaše prodavnice. Crvenom i žutom su označeni proizvodi kojih nema ili ih je malo.";

        protected override IEnumerable<DataGridViewColumn> Kolone() =>
        [
            Tabela.Kolona("Bar kod", nameof(Proizvod.BarKod), 85),
            Tabela.Kolona("Naziv", nameof(Proizvod.Naziv), 140, podebljano: true),
            Tabela.Kolona("Proizvođač", nameof(Proizvod.Proizvodjac), 80),
            Tabela.Novac("Cijena", nameof(Proizvod.Cijena), 50),
            Tabela.Kolona("Akcija", nameof(Proizvod.Popust), 40, "0'%';;'–'", DataGridViewContentAlignment.MiddleRight),
            Tabela.Novac("Prodajna", nameof(Proizvod.CijenaSaPopustom), 55),
            Tabela.Broj("Zalihe", nameof(Proizvod.Zalihe), 45),
            Tabela.Broj("Min.", nameof(Proizvod.MinZalihe), 35),
            Tabela.Kolona("Stanje", nameof(Proizvod.StatusZaliha), 45),
            Tabela.Datum("Rok trajanja", nameof(Proizvod.DatumIsteka), 60),
        ];

        protected override async Task<List<Proizvod>> UcitajPodatkeAsync()
        {
            if (proizvodjaci.Count == 0) proizvodjaci = await K.Repo.Proizvodi.ProizvodjaciAsync();
            var lista = await K.Repo.Proizvodi.PoProdavniciAsync(K.Sesija.IdProdavnice!.Value);
            if (IsHandleCreated) BeginInvoke(() => skener.Polje.Focus());
            return lista;
        }

        protected override string TekstZaPretragu(Proizvod p) => $"{p.Naziv} {p.BarKod} {p.Proizvodjac} {p.Opis}";
        protected override Task DvostrukiKlikAsync() => IzmijeniAsync();

        private async Task SkeniranoAsync()
        {
            string kod = Barkod.Normalizuj(skener.Text);
            skener.Text = string.Empty;
            if (kod.Length == 0) return;

            Proizvod? proizvod = null;
            bool uspjeh = await RadiAsync(async () => proizvod = await K.Repo.Proizvodi.PoBarkoduAsync(K.Sesija.IdProdavnice!.Value, kod));
            if (!uspjeh) return;

            if (proizvod != null)
            {
                OdaberiRed(proizvod.Id);
                await PrijemAsync(proizvod);
            }
            else if (!Barkod.JeDozvoljenFormat(kod))
            {
                K.Shell.Obavijesti($"„{kod}” nije ispravan bar kod.", VrstaObavjestenja.Greska);
            }
            else
            {
                K.Shell.Obavijesti($"Bar kod {kod} nije u katalogu – unesite novi proizvod.", VrstaObavjestenja.Info);
                await NoviAsync(kod);
            }
            skener.Polje.Focus();
        }

        private void OdaberiRed(int id)
        {
            foreach (DataGridViewRow red in Lista.Rows)
            {
                if (red.DataBoundItem is Proizvod p && p.Id == id)
                {
                    Lista.CurrentCell = red.Cells[1];
                    Lista.FirstDisplayedScrollingRowIndex = Math.Max(0, red.Index - 3);
                    return;
                }
            }
        }

        private bool Uredi(string naslov, Proizvod p, bool novi, Func<Proizvod, Task> sacuvaj)
        {
            var barkod = new BarkodPolje("Bar kod", p.BarKod, puna: true) { Pomoc = "EAN/UPC kod se provjerava po kontrolnoj cifri." };
            var naziv = new TekstPolje("Naziv", p.Naziv, puna: true, maxDuzina: 45);
            var proizvodjac = new IzborPolje<Proizvodjac>("Proizvođač", proizvodjaci, x => $"{x.Naziv} ({x.Drzava})", x => x.Id == p.IdProizvodjaca);
            var cijena = new BrojPolje("Cijena (KM)", p.Cijena == 0 ? 1 : p.Cijena, 0, 1_000_000, 2);
            var zalihe = new BrojPolje(novi ? "Početne zalihe" : "Zalihe (korekcija)", p.Zalihe, 0, 1_000_000);
            var min = new BrojPolje("Minimalne zalihe", p.MinZalihe, 0, 1_000_000) { Pomoc = "Ispod ovoga proizvod je u upozorenjima." };
            var rok = new DatumPolje("Rok trajanja", p.DatumIsteka, opciono: true);
            var opis = new TekstPolje("Opis", p.Opis, maxDuzina: 255);

            return Dijalog.Prikazi(Vlasnik, naslov, [barkod, naziv, proizvodjac, cijena, zalihe, min, rok, opis], () => sacuvaj(new Proizvod
            {
                Id = p.Id, IdProdavnice = K.Sesija.IdProdavnice!.Value, BarKod = barkod.Vrijednost, Naziv = naziv.Vrijednost,
                IdProizvodjaca = proizvodjac.Vrijednost?.Id ?? 0, Cijena = cijena.Vrijednost, Zalihe = zalihe.Cijeli, MinZalihe = min.Cijeli,
                DatumIsteka = rok.Vrijednost, Opis = string.IsNullOrWhiteSpace(opis.Vrijednost) ? null : opis.Vrijednost
            }));
        }

        private async Task NoviAsync(string? barkod)
        {
            int noviId = 0;
            if (Uredi("Novi proizvod", new Proizvod { BarKod = barkod ?? string.Empty, MinZalihe = 10, DatumIsteka = DateTime.Today.AddYears(1) }, true,
                    async p => noviId = await K.Repo.Proizvodi.DodajAsync(p, K.Sesija.Id)))
            {
                K.Shell.Obavijesti("Proizvod je dodan u katalog.", VrstaObavjestenja.Uspjeh);
                await UcitajAsync();
                OdaberiRed(noviId);
            }
        }

        private async Task IzmijeniAsync()
        {
            if (Odabrani is { } p && Uredi($"Izmjena: {p.Naziv}", p, false, x => K.Repo.Proizvodi.IzmijeniAsync(x)))
            {
                K.Shell.Obavijesti("Izmjene su sačuvane.", VrstaObavjestenja.Uspjeh);
                await UcitajAsync();
            }
        }

        private async Task PrijemAsync(Proizvod? p)
        {
            if (p == null) return;
            if (PrijemRobe.Prikazi(Vlasnik, K, p))
            {
                await UcitajAsync();
                OdaberiRed(p.Id);
            }
        }

        private async Task ObrisiAsync()
        {
            if (Odabrani is not { } p || !Dijalog.Potvrdi(Vlasnik, "Povlačenje iz prodaje",
                    $"Povući „{p.Naziv}” iz prodaje? Proizvod koji je prodavan ostaje u bazi zbog računa, ostali se brišu.", "Povuci")) return;
            if (await RadiAsync(() => K.Repo.Proizvodi.ObrisiAsync(p.Id), "Proizvod je povučen iz prodaje.")) await UcitajAsync();
        }
    }

    internal static class PrijemRobe
    {
        public static bool Prikazi(IWin32Window vlasnik, Kontekst k, Proizvod p)
        {
            var kolicina = new BrojPolje("Primljena količina", Math.Max(1, p.MinZalihe * 2 - p.Zalihe), 1, 100_000, puna: true);
            if (Dijalog.Prikazi(vlasnik, "Prijem robe", [kolicina], () => k.Repo.Proizvodi.PrijemRobeAsync(p.Id, kolicina.Cijeli, k.Sesija.KorisnickoIme),
                    $"{p.Naziv} · {p.BarKod}\nTrenutno na stanju: {p.Zalihe} kom. (minimum {p.MinZalihe})", "Primi robu"))
            {
                k.Shell.Obavijesti($"Primljeno {kolicina.Cijeli} kom. – {p.Naziv}.", VrstaObavjestenja.Uspjeh);
                return true;
            }
            return false;
        }
    }

    /// <summary>Upozorenja o zalihama i istorija promjena za prodavnicu radnika.</summary>
    internal sealed class ZaliheStranica : Stranica
    {
        private readonly DataGridView niske;
        private readonly DataGridView istorija;
        private readonly Dugme prijem;

        public ZaliheStranica(Kontekst k) : base(k)
        {
            niske = Tabela.Nova();
            niske.Columns.AddRange(
                Tabela.Kolona("Proizvod", nameof(Proizvod.Naziv), 140, podebljano: true),
                Tabela.Kolona("Bar kod", nameof(Proizvod.BarKod), 90),
                Tabela.Broj("Zalihe", nameof(Proizvod.Zalihe), 45),
                Tabela.Broj("Minimum", nameof(Proizvod.MinZalihe), 45),
                Tabela.Kolona("Stanje", nameof(Proizvod.StatusZaliha), 45));
            Tabela.BojaStatusa(niske, nameof(Proizvod.StatusZaliha));
            niske.SelectionChanged += (_, _) => prijem!.Enabled = Tabela.Odabrani<Proizvod>(niske) != null;
            niske.CellDoubleClick += async (_, e) => { if (e.RowIndex >= 0) await PrijemAsync(); };

            istorija = Tabela.Nova();
            istorija.Columns.AddRange(
                Tabela.Datum("Vrijeme", nameof(PromjenaZaliha.Vrijeme), 80, vrijeme: true),
                Tabela.Kolona("Proizvod", nameof(PromjenaZaliha.Naziv), 140, podebljano: true),
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

            prijem = new Dugme("Prijem robe", Ikona.Prijem, Varijanta.Primarno) { Enabled = false };
            prijem.Click += async (_, _) => await PrijemAsync();
            var karticaNiske = Raspored.UKartici("Za naručiti", niske);
            karticaNiske.Podnaslov = "· proizvodi na ili ispod minimuma";
            var alati = Raspored.Red(prijem);
            karticaNiske.Controls.Add(alati);
            alati.BringToFront();
            karticaNiske.Resize += (_, _) => alati.Location = new Point(karticaNiske.Width - alati.Width - 18, 8);
            karticaNiske.Margin = new Padding(0, 0, 0, 16);
            var karticaIstorije = Raspored.UKartici("Istorija zaliha", istorija);
            karticaIstorije.Podnaslov = "· posljednjih 14 dana, upisuje je trigger";
            karticaIstorije.Margin = new Padding(0);

            var mreza = Raspored.Mreza(1, 2);
            mreza.RowStyles.Add(new RowStyle(SizeType.Percent, 45));
            mreza.RowStyles.Add(new RowStyle(SizeType.Percent, 55));
            mreza.Controls.Add(karticaNiske, 0, 0);
            mreza.Controls.Add(karticaIstorije, 0, 1);
            Controls.Add(mreza);
        }

        public override string Naslov => "Zalihe";
        public override string? Opis => "Šta treba naručiti i kako su se zalihe mijenjale (prodaja, prijem, storno, korekcije).";

        public override Task UcitajAsync() => RadiAsync(async () =>
        {
            int id = K.Sesija.IdProdavnice!.Value;
            var n = K.Repo.Proizvodi.NiskeZaliheAsync(id);
            var i = K.Repo.Izvjestaji.IstorijaZalihaAsync(DateTime.Today.AddDays(-13), DateTime.Today, id);
            await Task.WhenAll(n, i);
            Tabela.Postavi(niske, n.Result);
            Tabela.Postavi(istorija, i.Result);
        });

        private async Task PrijemAsync()
        {
            if (Tabela.Odabrani<Proizvod>(niske) is { } p && PrijemRobe.Prikazi(Vlasnik, K, p)) await UcitajAsync();
        }
    }

    internal sealed class ProizvodjaciStranica : TabelaStranica<Proizvodjac>
    {
        public ProizvodjaciStranica(Kontekst k) : base(k)
        {
            DodajAlat("Novi proizvođač", Ikona.Dodaj, NoviAsync, Varijanta.Primarno);
            DodajAlat("Izmijeni", Ikona.Izmijeni, IzmijeniAsync, trebaOdabir: true);
            DodajAlat("Obriši", Ikona.Obrisi, ObrisiAsync, trebaOdabir: true);
        }

        public override string Naslov => "Proizvođači";
        public override string? Opis => "Dobavljači robe za cijeli lanac.";

        protected override IEnumerable<DataGridViewColumn> Kolone() =>
        [
            Tabela.Kolona("Naziv", nameof(Proizvodjac.Naziv), 100, podebljano: true),
            Tabela.Kolona("Email", nameof(Proizvodjac.Email), 120),
            Tabela.Kolona("Adresa", nameof(Proizvodjac.PunaAdresa), 200),
            Tabela.Broj("Proizvoda", nameof(Proizvodjac.BrojProizvoda)),
        ];

        protected override Task<List<Proizvodjac>> UcitajPodatkeAsync() => K.Repo.Proizvodi.ProizvodjaciAsync();
        protected override string TekstZaPretragu(Proizvodjac p) => $"{p.Naziv} {p.Email} {p.PunaAdresa}";
        protected override Task DvostrukiKlikAsync() => IzmijeniAsync();

        private bool Uredi(string naslov, Proizvodjac p, Func<Proizvodjac, Task> sacuvaj)
        {
            var naziv = new TekstPolje("Naziv", p.Naziv, maxDuzina: 45);
            var email = new TekstPolje("Email", p.Email, maxDuzina: 100);
            var ulica = new TekstPolje("Ulica i broj", p.Ulica, puna: true, maxDuzina: 100);
            var grad = new TekstPolje("Grad", p.Grad, maxDuzina: 45);
            var drzava = new TekstPolje("Država", p.Drzava, maxDuzina: 45);
            return Dijalog.Prikazi(Vlasnik, naslov, [naziv, email, ulica, grad, drzava], () => sacuvaj(new Proizvodjac
            {
                Id = p.Id, Naziv = naziv.Vrijednost, Email = email.Vrijednost, Ulica = ulica.Vrijednost, Grad = grad.Vrijednost, Drzava = drzava.Vrijednost
            }));
        }

        private async Task NoviAsync()
        {
            if (Uredi("Novi proizvođač", new Proizvodjac(), p => K.Repo.Proizvodi.DodajProizvodjacaAsync(p)))
            {
                K.Shell.Obavijesti("Proizvođač je dodan.", VrstaObavjestenja.Uspjeh);
                await UcitajAsync();
            }
        }

        private async Task IzmijeniAsync()
        {
            if (Odabrani is { } p && Uredi("Izmjena proizvođača", p, x => K.Repo.Proizvodi.IzmijeniProizvodjacaAsync(x)))
            {
                K.Shell.Obavijesti("Izmjene su sačuvane.", VrstaObavjestenja.Uspjeh);
                await UcitajAsync();
            }
        }

        private async Task ObrisiAsync()
        {
            if (Odabrani is not { } p || !Dijalog.Potvrdi(Vlasnik, "Brisanje proizvođača", $"Obrisati proizvođača {p.Naziv}?")) return;
            if (await RadiAsync(() => K.Repo.Proizvodi.ObrisiProizvodjacaAsync(p.Id), "Proizvođač je obrisan.")) await UcitajAsync();
        }
    }
}
