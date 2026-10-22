using OnlineKupovina.Data;
using OnlineKupovinaDB;

namespace OnlineKupovinaGUI.Ui
{
    /// <summary>Jedno polje u dijalogu: oznaka + kontrola, preko cijele sirine ili pola.</summary>
    internal abstract class Polje
    {
        protected Polje(string naziv, bool puna)
        {
            Naziv = naziv;
            Puna = puna;
        }

        public string Naziv { get; }
        public bool Puna { get; }
        public abstract Control Kontrola { get; }
        public string? Pomoc { get; init; }
    }

    internal sealed class TekstPolje : Polje
    {
        private readonly Unos unos;

        public TekstPolje(string naziv, string? vrijednost = null, bool puna = false, bool lozinka = false, string? placeholder = null, int maxDuzina = 255)
            : base(naziv, puna)
        {
            unos = new Unos(placeholder, lozinka: lozinka) { Dock = DockStyle.Top };
            unos.Polje.MaxLength = maxDuzina;
            unos.Text = vrijednost ?? string.Empty;
        }

        public string Vrijednost => unos.Text.Trim();
        public string Sirovo => unos.Text;
        public TextBox Polje => unos.Polje;
        public override Control Kontrola => unos;
    }

    internal sealed class BrojPolje : Polje
    {
        private readonly NumericUpDown broj;

        public BrojPolje(string naziv, decimal vrijednost, decimal min, decimal max, int decimale = 0, bool puna = false) : base(naziv, puna)
        {
            broj = new NumericUpDown
            {
                Minimum = min,
                Maximum = max,
                DecimalPlaces = decimale,
                Increment = decimale > 0 ? 0.10m : 1,
                ThousandsSeparator = true,
                Font = Tema.Unos,
                Dock = DockStyle.Top,
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = HorizontalAlignment.Right
            };
            broj.Value = Math.Clamp(vrijednost, min, max);
            broj.Enter += (_, _) => broj.Select(0, broj.Text.Length);
        }

        public decimal Vrijednost => broj.Value;
        public int Cijeli => (int)broj.Value;
        public NumericUpDown Polje => broj;
        public override Control Kontrola => broj;
    }

    internal sealed class DatumPolje : Polje
    {
        private readonly DateTimePicker datum;

        public DatumPolje(string naziv, DateTime? vrijednost, bool opciono = false, bool puna = false) : base(naziv, puna)
        {
            datum = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Font = Tema.Unos,
                Dock = DockStyle.Top,
                ShowCheckBox = opciono
            };
            datum.Value = vrijednost ?? DateTime.Today;
            if (opciono) datum.Checked = vrijednost.HasValue;
        }

