namespace OnlineKupovinaGUI
{
    public partial class Login : Form
    {


        public Login()
        {
            InitializeComponent();
        }
        private void label3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnLogIn_Click(object sender, EventArgs e)
        {
            string selectedRole = GetSelectedRole();
            string userName = txtUserName.Text;
            string password = txtPassword.Text;
            Password.SaveData(selectedRole, userName);
            string hashedPasswordFromDB = Password.GetHashedPassword();

            if (selectedRole == null)
            {
                MessageBox.Show("Izaberite ulogu");
                return;
            }

            if (Password.Verify(password) && hashedPasswordFromDB != "")
            {

                switch (selectedRole)
                {
                    case "kupac":
                        GUIUtil.ShowNextForm(this, new KupacForm());
                        break;
                    case "administrator":
                        GUIUtil.ShowNextForm(this, new AdministratorForm());
                        break;
                    case "zaposleni":
                        GUIUtil.ShowNextForm(this, new ZaposleniForm());
                        break;
                    default:
                        MessageBox.Show("Pogrešan unos");
                        break;
                }
            }
            else
            {
                MessageBox.Show("Pogrešan unos");
            }


        }
        private string GetSelectedRole()
        {
            if (rdBtnKorisnik.Checked)
            {
                return "kupac";
            }
            else if (rdBtnZaposleni.Checked)
            {
                return "zaposleni";
            }
            else if (rdBttnAdmin.Checked)
            {
                return "administrator";
            }
            else
            {
                return null;
            }

        }
        private void lblClearFields_Click(object sender, EventArgs e)
        {

            txtPassword.Clear();
            txtUserName.Clear();
            txtUserName.Focus();
        }

        private void txtUserName_TextChanged(object sender, EventArgs e)
        {

        }
    }
}