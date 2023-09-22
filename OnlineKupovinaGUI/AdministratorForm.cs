using System.Runtime.InteropServices;

namespace OnlineKupovinaGUI
{
    public partial class AdministratorForm : Form
    {
        private Button currentButton;
        private Random random;
        private int tempIndex;
        private Form activeForm;

        public AdministratorForm()
        {
            InitializeComponent();
            random = new Random();
            btnCloseChldForm.Visible = false;
            this.Text = string.Empty;
            this.ControlBox = false;
            MaximizedBounds = Screen.FromHandle(Handle).WorkingArea;
        }
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);
        private Color SelectThemeColor()
        {
            int index = random.Next(ThemeColor.ColorList.Count);
            while (tempIndex == index)
            {
                index = random.Next(ThemeColor.ColorList.Count);
            }
            tempIndex = index;
            string color = ThemeColor.ColorList[tempIndex];
            return ColorTranslator.FromHtml(color);
        }
        private void ActivateButton(object btnSender)
        {
            if (btnSender != null)
            {
                if (currentButton != (Button)btnSender)
                {
                    DisableButton();
                    Color color = SelectThemeColor();
                    currentButton = (Button)btnSender;
                    currentButton.BackColor = color;
                    currentButton.ForeColor = Color.White;
                    currentButton.Font = new Font("Microsoft Sans Serif", 12.5F, FontStyle.Regular, GraphicsUnit.Point);
                    panelTitleBar.BackColor = color;
                    panelLogo.BackColor = ThemeColor.ChangeColorBrightness(color, -0.3);
                    ThemeColor.PrimaryColor = color;
                    ThemeColor.SecondaryColor = ThemeColor.ChangeColorBrightness(color, -0.3);
                    btnCloseChldForm.Visible = true;
                }
            }
        }
        private void DisableButton()
        {
            foreach (Control perviousBtn in panelMenu.Controls)
            {
                if (perviousBtn.GetType() == typeof(Button))
                {
                    perviousBtn.BackColor = Color.FromArgb(51, 51, 76);
                    perviousBtn.ForeColor = Color.Gainsboro;
                    perviousBtn.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point);

                }
            }
        }
        private void OpenChildForm(Form childForm, object btnSender)
        {
            if (activeForm != null)
            {
                activeForm.Close();
            }
            ActivateButton(btnSender);
            activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            panelDesktop.Controls.Add(childForm);
            panelDesktop.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
            lblTitle.Text = childForm.Text;
        }

        private void btnStore_Click(object sender, EventArgs e)
        {
            OpenChildForm(new HelperForms.AdminStore(), sender);
        }

        private void btnBill_Click(object sender, EventArgs e)
        {
            OpenChildForm(new HelperForms.Staff(), sender);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenChildForm(new HelperForms.Customers(), sender);
        }

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            GUIUtil.ShowNextForm(this, new Login());
        }

        private void btnCloseChldForm_Click(object sender, EventArgs e)
        {
            if (activeForm != null)
                activeForm.Close();
            Reset();
        }
        private void Reset()
        {
            DisableButton();
            lblTitle.Text = "HOME";
            panelTitleBar.BackColor = Color.FromArgb(0, 74, 173);
            panelLogo.BackColor = Color.FromArgb(39, 39, 58);
            currentButton = null;
            btnCloseChldForm.Visible = false;

        }


        private void btnMin_Click_1(object sender, EventArgs e)
        {
            this.WindowState |= FormWindowState.Minimized;
        }

        private void btnMax_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
                this.WindowState = FormWindowState.Maximized;
            else
                this.WindowState = FormWindowState.Normal;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void panelTitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void btnAdmin_Click(object sender, EventArgs e)
        {
            OpenChildForm(new HelperForms.Admin(), sender);
        }

        private void btnPodesavanja_Click(object sender, EventArgs e)
        {
            OpenChildForm(new HelperForms.Settings(), sender);
        }
    }
}
