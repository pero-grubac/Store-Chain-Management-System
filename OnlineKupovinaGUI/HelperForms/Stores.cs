namespace OnlineKupovinaGUI.HelperForms
{
    public partial class Stores : Form
    {


        public Stores()
        {
            InitializeComponent();
            LoadTheme();

        }


        private void LoadTheme()
        {
            foreach (Control btns in tabPage1.Controls)
            {
                if (btns.GetType() == typeof(Button))
                {
                    Button btn = (Button)btns;
                    btn.BackColor = ThemeColor.PrimaryColor;
                    btn.ForeColor = Color.White;
                    btn.FlatAppearance.BorderColor = ThemeColor.SecondaryColor;
                    if (btn.BackColor == Color.Transparent)
                        btn.BackColor = Color.FromArgb(0, 74, 173);
                }
            }

            foreach (Control btns in tabPage2.Controls)
            {
                if (btns.GetType() == typeof(Button))
                {
                    Button btn = (Button)btns;
                    btn.BackColor = ThemeColor.SecondaryColor;
                    btn.ForeColor = Color.White;
                    btn.FlatAppearance.BorderColor = ThemeColor.PrimaryColor;
                    if (btn.BackColor == Color.Transparent)
                        btn.BackColor = Color.FromArgb(0, 74, 173);
                }
            }
            foreach (Control btns in tabPage3.Controls)
            {
                if (btns.GetType() == typeof(Button))
                {
                    Button btn = (Button)btns;
                    btn.BackColor = ThemeColor.PrimaryColor;
                    btn.ForeColor = Color.White;
                    btn.FlatAppearance.BorderColor = ThemeColor.SecondaryColor;
                    if (btn.BackColor == Color.Transparent)
                        btn.BackColor = Color.FromArgb(0, 74, 173);
                }
            }
            foreach (Control btns in tabPage4.Controls)
            {
                if (btns.GetType() == typeof(Button))
                {
                    Button btn = (Button)btns;
                    btn.BackColor = ThemeColor.SecondaryColor;
                    btn.ForeColor = Color.White;
                    btn.FlatAppearance.BorderColor = ThemeColor.PrimaryColor;
                    if (btn.BackColor == Color.Transparent)
                        btn.BackColor = Color.FromArgb(0, 74, 173);
                }
            }

        }


        protected virtual void btnReadAll_Click(object sender, EventArgs e)
        {

        }


        protected virtual void btnSave_Click(object sender, EventArgs e)
        {

        }

        protected virtual void dataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {

        }

        protected virtual void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

        }

        protected virtual void btnDelete_Click(object sender, EventArgs e)
        {

        }

        protected virtual void btnReadAllAdd_Click(object sender, EventArgs e)
        {

        }

        protected virtual void btnDeleteAdd_Click(object sender, EventArgs e)
        {

        }

        protected virtual void dgvAdd_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {

        }

        protected virtual void dgvAdd_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

        }

        protected virtual void btnSaveAdd_Click(object sender, EventArgs e)
        {

        }
    }
}
