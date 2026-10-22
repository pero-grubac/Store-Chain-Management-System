using System.Drawing.Drawing2D;

namespace OnlineKupovinaGUI.Ui
{
    internal enum VrstaObavjestenja
    {
        Uspjeh,
        Greska,
        Info
    }

    /// <summary>Kratko obavjestenje u gornjem desnom uglu koje samo nestane.</summary>
    internal sealed class Obavjestenje : Control
    {
        private readonly System.Windows.Forms.Timer tajmer = new() { Interval = 4000 };
        private readonly VrstaObavjestenja vrsta;

        private Obavjestenje(string poruka, VrstaObavjestenja vrsta)
        {
            this.vrsta = vrsta;
            Text = poruka;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
            Font = Tema.Polu;
            var velicina = TextRenderer.MeasureText(poruka, Font, new Size(420, 0), TextFormatFlags.WordBreak);
            Size = new Size(Math.Max(280, velicina.Width + 76), Math.Max(56, velicina.Height + 30));
            Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Cursor = Cursors.Hand;
            Click += (_, _) => Dispose();
            tajmer.Tick += (_, _) => Dispose();
            tajmer.Start();
        }

        public static void Prikazi(Control domacin, string poruka, VrstaObavjestenja vrsta)
        {
            foreach (var staro in domacin.Controls.OfType<Obavjestenje>().ToList()) staro.Dispose();
            var o = new Obavjestenje(poruka, vrsta);
            o.Location = new Point(domacin.ClientSize.Width - o.Width - 24, 20);
            domacin.Controls.Add(o);
            o.BringToFront();
        }

        private Color Boja => vrsta switch
        {
            VrstaObavjestenja.Uspjeh => Tema.Uspjeh,
            VrstaObavjestenja.Greska => Tema.Opasnost,
            _ => Tema.Akcent
        };

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Tema.PozadinaIza(this));
            var r = new RectangleF(0.5f, 0.5f, Width - 1.5f, Height - 1.5f);
            using var path = Tema.Zaobljeno(r, 10);
            using var pozadina = new SolidBrush(Tema.Tekst);
            g.FillPath(pozadina, path);

            var krug = new Rectangle(14, (Height - 28) / 2, 28, 28);
            using var cetka = new SolidBrush(Boja);
            g.FillEllipse(cetka, krug);
            using var f = Tema.Ikone(11f);
            string ikona = vrsta switch { VrstaObavjestenja.Uspjeh => Ikona.Kvacica, VrstaObavjestenja.Greska => Ikona.Zatvori, _ => Ikona.Info };
            TextRenderer.DrawText(g, ikona, f, krug, Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            TextRenderer.DrawText(g, Text, Font, new Rectangle(54, 0, Width - 66, Height), Color.White,
                TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) tajmer.Dispose();
            base.Dispose(disposing);
        }
    }
}
