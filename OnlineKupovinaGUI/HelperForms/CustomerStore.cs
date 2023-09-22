using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using OnlineKupovinaDB;
using System.Data;

namespace OnlineKupovinaGUI.HelperForms
{
    public partial class CustomerStore : Form
    {
        private static List<Prodavnica> stores;
        private static int idCurrentStore;
        private static List<Proizvod> products;
        private static List<Proizvod> currentProducts;
        private static List<int> currentIdProducts;
        private static List<Stavka> korpa;
        private static int idKupca;
        private static int idProizvoda;

        private static string selectIdKupca = @"Select idKUPAC FROM kupac WHERE KorisnickoIme=@korisnickoIme";
        private static string getAll = @"SELECT p.idProdavnica, p.Ime,a.idADRESA, a.Drzava,a.Grad,a.Ulica FROM prodavnica p JOIN adresa a ON p.ADRESA_idADRESA = a.idADRESA;";
        private static string selectIdProdavnice = @"SELECT Prodavnica_idProdavnica FROM ZAPOSLENI WHERE KorisnickoIme =@UserName;";
        private static string selectProizvod = @"SELECT idPROIZVOD,Naziv,PROIZVODJAC_idPROIZVODJAC,PRODVNICA_idProdavnica FROM proizvod";

        public CustomerStore()
        {
            InitializeComponent();
            stores = ReadAllProdavnice();
            korpa = new List<Stavka>();
            idKupca = GetIdKupac();
            currentProducts = new List<Proizvod>();
            currentIdProducts = new List<int>();
        }

