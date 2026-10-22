using OnlineKupovina.Data;
using OnlineKupovinaDB;
using OnlineKupovinaGUI.Ui;
using OnlineKupovinaGUI.Views.Stranice;

namespace OnlineKupovinaGUI.Views
{
    /// <summary>Okvir nakon prijave: bocni meni za ulogu, zaglavlje stranice i sadrzaj.</summary>
    internal sealed class ShellView : UserControl
    {
        private sealed record StavkaMenija(string Tekst, string Ikona, Func<Kontekst, Stranica> Napravi);

        private readonly Kontekst kontekst;
        private readonly Panel sadrzaj;
        private readonly Label naslov;
        private readonly Label opis;
        private readonly ProgressBar zauzeto;
        private readonly FlowLayoutPanel meni;
        private readonly List<(Dugme Dugme, StavkaMenija Stavka)> dugmad = new();
        private int brojacZauzetosti;
        private Stranica? aktivna;
        private StavkaMenija? aktivnaStavka;

        public ShellView(Repozitoriji repo, Sesija sesija)
        {
            kontekst = new Kontekst(repo, sesija, this);
            Dock = DockStyle.Fill;
            BackColor = Tema.Pozadina;
            DoubleBuffered = true;

            // ---------------------------------------------------------- bocni meni
            var bocni = new Panel { Dock = DockStyle.Left, Width = 260, BackColor = Tema.Meni, Padding = new Padding(16, 20, 16, 16) };
            var logo = new Panel { Dock = DockStyle.Top, Height = 64, BackColor = Tema.Meni };
            logo.Paint += (_, e) =>
            {
                var krug = new Rectangle(4, 6, 40, 40);
                using var cetka = new SolidBrush(Tema.Akcent);
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var path = Tema.Zaobljeno(krug, 10)) e.Graphics.FillPath(cetka, path);
                using var f = Tema.Ikone(16f);
                TextRenderer.DrawText(e.Graphics, Ikona.Korpa, f, krug, Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                TextRenderer.DrawText(e.Graphics, "Store Chain", Tema.NaslovKartice, new Point(54, 6), Color.White);
                TextRenderer.DrawText(e.Graphics, "Management System", Tema.Mali, new Point(55, 28), Tema.MeniTekst);
            };

            meni = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, BackColor = Tema.Meni, Padding = new Padding(0, 12, 0, 0) };
            foreach (var stavka in Meni(sesija.Uloga))
            {
                var d = new Dugme(stavka.Tekst, stavka.Ikona, Varijanta.Meni)
                {
                    AutoSize = false, Width = 228, Height = 44, TextAlign = ContentAlignment.MiddleLeft, Margin = new Padding(0, 0, 0, 4)
                };
                d.PosebnoCrtanje = CrtajStavkuMenija;
                d.Click += async (_, _) => await OtvoriAsync(stavka);
                dugmad.Add((d, stavka));
                meni.Controls.Add(d);
            }

            var korisnik = new Panel { Dock = DockStyle.Bottom, Height = 124, BackColor = Tema.Meni };
            korisnik.Paint += (_, e) =>
            {
                using var linija = new Pen(Tema.MeniHover);
                e.Graphics.DrawLine(linija, 0, 0, korisnik.Width, 0);
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                var avatar = new Rectangle(2, 18, 40, 40);
                using var cetka = new SolidBrush(BojaUloge(sesija.Uloga));
                e.Graphics.FillEllipse(cetka, avatar);
                string inicijali = string.Concat(sesija.PunoIme.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2).Select(s => char.ToUpper(s[0])));
                TextRenderer.DrawText(e.Graphics, inicijali, Tema.Polu, avatar, Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                TextRenderer.DrawText(e.Graphics, sesija.PunoIme, Tema.Polu, new Rectangle(52, 18, korisnik.Width - 52, 20), Color.White, TextFormatFlags.EndEllipsis);
                string uloga = sesija.Uloga.Naziv() + (sesija.ImeProdavnice != null ? " · " + sesija.ImeProdavnice : string.Empty);
                TextRenderer.DrawText(e.Graphics, uloga, Tema.Mali, new Rectangle(52, 40, korisnik.Width - 52, 20), Tema.MeniTekst, TextFormatFlags.EndEllipsis);
            };
            var odjava = new Dugme("Odjava", Ikona.Odjava, Varijanta.Meni) { AutoSize = false, Width = 228, Height = 40, Location = new Point(0, 74) };
            odjava.PosebnoCrtanje = CrtajStavkuMenija;
            odjava.Click += (_, _) => Odjava?.Invoke(this, EventArgs.Empty);
            korisnik.Controls.Add(odjava);

            bocni.Controls.Add(meni);
            bocni.Controls.Add(korisnik);
            bocni.Controls.Add(logo);

