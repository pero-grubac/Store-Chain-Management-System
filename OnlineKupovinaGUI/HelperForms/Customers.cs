using MySql.Data.MySqlClient;
using OnlineKupovinaDB;
using System.Data;

namespace OnlineKupovinaGUI.HelperForms
{
    public partial class Customers : Stores
    {
        private static List<Kupac> customers;
        private static string getAll = @"SELECT k.idKUPAC,k.KorisnickoIme,k.Email,a.idADRESA, a.Drzava,a.Grad,a.Ulica FROM kupac k JOIN adresa a ON k.ADRESA_idADRESA=a.idADRESA;";
        private static string deleteCustomer = @"DELETE FROM kupac WHERE idKUPAC=@id; ";
        private static string updateCustomer = @"UPDATE  kupac SET KorisnickoIme=@korisnickoIme, Email=@email,ADRESA_idADRESA=@idAdrese WHERE idKUPAC=@id;";

        private static string originalKorisnickoIme;
        private static string originalEmail;
        private static int originalIdAdrese;

        public Customers()
        {
            InitializeComponent();
            customers = ReadAll();
        }
        private static List<Kupac> ReadAll()
        {
            List<Kupac> kupci = new List<Kupac>();
            using (var connection = DBUtil.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(getAll, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            return customers;
                        }
                        while (reader.Read())
                        {
                            int id = reader.GetInt32("idADRESA");
                            string grad = reader.GetString("Grad");
                            string drzava = reader.GetString("Drzava");
                            string ulica = reader.GetString("Ulica");
                            Adresa adresa = new Adresa(id, grad, ulica, drzava);
                            int idKupca = reader.GetInt32("idKUPAC");
                            string korisnickoIme = reader.GetString("KorisnickoIme");
                            string email = reader.GetString("Email");
                            Kupac temp = new Kupac(idKupca, korisnickoIme, email, adresa);
                            kupci.Add(temp);
                        }
                        return kupci;

                    }
                }
            }
        }
        private void btnReadAll_Click_1(object sender, EventArgs e)
        {
            dataGridView.DataSource = customers;
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
            int idAdrese = int.Parse(txtIdAdrese.Text);

            string hash = Password.HashValue(lozinka);

            using (var connection = DBUtil.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new("dodaj_kupca", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    //pKorisnickoIme, pLozinka, pEmail, pIdAdresa
                    command.Parameters.AddWithValue("@pKorisnickoIme", korisnickoIme);
                    command.Parameters.AddWithValue("@pLozinka", hash);
                    command.Parameters.AddWithValue("@pEmail", email);
                    command.Parameters.AddWithValue("@pIdAdresa", idAdrese);
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
            customers = ReadAll();
            dataGridView.DataSource = customers;
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
                int idToDelete = Convert.ToInt32(selectedRow.Cells["IdKupca"].Value);

                using (var connection = DBUtil.GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand(deleteCustomer, connection))
                    {
                        command.Parameters.AddWithValue("@id", idToDelete);
                        command.ExecuteNonQuery();
                    }
                }
                customers.RemoveAll(customer => customer.IdKupca == idToDelete);
                dataGridView.DataSource = null;
                dataGridView.DataSource = customers;
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
            originalIdAdrese = Convert.ToInt32(dataGridView.Rows[e.RowIndex].Cells["idAdresa"].Value);
        }
        protected override void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string modifiedKorisnickoIme = dataGridView.Rows[e.RowIndex].Cells["KorisnickoIme"].Value.ToString();
            string modifiedEmail = dataGridView.Rows[e.RowIndex].Cells["Email"].Value.ToString();
            int modifiedIdAdrese = Convert.ToInt32(dataGridView.Rows[e.RowIndex].Cells["idAdresa"].Value);
            int idToDelete = Convert.ToInt32(dataGridView.Rows[e.RowIndex].Cells["IdKupca"].Value);
            Kupac kupac = new Kupac(idToDelete, modifiedKorisnickoIme, modifiedEmail, modifiedIdAdrese);
            if (modifiedKorisnickoIme != originalKorisnickoIme || modifiedIdAdrese != originalIdAdrese || modifiedEmail != originalEmail)
            {
                using (var connection = DBUtil.GetConnection())
                {
                    connection.Open();


                    using (MySqlCommand command = new MySqlCommand(updateCustomer, connection))
                    {
                        command.Parameters.AddWithValue("@korisnickoIme", modifiedKorisnickoIme);
                        command.Parameters.AddWithValue("@email", modifiedEmail);
                        command.Parameters.AddWithValue("@idAdrese", modifiedIdAdrese);
                        command.Parameters.AddWithValue("@id", idToDelete);
                        command.ExecuteNonQuery();
                    }
                }
                int index = customers.FindIndex(customer => customer.IdKupca == idToDelete);
                if (index != -1)
                {
                    customers[index] = kupac;
                }
            }
            dataGridView.DataSource = customers;
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }
    }
}