        public DateTime? Vrijednost => datum.ShowCheckBox && !datum.Checked ? null : datum.Value.Date;
        public override Control Kontrola => datum;
    }

    internal sealed class IzborPolje<T> : Polje where T : class
    {
        private sealed record Stavka(T Vrijednost, string Prikaz)
        {
            public override string ToString() => Prikaz;
        }

        private readonly ComboBox combo;

        public IzborPolje(string naziv, IEnumerable<T> stavke, Func<T, string> prikaz, Func<T, bool>? izabrano = null, bool puna = false)
            : base(naziv, puna)
        {
            combo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.System,
                Font = Tema.Unos,
                Dock = DockStyle.Top,
                MaxDropDownItems = 14
            };
            var lista = stavke.Select(s => new Stavka(s, prikaz(s))).ToArray();
            combo.Items.AddRange(lista);
            int indeks = izabrano == null ? -1 : Array.FindIndex(lista, s => izabrano(s.Vrijednost));
            combo.SelectedIndex = indeks >= 0 ? indeks : (lista.Length == 1 ? 0 : -1);
        }

        public T? Vrijednost => (combo.SelectedItem as Stavka)?.Vrijednost;
        public override Control Kontrola => combo;
    }

    /// <summary>Bar kod sa dugmetom za interni EAN-13; skener moze upisati kod direktno u polje.</summary>
    internal sealed class BarkodPolje : Polje
    {
        private readonly Panel panel;
        private readonly Unos unos;

        public BarkodPolje(string naziv, string? vrijednost, bool puna = false) : base(naziv, puna)
        {
            panel = new Panel { Height = 40, Dock = DockStyle.Top, BackColor = Color.White };
            unos = new Unos("Skenirajte ili upišite bar kod", Ikona.Skener) { Dock = DockStyle.Fill, Text = vrijednost ?? string.Empty };
            unos.Polje.MaxLength = Barkod.MaxDuzina;
            var generisi = new Dugme("Generiši", null, Varijanta.Sekundarno) { Dock = DockStyle.Right, AutoSize = false, Width = 100, Margin = new Padding(0) };
            generisi.Click += (_, _) => unos.Text = Barkod.GenerisiInterni();
            new ToolTip().SetToolTip(generisi, "Interni EAN-13 (prefiks 20) za robu bez bar koda");
            panel.Controls.Add(unos);
            panel.Controls.Add(new Panel { Dock = DockStyle.Right, Width = 8, BackColor = Color.White });
            panel.Controls.Add(generisi);
        }

        public string Vrijednost => Barkod.Normalizuj(unos.Text);
        public override Control Kontrola => panel;
    }

    /// <summary>
    /// Modalni dijalog za unos ili potvrdu. Cuvanje je asinhrono; greske validacije
    /// i baze se prikazuju u samom dijalogu, pa korisnik ne gubi unesene podatke.
    /// </summary>
    internal sealed class Dijalog : Form
    {
        private readonly Func<Task>? sacuvaj;
        private readonly Label greska;
        private readonly Dugme potvrdi;
        private readonly Dugme otkazi;

        private Dijalog(string naslov, string? opis, IReadOnlyList<Polje> polja, Func<Task>? sacuvaj, string tekstPotvrde, Varijanta varijanta)
        {
            this.sacuvaj = sacuvaj;
            Text = naslov;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;
            Font = Tema.Osnovni;
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(polja.Count == 0 ? 460 : 620, 100);

            var sadrzaj = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                Padding = new Padding(28, 24, 28, 12),
                BackColor = Color.White,
                AutoSize = true
            };
            sadrzaj.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            sadrzaj.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            var naslovLbl = new Label { Text = naslov, Font = Tema.NaslovKartice, ForeColor = Tema.Tekst, AutoSize = true, Margin = new Padding(0, 0, 0, 4) };
            sadrzaj.Controls.Add(naslovLbl);
            sadrzaj.SetColumnSpan(naslovLbl, 2);
            if (opis != null)
            {
                var opisLbl = new Label
                {
                    Text = opis, ForeColor = Tema.TekstSlab, AutoSize = true, MaximumSize = new Size(ClientSize.Width - 60, 0), Margin = new Padding(0, 0, 0, 12)
                };
                sadrzaj.Controls.Add(opisLbl);
                sadrzaj.SetColumnSpan(opisLbl, 2);
            }

            int kolona = 0;
            foreach (var polje in polja)
            {
                var celija = new Panel { Dock = DockStyle.Top, Height = 72, Margin = new Padding(kolona == 0 ? 0 : 8, 6, kolona == 0 && !polje.Puna ? 8 : 0, 0), BackColor = Color.White };
                var oznaka = new Label { Text = polje.Naziv, Font = Tema.MaliPolu, ForeColor = Tema.TekstSlab, Dock = DockStyle.Top, Height = 22 };
                polje.Kontrola.Dock = DockStyle.Top;
                polje.Kontrola.Name = "polje:" + polje.Naziv;
                celija.Controls.Add(polje.Kontrola);
                celija.Controls.Add(oznaka);
                if (polje.Pomoc != null)
                {
                    var pomoc = new Label { Text = polje.Pomoc, Font = Tema.Mali, ForeColor = Tema.TekstSlab, Dock = DockStyle.Bottom, Height = 18 };
                    celija.Controls.Add(pomoc);
                    celija.Height += 10;
                }
                if (polje.Kontrola is TextBox { Multiline: true }) celija.Height = 110;

                if (polje.Puna && kolona == 1) kolona = 0;
                sadrzaj.Controls.Add(celija);
                if (polje.Puna)
                {
                    sadrzaj.SetColumnSpan(celija, 2);
                    celija.Margin = new Padding(0, 6, 0, 0);
                    kolona = 0;
                }
                else
                {
                    kolona = 1 - kolona;
                }
            }
            if (kolona == 1) sadrzaj.Controls.Add(new Panel { Height = 1, BackColor = Color.White });

            greska = new Label
            {
                ForeColor = Tema.Opasnost, BackColor = Tema.OpasnostSvijetli, AutoSize = true, Visible = false,
                Padding = new Padding(10, 8, 10, 8), Margin = new Padding(0, 12, 0, 0), MaximumSize = new Size(ClientSize.Width - 56, 0)
            };
            sadrzaj.Controls.Add(greska);
            sadrzaj.SetColumnSpan(greska, 2);

            var podnozje = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                FlowDirection = FlowDirection.RightToLeft,
                Height = 72,
                Padding = new Padding(20, 16, 20, 16),
                BackColor = Tema.ZaglavljeTabele
            };
            potvrdi = new Dugme(tekstPotvrde, null, varijanta) { Margin = new Padding(8, 0, 0, 0) };
            otkazi = new Dugme("Otkaži") { Margin = new Padding(0), DialogResult = DialogResult.Cancel };
            podnozje.Controls.Add(potvrdi);
            podnozje.Controls.Add(otkazi);
            potvrdi.Click += async (_, _) => await PotvrdiAsync();

            Controls.Add(sadrzaj);
            Controls.Add(podnozje);
            AcceptButton = potvrdi;
            CancelButton = otkazi;

            Shown += (_, _) =>
            {
                var prva = polja.FirstOrDefault()?.Kontrola;
                (prva is Unos u ? u.Polje : prva is Panel p ? p.Controls.OfType<Unos>().FirstOrDefault()?.Polje : prva)?.Focus();
            };
            Load += (_, _) => PrilagodiVisinu(sadrzaj);
        }

        private void PrilagodiVisinu(TableLayoutPanel sadrzaj)
        {
            sadrzaj.PerformLayout();
            int visina = sadrzaj.GetPreferredSize(new Size(ClientSize.Width, 0)).Height + 72 + 24;
            ClientSize = new Size(ClientSize.Width, Math.Min(visina, Screen.FromControl(this).WorkingArea.Height - 80));
        }

        private async Task PotvrdiAsync()
        {
            if (sacuvaj == null)
            {
                DialogResult = DialogResult.OK;
                return;
            }
            potvrdi.Enabled = otkazi.Enabled = false;
            UseWaitCursor = true;
            greska.Visible = false;
            try
            {
                await sacuvaj();
                DialogResult = DialogResult.OK;
            }
            catch (Exception e) when (e is PoslovnaGreskaException or BazaNedostupnaException or InvalidOperationException or ArgumentException)
            {
                PrikaziGresku(e.Message);
            }
            catch (Exception e)
            {
                PrikaziGresku("Neočekivana greška: " + e.Message);
            }
            finally
            {
                potvrdi.Enabled = otkazi.Enabled = true;
                UseWaitCursor = false;
            }
        }

        private void PrikaziGresku(string poruka)
        {
            greska.Text = poruka;
            greska.Visible = true;
            PrilagodiVisinu((TableLayoutPanel)Controls[0]);
        }

        public static bool Prikazi(IWin32Window vlasnik, string naslov, IReadOnlyList<Polje> polja, Func<Task> sacuvaj,
            string? opis = null, string tekstPotvrde = "Sačuvaj")
        {
            using var d = new Dijalog(naslov, opis, polja, sacuvaj, tekstPotvrde, Varijanta.Primarno);
            return d.ShowDialog(vlasnik) == DialogResult.OK;
        }

        public static bool Potvrdi(IWin32Window vlasnik, string naslov, string poruka, string tekstPotvrde = "Obriši", bool opasno = true)
        {
            using var d = new Dijalog(naslov, poruka, [], null, tekstPotvrde, opasno ? Varijanta.Opasno : Varijanta.Primarno);
            return d.ShowDialog(vlasnik) == DialogResult.OK;
        }
    }
}
