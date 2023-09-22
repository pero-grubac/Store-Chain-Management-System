using MySql.Data.MySqlClient;
using OnlineKupovinaDB;

namespace OnlineKupovinaGUI.HelperForms
{
    public partial class AdminStore : Stores
    {
        private static List<Prodavnica> stores;
        private static List<Adresa> adrese;
        private static string getAll = @"SELECT p.idProdavnica, p.Ime,a.idADRESA, a.Drzava,a.Grad,a.Ulica FROM prodavnica p JOIN adresa a ON p.ADRESA_idADRESA = a.idADRESA;";
        private static string getAllAdd = @"SELECT * FROM adresa;";
        private static string addStore = @"INSERT INTO Prodavnica (ime, ADRESA_idADRESA) VALUES (@ime, @adresa_id);";
        private static string addAddress = @"INSERT INTO adresa (Drzava, Grad, Ulica) VALUES (@drzava, @grad, @ulica);";
        private static string updateStore = @"UPDATE Prodavnica SET ime = @ime, ADRESA_idADRESA = @idAdrese WHERE idProdavnica = @id";
        private static string updateAddress = @"UPDATE adresa SET Drzava = @drzava, Grad = @grad, Ulica = @ulica WHERE idADRESA = @idAdresa;";
        private static string deleteStore = @"DELETE FROM Prodavnica WHERE idProdavnica = @id";
        private static string deleteAddress = @"DELETE FROM adresa WHERE idADRESA = @id";
        private static string originalIme;
        private static int originalIdAdrese;
        private string originalDrzava;
        private string originalGrad;
        private string originalUlica;

        public AdminStore()
        {
            InitializeComponent();
            stores = ReadAll();
            adrese = ReadAllAdd();
        }
        private static List<Prodavnica> ReadAll()
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

        protected override void btnReadAll_Click(object sender, EventArgs e)
        {
            dataGridView.DataSource = stores;
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }


        protected override void btnSave_Click(object sender, EventArgs e)
        {
            string ime = txt1.Text;
            int idAdrese = int.Parse(txt2.Text);
            using (var connection = DBUtil.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(addStore, connection))
                {
                    command.Parameters.AddWithValue("@ime", ime);
                    command.Parameters.AddWithValue("@adresa_id", idAdrese);
                    command.ExecuteNonQuery();
                }
            }
            txt2.Clear();
            txt1.Clear();
            stores = ReadAll();
            dataGridView.DataSource = stores;
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }

