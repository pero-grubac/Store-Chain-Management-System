using OnlineKupovina.Data;
using OnlineKupovinaDB;
using OnlineKupovinaGUI.Views;

namespace OnlineKupovinaGUI.Ui
{
    /// <summary>Sve sto stranica treba: repozitorije, prijavljenog korisnika i shell (obavjestenja, zauzetost).</summary>
    internal sealed record Kontekst(Repozitoriji Repo, Sesija Sesija, ShellView Shell);

    /// <summary>
    /// Osnova svih stranica. <see cref="RadiAsync"/> pokrece rad sa bazom bez blokiranja prozora
    /// i svaku gresku pretvara u obavjestenje umjesto pada aplikacije.
    /// </summary>
    internal abstract class Stranica : UserControl
    {
        protected Stranica(Kontekst k)
        {
            K = k;
            Dock = DockStyle.Fill;
            BackColor = Tema.Pozadina;
            Font = Tema.Osnovni;
            DoubleBuffered = true;
        }

        protected Kontekst K { get; }
        public abstract string Naslov { get; }
        public virtual string? Opis => null;

        public abstract Task UcitajAsync();

        protected async Task<bool> RadiAsync(Func<Task> rad, string? uspjeh = null)
        {
            K.Shell.Zauzeto(true);
            try
            {
                await rad();
                if (uspjeh != null) K.Shell.Obavijesti(uspjeh, VrstaObavjestenja.Uspjeh);
                return true;
            }
            catch (Exception e) when (e is PoslovnaGreskaException or BazaNedostupnaException or InvalidOperationException or ArgumentException)
            {
                K.Shell.Obavijesti(e.Message, VrstaObavjestenja.Greska);
            }
            catch (Exception e)
            {
                K.Shell.Obavijesti("Neočekivana greška: " + e.Message, VrstaObavjestenja.Greska);
            }
            finally
            {
                K.Shell.Zauzeto(false);
            }
            return false;
        }

        protected IWin32Window Vlasnik => FindForm()!;
    }

    /// <summary>Stranica sa alatnom trakom, pretragom i jednom tabelom (CRUD liste).</summary>
    internal abstract class TabelaStranica<T> : Stranica where T : class
    {
        private readonly List<(Dugme Dugme, bool TrebaOdabir)> alati = new();
        private readonly Label brojac;

        protected TabelaStranica(Kontekst k) : base(k)
        {
            Lista = Ui.Tabela.Nova();
            foreach (var kolona in Kolone()) Lista.Columns.Add(kolona);

            Pretraga = new Unos("Pretraga...", Ikona.Pretraga) { Width = 280, Margin = new Padding(0, 0, 12, 0) };
            Pretraga.Polje.TextChanged += (_, _) => Filtriraj();

            Alati = Raspored.Red();
            brojac = new Label { AutoSize = true, ForeColor = Tema.TekstSlab, Font = Tema.Mali, Anchor = AnchorStyles.Right, Margin = new Padding(0, 12, 0, 0) };

            var traka = new TableLayoutPanel { Dock = DockStyle.Top, Height = 54, ColumnCount = 3, BackColor = Tema.Pozadina, Padding = new Padding(0, 0, 0, 14) };
            traka.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            traka.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            traka.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            traka.Controls.Add(Pretraga, 0, 0);
            traka.Controls.Add(Alati, 1, 0);
            traka.Controls.Add(brojac, 2, 0);

            KarticaTabele = new Kartica { Dock = DockStyle.Fill, Padding = new Padding(2, 8, 2, 8), Margin = new Padding(0) };
            KarticaTabele.Controls.Add(Lista);

            Controls.Add(KarticaTabele);
            Controls.Add(traka);

            Lista.SelectionChanged += (_, _) => OsvjeziAlate();
            Lista.CellDoubleClick += async (_, e) =>
            {
                if (e.RowIndex >= 0) await DvostrukiKlikAsync();
            };
        }

        protected DataGridView Lista { get; }
        protected Unos Pretraga { get; }
        protected FlowLayoutPanel Alati { get; }
        protected Kartica KarticaTabele { get; }
        protected List<T> Svi { get; private set; } = new();
        protected T? Odabrani => Ui.Tabela.Odabrani<T>(Lista);

        protected abstract IEnumerable<DataGridViewColumn> Kolone();
        protected abstract Task<List<T>> UcitajPodatkeAsync();
        protected abstract string TekstZaPretragu(T stavka);
        protected virtual Task DvostrukiKlikAsync() => Task.CompletedTask;

        public override Task UcitajAsync() => RadiAsync(async () =>
        {
            Svi = await UcitajPodatkeAsync();
            Filtriraj();
        });

        protected void Filtriraj()
        {
            string upit = Pretraga.Text.Trim();
            var lista = upit.Length == 0
                ? Svi
                : Svi.Where(s => TekstZaPretragu(s).Contains(upit, StringComparison.CurrentCultureIgnoreCase)).ToList();
            Ui.Tabela.Postavi(Lista, lista);
            brojac.Text = lista.Count == Svi.Count ? $"{Svi.Count} zapisa" : $"{lista.Count} od {Svi.Count} zapisa";
            OsvjeziAlate();
        }

        protected Dugme DodajAlat(string tekst, string ikona, Func<Task> akcija, Varijanta varijanta = Varijanta.Sekundarno, bool trebaOdabir = false)
        {
            var d = new Dugme(tekst, ikona, varijanta);
            d.Click += async (_, _) => await akcija();
            alati.Add((d, trebaOdabir));
            Alati.Controls.Add(d);
            OsvjeziAlate();
            return d;
        }

        protected virtual void OsvjeziAlate()
        {
            bool imaOdabir = Odabrani != null;
            foreach (var (dugme, trebaOdabir) in alati)
            {
                if (trebaOdabir) dugme.Enabled = imaOdabir;
            }
        }
    }
}
