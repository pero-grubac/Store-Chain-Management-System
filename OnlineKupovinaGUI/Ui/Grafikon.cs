using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace OnlineKupovinaGUI.Ui
{
    internal enum VrstaGrafikona
    {
        Stupci,
        Trake
    }

    /// <summary>Jednostavan grafikon bez dodatnih biblioteka: stupci (vrijeme) ili horizontalne trake (poredjenje).</summary>
    internal sealed class Grafikon : Control
    {
        private IReadOnlyList<(string Oznaka, decimal Vrijednost)> podaci = [];
        private int hoverIndeks = -1;

        public Grafikon(VrstaGrafikona vrsta)
        {
            Vrsta = vrsta;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            BackColor = Color.White;
            Font = Tema.Mali;
            Dock = DockStyle.Fill;
        }

        public VrstaGrafikona Vrsta { get; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color Boja { get; set; } = Tema.Akcent;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<decimal, string> Format { get; set; } = v => v.ToString("N0");

        public void Postavi(IEnumerable<(string Oznaka, decimal Vrijednost)> vrijednosti)
        {
            podaci = vrijednosti.ToList();
            hoverIndeks = -1;
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            int novi = IndeksNa(e.Location);
            if (novi != hoverIndeks)
            {
                hoverIndeks = novi;
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            hoverIndeks = -1;
            Invalidate();
        }

        private Rectangle Podrucje() => Vrsta == VrstaGrafikona.Stupci
            ? new Rectangle(64, 12, Width - 76, Height - 40)
            : new Rectangle(170, 4, Width - 260, Height - 8);

        private int IndeksNa(Point p)
        {
            if (podaci.Count == 0) return -1;
            var r = Podrucje();
            if (Vrsta == VrstaGrafikona.Stupci)
            {
                if (p.X < r.Left || p.X > r.Right) return -1;
                return Math.Clamp((int)((p.X - r.Left) / (r.Width / (float)podaci.Count)), 0, podaci.Count - 1);
            }
            float visina = r.Height / (float)podaci.Count;
            return p.Y < r.Top || p.Y > r.Bottom ? -1 : Math.Clamp((int)((p.Y - r.Top) / visina), 0, podaci.Count - 1);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(BackColor);
            if (podaci.Count == 0 || Width < 120 || Height < 60)
            {
                TextRenderer.DrawText(g, "Nema podataka za izabrani period", Tema.Osnovni, ClientRectangle, Tema.TekstSlab,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                return;
            }
            if (Vrsta == VrstaGrafikona.Stupci) CrtajStupce(g);
            else CrtajTrake(g);
        }

        /// <summary>Gornja granica ose djeljiva na 4 okrugla koraka (npr. 60 = 4 x 15).</summary>
        private static decimal ZaokruziMax(decimal max)
        {
            if (max <= 0) return 4;
            double korak = (double)max / 4;
            double red = Math.Pow(10, Math.Floor(Math.Log10(korak)));
            double lijep = new[] { 1, 1.5, 2, 2.5, 3, 4, 5, 6, 8, 10 }.First(k => k * red >= korak);
            return (decimal)(lijep * red * 4);
        }

        private void CrtajStupce(Graphics g)
        {
            var r = Podrucje();
            decimal max = ZaokruziMax(podaci.Max(p => p.Vrijednost));
            using var linija = new Pen(Tema.Okvir) { DashStyle = DashStyle.Dash };

            for (int i = 0; i <= 4; i++)
            {
                int y = r.Bottom - (int)(r.Height * i / 4f);
                g.DrawLine(linija, r.Left, y, r.Right, y);
                TextRenderer.DrawText(g, Format(max * i / 4), Font, new Rectangle(0, y - 9, r.Left - 8, 18), Tema.TekstSlab,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
            }

            float sirinaMjesta = r.Width / (float)podaci.Count;
            float sirinaStupca = Math.Max(3, sirinaMjesta * 0.62f);
            int korakOznaka = Math.Max(1, (int)Math.Ceiling(podaci.Count / (r.Width / 64f)));
            for (int i = 0; i < podaci.Count; i++)
            {
                var (oznaka, vrijednost) = podaci[i];
                float visina = (float)(vrijednost / max) * r.Height;
                float x = r.Left + i * sirinaMjesta + (sirinaMjesta - sirinaStupca) / 2;
                var stupac = new RectangleF(x, r.Bottom - visina, sirinaStupca, visina);
                if (visina > 0)
                {
                    using var path = Tema.Zaobljeno(stupac, Math.Min(4, sirinaStupca / 2));
                    using var cetka = new SolidBrush(i == hoverIndeks ? ControlPaint.Dark(Boja, 0.1f) : Color.FromArgb(i == hoverIndeks ? 255 : 210, Boja));
                    g.FillPath(cetka, path);
                }
                if (i % korakOznaka == 0)
                {
                    TextRenderer.DrawText(g, oznaka, Font, new Rectangle((int)(r.Left + i * sirinaMjesta - 20), r.Bottom + 6, (int)sirinaMjesta + 40, 18),
                        Tema.TekstSlab, TextFormatFlags.HorizontalCenter);
                }
            }

            if (hoverIndeks >= 0)
            {
                var (oznaka, vrijednost) = podaci[hoverIndeks];
                string tekst = $"{oznaka}: {Format(vrijednost)}";
                var velicina = TextRenderer.MeasureText(tekst, Tema.MaliPolu);
                float x = Math.Clamp(r.Left + hoverIndeks * sirinaMjesta + sirinaMjesta / 2 - velicina.Width / 2f - 8, r.Left, r.Right - velicina.Width - 16);
                var okvir = new RectangleF(x, r.Top, velicina.Width + 16, velicina.Height + 10);
                using var path = Tema.Zaobljeno(okvir, 6);
                using var pozadina = new SolidBrush(Tema.Tekst);
                g.FillPath(pozadina, path);
                TextRenderer.DrawText(g, tekst, Tema.MaliPolu, Rectangle.Round(okvir), Color.White,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }

        private void CrtajTrake(Graphics g)
        {
            var r = Podrucje();
            decimal max = Math.Max(podaci.Max(p => p.Vrijednost), 1);
            float visinaReda = Math.Min(48, r.Height / (float)podaci.Count);
            float debljina = Math.Max(8, visinaReda * 0.5f);

            for (int i = 0; i < podaci.Count; i++)
            {
                var (oznaka, vrijednost) = podaci[i];
                float y = r.Top + i * visinaReda;
                float centar = y + visinaReda / 2;
                TextRenderer.DrawText(g, oznaka, i == hoverIndeks ? Tema.MaliPolu : Font, new Rectangle(0, (int)y, r.Left - 12, (int)visinaReda),
                    Tema.Tekst, TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

                var pozadinaTrake = new RectangleF(r.Left, centar - debljina / 2, r.Width, debljina);
                using (var path = Tema.Zaobljeno(pozadinaTrake, debljina / 2))
                using (var cetka = new SolidBrush(Tema.Pozadina))
                {
                    g.FillPath(cetka, path);
                }

                float duzina = (float)(vrijednost / max) * r.Width;
                if (duzina > 0)
                {
                    using var path = Tema.Zaobljeno(new RectangleF(r.Left, centar - debljina / 2, Math.Max(duzina, debljina), debljina), debljina / 2);
                    float udio = 1 - i * 0.12f;
                    using var cetka = new SolidBrush(Color.FromArgb((int)(255 * Math.Max(0.45f, udio)), Boja));
                    g.FillPath(cetka, path);
                }
                TextRenderer.DrawText(g, Format(vrijednost), Tema.MaliPolu, new Rectangle(r.Right + 10, (int)y, 90, (int)visinaReda),
                    Tema.Tekst, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            }
        }
    }
}
