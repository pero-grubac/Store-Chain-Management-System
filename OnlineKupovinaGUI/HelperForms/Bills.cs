using MySql.Data.MySqlClient;
using OnlineKupovinaDB;
using System.Data;

namespace OnlineKupovinaGUI.HelperForms
{
    public partial class Bills : Form
    {
        private static List<Stavka> stavke;
        private static List<(int, double)> ukupno;
        private static int idKupca;

        private static string selectIdKupca = @"Select idKUPAC FROM kupac WHERE KorisnickoIme=@korisnickoIme";
        public Bills()
        {
            InitializeComponent();
            // LoadTheme();
            idKupca = GetIdKupac();
            stavke = DisplayStavka();
            ukupno = IzracunajUkupno();
        }

        private List<(int, double)> IzracunajUkupno()
        {
            List<(int, double)> result = new List<(int, double)>();
            List<Stavka> stavke = ReadAllStavke();
            result = stavke
                            .GroupBy(temp => temp.IdRacun)
                            .Select(group => ((int)group.Key, group.Sum(temp => temp.Ukupno)))
                            .ToList();
            return result;
        }

        private int GetIdKupac()
        {
            using (var connection = DBUtil.GetConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand(selectIdKupca, connection))
                {
                    object idProdavnice = null;
                    string userName = Password.GetKoriscnickoIme();
                    command.Parameters.AddWithValue("@korisnickoIme", userName);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            idProdavnice = reader["idKUPAC"];
                        }
                        return int.Parse(idProdavnice.ToString());
                    }
                }
            }
        }

        private static List<Stavka> ReadAllStavke()
        {
            List<Stavka> temp = new List<Stavka>();
            using (var connection = DBUtil.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand("detalji_racuna", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("pIdKupca", idKupca);
                    using (var reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            int idRacuna = Convert.ToInt32(reader["ID racuna"]);
                            DateTime datumKupovine = Convert.ToDateTime(reader["Datum kupovine"]);
                            int kolicina = Convert.ToInt32(reader["Kolicina"]);
                            double cijena = Convert.ToDouble(reader["Cijena"]);
                            string imeProdavnice = reader["Ime prodavnice"].ToString();
                            string nazivProizvoda = reader["Naziv proizvoda"].ToString();

                            Stavka stavka = new Stavka(idRacuna, datumKupovine, imeProdavnice, nazivProizvoda, kolicina, cijena);
                            temp.Add(stavka);
                        }
                    }
                }
            }
            return temp;
        }
        private static List<Stavka> DisplayStavka()
        {
            List<Stavka> temp = ReadAllStavke();
            List<int?> idStavki = new List<int?>();
            foreach (var stavka in temp)
            {
                if (!idStavki.Contains(stavka.IdRacun))
                {
                    idStavki.Add(stavka.IdRacun);
                }
                else
                {
                    stavka.IdRacun = null;
                }
            }
            return temp;
        }
        private void LoadTheme()
        {
            foreach (Control btns in this.Controls)
            {
                if (btns.GetType() == typeof(Button))
                {
                    Button btn = (Button)btns;
                    btn.BackColor = ThemeColor.PrimaryColor;
                    btn.ForeColor = Color.White;
                    btn.FlatAppearance.BorderColor = ThemeColor.SecondaryColor;

                }
            }
        }

        private void btnReadBill_Click(object sender, EventArgs e)
        {
            stavke = DisplayStavka();
            dgvBill.DataSource = stavke;
            foreach (DataGridViewColumn column in dgvBill.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            ukupno = IzracunajUkupno();
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("ID", typeof(int));
            dataTable.Columns.Add("Racun", typeof(double));
            foreach (var item in ukupno)
            {
                dataTable.Rows.Add(item.Item1, item.Item2);
            }
            dgvUkupnePare.DataSource = dataTable;
            foreach (DataGridViewColumn column in dgvUkupnePare.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }
    }
}
