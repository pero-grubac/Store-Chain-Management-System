namespace OnlineKupovinaGUI
{
    partial class AdministratorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdministratorForm));
            panelMenu = new Panel();
            btnAdmin = new Button();
            btnCustomers = new Button();
            btnLogOut = new Button();
            btnStaff = new Button();
            btnStore = new Button();
            panelLogo = new Panel();
            lbl = new Label();
            panelTitleBar = new Panel();
            btnMin = new Button();
            btnMax = new Button();
            btnClose = new Button();
            btnCloseChldForm = new Button();
            lblTitle = new Label();
            panelDesktop = new Panel();
            pictureBox1 = new PictureBox();
            btnPodesavanja = new Button();
            panelMenu.SuspendLayout();
            panelLogo.SuspendLayout();
            panelTitleBar.SuspendLayout();
            panelDesktop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // panelMenu
            // 
            panelMenu.BackColor = Color.FromArgb(51, 51, 76);
            panelMenu.Controls.Add(btnPodesavanja);
            panelMenu.Controls.Add(btnAdmin);
            panelMenu.Controls.Add(btnCustomers);
            panelMenu.Controls.Add(btnLogOut);
            panelMenu.Controls.Add(btnStaff);
            panelMenu.Controls.Add(btnStore);
            panelMenu.Controls.Add(panelLogo);
            panelMenu.Dock = DockStyle.Left;
            panelMenu.Location = new Point(0, 0);
            panelMenu.Name = "panelMenu";
            panelMenu.Size = new Size(220, 575);
            panelMenu.TabIndex = 1;
            // 
            // btnAdmin
            // 
            btnAdmin.Dock = DockStyle.Top;
            btnAdmin.FlatAppearance.BorderSize = 0;
            btnAdmin.FlatStyle = FlatStyle.Flat;
            btnAdmin.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point);
            btnAdmin.ForeColor = Color.Gainsboro;
            btnAdmin.Image = (Image)resources.GetObject("btnAdmin.Image");
            btnAdmin.ImageAlign = ContentAlignment.MiddleLeft;
            btnAdmin.Location = new Point(0, 260);
            btnAdmin.Name = "btnAdmin";
            btnAdmin.Padding = new Padding(12, 0, 0, 0);
            btnAdmin.Size = new Size(220, 60);
            btnAdmin.TabIndex = 6;
            btnAdmin.Text = "   Administratori";
            btnAdmin.TextAlign = ContentAlignment.MiddleLeft;
            btnAdmin.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnAdmin.UseVisualStyleBackColor = true;
            btnAdmin.Click += btnAdmin_Click;
            // 
            // btnCustomers
            // 
            btnCustomers.Dock = DockStyle.Top;
            btnCustomers.FlatAppearance.BorderSize = 0;
            btnCustomers.FlatStyle = FlatStyle.Flat;
            btnCustomers.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point);
            btnCustomers.ForeColor = Color.Gainsboro;
            btnCustomers.Image = (Image)resources.GetObject("btnCustomers.Image");
            btnCustomers.ImageAlign = ContentAlignment.MiddleLeft;
            btnCustomers.Location = new Point(0, 200);
            btnCustomers.Name = "btnCustomers";
            btnCustomers.Padding = new Padding(12, 0, 0, 0);
            btnCustomers.Size = new Size(220, 60);
            btnCustomers.TabIndex = 5;
            btnCustomers.Text = "   Kupci";
            btnCustomers.TextAlign = ContentAlignment.MiddleLeft;
            btnCustomers.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnCustomers.UseVisualStyleBackColor = true;
            btnCustomers.Click += button4_Click;
            // 
            // btnLogOut
            // 
            btnLogOut.Dock = DockStyle.Bottom;
            btnLogOut.FlatAppearance.BorderSize = 0;
            btnLogOut.FlatStyle = FlatStyle.Flat;
            btnLogOut.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point);
            btnLogOut.ForeColor = Color.Gainsboro;
            btnLogOut.Image = (Image)resources.GetObject("btnLogOut.Image");
            btnLogOut.ImageAlign = ContentAlignment.MiddleLeft;
            btnLogOut.Location = new Point(0, 515);
            btnLogOut.Name = "btnLogOut";
            btnLogOut.Padding = new Padding(12, 0, 0, 0);
            btnLogOut.Size = new Size(220, 60);
            btnLogOut.TabIndex = 4;
            btnLogOut.Text = "   Log out";
            btnLogOut.TextAlign = ContentAlignment.MiddleLeft;
            btnLogOut.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnLogOut.UseVisualStyleBackColor = true;
            btnLogOut.Click += btnLogOut_Click;
            // 
            // btnStaff
            // 
            btnStaff.Dock = DockStyle.Top;
            btnStaff.FlatAppearance.BorderSize = 0;
            btnStaff.FlatStyle = FlatStyle.Flat;
            btnStaff.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point);
            btnStaff.ForeColor = Color.Gainsboro;
            btnStaff.Image = (Image)resources.GetObject("btnStaff.Image");
            btnStaff.ImageAlign = ContentAlignment.MiddleLeft;
            btnStaff.Location = new Point(0, 140);
            btnStaff.Name = "btnStaff";
            btnStaff.Padding = new Padding(12, 0, 0, 0);
            btnStaff.Size = new Size(220, 60);
            btnStaff.TabIndex = 2;
            btnStaff.Text = "   Zaposleni";
            btnStaff.TextAlign = ContentAlignment.MiddleLeft;
            btnStaff.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnStaff.UseVisualStyleBackColor = true;
            btnStaff.Click += btnBill_Click;
            // 
            // btnStore
            // 
            btnStore.Dock = DockStyle.Top;
            btnStore.FlatAppearance.BorderSize = 0;
            btnStore.FlatStyle = FlatStyle.Flat;
            btnStore.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point);
            btnStore.ForeColor = Color.Gainsboro;
            btnStore.Image = (Image)resources.GetObject("btnStore.Image");
            btnStore.ImageAlign = ContentAlignment.MiddleLeft;
            btnStore.Location = new Point(0, 80);
            btnStore.Name = "btnStore";
            btnStore.Padding = new Padding(12, 0, 0, 0);
            btnStore.Size = new Size(220, 60);
            btnStore.TabIndex = 1;
            btnStore.Text = "   Prodavnice";
            btnStore.TextAlign = ContentAlignment.MiddleLeft;
            btnStore.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnStore.UseVisualStyleBackColor = true;
            btnStore.Click += btnStore_Click;
            // 
            // panelLogo
            // 
            panelLogo.BackColor = Color.FromArgb(39, 39, 58);
            panelLogo.Controls.Add(lbl);
            panelLogo.Dock = DockStyle.Top;
            panelLogo.Location = new Point(0, 0);
            panelLogo.Name = "panelLogo";
            panelLogo.Size = new Size(220, 80);
            panelLogo.TabIndex = 0;
            // 
            // lbl
            // 
            lbl.AutoSize = true;
            lbl.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lbl.ForeColor = Color.LightGray;
            lbl.Location = new Point(61, 30);
            lbl.Name = "lbl";
            lbl.Size = new Size(70, 25);
            lbl.TabIndex = 0;
            lbl.Text = "eShop";
            // 
            // panelTitleBar
            // 
            panelTitleBar.BackColor = Color.FromArgb(0, 74, 173);
            panelTitleBar.Controls.Add(btnMin);
            panelTitleBar.Controls.Add(btnMax);
            panelTitleBar.Controls.Add(btnClose);
            panelTitleBar.Controls.Add(btnCloseChldForm);
            panelTitleBar.Controls.Add(lblTitle);
            panelTitleBar.Dock = DockStyle.Top;
            panelTitleBar.Location = new Point(220, 0);
            panelTitleBar.Name = "panelTitleBar";
            panelTitleBar.Size = new Size(872, 80);
            panelTitleBar.TabIndex = 2;
            panelTitleBar.MouseDown += panelTitleBar_MouseDown;
            // 
            // btnMin
            // 
            btnMin.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMin.FlatAppearance.BorderSize = 0;
            btnMin.FlatStyle = FlatStyle.Flat;
            btnMin.Font = new Font("Bookman Old Style", 15F, FontStyle.Regular, GraphicsUnit.Point);
            btnMin.ForeColor = Color.White;
            btnMin.Image = Properties.Resources.rec;
            btnMin.Location = new Point(767, 0);
            btnMin.Name = "btnMin";
            btnMin.Size = new Size(30, 37);
            btnMin.TabIndex = 7;
            btnMin.UseVisualStyleBackColor = true;
            btnMin.Click += btnMin_Click_1;
            // 
            // btnMax
            // 
            btnMax.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMax.FlatAppearance.BorderSize = 0;
            btnMax.FlatStyle = FlatStyle.Flat;
            btnMax.Font = new Font("Book Antiqua", 15F, FontStyle.Bold, GraphicsUnit.Point);
            btnMax.ForeColor = Color.White;
            btnMax.Image = Properties.Resources.rec;
            btnMax.Location = new Point(803, 0);
            btnMax.Name = "btnMax";
            btnMax.Size = new Size(30, 37);
            btnMax.TabIndex = 6;
            btnMax.UseVisualStyleBackColor = true;
            btnMax.Click += btnMax_Click;
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Font = new Font("Book Antiqua", 15F, FontStyle.Bold, GraphicsUnit.Point);
            btnClose.ForeColor = Color.White;
            btnClose.Image = Properties.Resources.rec;
            btnClose.Location = new Point(839, 0);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(30, 37);
            btnClose.TabIndex = 5;
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // btnCloseChldForm
            // 
            btnCloseChldForm.Dock = DockStyle.Left;
            btnCloseChldForm.FlatAppearance.BorderSize = 0;
            btnCloseChldForm.FlatStyle = FlatStyle.Flat;
            btnCloseChldForm.Image = (Image)resources.GetObject("btnCloseChldForm.Image");
            btnCloseChldForm.Location = new Point(0, 0);
            btnCloseChldForm.Name = "btnCloseChldForm";
            btnCloseChldForm.Size = new Size(59, 80);
            btnCloseChldForm.TabIndex = 1;
            btnCloseChldForm.UseVisualStyleBackColor = true;
            btnCloseChldForm.Click += btnCloseChldForm_Click;
            // 
            // lblTitle
            // 
            lblTitle.Anchor = AnchorStyles.None;
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Modern No. 20", 16.2F, FontStyle.Regular, GraphicsUnit.Point);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(370, 23);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(97, 30);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "HOME";
            // 
            // panelDesktop
            // 
            panelDesktop.Controls.Add(pictureBox1);
            panelDesktop.Dock = DockStyle.Fill;
            panelDesktop.Location = new Point(220, 80);
            panelDesktop.Name = "panelDesktop";
            panelDesktop.Size = new Size(872, 495);
            panelDesktop.TabIndex = 3;
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.None;
            pictureBox1.Image = Properties.Resources.laptop;
            pictureBox1.Location = new Point(126, 6);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(512, 512);
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // btnPodesavanja
            // 
            btnPodesavanja.Dock = DockStyle.Top;
            btnPodesavanja.FlatAppearance.BorderSize = 0;
            btnPodesavanja.FlatStyle = FlatStyle.Flat;
            btnPodesavanja.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point);
            btnPodesavanja.ForeColor = Color.Gainsboro;
            btnPodesavanja.Image = (Image)resources.GetObject("btnPodesavanja.Image");
            btnPodesavanja.ImageAlign = ContentAlignment.MiddleLeft;
            btnPodesavanja.Location = new Point(0, 320);
            btnPodesavanja.Name = "btnPodesavanja";
            btnPodesavanja.Padding = new Padding(12, 0, 0, 0);
            btnPodesavanja.Size = new Size(220, 60);
            btnPodesavanja.TabIndex = 7;
            btnPodesavanja.Text = "   Podesavanja";
            btnPodesavanja.TextAlign = ContentAlignment.MiddleLeft;
            btnPodesavanja.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnPodesavanja.UseVisualStyleBackColor = true;
            btnPodesavanja.Click += btnPodesavanja_Click;
            // 
            // Administrator
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1092, 575);
            Controls.Add(panelDesktop);
            Controls.Add(panelTitleBar);
            Controls.Add(panelMenu);
            Name = "Administrator";
            Text = "Administrator";
            panelMenu.ResumeLayout(false);
            panelLogo.ResumeLayout(false);
            panelLogo.PerformLayout();
            panelTitleBar.ResumeLayout(false);
            panelTitleBar.PerformLayout();
            panelDesktop.ResumeLayout(false);
            panelDesktop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelMenu;
        private Button btnLogOut;
        private Button btnStaff;
        private Button btnStore;
        private Panel panelLogo;
        private Label lbl;
        private Panel panelTitleBar;
        private Button btnCloseChldForm;
        private Label lblTitle;
        private Panel panelDesktop;
        private Button btnCustomers;
        private Button btnMin;
        private Button btnMax;
        private Button btnClose;
        private PictureBox pictureBox1;
        private Button btnAdmin;
        private Button btnPodesavanja;
    }
}