        private List<Prodavnica> ReadAllProdavnice()
        {
            List<Prodavnica> prodavnice = new();
            using (var connection = DBUtil.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(getAll, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            return stores;
                        }
                        while (reader.Read())
                        {
                            int id = reader.GetInt32("idADRESA");
                            string grad = reader.GetString("Grad");
                            string drzava = reader.GetString("Drzava");
                            string ulica = reader.GetString("Ulica");
                            Adresa adresa = new Adresa(id, grad, ulica, drzava);
                            int idProdavnica = reader.GetInt32("idProdavnica");
                            string ime = reader.GetString("Ime");
                            Prodavnica prod = new Prodavnica(idProdavnica, ime, adresa);
                            prodavnice.Add(prod);
                        }
                        return prodavnice;
                    }
                }
            }
        }

        private void btnSelectStore_Click(object sender, EventArgs e)
        {

            tab.SelectedIndex = 1;
            if (dgvStore.SelectedRows.Count > 0)
            {

                idCurrentStore = Convert.ToInt32(dgvStore.SelectedRows[0].Cells[0].Value);

                products = ReadAllProducts();
                dgvProducts.DataSource = null;
                dgvProducts.DataSource = products;
                foreach (DataGridViewColumn column in dgvProducts.Columns)
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
            }
        }

        private static List<Proizvod> ReadAllProducts()
        {
            List<Proizvod> proizvodi = new List<Proizvod>();
            using (var connection = DBUtil.GetConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand("get_proizvod_by_prodavnica", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("pIdProdavnica", idCurrentStore);
                    using (var reader = command.ExecuteReader())
                    {

                        if (!reader.HasRows)
                        {

                            return proizvodi;
                        }
                        while (reader.Read())
                        {
                            int idProizvod = reader.GetInt32("Id proizvoda");
                            string naziv = reader.GetString("Naziv");
                            DateTime datumIsteka = reader.GetDateTime("Rok trajanja");
                            int barkod = reader.GetInt32("Bar kod");
                            string opis = reader.GetString("Opis");
                            string imeProizvodjaca = reader.GetString("Ime proizvodjaca");
                            string email = reader.GetString("Kontak email");
                            string drzava = reader.GetString("Drzava porijekla");
                            int idProizvodjaca = reader.GetInt32("Id proizvodjaca");
                            int zalihe = reader.GetInt32("Zalihe");
                            double cijena = reader.GetDouble("Cijena");
                            Proizvodjac proizvodjac = new Proizvodjac(idProizvodjaca, email, imeProizvodjaca, drzava);

                            Proizvod proizvod = new Proizvod(idProizvod, naziv, cijena, datumIsteka, barkod, opis, zalihe, proizvodjac);
                            proizvodi.Add(proizvod);
                        }
                    }

                }
            }
            return proizvodi;
        }

        private void btnReadStore_Click(object sender, EventArgs e)
        {
            dgvStore.DataSource = stores;
            foreach (DataGridViewColumn column in dgvStore.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            if (dgvStore.SelectedRows.Count > 0)
            {
                int kolicina;
                if (int.TryParse(txt.Text, out kolicina))
                {
                    // foreach (DataGridViewColumn column in dgvStore.Columns) MessageBox.Show(column.Name);
                    DataGridViewRow selectedRow = dgvProducts.SelectedRows[0];
                    DateTime datum = Convert.ToDateTime(selectedRow.Cells["Datum"].Value);
                    string opis = selectedRow.Cells["Opis"].Value.ToString();
                    int barkod = Convert.ToInt32(selectedRow.Cells["BarKod"].Value);
                    int idProizvodjaca = Convert.ToInt32(selectedRow.Cells["IdProizvodjaca"].Value);
                    string imeProizvodjaca = selectedRow.Cells["ImeProizvodjaca"].Value.ToString();
                    string email = selectedRow.Cells["EmailProizvodjaca"].Value.ToString();
                    string drzava = selectedRow.Cells["DrzavaPorijekla"].Value.ToString();
                    double cijena = Convert.ToDouble(selectedRow.Cells["Cijena"].Value);
                    string naziv = selectedRow.Cells["Naziv"].Value.ToString();


                    DateTime currentDate = DateTime.Now;
                    currentProducts = ReadAllProducts(idCurrentStore);
                    Proizvod pom = currentProducts.FirstOrDefault(proizvod => proizvod.Naziv == naziv &&
                    proizvod.Datum == datum && proizvod.BarKod == barkod && proizvod.ImeProizvodjaca == imeProizvodjaca
                    && proizvod.IdProizvodjaca == idProizvodjaca && proizvod.Opis == opis && proizvod.EmailProizvodjaca == email
                    && proizvod.DrzavaPorijekla == drzava && proizvod.Cijena == cijena);
                    idProizvoda = pom.ID;
                    currentIdProducts.Add(idProizvoda);
                    Stavka stavka = new(idProizvoda, idCurrentStore, kolicina, cijena, currentDate);
                    korpa.Add(stavka);
                    txt.Clear();
                }
            }
        }
        private static List<Proizvod> ReadAllProducts(int idProdavnice)
        {
            List<Proizvod> proizvodi = new List<Proizvod>();
            using (var connection = DBUtil.GetConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand("get_proizvod_by_prodavnica", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("pIdProdavnica", idProdavnice);
                    using (var reader = command.ExecuteReader())
                    {

                        if (!reader.HasRows)
                        {

                            return proizvodi;
                        }
                        while (reader.Read())
                        {
                            int idProizvod = reader.GetInt32("Id proizvoda");
                            string naziv = reader.GetString("Naziv");
                            DateTime datumIsteka = reader.GetDateTime("Rok trajanja");
                            int barkod = reader.GetInt32("Bar kod");
                            string opis = reader.GetString("Opis");
                            string imeProizvodjaca = reader.GetString("Ime proizvodjaca");
                            string email = reader.GetString("Kontak email");
                            string drzava = reader.GetString("Drzava porijekla");
                            int idProizvodjaca = reader.GetInt32("Id proizvodjaca");
                            int zalihe = reader.GetInt32("Zalihe");
                            double cijena = reader.GetDouble("Cijena");
                            Proizvodjac proizvodjac = new Proizvodjac(idProizvodjaca, email, imeProizvodjaca, drzava);

                            Proizvod proizvod = new Proizvod(idProizvod, naziv, cijena, datumIsteka, barkod, opis, zalihe, proizvodjac);
                            proizvodi.Add(proizvod);
                        }
                    }

                }
            }
            return proizvodi;
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

        private void btnBuy_Click(object sender, EventArgs e)
        {
            using (var connection = DBUtil.GetConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand("narucivanje", connection))
                {
                    List<int> kolicinaList = korpa.Select(item => item.Kolicina).ToList();
                    string kolicinaJSON = JsonConvert.SerializeObject(kolicinaList);

                    List<double> cijenaList = korpa.Select(item => item.Cijena).ToList();
                    string cijenaJSON = JsonConvert.SerializeObject(cijenaList);
                    Stavka pom = korpa[0];
                    string idProizvodaJSON = JsonConvert.SerializeObject(currentIdProducts);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pIdKupca", idKupca);
                    command.Parameters.AddWithValue("@pIdProizvoda", idProizvodaJSON);
                    command.Parameters.AddWithValue("@pIdProdavnice", idCurrentStore);
                    command.Parameters.AddWithValue("@pKolicina", kolicinaJSON);
                    command.Parameters.AddWithValue("@pCijena", cijenaJSON);
                    command.Parameters.AddWithValue("@pDatumKupovine", pom.DatumKupovine);
                    command.ExecuteNonQuery();
                }
            }
            currentProducts = null;
            currentIdProducts = null;
            tab.SelectedIndex = 0;
            products = ReadAllProducts();
            dgvProducts.DataSource = null;
            dgvProducts.DataSource = products;
            foreach (DataGridViewColumn column in dgvProducts.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

        }
    }
}
