using OnlineKupovinaDB;
using OnlineKupovinaGUI.Ui;

namespace OnlineKupovinaGUI.Views.Stranice
{
    /// <summary>
    /// Kupovina: lijevo proizvodi izabrane prodavnice, desno korpa sa kuponom.
    /// Korpa prikazuje cijene kao baza, a konacan racun pravi procedura narucivanje.
    /// </summary>
    internal sealed class KupovinaStranica : Stranica
    {
        private readonly Korpa korpa = new();
        private readonly ComboBox prodavnica;
        private readonly Unos pretraga;
        private readonly DataGridView proizvodi;
        private readonly NumericUpDown kolicina;
        private readonly DataGridView stavke;
        private readonly Unos kupon;
        private readonly Dugme kuponDugme;
        private readonly Label kuponInfo;
        private readonly Label medjuzbir;
        private readonly Label usteda;
        private readonly Label ukupno;
        private readonly Dugme kupi;
        private readonly Kartica karticaKorpe;
        private List<Proizvod> svi = new();
        private int? ucitanaProdavnica;

        public KupovinaStranica(Kontekst k) : base(k)
        {
            // ------------------------------------------------------------ proizvodi
            prodavnica = Raspored.Izbor(240);
            prodavnica.Margin = new Padding(0, 4, 12, 0);
            prodavnica.SelectedIndexChanged += async (_, _) => await PromijeniProdavnicuAsync();
            pretraga = new Unos("Pretraga ili bar kod (Enter dodaje u korpu)", Ikona.Skener) { Width = 360, Name = "pretraga" };
            pretraga.Polje.TextChanged += (_, _) => Filtriraj();
            pretraga.Polje.KeyDown += (_, e) =>
            {
                if (e.KeyCode != Keys.Enter) return;
                e.SuppressKeyPress = true;
                DodajPoBarkodu();
            };

            proizvodi = Tabela.Nova();
            proizvodi.Columns.AddRange(
                Tabela.Kolona("Proizvod", nameof(Proizvod.Naziv), 150, podebljano: true),
                Tabela.Kolona("Proizvođač", nameof(Proizvod.Proizvodjac), 80),
                Tabela.Novac("Cijena", nameof(Proizvod.Cijena), 50),
                Tabela.Kolona("Akcija", nameof(Proizvod.Popust), 45, "'-'0'%';;''", DataGridViewContentAlignment.MiddleCenter),
                Tabela.Novac("Za platiti", nameof(Proizvod.CijenaSaPopustom), 55, podebljano: true),
                Tabela.Broj("Na stanju", nameof(Proizvod.Zalihe), 50));
            proizvodi.CellFormatting += (_, e) =>
            {
                if (e.RowIndex < 0 || proizvodi.Rows[e.RowIndex].DataBoundItem is not Proizvod p) return;
                string kolona = proizvodi.Columns[e.ColumnIndex].Name;
                if (p.NaAkciji && kolona is nameof(Proizvod.Popust) or nameof(Proizvod.CijenaSaPopustom))
                {
                    e.CellStyle!.ForeColor = Tema.Opasnost;
                    e.CellStyle.SelectionForeColor = Tema.Opasnost;
                    e.CellStyle.Font = Tema.Polu;
                }
                if (p.NaAkciji && kolona == nameof(Proizvod.Cijena))
                {
                    e.CellStyle!.Font = new Font(Tema.Osnovni, FontStyle.Strikeout);
                    e.CellStyle.ForeColor = Tema.TekstSlab;
                }
                if (kolona == nameof(Proizvod.Zalihe) && p.Zalihe <= p.MinZalihe) e.CellStyle!.ForeColor = Tema.Upozorenje;
            };
            proizvodi.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) DodajOdabrani(1); };

            kolicina = new NumericUpDown { Minimum = 1, Maximum = 999, Value = 1, Width = 80, Font = Tema.Unos, TextAlign = HorizontalAlignment.Right, Margin = new Padding(0, 4, 8, 0), Name = "kolicina" };
            var dodaj = new Dugme("Dodaj u korpu", Ikona.Korpa, Varijanta.Primarno) { Name = "dodaj" };
            dodaj.Click += (_, _) => DodajOdabrani((int)kolicina.Value);

