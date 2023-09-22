using MySql.Data.MySqlClient;
using OnlineKupovinaDB;
using System.Data;

namespace OnlineKupovinaGUI.HelperForms
{
    public partial class Staff : Stores
    {
        private static List<Zaposleni> staff;
        private static string getAll = @"SELECT * FROM zaposleni";
        private static string addEmployee = @"INSERT INTO zaposleni (Ime, Prezime,Email,BrojTelefona, PRODAVNICA_idPRODAVNICA, plata,Lozinka,KorisnickoIme) VALUES (@pIme, @pPrezime,@pEmail,@pBrojtelefona,  @pIdProdavnice, @pPlata,@pLozinka,@pKorisnickoime);";
        private static string deleteEmployee = @"DELETE FROM zaposleni WHERE idZAPOSLENI=@id;";
        private static string updateEmployee = @"UPDATE zaposleni SET Ime=@ime, Prezime=@prezime, Email=@email, BrojTelefona=@brojTelefona, plata=@plata, KorisnickoIme=@korisnickoIme,Prodavnica_idProdavnica=@idProdavnice WHERE idZAPOSLENI=@id;";

        private static string originalIme;
        private static string originalPrezime;
        private static string originalEmail;
        private static int originalPlata;
        private static string originalBrojTelefona;
        private static string originalKorisnickoIme;
        private static int originalIdProdavnice;
        public Staff()
        {
            InitializeComponent();
            staff = ReadAll();
        }
        private static List<Zaposleni> ReadAll()
        {
            List<Zaposleni> staff = new List<Zaposleni>();
            using (var connection = DBUtil.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(getAll, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            return staff;
                        }
                        while (reader.Read())
                        {
                            int id = reader.GetInt32("idZAPOSLENI");
                            string ime = reader.GetString("Ime");
                            string prezime = reader.GetString("Prezime");
                            string email = reader.GetString("Email");
                            string brojTelefona = reader.GetString("BrojTelefona");
                            int idProdavnice = reader.GetInt32("Prodavnica_idProdavnica");
                            double plata = reader.GetDouble("Plata");
                            string korisnickoIme = reader.GetString("KorisnickoIme");
                            Zaposleni temp = new Zaposleni(id, ime, prezime, email, brojTelefona, idProdavnice, plata, korisnickoIme);
                            staff.Add(temp);
                        }
                        return staff;

                    }
                }
            }
        }
        protected override void btnReadAll_Click(object sender, EventArgs e)
        {
            dataGridView.DataSource = staff;
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {
            string ime = txt1.Text;
            string prezime = txt2.Text;
            string email = txtEmail.Text;
            string brojTelefona = txtTelefon.Text;
            int idProdavnice = int.Parse(txtIdProdavnice.Text);
            double plata = double.Parse(txtPlata.Text);
            string korisnickoIme = txtKorisnickoIme.Text;
            string lozinka = txtLozinka.Text;


            string hash = Password.HashValue(lozinka);
            using (var connection = DBUtil.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new("dodaj_radnika", connection))
                {

                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pIme", ime);
                    command.Parameters.AddWithValue("@pPrezime", prezime);
                    command.Parameters.AddWithValue("@pEmail", email);
                    command.Parameters.AddWithValue("@pBrojTelefona", brojTelefona);
                    command.Parameters.AddWithValue("@pIdProdavnice", idProdavnice);
                    command.Parameters.AddWithValue("@pPlata", plata);
                    command.Parameters.AddWithValue("@pLozinka", hash);
                    command.Parameters.AddWithValue("@pKorisnickoIme", korisnickoIme);
                    //  command.Prepare();
                    command.ExecuteNonQuery();

                }
            }
            staff = ReadAll();
            dataGridView.DataSource = staff;
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            tab.SelectedTab = tabPage1;
        }

        private void btnDelete_Click_1(object sender, EventArgs e)
        {
            DataGridViewRow selectedRow = dataGridView.SelectedRows[0];

            if (selectedRow.Cells["ID"].Value != null)
            {
                int idToDelete = Convert.ToInt32(selectedRow.Cells["ID"].Value);

                using (var connection = DBUtil.GetConnection())
                {
                    connection.Open();


                    using (var command = new MySqlCommand(deleteEmployee, connection))
                    {
                        command.Parameters.AddWithValue("@id", idToDelete);
                        command.ExecuteNonQuery();
                    }
                }

                staff.RemoveAll(zaposleni => zaposleni.ID == idToDelete);

                dataGridView.DataSource = null;
                dataGridView.DataSource = staff;

                foreach (DataGridViewColumn column in dataGridView.Columns)
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
            }

        }
        protected override void dataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            originalIme = dataGridView.Rows[e.RowIndex].Cells["Ime"].Value.ToString();
            originalPrezime = dataGridView.Rows[e.RowIndex].Cells["Prezime"].Value.ToString();
            originalEmail = dataGridView.Rows[e.RowIndex].Cells["Email"].Value.ToString();
            originalBrojTelefona = dataGridView.Rows[e.RowIndex].Cells["BrojTelefona"].Value.ToString();
            originalPlata = Convert.ToInt32(dataGridView.Rows[e.RowIndex].Cells["Plata"].Value);
            originalKorisnickoIme = dataGridView.Rows[e.RowIndex].Cells["KorisnickoIme"].Value.ToString();
            originalIdProdavnice = Convert.ToInt32(dataGridView.Rows[e.RowIndex].Cells["IdProdavnice"].Value);
        }
        protected override void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                string modifiedIme = dataGridView.Rows[e.RowIndex].Cells["Ime"].Value.ToString();
                string modifiedPrezime = dataGridView.Rows[e.RowIndex].Cells["Prezime"].Value.ToString();
                string modifiedEmail = dataGridView.Rows[e.RowIndex].Cells["Email"].Value.ToString();
                string modifiedBrojTelefona = dataGridView.Rows[e.RowIndex].Cells["BrojTelefona"].Value.ToString();
                int modifiedPlata = Convert.ToInt32(dataGridView.Rows[e.RowIndex].Cells["Plata"].Value);
                string modifiedKorisnickoIme = dataGridView.Rows[e.RowIndex].Cells["KorisnickoIme"].Value.ToString();
                int modifiedIdProdavnice = Convert.ToInt32(dataGridView.Rows[e.RowIndex].Cells["IdProdavnice"].Value);
                int idToDelete = Convert.ToInt32(dataGridView.Rows[e.RowIndex].Cells["ID"].Value);
                Zaposleni zaposleni = new Zaposleni(idToDelete, modifiedIme, modifiedPrezime, modifiedEmail, modifiedBrojTelefona, modifiedIdProdavnice, modifiedPlata, modifiedKorisnickoIme);
                if (modifiedBrojTelefona != originalBrojTelefona || modifiedEmail != originalEmail || modifiedIme != originalIme
                       || modifiedKorisnickoIme != originalKorisnickoIme || modifiedPlata != originalPlata || modifiedPrezime != originalPrezime || modifiedIdProdavnice != originalIdProdavnice)
                {
                    using (var connection = DBUtil.GetConnection())
                    {
                        connection.Open();
                        using (MySqlCommand command = new MySqlCommand(updateEmployee, connection))
                        {
                            command.Parameters.AddWithValue("@ime", modifiedIme);
                            command.Parameters.AddWithValue("@prezime", modifiedPrezime);
                            command.Parameters.AddWithValue("@email", modifiedEmail);
                            command.Parameters.AddWithValue("@brojTelefona", modifiedBrojTelefona);
                            command.Parameters.AddWithValue("@plata", modifiedPlata);
                            command.Parameters.AddWithValue("@korisnickoIme", modifiedKorisnickoIme);
                            command.Parameters.AddWithValue("@idProdavnice", modifiedIdProdavnice);
                            command.Parameters.AddWithValue("@id", idToDelete);
                            command.ExecuteNonQuery();
                        }
                    }
                }
                int indexToReplace = staff.FindIndex(zaposleni => zaposleni.ID == idToDelete);
                if (indexToReplace != -1)
                {
                    staff[indexToReplace] = zaposleni;
                }
            }

            dataGridView.DataSource = staff;
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }
    }
}
