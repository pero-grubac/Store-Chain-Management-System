using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace OnlineKupovinaGUI.Ui
{
    internal enum Varijanta
    {
        Primarno,
        Sekundarno,
        Opasno,
        Uspjeh,
        Meni
    }

    /// <summary>Zaobljeno dugme sa opcionom ikonom; crta se samo, pa izgleda isto na svakom sistemu.</summary>
    internal sealed class Dugme : Button
    {
        private bool hover;
        private bool pritisnuto;

        public Dugme(string tekst, string? ikona = null, Varijanta varijanta = Varijanta.Sekundarno)
        {
            Text = tekst;
            IkonaZnak = ikona;
            Varijanta = varijanta;
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer
                     | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            Font = Tema.Polu;
            Height = 38;
            Cursor = Cursors.Hand;
            Margin = new Padding(0, 0, 8, 0);
            BackColor = Color.Transparent;
            AutoSize = true;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? IkonaZnak { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Varijanta Varijanta { get; set; }
        public bool Hover => hover;

        /// <summary>Kad je postavljeno, dugme crta ovaj delegat umjesto standardnog izgleda.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Action<Dugme, PaintEventArgs>? PosebnoCrtanje { get; set; }

        public override Size GetPreferredSize(Size proposedSize)
        {
            var tekst = TextRenderer.MeasureText(Text, Font);
            int sirina = tekst.Width + 32 + (IkonaZnak != null ? 24 : 0);
            return new Size(Math.Max(sirina, 44), Height);
        }

        protected override void OnMouseEnter(EventArgs e) { hover = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { hover = false; pritisnuto = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs e) { pritisnuto = true; Invalidate(); base.OnMouseDown(e); }
        protected override void OnMouseUp(MouseEventArgs e) { pritisnuto = false; Invalidate(); base.OnMouseUp(e); }
        protected override void OnEnabledChanged(EventArgs e) { Invalidate(); base.OnEnabledChanged(e); }

        private (Color Pozadina, Color Tekst, Color? Okvir) Boje()
        {
            return Varijanta switch
            {
                Varijanta.Primarno => (hover ? Tema.AkcentTamni : Tema.Akcent, Color.White, null),
                Varijanta.Opasno => (hover ? Tema.Hex("#B91C1C") : Tema.Opasnost, Color.White, null),
                Varijanta.Uspjeh => (hover ? Tema.Hex("#15803D") : Tema.Uspjeh, Color.White, null),
                Varijanta.Meni => (hover ? Tema.MeniHover : Tema.Meni, Tema.MeniTekst, null),
                _ => (hover ? Tema.Pozadina : Color.White, Tema.Tekst, Tema.Okvir)
            };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (PosebnoCrtanje != null)
            {
                PosebnoCrtanje(this, e);
                return;
            }
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Tema.PozadinaIza(this));

            var (pozadina, tekst, okvir) = Boje();
            if (!Enabled)
            {
                pozadina = Varijanta == Varijanta.Sekundarno ? Tema.Pozadina : ControlPaint.Light(pozadina, 0.6f);
                tekst = Varijanta == Varijanta.Sekundarno ? Tema.TekstSlab : Color.White;
            }

            var r = new RectangleF(0.5f, pritisnuto ? 1.5f : 0.5f, Width - 1.5f, Height - 2f);
            using (var path = Tema.Zaobljeno(r, 8))
            using (var cetka = new SolidBrush(pozadina))
            {
                g.FillPath(cetka, path);
                if (okvir.HasValue)
                {
                    using var olovka = new Pen(Focused && ShowFocusCues ? Tema.OkvirFokus : okvir.Value);
                    g.DrawPath(olovka, path);
                }
                else if (Focused && ShowFocusCues)
                {
                    using var olovka = new Pen(Tema.OkvirFokus, 2);
                    g.DrawPath(olovka, path);
                }
            }

            var velicinaTeksta = TextRenderer.MeasureText(Text, Font);
            int sirinaIkone = IkonaZnak != null ? 24 : 0;
            int x = (Width - velicinaTeksta.Width - sirinaIkone) / 2;
            int y = (int)r.Y;
            if (IkonaZnak != null)
            {
                using var ikonaFont = Tema.Ikone(11f);
                TextRenderer.DrawText(g, IkonaZnak, ikonaFont, new Rectangle(x, y, 20, Height), tekst,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.NoPadding);
            }
            TextRenderer.DrawText(g, Text, Font, new Rectangle(x + sirinaIkone, y, velicinaTeksta.Width + 2, Height), tekst,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
        }
    }

    /// <summary>Bijela zaobljena kartica sa opcionim naslovom.</summary>
    internal class Kartica : Panel
    {
        private string? naslov;

        public Kartica(string? naslov = null)
        {
            this.naslov = naslov;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            BackColor = Tema.Pozadina;
            Padding = new Padding(18, naslov == null ? 16 : 52, 18, 16);
            Margin = new Padding(0, 0, 16, 16);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? Naslov
        {
            get => naslov;
            set { naslov = value; Invalidate(); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? Podnaslov { get; set; }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Tema.PozadinaIza(this));
            var r = new RectangleF(0.5f, 0.5f, Width - 1.5f, Height - 1.5f);
            using var path = Tema.Zaobljeno(r, 12);
            using var pozadina = new SolidBrush(Tema.Kartica);
            using var okvir = new Pen(Tema.Okvir);
            g.FillPath(pozadina, path);
            g.DrawPath(okvir, path);

            if (naslov != null)
            {
                TextRenderer.DrawText(g, naslov, Tema.NaslovKartice, new Point(18, 16), Tema.Tekst);
                if (Podnaslov != null)
                {
                    int sirina = TextRenderer.MeasureText(naslov, Tema.NaslovKartice).Width;
                    TextRenderer.DrawText(g, Podnaslov, Tema.Mali, new Point(18 + sirina + 6, 19), Tema.TekstSlab);
                }
            }
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            if (e.Control is not Dugme && e.Control!.BackColor == Tema.Pozadina) e.Control.BackColor = Tema.Kartica;
        }
    }

    /// <summary>Polje za unos teksta sa zaobljenim okvirom koji se oboji kad je u fokusu.</summary>
    internal sealed class Unos : Panel
    {
        private bool fokus;

        public Unos(string? placeholder = null, string? ikona = null, bool lozinka = false)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            Height = 40;
            BackColor = Color.White;
            Polje = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Font = Tema.Unos,
                PlaceholderText = placeholder ?? string.Empty,
                UseSystemPasswordChar = lozinka,
                BackColor = Color.White,
                ForeColor = Tema.Tekst
            };
            IkonaZnak = ikona;
            Controls.Add(Polje);
            Polje.GotFocus += (_, _) => { fokus = true; Invalidate(); };
            Polje.LostFocus += (_, _) => { fokus = false; Invalidate(); };
            Click += (_, _) => Polje.Focus();
            Padding = new Padding(ikona != null ? 38 : 12, 0, 12, 0);
            Raspored();
        }

        public TextBox Polje { get; }
        public string? IkonaZnak { get; }

        public override string Text
        {
            get => Polje.Text;
#pragma warning disable CS8765
            set => Polje.Text = value;
#pragma warning restore CS8765
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            Raspored();
        }

        private void Raspored()
        {
            if (Polje == null) return;
            Polje.Width = Width - Padding.Horizontal;
            Polje.Location = new Point(Padding.Left, (Height - Polje.Height) / 2);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Tema.PozadinaIza(this));
            var r = new RectangleF(0.5f, 0.5f, Width - 1.5f, Height - 1.5f);
            using var path = Tema.Zaobljeno(r, 8);
            using var pozadina = new SolidBrush(Color.White);
            using var okvir = new Pen(fokus ? Tema.OkvirFokus : Tema.Okvir, fokus ? 2 : 1);
            g.FillPath(pozadina, path);
            g.DrawPath(okvir, path);
            if (IkonaZnak != null)
            {
                using var f = Tema.Ikone(12f);
                TextRenderer.DrawText(g, IkonaZnak, f, new Rectangle(10, 0, 22, Height), Tema.TekstSlab,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
            }
        }
    }

    /// <summary>Kartica sa jednim brojem (KPI).</summary>
    internal sealed class StatKartica : Kartica
    {
        private readonly Label vrijednost;
        private readonly Label opis;
        private readonly string ikona;
        private readonly Color boja;

        public StatKartica(string naslov, string ikona, Color boja)
        {
            this.ikona = ikona;
            this.boja = boja;
            Padding = new Padding(18);
            var oznaka = new Label { Text = naslov, Font = Tema.Polu, ForeColor = Tema.TekstSlab, AutoSize = true, Location = new Point(18, 16) };
            vrijednost = new Label { Text = "–", Font = Tema.Veliki, ForeColor = Tema.Tekst, AutoSize = true, Location = new Point(14, 38) };
            opis = new Label { Text = string.Empty, Font = Tema.Mali, ForeColor = Tema.TekstSlab, AutoSize = true, Location = new Point(18, 84) };
            Controls.AddRange([oznaka, vrijednost, opis]);
        }

        public void Postavi(string vrijednostTekst, string? opisTekst = null)
        {
            vrijednost.Text = vrijednostTekst;
            opis.Text = opisTekst ?? string.Empty;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            var krug = new Rectangle(Width - 62, 18, 42, 42);
            using var pozadina = new SolidBrush(Color.FromArgb(30, boja));
            g.FillEllipse(pozadina, krug);
            using var f = Tema.Ikone(16f);
            TextRenderer.DrawText(g, ikona, f, krug, boja, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }

    /// <summary>Mala obojena oznaka (status, uloga).</summary>
    internal sealed class Znacka : Label
    {
        public Znacka(string tekst, Color boja)
        {
            Text = tekst;
            Boja = boja;
            AutoSize = false;
            Font = Tema.MaliPolu;
            TextAlign = ContentAlignment.MiddleCenter;
            Size = new Size(TextRenderer.MeasureText(tekst, Font).Width + 20, 24);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color Boja { get; set; }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Tema.PozadinaIza(this));
            using var path = Tema.Zaobljeno(new RectangleF(0, 0, Width - 1, Height - 1), Height / 2f);
            using var pozadina = new SolidBrush(Color.FromArgb(36, Boja));
            g.FillPath(pozadina, path);
            TextRenderer.DrawText(g, Text, Font, ClientRectangle, Boja, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }

    internal static class Raspored
    {
        /// <summary>Kartica sa naslovom koja popunjava celiju i sadrzi jednu kontrolu.</summary>
        public static Kartica UKartici(string? naslov, Control sadrzaj)
        {
            var k = new Kartica(naslov) { Dock = DockStyle.Fill };
            sadrzaj.Dock = DockStyle.Fill;
            k.Controls.Add(sadrzaj);
            return k;
        }

        public static Label Oznaka(string tekst, Font? font = null, Color? boja = null) => new()
        {
            Text = tekst,
            Font = font ?? Tema.Osnovni,
            ForeColor = boja ?? Tema.Tekst,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 4)
        };

        /// <summary>Red kontrola s lijeva na desno (alatna traka).</summary>
        public static FlowLayoutPanel Red(params Control[] kontrole)
        {
            var red = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Margin = new Padding(0),
                Padding = new Padding(0),
                BackColor = Color.Transparent
            };
            red.Controls.AddRange(kontrole);
            return red;
        }

        public static TableLayoutPanel Mreza(int kolona, int redova)
        {
            var t = new TableLayoutPanel
            {
                ColumnCount = kolona,
                RowCount = redova,
                Dock = DockStyle.Fill,
                Margin = new Padding(0),
                Padding = new Padding(0),
                BackColor = Tema.Pozadina
            };
            return t;
        }

        public static ComboBox Izbor(int sirina = 220) => new()
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            FlatStyle = FlatStyle.Flat,
            Font = Tema.Unos,
            Width = sirina,
            Margin = new Padding(0, 5, 8, 0),
            BackColor = Color.White
        };

        public static DateTimePicker Datum(DateTime vrijednost) => new()
        {
            Format = DateTimePickerFormat.Short,
            Font = Tema.Unos,
            Width = 140,
            Value = vrijednost,
            Margin = new Padding(0, 5, 8, 0)
        };
    }
}