            var karticaProizvoda = new Kartica { Dock = DockStyle.Fill, Padding = new Padding(16, 14, 16, 14) };
            var vrhProizvoda = Raspored.Red(prodavnica, pretraga);
            vrhProizvoda.Dock = DockStyle.Top;
            vrhProizvoda.Padding = new Padding(0, 0, 0, 12);
            var dnoProizvoda = Raspored.Red(Raspored.Oznaka("Količina"), kolicina, dodaj);
            dnoProizvoda.Controls[0].Margin = new Padding(0, 12, 8, 0);
            dnoProizvoda.Dock = DockStyle.Bottom;
            dnoProizvoda.Padding = new Padding(0, 12, 0, 0);
            karticaProizvoda.Controls.Add(proizvodi);
            karticaProizvoda.Controls.Add(vrhProizvoda);
            karticaProizvoda.Controls.Add(dnoProizvoda);

            // ------------------------------------------------------------ korpa
            stavke = Tabela.Nova();
            stavke.RowTemplate.Height = 40;
            stavke.Columns.AddRange(
                Tabela.Kolona("Proizvod", nameof(StavkaKorpe.Naziv), 150, podebljano: true),
                Tabela.Broj("Kol.", nameof(StavkaKorpe.Kolicina), 52),
                Tabela.Kolona("%", nameof(StavkaKorpe.Popust), 48, "'-'0'%';;''", DataGridViewContentAlignment.MiddleCenter),
                Tabela.Novac("Ukupno", nameof(StavkaKorpe.Ukupno), 78, podebljano: true));
            foreach (DataGridViewColumn c in stavke.Columns) c.DefaultCellStyle.Padding = c.HeaderCell.Style.Padding = new Padding(6, 0, 6, 0);
            stavke.CellFormatting += (_, e) =>
            {
                if (e.ColumnIndex >= 0 && stavke.Columns[e.ColumnIndex].Name == nameof(StavkaKorpe.Popust)) e.CellStyle!.ForeColor = Tema.Opasnost;
            };

            var manje = new Dugme("", "") { AutoSize = false, Width = 44 };
            manje.Click += (_, _) => PromijeniOdabranu(-1);
            var vise = new Dugme("", Ikona.Dodaj) { AutoSize = false, Width = 44 };
            vise.Click += (_, _) => PromijeniOdabranu(+1);
            var ukloni = new Dugme("Ukloni", Ikona.Obrisi);
            ukloni.Click += (_, _) => { if (Tabela.Odabrani<StavkaKorpe>(stavke) is { } s) { korpa.Ukloni(s.IdProizvoda); OsvjeziKorpu(); } };
            var alatiKorpe = Raspored.Red(manje, vise, ukloni);
            alatiKorpe.Dock = DockStyle.Top;
            alatiKorpe.Padding = new Padding(0, 8, 0, 12);