        protected override void dataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            originalIme = dataGridView.Rows[e.RowIndex].Cells["ime"].Value.ToString();
            originalIdAdrese = Convert.ToInt32(dataGridView.Rows[e.RowIndex].Cells["idAdresa"].Value);
        }

        protected override void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string modifiedIme = dataGridView.Rows[e.RowIndex].Cells["ime"].Value.ToString();
            int modifiedIdAdrese = Convert.ToInt32(dataGridView.Rows[e.RowIndex].Cells["idAdresa"].Value);
            int idToDelete = Convert.ToInt32(dataGridView.Rows[e.RowIndex].Cells["ID"].Value);
            Prodavnica prodavnica = new Prodavnica(idToDelete, modifiedIme, modifiedIdAdrese);
            if (modifiedIme != originalIme || modifiedIdAdrese != originalIdAdrese)
            {
                using (var connection = DBUtil.GetConnection())
                {
                    connection.Open();


                    using (MySqlCommand command = new MySqlCommand(updateStore, connection))
                    {
                        command.Parameters.AddWithValue("@ime", modifiedIme);
                        command.Parameters.AddWithValue("@idAdrese", modifiedIdAdrese);
                        command.Parameters.AddWithValue("@id", idToDelete);
                        command.ExecuteNonQuery();
                    }
                }
                int index = stores.FindIndex(proda => proda.ID == idToDelete);
                if (index != -1)
                {
                    stores[index] = prodavnica;
                }
            }
            dataGridView.DataSource = stores;
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }

        protected override void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                int selectedIndex = dataGridView.SelectedRows[0].Index;
                int idToDelete = Convert.ToInt32(dataGridView.Rows[selectedIndex].Cells["id"].Value);

                using (var connection = DBUtil.GetConnection())
                {
                    connection.Open();


                    using (MySqlCommand command = new MySqlCommand(deleteStore, connection))
                    {
                        command.Parameters.AddWithValue("@id", idToDelete);

                        command.ExecuteNonQuery();
                    }
                }
                stores.RemoveAll(prodavnica => prodavnica.ID == idToDelete);
                dataGridView.DataSource = null;
                dataGridView.DataSource = stores;
                foreach (DataGridViewColumn column in dataGridView.Columns)
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
            }
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
        protected override void btnReadAllAdd_Click(object sender, EventArgs e)
        {
            dgvAdd.DataSource = adrese;
            foreach (DataGridViewColumn column in dgvAdd.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }

        protected override void btnDeleteAdd_Click(object sender, EventArgs e)
        {
            if (dgvAdd.SelectedRows.Count > 0)
            {
                int selectedIndex = dgvAdd.SelectedRows[0].Index;
                int idToDelete = Convert.ToInt32(dgvAdd.Rows[selectedIndex].Cells["idAdresa"].Value);

                using (var connection = DBUtil.GetConnection())
                {
                    connection.Open();

                    using (MySqlCommand command = new MySqlCommand(deleteAddress, connection))
                    {
                        command.Parameters.AddWithValue("@id", idToDelete);

                        command.ExecuteNonQuery();
                    }
                }
                adrese = ReadAllAdd();
                dgvAdd.DataSource = null;
                dgvAdd.DataSource = adrese;
                foreach (DataGridViewColumn column in dgvAdd.Columns)
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }

            }

        }

        protected override void dgvAdd_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            originalDrzava = dgvAdd.Rows[e.RowIndex].Cells["drzava"].Value.ToString();
            originalGrad = dgvAdd.Rows[e.RowIndex].Cells["grad"].Value.ToString();
            originalUlica = dgvAdd.Rows[e.RowIndex].Cells["ulica"].Value.ToString();
        }

        protected override void dgvAdd_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string modifiedDrzava = dgvAdd.Rows[e.RowIndex].Cells["drzava"].Value.ToString();
            string modifiedGrad = dgvAdd.Rows[e.RowIndex].Cells["grad"].Value.ToString();
            string modifiedUlica = dgvAdd.Rows[e.RowIndex].Cells["ulica"].Value.ToString();
            int idToDelete = Convert.ToInt32(dgvAdd.Rows[e.RowIndex].Cells["idAdresa"].Value);
            Adresa adresa = new Adresa(idToDelete, modifiedGrad, modifiedUlica, modifiedDrzava);

            if (modifiedDrzava != originalDrzava || modifiedGrad != originalGrad || modifiedUlica != originalUlica)
            {
                using (var connection = DBUtil.GetConnection())
                {
                    connection.Open();

                    using (MySqlCommand command = new MySqlCommand(updateAddress, connection))
                    {
                        command.Parameters.AddWithValue("@drzava", modifiedDrzava);
                        command.Parameters.AddWithValue("@grad", modifiedGrad);
                        command.Parameters.AddWithValue("@ulica", modifiedUlica);
                        command.Parameters.AddWithValue("@idAdresa", idToDelete);
                        command.ExecuteNonQuery();
                    }
                }
                int index = adrese.FindIndex(ad => ad.idAdresa == idToDelete);
                if (index != -1) { adrese[index] = adresa; }
            }
            dgvAdd.DataSource = adrese;
            foreach (DataGridViewColumn column in dgvAdd.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }

        protected override void btnSaveAdd_Click(object sender, EventArgs e)
        {
            string drzava = txt3.Text;
            string grad = txt4.Text;
            string ulica = txt5.Text;
            using (var connection = DBUtil.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(addAddress, connection))
                {
                    command.Parameters.AddWithValue("@drzava", drzava);
                    command.Parameters.AddWithValue("@grad", grad);
                    command.Parameters.AddWithValue("@ulica", ulica);
                    command.ExecuteNonQuery();
                }
            }
            txt3.Clear();
            txt4.Clear();
            txt5.Clear();
            adrese = ReadAllAdd();
            dgvAdd.DataSource = adrese;
            foreach (DataGridViewColumn column in dgvAdd.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }
    }
}