            // ---------------------------------------------------------- zaglavlje i sadrzaj
            var desno = new Panel { Dock = DockStyle.Fill, BackColor = Tema.Pozadina, Padding = new Padding(32, 24, 32, 16) };
            var zaglavlje = new Panel { Dock = DockStyle.Top, Height = 84, BackColor = Tema.Pozadina };
            naslov = new Label { Font = Tema.Naslov, ForeColor = Tema.Tekst, AutoSize = true, Location = new Point(0, 0) };
            opis = new Label { Font = Tema.Osnovni, ForeColor = Tema.TekstSlab, AutoSize = true, Location = new Point(2, 42) };
            zauzeto = new ProgressBar
            {
                Style = ProgressBarStyle.Marquee, MarqueeAnimationSpeed = 25, Height = 4, Width = 160, Visible = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            var datum = new Label
            {
                Text = DateTime.Today.ToString("dddd, d. MMMM yyyy."), Font = Tema.Osnovni, ForeColor = Tema.TekstSlab, AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            zaglavlje.Controls.AddRange([naslov, opis, datum, zauzeto]);
            zaglavlje.Resize += (_, _) =>
            {
                datum.Location = new Point(zaglavlje.Width - datum.Width, 10);
                zauzeto.Location = new Point(zaglavlje.Width - zauzeto.Width, 40);
            };

            sadrzaj = new Panel { Dock = DockStyle.Fill, BackColor = Tema.Pozadina };
            desno.Controls.Add(sadrzaj);
            desno.Controls.Add(zaglavlje);

            Controls.Add(desno);
            Controls.Add(bocni);
        }

        public event EventHandler? Odjava;

        public Task PrvaStranicaAsync() => OtvoriAsync(dugmad[0].Stavka);

        private static Color BojaUloge(Uloga uloga) => uloga switch
        {
            Uloga.Administrator => Tema.Akcent,
            Uloga.Zaposleni => Tema.Hex("#0D9488"),
            _ => Tema.Hex("#E11D48")
        };

        private static IEnumerable<StavkaMenija> Meni(Uloga uloga) => uloga switch
        {
            Uloga.Administrator =>
            [
                new("Pregled", Ikona.Pregled, k => new PregledStranica(k)),
                new("Prodavnice", Ikona.Prodavnica, k => new ProdavniceStranica(k)),
                new("Zaposleni", Ikona.Ljudi, k => new ZaposleniStranica(k)),
                new("Kupci", Ikona.Osoba, k => new KupciStranica(k)),
                new("Akcije i kuponi", Ikona.Akcija, k => new AkcijeStranica(k)),
                new("Računi", Ikona.Racun, k => new RacuniStranica(k)),
                new("Izvještaji", Ikona.Izvjestaj, k => new IzvjestajiStranica(k)),
                new("Administratori", Ikona.Admin, k => new AdministratoriStranica(k)),
                new("Podešavanja", Ikona.Podesavanja, k => new PodesavanjaStranica(k)),
            ],
            Uloga.Zaposleni =>
            [
                new("Proizvodi", Ikona.Proizvod, k => new ProizvodiStranica(k)),
                new("Zalihe", Ikona.Upozorenje, k => new ZaliheStranica(k)),
                new("Proizvođači", Ikona.Fabrika, k => new ProizvodjaciStranica(k)),
                new("Podešavanja", Ikona.Podesavanja, k => new PodesavanjaStranica(k)),
            ],
            _ =>
            [
                new("Kupovina", Ikona.Korpa, k => new KupovinaStranica(k)),
                new("Moji računi", Ikona.Racun, k => new MojiRacuniStranica(k)),
                new("Podešavanja", Ikona.Podesavanja, k => new PodesavanjaStranica(k)),
            ]
        };

        private void CrtajStavkuMenija(Dugme d, PaintEventArgs e)
        {
            // Dugme crta pozadinu; ovdje se tekst poravnava lijevo i oznacava aktivna stavka
            bool aktivno = dugmad.Any(x => x.Dugme == d && x.Stavka == aktivnaStavka);
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            var r = new RectangleF(0, 0, d.Width - 1, d.Height - 1);
            using (var path = Tema.Zaobljeno(r, 8))
            using (var cetka = new SolidBrush(aktivno || d.Hover ? Tema.MeniHover : Tema.Meni))
            {
                g.Clear(Tema.Meni);
                g.FillPath(cetka, path);
            }
            if (aktivno)
            {
                using var akcent = new SolidBrush(Tema.Akcent);
                using var path = Tema.Zaobljeno(new RectangleF(0, 10, 4, d.Height - 20), 2);
                g.FillPath(akcent, path);
            }
            var boja = aktivno ? Color.White : Tema.MeniTekst;
            using var f = Tema.Ikone(13f);
            TextRenderer.DrawText(g, d.IkonaZnak, f, new Rectangle(14, 0, 24, d.Height), boja, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
            TextRenderer.DrawText(g, d.Text, aktivno ? Tema.Polu : Tema.Osnovni, new Rectangle(50, 0, d.Width - 56, d.Height), boja, TextFormatFlags.VerticalCenter);
        }

        private async Task OtvoriAsync(StavkaMenija stavka)
        {
            if (stavka == aktivnaStavka) return;

            var nova = stavka.Napravi(kontekst);
            aktivnaStavka = stavka;
            sadrzaj.SuspendLayout();
            aktivna?.Dispose();
            sadrzaj.Controls.Clear();
            aktivna = nova;
            naslov.Text = nova.Naslov;
            opis.Text = nova.Opis ?? string.Empty;
            sadrzaj.Controls.Add(nova);
            sadrzaj.ResumeLayout();
            foreach (var (d, _) in dugmad) d.Invalidate();
            await nova.UcitajAsync();
        }

        public void Zauzeto(bool da)
        {
            brojacZauzetosti = Math.Max(0, brojacZauzetosti + (da ? 1 : -1));
            zauzeto.Visible = brojacZauzetosti > 0;
            Cursor = brojacZauzetosti > 0 ? Cursors.AppStarting : Cursors.Default;
        }

        public void Obavijesti(string poruka, VrstaObavjestenja vrsta) => Obavjestenje.Prikazi(this, poruka, vrsta);
    }
}
