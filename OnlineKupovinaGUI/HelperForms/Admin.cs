using MySql.Data.MySqlClient;
using OnlineKupovinaDB;
using System.Data;

namespace OnlineKupovinaGUI.HelperForms
{
    public partial class Admin : Stores
    {
        private static List<Administrator> administrators;
        private static string getAll = @"SELECT * FROM administrator;";
        private static string deleteAdmin = @"DELETE FROM administrator WHERE idAdministrator=@id;";
        private static string updateAdmin = @"UPDATE administrator SET Ime=@ime, Prezime=@prezime, Email=@email, KorisnickoIme=@korisnickoIme  WHERE idAdministrator=@id;";

        private static string originalIme;
        private static string originalPrezime;
        private static string originalEmail;
        private static string originalKorisnickoIme;
        public Admin()
        {
            InitializeComponent();
            administrators = ReadAll();
        }
        private static List<Administrator> ReadAll()
        {
            List<Administrator> administratori = new List<Administrator>();
            using (var connection = DBUtil.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(getAll, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            return administratori;
                        }
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(@"idAdministrator");
                            string ime = reader.GetString(@"Ime");
                            string prezime = reader.GetString(@"Prezime");
                            string korisnickoIme = reader.GetString(@"KorisnickoIme");
                            string email = reader.GetString(@"Email");
                            Administrator temp = new(id, ime, prezime, email, korisnickoIme);
                            administratori.Add(temp);
                        }
                        return administratori;

                    }
                }
            }
        }
        private void btnReadAll_Click_1(object sender, EventArgs e)
        {
            dataGridView.DataSource = administrators;
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {
            string korisnickoIme = txt1.Text;
            string lozinka = txt2.Text;
            string email = txtEmail.Text;
            string ime = txtIme.Text;
            string prezime = txtPrezime.Text;

            string hash = Password.HashValue(lozinka);
            using (var connection = DBUtil.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new("dodaj_administratora", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pIme", ime);
                    command.Parameters.AddWithValue("@pPrezime", prezime);
                    command.Parameters.AddWithValue("@pEmail", email);
                    command.Parameters.AddWithValue("@pLozinka", hash);
                    command.Parameters.AddWithValue("@pKorisnickoIme", korisnickoIme);
                    command.ExecuteNonQuery();
                }
            }
            foreach (Control txts in tabPage2.Controls)
            {
                if (txts.GetType() == typeof(TextBox))
                {
                    TextBox txt = (TextBox)txts;
                    txt.Clear();
                }
            }
            administrators = ReadAll();
            dataGridView.DataSource = administrators;
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

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
                    using (var command = new MySqlCommand(deleteAdmin, connection))
                    {
                        command.Parameters.AddWithValue("@id", idToDelete);
                        command.ExecuteNonQuery();
                    }
                }
                administrators.RemoveAll(admin => admin.ID == idToDelete);
                dataGridView.DataSource = null;
                dataGridView.DataSource = administrators;
                foreach (DataGridViewColumn column in dataGridView.Columns)
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
            }
        }
        protected override void dataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            originalKorisnickoIme = dataGridView.Rows[e.RowIndex].Cells["KorisnickoIme"].Value.ToString();
            originalEmail = dataGridView.Rows[e.RowIndex].Cells["Email"].Value.ToString();
            originalIme = dataGridView.Rows[e.RowIndex].Cells["Ime"].Value.ToString();
            originalPrezime = dataGridView.Rows[e.RowIndex].Cells["Prezime"].Value.ToString();
        }
        protected override void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string modifiedKorisnickoIme = dataGridView.Rows[e.RowIndex].Cells["KorisnickoIme"].Value.ToString();
            string modifiedEmail = dataGridView.Rows[e.RowIndex].Cells["Email"].Value.ToString();
            string modifiedIme = dataGridView.Rows[e.RowIndex].Cells["Ime"].Value.ToString();
            string modifiedPrezime = dataGridView.Rows[e.RowIndex].Cells["Prezime"].Value.ToString();
            int idToDelete = Convert.ToInt32(dataGridView.Rows[e.RowIndex].Cells["ID"].Value);
            Administrator administrator = new Administrator(idToDelete, modifiedIme, modifiedPrezime, modifiedEmail, modifiedKorisnickoIme);
            if (modifiedKorisnickoIme != originalKorisnickoIme || modifiedEmail != originalEmail
                || modifiedIme != originalIme || modifiedPrezime != originalPrezime)
            {
                using (var connection = DBUtil.GetConnection())
                {
                    connection.Open();


                    using (MySqlCommand command = new MySqlCommand(updateAdmin, connection))
                    {
                        try
                        {
                            command.Parameters.AddWithValue("@korisnickoIme", modifiedKorisnickoIme);
                            command.Parameters.AddWithValue("@email", modifiedEmail);
                            command.Parameters.AddWithValue("@ime", modifiedIme);
                            command.Parameters.AddWithValue("@prezime", modifiedPrezime);
                            command.Parameters.AddWithValue("@id", idToDelete);
                            command.ExecuteNonQuery();
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message); }
                    }
                }
                int index = administrators.FindIndex(admin => admin.ID == idToDelete);
                if (index != -1)
                {
                    administrators[index] = administrator;
                }
            }
            dataGridView.DataSource = administrators;
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }
    }
}
