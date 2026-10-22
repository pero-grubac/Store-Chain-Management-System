using OnlineKupovina.Data;
using OnlineKupovinaDB;
using OnlineKupovinaGUI.Ui;

namespace OnlineKupovinaGUI.Views
{
    /// <summary>Jedini prozor aplikacije: prikazuje prijavu ili shell. F11 prebacuje na cijeli ekran.</summary>
    internal sealed class GlavnaForma : Form
    {
        private readonly Repozitoriji repo;
        private FormWindowState stanjePrijeCijelogEkrana = FormWindowState.Maximized;

        public GlavnaForma(Repozitoriji repo, bool cijeliEkran)
        {
            this.repo = repo;
            Text = "Store Chain Management System";
            BackColor = Tema.Pozadina;
            Font = Tema.Osnovni;
            MinimumSize = new Size(1180, 720);
            Size = new Size(1440, 900);
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            KeyPreview = true;
            AutoScaleMode = AutoScaleMode.None;
            Icon = NapraviIkonu();

            KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.F11)
                {
                    PrebaciCijeliEkran();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Escape && FormBorderStyle == FormBorderStyle.None)
                {
                    PrebaciCijeliEkran();
                }
            };

            if (cijeliEkran) Load += (_, _) => PrebaciCijeliEkran();
            PrikaziPrijavu();
        }

        public bool JeCijeliEkran => FormBorderStyle == FormBorderStyle.None;

        public void PrebaciCijeliEkran()
        {
            SuspendLayout();
            if (!JeCijeliEkran)
            {
                stanjePrijeCijelogEkrana = WindowState;
                WindowState = FormWindowState.Normal;
                FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                FormBorderStyle = FormBorderStyle.Sizable;
                WindowState = stanjePrijeCijelogEkrana;
            }
            ResumeLayout();
        }

        private void Zamijeni(Control novi)
        {
            SuspendLayout();
            var stari = Controls.Cast<Control>().ToList();
            Controls.Add(novi);
            foreach (var c in stari)
            {
                Controls.Remove(c);
                c.Dispose();
            }
            ResumeLayout();
        }

        private void PrikaziPrijavu()
        {
            var prijava = new PrijavaView(repo);
            prijava.Prijavljen += async (_, sesija) => await PrikaziShellAsync(sesija);
            Zamijeni(prijava);
            if (IsHandleCreated) BeginInvoke(prijava.Fokusiraj);
            else Shown += (_, _) => prijava.Fokusiraj();
        }

        private async Task PrikaziShellAsync(Sesija sesija)
        {
            var shell = new ShellView(repo, sesija);
            shell.Odjava += (_, _) => PrikaziPrijavu();
            Zamijeni(shell);
            Text = $"Store Chain Management System – {sesija.PunoIme} ({sesija.Uloga.Naziv()})";
            await shell.PrvaStranicaAsync();
        }

        private static Icon NapraviIkonu()
        {
            using var bmp = new Bitmap(64, 64);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                using var path = Tema.Zaobljeno(new RectangleF(0, 0, 63, 63), 16);
                using var cetka = new SolidBrush(Tema.Akcent);
                g.FillPath(cetka, path);
                using var f = Tema.Ikone(30f);
                TextRenderer.DrawText(g, Ikona.Korpa, f, new Rectangle(0, 0, 64, 64), Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
            return Icon.FromHandle(bmp.GetHicon());
        }
    }
}