            kupon = new Unos("Šifra kupona", Ikona.Akcija) { Width = 190, Name = "kupon" };
            kupon.Polje.CharacterCasing = CharacterCasing.Upper;
            kupon.Polje.KeyDown += async (_, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; await KuponAsync(); } };
            kuponDugme = new Dugme("Primijeni", null) { Name = "primijeniKupon" };
            kuponDugme.Click += async (_, _) => await KuponAsync();
            var redKupona = Raspored.Red(kupon, kuponDugme);
            redKupona.Controls[0].Margin = new Padding(0, 0, 8, 0);
            kuponInfo = new Label { AutoSize = true, Font = Tema.Mali, ForeColor = Tema.Uspjeh, Margin = new Padding(0, 6, 0, 0) };

            medjuzbir = Iznos(Tema.Osnovni, Tema.Tekst);
            usteda = Iznos(Tema.Polu, Tema.Uspjeh);
            ukupno = Iznos(new Font("Segoe UI Semibold", 20f), Tema.Tekst);
            var sazetak = new TableLayoutPanel { ColumnCount = 2, RowCount = 3, Dock = DockStyle.Top, Height = 112, BackColor = Color.White, Margin = new Padding(0, 10, 0, 0) };
            sazetak.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            sazetak.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            sazetak.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
            sazetak.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
            sazetak.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
            sazetak.Controls.Add(Raspored.Oznaka("Međuzbir", null, Tema.TekstSlab), 0, 0);
            sazetak.Controls.Add(medjuzbir, 1, 0);
            sazetak.Controls.Add(Raspored.Oznaka("Ušteda", null, Tema.TekstSlab), 0, 1);
            sazetak.Controls.Add(usteda, 1, 1);
            sazetak.Controls.Add(Raspored.Oznaka("Ukupno", Tema.NaslovKartice), 0, 2);
            sazetak.Controls.Add(ukupno, 1, 2);
            foreach (Control c in sazetak.Controls) c.Margin = new Padding(0);
            foreach (Label l in new[] { medjuzbir, usteda, ukupno }) l.Anchor = AnchorStyles.Right;
            foreach (Control c in sazetak.Controls) if (c is Label l && l.Anchor != AnchorStyles.Right) l.Anchor = AnchorStyles.Left;

            kupi = new Dugme("Kupi", Ikona.Kvacica, Varijanta.Uspjeh) { AutoSize = false, Dock = DockStyle.Bottom, Height = 50, Name = "kupi" };
            kupi.Font = new Font("Segoe UI Semibold", 12f);
            kupi.Click += async (_, _) => await KupiAsync();

            var dnoKorpe = new FlowLayoutPanel { Dock = DockStyle.Bottom, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoSize = true, BackColor = Color.White, Padding = new Padding(0, 8, 0, 12) };
            var linija = new Panel { Height = 1, Width = 340, BackColor = Tema.Okvir, Margin = new Padding(0, 0, 0, 12) };
            dnoKorpe.Controls.AddRange([linija, redKupona, kuponInfo, sazetak]);
            dnoKorpe.Resize += (_, _) => { linija.Width = sazetak.Width = dnoKorpe.Width; };

            karticaKorpe = new Kartica("Korpa") { Dock = DockStyle.Fill, Margin = new Padding(0) };
            karticaKorpe.Controls.Add(stavke);
            karticaKorpe.Controls.Add(alatiKorpe);
            karticaKorpe.Controls.Add(dnoKorpe);
            karticaKorpe.Controls.Add(kupi);

            var mreza = Raspored.Mreza(2, 1);
            mreza.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            mreza.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 420));
            mreza.Controls.Add(karticaProizvoda, 0, 0);
            mreza.Controls.Add(karticaKorpe, 1, 0);
            karticaProizvoda.Margin = new Padding(0, 0, 16, 0);
            Controls.Add(mreza);
            OsvjeziKorpu();
        }

        public override string Naslov => "Kupovina";
        public override string? Opis => "Izaberite prodavnicu, dodajte proizvode u korpu i kupite. Akcije i kuponi se obračunavaju automatski.";

        private static Label Iznos(Font font, Color boja) => new()
        {
            Font = font, ForeColor = boja, AutoSize = true, Anchor = AnchorStyles.Right, TextAlign = ContentAlignment.MiddleRight
        };

        public override Task UcitajAsync() => RadiAsync(async () =>
        {
            if (prodavnica.Items.Count == 0)
            {
                var lista = await K.Repo.Prodavnice.SveAsync();
                prodavnica.Items.AddRange(lista.ToArray<object>());
                prodavnica.SelectedIndex = 0;   // pokrece ucitavanje proizvoda
            }
        });

        private Prodavnica? IzabranaProdavnica => prodavnica.SelectedItem as Prodavnica;

        private async Task PromijeniProdavnicuAsync()
        {
            if (IzabranaProdavnica is not { } p || p.Id == ucitanaProdavnica) return;
            if (!korpa.JePrazna)
            {
                if (!Dijalog.Potvrdi(Vlasnik, "Promjena prodavnice", "Korpa sadrži proizvode druge prodavnice. Isprazniti korpu?", "Isprazni korpu"))
                {
                    prodavnica.SelectedItem = prodavnica.Items.Cast<Prodavnica>().First(x => x.Id == ucitanaProdavnica);
                    return;
                }
                korpa.Isprazni();
                kupon.Text = string.Empty;
                OsvjeziKorpu();
            }
            await UcitajProizvodeAsync(p.Id);
        }

        private async Task UcitajProizvodeAsync(int idProdavnice)
        {
            await RadiAsync(async () =>
            {
                svi = await K.Repo.Proizvodi.PoProdavniciAsync(idProdavnice);
                ucitanaProdavnica = idProdavnice;
                Filtriraj();
            });
        }

        private void Filtriraj()
        {
            string upit = pretraga.Text.Trim();
            var lista = upit.Length == 0 ? svi : svi.Where(p =>
                $"{p.Naziv} {p.Proizvodjac} {p.BarKod}".Contains(upit, StringComparison.CurrentCultureIgnoreCase)).ToList();
            Tabela.Postavi(proizvodi, lista);
        }

        private void Dodaj(Proizvod p, int kol)
        {
            try
            {
                korpa.Dodaj(p, kol);
                OsvjeziKorpu(p.Id);
                K.Shell.Obavijesti($"{p.Naziv} × {kol} je u korpi.", VrstaObavjestenja.Uspjeh);
            }
            catch (Exception e) when (e is InvalidOperationException or ArgumentException)
            {
                K.Shell.Obavijesti(e.Message, VrstaObavjestenja.Greska);
            }
        }

        private void DodajOdabrani(int kol)
        {
            if (Tabela.Odabrani<Proizvod>(proizvodi) is { } p)
            {
                Dodaj(p, kol);
                kolicina.Value = 1;
            }
            else
            {
                K.Shell.Obavijesti("Izaberite proizvod u tabeli.", VrstaObavjestenja.Info);
            }
        }

        private void DodajPoBarkodu()
        {
            string kod = Barkod.Normalizuj(pretraga.Text);
            var p = svi.FirstOrDefault(x => x.BarKod == kod);
            if (p != null)
            {
                Dodaj(p, 1);
                pretraga.Text = string.Empty;
            }
            else if (proizvodi.Rows.Count == 1)
            {
                DodajOdabrani(1);
            }
        }

        private void PromijeniOdabranu(int promjena)
        {
            if (Tabela.Odabrani<StavkaKorpe>(stavke) is not { } s) return;
            try
            {
                korpa.PromijeniKolicinu(s.IdProizvoda, s.Kolicina + promjena);
                OsvjeziKorpu(s.IdProizvoda);
            }
            catch (InvalidOperationException e)
            {
                K.Shell.Obavijesti(e.Message, VrstaObavjestenja.Greska);
            }
        }

        private void OsvjeziKorpu(int? odabrati = null)
        {
            Tabela.Postavi(stavke, korpa.Stavke);
            if (odabrati.HasValue)
            {
                foreach (DataGridViewRow red in stavke.Rows)
                {
                    if (red.DataBoundItem is StavkaKorpe s && s.IdProizvoda == odabrati) stavke.CurrentCell = red.Cells[0];
                }
            }
            karticaKorpe.Naslov = korpa.JePrazna ? "Korpa" : $"Korpa ({korpa.BrojArtikala} kom.)";
            medjuzbir.Text = Cijene.KM(korpa.Medjuzbir);
            usteda.Text = korpa.Usteda > 0 ? "−" + Cijene.KM(korpa.Usteda) : Cijene.KM(0);
            ukupno.Text = Cijene.KM(korpa.Ukupno);
            kupi.Enabled = !korpa.JePrazna;
            kupi.Text = korpa.JePrazna ? "Korpa je prazna" : $"Kupi · {Cijene.KM(korpa.Ukupno)}";
            kuponDugme.Text = korpa.Kupon == null ? "Primijeni" : "Ukloni";
            kuponDugme.Width = kuponDugme.GetPreferredSize(Size.Empty).Width;
        }

        private async Task KuponAsync()
        {
            if (korpa.Kupon != null)
            {
                korpa.UkloniKupon();
                kupon.Text = string.Empty;
                kuponInfo.Text = string.Empty;
                OsvjeziKorpu();
                return;
            }
            if (ucitanaProdavnica is not { } id) return;
            await RadiAsync(async () =>
            {
                var popusti = await K.Repo.Prodaja.ProvjeriKuponAsync(kupon.Text, id);
                korpa.PrimijeniKupon(kupon.Text, popusti);
                int uKorpi = korpa.Stavke.Count(s => s.PopustKupon > 0);
                kuponInfo.ForeColor = uKorpi > 0 ? Tema.Uspjeh : Tema.TekstSlab;
                kuponInfo.Text = popusti.Count == 0
                    ? $"Kupon {korpa.Kupon} ne važi ni za jedan proizvod ove prodavnice."
                    : $"Kupon {korpa.Kupon}: popust na {popusti.Count} proizvoda" + (uKorpi > 0 ? $", {uKorpi} u korpi." : ".");
                OsvjeziKorpu();
            });
        }

        private async Task KupiAsync()
        {
            if (korpa.JePrazna) return;
            decimal iznos = korpa.Ukupno;
            int brojRacuna = 0;
            kupi.Enabled = false;
            if (await RadiAsync(async () => brojRacuna = await K.Repo.Prodaja.NaruciAsync(K.Sesija.Id, korpa, K.Sesija.KorisnickoIme)))
            {
                K.Shell.Obavijesti($"Kupovina uspješna! Račun R-{brojRacuna:D6} na {Cijene.KM(iznos)}.", VrstaObavjestenja.Uspjeh);
                korpa.Isprazni();
                kupon.Text = string.Empty;
                kuponInfo.Text = string.Empty;
                OsvjeziKorpu();
                await UcitajProizvodeAsync(ucitanaProdavnica!.Value);
            }
            else
            {
                OsvjeziKorpu();
            }
        }
    }

    /// <summary>Racuni prijavljenog kupca sa stavkama.</summary>
    internal sealed class MojiRacuniStranica : Stranica
    {
        private readonly DataGridView racuni;
        private readonly DataGridView stavke;
        private readonly StatKartica broj;
        private readonly StatKartica potroseno;
        private readonly StatKartica prosjek;
        private readonly Kartica karticaStavki;

        public MojiRacuniStranica(Kontekst k) : base(k)
        {
            broj = new StatKartica("Računa", Ikona.Racun, Tema.Akcent) { Dock = DockStyle.Fill };
            potroseno = new StatKartica("Ukupno potrošeno", Ikona.Kartica, Tema.Hex("#0D9488")) { Dock = DockStyle.Fill };
            prosjek = new StatKartica("Prosječan račun", Ikona.Trend, Tema.Hex("#DB2777")) { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 0, 16) };

            racuni = Tabela.Nova();
            racuni.Columns.AddRange(
                Tabela.Kolona("Broj", nameof(Racun.Broj), 60, podebljano: true),
                Tabela.Datum("Datum", nameof(Racun.Datum), 80, vrijeme: true),
                Tabela.Kolona("Prodavnica", nameof(Racun.Prodavnica), 100),
                Tabela.Broj("Artikala", nameof(Racun.BrojArtikala), 45),
                Tabela.Kolona("Kupon", nameof(Racun.Kupon), 50),
                Tabela.Novac("Ukupno (KM)", nameof(Racun.Ukupno), 60, podebljano: true),
                Tabela.Kolona("Status", nameof(Racun.Status), 50));
            Tabela.BojaStatusa(racuni, nameof(Racun.Status));
            racuni.SelectionChanged += async (_, _) => await StavkeAsync();
            stavke = StavkeRacuna.NovaTabela();

            var mreza = Raspored.Mreza(3, 2);
            for (int i = 0; i < 3; i++) mreza.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3f));
            mreza.RowStyles.Add(new RowStyle(SizeType.Absolute, 132));
            mreza.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mreza.Controls.Add(broj, 0, 0);
            mreza.Controls.Add(potroseno, 1, 0);
            mreza.Controls.Add(prosjek, 2, 0);

            var donja = Raspored.Mreza(2, 1);
            donja.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));
            donja.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
            var karticaRacuna = Raspored.UKartici("Računi", racuni);
            karticaStavki = Raspored.UKartici("Stavke", stavke);
            karticaStavki.Margin = new Padding(0);
            donja.Controls.Add(karticaRacuna, 0, 0);
            donja.Controls.Add(karticaStavki, 1, 0);
            mreza.Controls.Add(donja, 0, 1);
            mreza.SetColumnSpan(donja, 3);
            Controls.Add(mreza);
        }

        public override string Naslov => "Moji računi";
        public override string? Opis => "Sve vaše kupovine sa stavkama i popustima.";

        public override Task UcitajAsync() => RadiAsync(async () =>
        {
            var lista = await K.Repo.Prodaja.RacuniKupcaAsync(K.Sesija.Id);
            var placeni = lista.Where(r => !r.JeStorniran).ToList();
            broj.Postavi(placeni.Count.ToString(), lista.Count > placeni.Count ? $"{lista.Count - placeni.Count} storniran" : "plaćenih");
            potroseno.Postavi(Cijene.KM(placeni.Sum(r => r.Ukupno)), placeni.Count > 0 ? $"od {placeni.Min(r => r.Datum):dd.MM.yyyy.}" : null);
            prosjek.Postavi(Cijene.KM(placeni.Count == 0 ? 0 : Math.Round(placeni.Average(r => r.Ukupno), 2)));
            Tabela.Postavi(racuni, lista);
            await StavkeAsync();
        });

        private async Task StavkeAsync()
        {
            var r = Tabela.Odabrani<Racun>(racuni);
            karticaStavki.Naslov = r == null ? "Stavke" : $"Stavke računa {r.Broj}";
            if (r == null) return;
            var lista = await K.Repo.Prodaja.StavkeAsync(r.Id);
            Tabela.Postavi(stavke, lista);
            karticaStavki.Podnaslov = lista.Sum(s => s.Usteda) is var u and > 0 ? $"· ušteda {Cijene.KM(u)}" : null;
            karticaStavki.Invalidate();
        }
    }
}
