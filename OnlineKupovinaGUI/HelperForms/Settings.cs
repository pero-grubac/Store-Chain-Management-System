namespace OnlineKupovinaGUI.HelperForms
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
            LoadTheme();
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

        private void btnSave_Click(object sender, EventArgs e)
        {

            string newPassOne = txtNewPass.Text;
            string newPssTwo = txtNewPass2.Text;
            if (newPassOne == newPssTwo)
            {
                if (!Password.UpdatePassword(newPassOne)) MessageBox.Show("Pokusaj ponovo");
                txtNewPass.Clear();
                txtNewPass2.Clear();
            }
            else MessageBox.Show("Pokusaj ponovo");

        }
    }
}
