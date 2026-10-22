using System.ComponentModel;
using System.Reflection;

namespace OnlineKupovinaGUI.Ui
{
    /// <summary>Tabela u stilu aplikacije: bez okvira, svijetlo zaglavlje, cijeli red se bira.</summary>
    internal static class Tabela
    {
        public const string FormatNovca = "N2";

        public static DataGridView Nova()
        {
            var g = new DataGridView
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                BackgroundColor = Color.White,
                GridColor = Tema.Okvir,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 40,
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = true,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoGenerateColumns = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = Tema.Osnovni,
                StandardTab = true
            };
            g.RowTemplate.Height = 38;
            g.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Tema.ZaglavljeTabele,
                ForeColor = Tema.TekstSlab,
                Font = Tema.MaliPolu,
                SelectionBackColor = Tema.ZaglavljeTabele,
                SelectionForeColor = Tema.TekstSlab,
                Padding = new Padding(10, 0, 10, 0),
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };
            g.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor = Tema.Tekst,
                SelectionBackColor = Tema.AkcentSvijetli,
                SelectionForeColor = Tema.Tekst,
                Padding = new Padding(10, 0, 10, 0),
                Font = Tema.Osnovni
            };
            g.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Tema.Hex("#FCFDFE") };

            // Bez treperenja pri skrolovanju
            typeof(DataGridView).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(g, true);
            return g;
        }

        public static DataGridViewTextBoxColumn Kolona(string naslov, string svojstvo, float tezina = 100, string? format = null,
            DataGridViewContentAlignment poravnanje = DataGridViewContentAlignment.MiddleLeft, bool podebljano = false)
        {
            var k = new DataGridViewTextBoxColumn
            {
                HeaderText = naslov,
                DataPropertyName = svojstvo,
                Name = svojstvo,
                FillWeight = tezina,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                MinimumWidth = 50
            };
            k.DefaultCellStyle.Alignment = poravnanje;
            if (format != null) k.DefaultCellStyle.Format = format;
            if (podebljano) k.DefaultCellStyle.Font = Tema.Polu;
            if (poravnanje == DataGridViewContentAlignment.MiddleRight)
            {
                k.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            return k;
        }

        public static DataGridViewTextBoxColumn Novac(string naslov, string svojstvo, float tezina = 70, bool podebljano = false) =>
            Kolona(naslov, svojstvo, tezina, FormatNovca, DataGridViewContentAlignment.MiddleRight, podebljano);

        public static DataGridViewTextBoxColumn Broj(string naslov, string svojstvo, float tezina = 50) =>
            Kolona(naslov, svojstvo, tezina, null, DataGridViewContentAlignment.MiddleRight);

        public static DataGridViewTextBoxColumn Datum(string naslov, string svojstvo, float tezina = 70, bool vrijeme = false) =>
            Kolona(naslov, svojstvo, tezina, vrijeme ? "dd.MM.yyyy. HH:mm" : "dd.MM.yyyy.");

        public static void Postavi<T>(DataGridView g, IEnumerable<T> podaci)
        {
            int? odabrani = g.CurrentRow?.Index;
            g.DataSource = new BindingList<T>(podaci.ToList());
            if (odabrani.HasValue && odabrani.Value < g.Rows.Count && g.Rows.Count > 0)
            {
                g.CurrentCell = g.Rows[odabrani.Value].Cells.Cast<DataGridViewCell>().First(c => c.Visible);
            }
        }

        public static T? Odabrani<T>(DataGridView g) where T : class => g.CurrentRow?.DataBoundItem as T;

        /// <summary>Boji celiju statusa kao znacku (OK/Nisko/Nema, Aktivan/Istekao, Placen/Storniran).</summary>
        public static void BojaStatusa(DataGridView g, string kolona)
        {
            g.CellFormatting += (_, e) =>
            {
                if (e.ColumnIndex < 0 || g.Columns[e.ColumnIndex].Name != kolona || e.Value is not string s) return;
                var (tekst, pozadina) = s switch
                {
                    "OK" or "Aktivan" or "Placen" => (Tema.Uspjeh, Tema.UspjehSvijetli),
                    "Nisko" or "Najavljen" => (Tema.Upozorenje, Tema.UpozorenjeSvijetli),
                    "Nema" or "Istekao" or "Storniran" => (Tema.Opasnost, Tema.OpasnostSvijetli),
                    _ => (Tema.Tekst, Color.White)
                };
                e.CellStyle!.ForeColor = tekst;
                e.CellStyle.SelectionForeColor = tekst;
                e.CellStyle.Font = Tema.MaliPolu;
                e.Value = s == "Placen" ? "Plaćen" : s;
                e.FormattingApplied = true;
            };
        }

        /// <summary>Obojeni red za proizvode kojih nema ili ih je malo.</summary>
        public static void BojaZaliha(DataGridView g, Func<object, string?> status)
        {
            g.RowPrePaint += (_, e) =>
            {
                var red = g.Rows[e.RowIndex];
                string? s = red.DataBoundItem == null ? null : status(red.DataBoundItem);
                red.DefaultCellStyle.BackColor = s switch
                {
                    "Nema" => Tema.Hex("#FEF2F2"),
                    "Nisko" => Tema.Hex("#FFFBEB"),
                    _ => Color.Empty
                };
            };
        }
    }
}
