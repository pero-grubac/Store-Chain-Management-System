using MySql.Data.MySqlClient;
using OnlineKupovinaDB;
using System.Data;

namespace OnlineKupovinaGUI.HelperForms
{
    public partial class StaffProducts : Stores
    {
        private static List<Proizvod> products;
        private static List<Proizvodjac> proizvodjaci;
        private static List<Adresa> adrese;
        private static int idProdavnice;

        private static string selectAllProizvodjace = @"SELECT p.*,a.Drzava,a.Grad,a.Ulica,a.idADRESA FROM proizvodjac p JOIN adresa a ON a.idADRESA=p.ADRESA_idADRESA";
        private static string selectIdProdavnice = @"SELECT Prodavnica_idProdavnica FROM ZAPOSLENI WHERE KorisnickoIme =@UserName;";
        private static string getAllAdd = @"SELECT * FROM adresa;";
        private static string deleteProduct = @"DELETE FROM proizvod WHERE idPROIZVOD=@id;";
        private static string deleteProizvodjac = @"DELETE FROM proizvodjac WHERE idPROIZVODJAC=@id;";
        private static string updateProduct = @"UPDATE Proizvod SET Naziv = @naziv, DatumIsteka = @datumIsteka, BarKod = @barKod, Opis = @opis, Zalihe = @zalihe,Cijena=@cijena PROIZVODJAC_idPROIZVODJAC = @idProizvodjaca WHERE idPROIZVOD = @id";
        private static string updateStore = @"UPDATE proizvodjac SET Ime = @ime, Email = @email, ADRESA_idADRESA = @idAdrese WHERE idPROIZVODJAC = @id";

