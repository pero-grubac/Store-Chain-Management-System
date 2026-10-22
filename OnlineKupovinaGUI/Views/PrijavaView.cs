using OnlineKupovina.Data;
using OnlineKupovinaDB;
using OnlineKupovinaGUI.Ui;
using System.Drawing.Drawing2D;

namespace OnlineKupovinaGUI.Views
{
    /// <summary>Prijava preko cijelog prozora: lijevo predstavljanje sistema, desno forma.</summary>
    internal sealed class PrijavaView : UserControl
    {
        private readonly Repozitoriji repo;
        private readonly Unos korisnickoIme;
        private readonly Unos lozinka;
        private readonly Label greska;
        private readonly Dugme prijava;
        private readonly List<(Dugme Dugme, Uloga Uloga)> uloge = new();
        private Uloga izabrana = Uloga.Kupac;

        public PrijavaView(Repozitoriji repo)
        {
            this.repo = repo;
            Dock = DockStyle.Fill;
            BackColor = Color.White;
            DoubleBuffered = true;

            var lijevo = new Panel { Dock = DockStyle.Left, BackColor = Tema.Meni };
            lijevo.Paint += CrtajPredstavljanje;
            Resize += (_, _) => { lijevo.Width = (int)(Width * 0.52); lijevo.Invalidate(); };

            var desno = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            var forma = new Panel { Size = new Size(400, 560), BackColor = Color.White };
            desno.Resize += (_, _) => forma.Location = new Point((desno.Width - forma.Width) / 2, Math.Max(20, (desno.Height - forma.Height) / 2));

            var naslov = new Label { Text = "Dobro došli", Font = Tema.Naslov, ForeColor = Tema.Tekst, AutoSize = true, Location = new Point(0, 0) };
            var podnaslov = new Label { Text = "Izaberite ulogu i prijavite se na svoj nalog.", ForeColor = Tema.TekstSlab, AutoSize = true, Location = new Point(2, 46) };

            var oznakaUloge = new Label { Text = "ULOGA", Font = Tema.MaliPolu, ForeColor = Tema.TekstSlab, AutoSize = true, Location = new Point(0, 96) };
            var izborUloge = new Panel { Location = new Point(0, 120), Size = new Size(400, 46), BackColor = Tema.Pozadina };
            izborUloge.Paint += (_, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.Clear(Color.White);
                using var path = Tema.Zaobljeno(new RectangleF(0, 0, izborUloge.Width - 1, izborUloge.Height - 1), 10);
                using var cetka = new SolidBrush(Tema.Pozadina);
                e.Graphics.FillPath(cetka, path);
            };
            int x = 4;
            foreach (var (uloga, ikona) in new[] { (Uloga.Kupac, Ikona.Korpa), (Uloga.Zaposleni, Ikona.Proizvod), (Uloga.Administrator, Ikona.Admin) })
            {
                var d = new Dugme(uloga.Naziv(), ikona) { AutoSize = false, Size = new Size(128, 38), Location = new Point(x, 4), Name = "uloga" + uloga };
                d.PosebnoCrtanje = CrtajUlogu;
                d.Click += (_, _) => { izabrana = uloga; foreach (var u in uloge) u.Dugme.Invalidate(); korisnickoIme!.Polje.Focus(); };
                uloge.Add((d, uloga));
                izborUloge.Controls.Add(d);
                x += 130;
            }

            var oznakaKorisnika = new Label { Text = "KORISNIČKO IME", Font = Tema.MaliPolu, ForeColor = Tema.TekstSlab, AutoSize = true, Location = new Point(0, 188) };
            korisnickoIme = new Unos("npr. kupac", Ikona.Osoba) { Location = new Point(0, 212), Width = 400, Height = 46, Name = "korisnickoIme" };
            var oznakaLozinke = new Label { Text = "LOZINKA", Font = Tema.MaliPolu, ForeColor = Tema.TekstSlab, AutoSize = true, Location = new Point(0, 274) };
            lozinka = new Unos("••••••••", Ikona.Brava, lozinka: true) { Location = new Point(0, 298), Width = 400, Height = 46, Name = "lozinka" };

            greska = new Label
            {
                ForeColor = Tema.Opasnost, BackColor = Tema.OpasnostSvijetli, AutoSize = false, Size = new Size(400, 38), Location = new Point(0, 358),
                TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(10, 0, 0, 0), Visible = false
            };
            prijava = new Dugme("Prijava", null, Varijanta.Primarno) { AutoSize = false, Location = new Point(0, 410), Size = new Size(400, 48), Name = "prijava" };
            prijava.Click += async (_, _) => await PrijaviAsync();
            korisnickoIme.Polje.KeyDown += async (_, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; lozinka.Polje.Focus(); } };
            lozinka.Polje.KeyDown += async (_, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; await PrijaviAsync(); } };

            var napomena = new Label
            {
                Text = "F11 – prikaz preko cijelog ekrana", Font = Tema.Mali, ForeColor = Tema.TekstSlab, AutoSize = false,
                Size = new Size(400, 20), Location = new Point(0, 480), TextAlign = ContentAlignment.MiddleCenter
            };

