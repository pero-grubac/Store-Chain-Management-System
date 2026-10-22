using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace OnlineKupovinaGUI.Ui
{
    /// <summary>Boje, fontovi i ikone cijele aplikacije na jednom mjestu.</summary>
    internal static class Tema
    {
        public static readonly Color Pozadina = Hex("#F1F5F9");
        public static readonly Color Kartica = Color.White;
        public static readonly Color Okvir = Hex("#E2E8F0");
        public static readonly Color OkvirFokus = Hex("#818CF8");
        public static readonly Color Tekst = Hex("#0F172A");
        public static readonly Color TekstSlab = Hex("#64748B");
        public static readonly Color Akcent = Hex("#4F46E5");
        public static readonly Color AkcentTamni = Hex("#4338CA");
        public static readonly Color AkcentSvijetli = Hex("#EEF2FF");
        public static readonly Color Uspjeh = Hex("#16A34A");
        public static readonly Color UspjehSvijetli = Hex("#DCFCE7");
        public static readonly Color Upozorenje = Hex("#D97706");
        public static readonly Color UpozorenjeSvijetli = Hex("#FEF3C7");
        public static readonly Color Opasnost = Hex("#DC2626");
        public static readonly Color OpasnostSvijetli = Hex("#FEE2E2");
        public static readonly Color Meni = Hex("#0F172A");
        public static readonly Color MeniHover = Hex("#1E293B");
        public static readonly Color MeniTekst = Hex("#94A3B8");
        public static readonly Color ZaglavljeTabele = Hex("#F8FAFC");

        public static readonly Font Osnovni = new("Segoe UI", 10f);
        public static readonly Font Polu = new("Segoe UI Semibold", 10f);
        public static readonly Font Mali = new("Segoe UI", 9f);
        public static readonly Font MaliPolu = new("Segoe UI Semibold", 9f);
        public static readonly Font Naslov = new("Segoe UI Semibold", 20f);
        public static readonly Font NaslovKartice = new("Segoe UI Semibold", 11.5f);
        public static readonly Font Veliki = new("Segoe UI Semibold", 22f);
        public static readonly Font Unos = new("Segoe UI", 10.5f);

        private static readonly string IkoneFont = OdaberiFontIkona();

        public static Font Ikone(float velicina) => new(IkoneFont, velicina);

        public static Color Hex(string hex) => ColorTranslator.FromHtml(hex);

        /// <summary>Prva neprovidna pozadina iznad kontrole (FlowLayoutPanel moze biti providan).</summary>
        public static Color PozadinaIza(Control kontrola)
        {
            for (var c = kontrola.Parent; c != null; c = c.Parent)
            {
                if (c.BackColor.A == 255) return c.BackColor;
            }
            return Pozadina;
        }

        /// <summary>Windows 11 ima Segoe Fluent Icons, Windows 10 Segoe MDL2 Assets (isti kodovi).</summary>
        private static string OdaberiFontIkona()
        {
            using var fontovi = new InstalledFontCollection();
            return fontovi.Families.Any(f => f.Name == "Segoe Fluent Icons") ? "Segoe Fluent Icons" : "Segoe MDL2 Assets";
        }

        public static GraphicsPath Zaobljeno(RectangleF r, float radius)
        {
            var path = new GraphicsPath();
            float d = Math.Min(radius * 2, Math.Min(r.Width, r.Height));
            if (d <= 0)
            {
                path.AddRectangle(r);
                return path;
            }
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    /// <summary>Kodovi ikona iz Segoe Fluent Icons / MDL2 Assets.</summary>
    internal static class Ikona
    {
        public const string Pregled = "";
        public const string Prodavnica = "";
        public const string Ljudi = "";
        public const string Osoba = "";
        public const string Admin = "";
        public const string Akcija = "";
        public const string Racun = "";
        public const string Izvjestaj = "";
        public const string Proizvod = "";
        public const string Fabrika = "";
        public const string Upozorenje = "";
        public const string Istorija = "";
        public const string Korpa = "";
        public const string Podesavanja = "";
        public const string Odjava = "";
        public const string Pretraga = "";
        public const string Dodaj = "";
        public const string Izmijeni = "";
        public const string Obrisi = "";
        public const string Osvjezi = "";
        public const string Sacuvaj = "";
        public const string Skener = "";
        public const string Prijem = "";
        public const string Izvoz = "";
        public const string Brava = "";
        public const string Zatvori = "";
        public const string Kvacica = "";
        public const string Info = "";
        public const string Trend = "";
        public const string Kartica = "";
        public const string Kalendar = "";
        public const string Procenat = "";
        public const string PunEkran = "";
    }
}