        private static string originalNaziv;
        private static DateTime originalDatumIsteka;
        private static int originalBarkKod;
        private static double originalCijena;
        private static string originalOpis;
        private static int originalZalihe;
        private static int originalIdProizvodjaca;
        private static string originalIme;
        private static string originalEmail;
        private static int originalIdAdrese;
        public StaffProducts()
        {
            InitializeComponent();
            idProdavnice = GetIdProdavnice();

            products = ReadAllProducts();
            proizvodjaci = ReadAllProizvodjaci();
            adrese = ReadAllAdd();
        }
        public static List<Adresa> ReadAllAdd()
        {
            List<Adresa> addresses = new List<Adresa>();
            using (var connection = DBUtil.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(getAllAdd, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            return adrese;
                        }
                        while (reader.Read())
                        {
                            int id = reader.GetInt32("idADRESA");
                            string grad = reader.GetString("Grad");
                            string drzava = reader.GetString("Drzava");
                            string ulica = reader.GetString("Ulica");
                            Adresa adresa = new Adresa(id, grad, ulica, drzava);
                            addresses.Add(adresa);

                        }
                        return addresses;
                    }
                }
            }
        }
        private List<Proizvodjac> ReadAllProizvodjaci()
        {
            List<Proizvodjac> temp = new List<Proizvodjac>();
            using (var connection = DBUtil.GetConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand(selectAllProizvodjace, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            return temp;
                        }
                        while (reader.Read())
                        {
                            int id = reader.GetInt32("idPROIZVODJAC");
                            string ime = reader.GetString("Ime");
                            string email = reader.GetString("Email");
                            string drzava = reader.GetString("Drzava");
                            string grad = reader.GetString("Grad");
                            string ulica = reader.GetString("Ulica");
                            int idAddresa = reader.GetInt32("idADRESA");
                            Adresa adresa = new Adresa(idAddresa, grad, ulica, drzava);
                            Proizvodjac proizvodjac = new Proizvodjac(email, id, ime, adresa);
                            temp.Add(proizvodjac);
                        }
                    }
                }
            }
            return temp;
        }

        private static int GetIdProdavnice()
        {
            using (var connection = DBUtil.GetConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand(selectIdProdavnice, connection))
                {
                    object idProdavnice = null;
                    string userName = Password.GetKoriscnickoIme();
                    command.Parameters.AddWithValue("@UserName", userName);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            idProdavnice = reader["Prodavnica_idProdavnica"];
                        }
                        return int.Parse(idProdavnice.ToString());
                    }
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

        private void btnReadAll_Click_1(object sender, EventArgs e)
        {
            dataGridView.DataSource = products;
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }



        private void btnReadAllAdd_Click_1(object sender, EventArgs e)
        {
            dgvAdd.DataSource = proizvodjaci;
            foreach (DataGridViewColumn column in dgvAdd.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dgvAdrese.DataSource = adrese;
            foreach (DataGridViewColumn column in dgvAdrese.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {
            string naziv = txt1.Text;
            DateTime datumIsteka = DateTime.Parse(txt2.Text);
            int barKod = int.Parse(txtBarKod.Text);
            string opis = txtOpis.Text;
            int zalihe = int.Parse(txtZalihe.Text);
            int idProizvodjaca = int.Parse(txtIdProizvodjaca.Text);
            double cijena = double.Parse(txtCijena.Text);
            using (var connection = DBUtil.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new("dodaj_proizvod", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pNaziv", naziv);
                    command.Parameters.AddWithValue("@pCijena", cijena);
                    command.Parameters.AddWithValue("@pDatumIsteka", datumIsteka);
                    command.Parameters.AddWithValue("@pBarKod", barKod);
                    command.Parameters.AddWithValue("@pIdProizvodjaca", idProizvodjaca);
                    command.Parameters.AddWithValue("@pOpis", opis);
                    command.Parameters.AddWithValue("@pZalihe", zalihe);
                    command.Parameters.AddWithValue("@pIdProdavnice", idProdavnice);
                    command.ExecuteNonQuery();
                }
            }
            products = ReadAllProducts();
            dataGridView.DataSource = products;
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            tab.SelectedTab = tabPage1;
        }

        private void btnDelete_Click_1(object sender, EventArgs e)
        {

            if (dataGridView.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView.SelectedRows[0];
                int idToDelete = Convert.ToInt32(selectedRow.Cells["ID"].Value);

                using (var connection = DBUtil.GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand(deleteProduct, connection))
                    {
                        command.Parameters.AddWithValue("@id", idToDelete);
                        command.ExecuteNonQuery();
                    }
                }
                products.RemoveAll(proizvod => proizvod.ID == idToDelete);
                dataGridView.DataSource = null;
                dataGridView.DataSource = products;
                foreach (DataGridViewColumn column in dataGridView.Columns)
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
            }
        }

        private void btnDeleteAdd_Click_1(object sender, EventArgs e)
        {
            if (dgvAdd.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dgvAdd.SelectedRows[0];
                int idToDelete = Convert.ToInt32(selectedRow.Cells["ID"].Value);

                using (var connection = DBUtil.GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand(deleteProizvodjac, connection))
                    {
                        command.Parameters.AddWithValue("@id", idToDelete);
                        command.ExecuteNonQuery();
                    }
                }
                proizvodjaci.RemoveAll(proizvodjac => proizvodjac.ID == idToDelete);
                dgvAdd.DataSource = null;
                dgvAdd.DataSource = proizvodjaci;
                foreach (DataGridViewColumn column in dgvAdd.Columns)
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
            }
        }
        protected override void dataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            originalNaziv = dataGridView.Rows[e.RowIndex].Cells["Naziv"].Value.ToString();
            originalDatumIsteka = Convert.ToDateTime(dataGridView.Rows[e.RowIndex].Cells["Datum"].Value);
            originalBarkKod = Convert.ToInt32(dataGridView.Rows[e.RowIndex].Cells["BarKod"].Value);
            originalOpis = dataGridView.Rows[e.RowIndex].Cells["Opis"].Value.ToString();
            originalZalihe = Convert.ToInt32(dataGridView.Rows[e.RowIndex].Cells["Zalihe"].Value);
            originalIdProizvodjaca = Convert.ToInt32(dataGridView.Rows[e.RowIndex].Cells["IdProizvodjaca"].Value);
            originalCijena = Convert.ToDouble(dataGridView.Rows[e.RowIndex].Cells["Cijena"].Value);
        }
        protected override void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string modifiedNaziv = dataGridView.Rows[e.RowIndex].Cells["Naziv"].Value.ToString();
            DateTime modifiedDatumIsteka = Convert.ToDateTime(dataGridView.Rows[e.RowIndex].Cells["Datum"].Value);
            int modifiedBarkKod = Convert.ToInt32(dataGridView.Rows[e.RowIndex].Cells["BarKod"].Value);
            string modifiedOpis = dataGridView.Rows[e.RowIndex].Cells["Opis"].Value.ToString();
            int modifiedZalihe = Convert.ToInt32(dataGridView.Rows[e.RowIndex].Cells["Zalihe"].Value);
            int modifiedIdProizvodjaca = Convert.ToInt32(dataGridView.Rows[e.RowIndex].Cells["IdProizvodjaca"].Value);
            int idToDelete = Convert.ToInt32(dataGridView.Rows[e.RowIndex].Cells["ID"].Value);
            double modifiedCijena = Convert.ToDouble(dataGridView.Rows[e.RowIndex].Cells["Cijena"].Value);

            if (modifiedNaziv != originalNaziv || modifiedDatumIsteka != originalDatumIsteka || modifiedBarkKod != originalBarkKod
                || modifiedOpis != originalOpis || modifiedZalihe != originalZalihe ||
                modifiedIdProizvodjaca != originalIdProizvodjaca || modifiedCijena != originalCijena)
            {
                using (var connection = DBUtil.GetConnection())
                {
                    connection.Open();

                    using (MySqlCommand command = new MySqlCommand(updateProduct, connection))
                    {
                        command.Parameters.AddWithValue("@naziv", modifiedNaziv);
                        command.Parameters.AddWithValue("@datumIsteka", modifiedDatumIsteka);
                        command.Parameters.AddWithValue("@barKod", modifiedBarkKod);
                        command.Parameters.AddWithValue("@opis", modifiedOpis);
                        command.Parameters.AddWithValue("@zalihe", modifiedZalihe);
                        command.Parameters.AddWithValue("@idProizvodjaca", modifiedIdProizvodjaca);
                        command.Parameters.AddWithValue("@cijena", modifiedIdProizvodjaca);
                        command.Parameters.AddWithValue("@id", idToDelete);
                        command.ExecuteNonQuery();
                    }
                }
            }
            products = ReadAllProducts();
            dataGridView.DataSource = null;
            dataGridView.DataSource = products;
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }

        private void btnSaveAdd_Click_1(object sender, EventArgs e)
        {
            string ime = txt3.Text;
            string email = txt4.Text;
            int idAdrese = int.Parse(txt5.Text);
            using (var connection = DBUtil.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand("dodaj_proizvodjaca", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pIme", ime);
                    command.Parameters.AddWithValue("@pEmail", email);
                    command.Parameters.AddWithValue("@pIdAdresa", idAdrese);
                    command.ExecuteNonQuery();
                }
            }
            txt3.Clear();
            txt4.Clear();
            txt5.Clear();
            proizvodjaci = ReadAllProizvodjaci();
            dgvAdd.DataSource = null;
            dgvAdd.DataSource = proizvodjaci;
            foreach (DataGridViewColumn column in dgvAdd.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }
        protected override void dgvAdd_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            originalIme = dgvAdd.Rows[e.RowIndex].Cells["Ime"].Value.ToString();
            originalEmail = dgvAdd.Rows[e.RowIndex].Cells["Email"].Value.ToString();
            originalIdAdrese = Convert.ToInt32(dgvAdd.Rows[e.RowIndex].Cells["IdAdresa"].Value);
        }

        protected override void dgvAdd_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string modifiedIme = dgvAdd.Rows[e.RowIndex].Cells["Ime"].Value.ToString();
            string modifiedEmail = dgvAdd.Rows[e.RowIndex].Cells["Email"].Value.ToString();
            int modifiedIdAdrese = Convert.ToInt32(dgvAdd.Rows[e.RowIndex].Cells["IdAdresa"].Value);
            int idToDelete = Convert.ToInt32(dgvAdd.Rows[e.RowIndex].Cells["ID"].Value);

            if (modifiedIme != originalIme || modifiedEmail != originalEmail || modifiedIdAdrese != originalIdAdrese)
            {
                using (var connection = DBUtil.GetConnection())
                {
                    connection.Open();

                    using (MySqlCommand command = new MySqlCommand(updateStore, connection))
                    {
                        command.Parameters.AddWithValue("@ime", modifiedIme);
                        command.Parameters.AddWithValue("@email", modifiedEmail);
                        command.Parameters.AddWithValue("@idAdrese", modifiedIdAdrese);
                        command.Parameters.AddWithValue("@id", idToDelete);
                        command.ExecuteNonQuery();
                    }
                }
            }
            proizvodjaci = ReadAllProizvodjaci();
            dgvAdd.DataSource = null;
            dgvAdd.DataSource = proizvodjaci;
            foreach (DataGridViewColumn column in dgvAdd.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

        }
    }
}