            forma.Controls.AddRange([naslov, podnaslov, oznakaUloge, izborUloge, oznakaKorisnika, korisnickoIme, oznakaLozinke, lozinka, greska, prijava, napomena]);
            desno.Controls.Add(forma);
            Controls.Add(desno);
            Controls.Add(lijevo);
        }

        public event EventHandler<Sesija>? Prijavljen;

        public void Fokusiraj() => korisnickoIme.Polje.Focus();

        private void CrtajUlogu(Dugme d, PaintEventArgs e)
        {
            bool aktivna = uloge.Any(u => u.Dugme == d && u.Uloga == izabrana);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Tema.Pozadina);
            if (aktivna || d.Hover)
            {
                using var path = Tema.Zaobljeno(new RectangleF(0, 0, d.Width - 1, d.Height - 1), 8);
                using var cetka = new SolidBrush(aktivna ? Color.White : Tema.Hex("#E9EEF5"));
                g.FillPath(cetka, path);
                if (aktivna)
                {
                    using var okvir = new Pen(Tema.Okvir);
                    g.DrawPath(okvir, path);
                }
            }
            var boja = aktivna ? Tema.Akcent : Tema.TekstSlab;
            var tekst = TextRenderer.MeasureText(d.Text, Tema.Polu);
            int x = (d.Width - tekst.Width - 22) / 2;
            using var f = Tema.Ikone(11f);
            TextRenderer.DrawText(g, d.IkonaZnak, f, new Rectangle(x, 0, 18, d.Height), boja, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
            TextRenderer.DrawText(g, d.Text, Tema.Polu, new Rectangle(x + 22, 0, tekst.Width + 4, d.Height), boja, TextFormatFlags.VerticalCenter);
        }

        private void CrtajPredstavljanje(object? sender, PaintEventArgs e)
        {
            var panel = (Panel)sender!;
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var gradijent = new LinearGradientBrush(panel.ClientRectangle, Tema.Hex("#312E81"), Tema.Hex("#0F172A"), 60f))
            {
                g.FillRectangle(gradijent, panel.ClientRectangle);
            }
            using (var krug = new SolidBrush(Color.FromArgb(28, 129, 140, 248)))
            {
                g.FillEllipse(krug, panel.Width - 260, -120, 420, 420);
                g.FillEllipse(krug, -140, panel.Height - 260, 380, 380);
            }

            int lijevo = Math.Max(48, panel.Width / 9);
            int vrh = Math.Max(60, panel.Height / 2 - 220);
            var logo = new Rectangle(lijevo, vrh, 56, 56);
            using (var path = Tema.Zaobljeno(logo, 14))
            using (var cetka = new SolidBrush(Tema.Akcent))
            {
                g.FillPath(cetka, path);
            }
            using (var f = Tema.Ikone(24f))
            {
                TextRenderer.DrawText(g, Ikona.Korpa, f, logo, Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }

            using var velikiNaslov = new Font("Segoe UI Semibold", 30f);
            TextRenderer.DrawText(g, "Store Chain", velikiNaslov, new Point(lijevo - 4, vrh + 80), Color.White);
            TextRenderer.DrawText(g, "Management System", velikiNaslov, new Point(lijevo - 4, vrh + 128), Tema.Hex("#A5B4FC"));
            using var tekst = new Font("Segoe UI", 12f);
            TextRenderer.DrawText(g, "Prodavnice, zaposleni, zalihe, akcije i kupovina\nza cijeli lanac na jednom mjestu.", tekst,
                new Rectangle(lijevo, vrh + 196, panel.Width - lijevo - 40, 60), Tema.Hex("#CBD5E1"), TextFormatFlags.WordBreak);

            string[] stavke =
            [
                "Kupovina sa korpom, akcijama i kuponima",
                "Skeniranje bar kodova i prijem robe",
                "Zalihe i iznosi računa koje čuvaju trigeri",
                "Izvještaji o prihodu i najprodavanijim proizvodima"
            ];
            int y = vrh + 284;
            foreach (var s in stavke)
            {
                var kvacica = new Rectangle(lijevo, y, 26, 26);
                using (var cetka = new SolidBrush(Color.FromArgb(60, 165, 180, 252))) g.FillEllipse(cetka, kvacica);
                using (var f = Tema.Ikone(10f)) TextRenderer.DrawText(g, Ikona.Kvacica, f, kvacica, Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                TextRenderer.DrawText(g, s, tekst, new Point(lijevo + 40, y + 2), Color.White);
                y += 42;
            }
        }

        private async Task PrijaviAsync()
        {
            greska.Visible = false;
            prijava.Enabled = false;
            prijava.Text = "Prijava...";
            UseWaitCursor = true;
            try
            {
                var sesija = await repo.Auth.PrijavaAsync(izabrana, korisnickoIme.Text, lozinka.Text);
                if (sesija == null)
                {
                    PrikaziGresku($"Pogrešno korisničko ime ili lozinka za ulogu {izabrana.Naziv().ToLowerInvariant()}.");
                    lozinka.Text = string.Empty;
                    lozinka.Polje.Focus();
                    return;
                }
                Prijavljen?.Invoke(this, sesija);
            }
            catch (BazaNedostupnaException e)
            {
                PrikaziGresku(e.Message);
            }
            catch (Exception e)
            {
                PrikaziGresku("Prijava nije uspjela: " + e.Message);
            }
            finally
            {
                if (!IsDisposed)
                {
                    prijava.Enabled = true;
                    prijava.Text = "Prijava";
                    UseWaitCursor = false;
                }
            }
        }

        private void PrikaziGresku(string poruka)
        {
            greska.Text = poruka;
            greska.Height = Math.Max(38, TextRenderer.MeasureText(poruka, greska.Font, new Size(380, 0), TextFormatFlags.WordBreak).Height + 16);
            greska.Visible = true;
        }
    }
}
