namespace OnlineKupovinaGUI.HelperForms
{
    partial class Staff
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
            label6 = new Label();
            txtLozinka = new TextBox();
            txtEmail = new TextBox();
            label7 = new Label();
            txtTelefon = new TextBox();
            label8 = new Label();
            txtPlata = new TextBox();
            label9 = new Label();
            txtIdProdavnice = new TextBox();
            label10 = new Label();
            txtKorisnickoIme = new TextBox();
            label11 = new Label();

            tab.TabPages.Remove(tabPage3);
            tab.TabPages.Remove(tabPage4);
            // 
            // tabPage1
            // 
            tabPage1.Text = "Zaposleni";
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(txtKorisnickoIme);
            tabPage2.Controls.Add(label11);
            tabPage2.Controls.Add(txtIdProdavnice);
            tabPage2.Controls.Add(label10);
            tabPage2.Controls.Add(txtPlata);
            tabPage2.Controls.Add(label9);
            tabPage2.Controls.Add(txtTelefon);
            tabPage2.Controls.Add(label8);
            tabPage2.Controls.Add(txtEmail);
            tabPage2.Controls.Add(label7);
            tabPage2.Controls.Add(txtLozinka);
            tabPage2.Controls.Add(label6);
            tabPage2.Text = "Dodavanje zaposelnog";
            tabPage2.Controls.SetChildIndex(label1, 0);
            tabPage2.Controls.SetChildIndex(txt1, 0);
            tabPage2.Controls.SetChildIndex(btnSave, 0);
            tabPage2.Controls.SetChildIndex(label5, 0);
            tabPage2.Controls.SetChildIndex(txt2, 0);
            tabPage2.Controls.SetChildIndex(label6, 0);
            tabPage2.Controls.SetChildIndex(txtLozinka, 0);
            tabPage2.Controls.SetChildIndex(label7, 0);
            tabPage2.Controls.SetChildIndex(txtEmail, 0);
            tabPage2.Controls.SetChildIndex(label8, 0);
            tabPage2.Controls.SetChildIndex(txtTelefon, 0);
            tabPage2.Controls.SetChildIndex(label9, 0);
            tabPage2.Controls.SetChildIndex(txtPlata, 0);
            tabPage2.Controls.SetChildIndex(label10, 0);
            tabPage2.Controls.SetChildIndex(txtIdProdavnice, 0);
            tabPage2.Controls.SetChildIndex(label11, 0);
            tabPage2.Controls.SetChildIndex(txtKorisnickoIme, 0);
            // 
            // btnDelete
            // 
            btnDelete.Click += btnDelete_Click_1;
            // 
            // label1
            // 
            label1.Size = new Size(44, 25);
            label1.Text = "Ime";
            // 
            // btnSave
            // 
            btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSave.Location = new Point(494, 298);
            btnSave.Click += btnSave_Click_1;
            // 
            // btnReadAll
            // 
            btnReadAll.Click += btnReadAll_Click;
            // 
            // txt2
            // 
            txt2.Anchor = AnchorStyles.Top;
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Top;
            label5.Size = new Size(83, 25);
            label5.Text = "Prezime";
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label6.AutoSize = true;
            label6.Location = new Point(494, 61);
            label6.Name = "label6";
            label6.Size = new Size(80, 25);
            label6.TabIndex = 13;
            label6.Text = "Lozinka";
            // 
            // txtLozinka
            // 
            txtLozinka.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txtLozinka.Location = new Point(494, 102);
            txtLozinka.Name = "txtLozinka";
            txtLozinka.Size = new Size(147, 30);
            txtLozinka.TabIndex = 14;
            // 
            // txtEmail
            // 
            txtEmail.Anchor = AnchorStyles.Right;
            txtEmail.Location = new Point(494, 202);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(147, 30);
            txtEmail.TabIndex = 16;
            // 
            // label7
            // 
            label7.Anchor = AnchorStyles.Right;
            label7.AutoSize = true;
            label7.Location = new Point(494, 161);
            label7.Name = "label7";
            label7.Size = new Size(60, 25);
            label7.TabIndex = 15;
            label7.Text = "Email";
            // 
            // txtTelefon
            // 
            txtTelefon.Anchor = AnchorStyles.None;
            txtTelefon.Location = new Point(278, 202);
            txtTelefon.Name = "txtTelefon";
            txtTelefon.Size = new Size(147, 30);
            txtTelefon.TabIndex = 18;
            // 
            // label8
            // 
            label8.Anchor = AnchorStyles.None;
            label8.AutoSize = true;
            label8.Location = new Point(278, 161);
            label8.Name = "label8";
            label8.Size = new Size(120, 25);
            label8.TabIndex = 17;
            label8.Text = "Broj telefona";
            // 
            // txtPlata
            // 
            txtPlata.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            txtPlata.Location = new Point(58, 312);
            txtPlata.Name = "txtPlata";
            txtPlata.Size = new Size(147, 30);
            txtPlata.TabIndex = 20;
            // 
            // label9
            // 
            label9.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label9.AutoSize = true;
            label9.Location = new Point(58, 271);
            label9.Name = "label9";
            label9.Size = new Size(56, 25);
            label9.TabIndex = 19;
            label9.Text = "Plata";
            // 
            // txtIdProdavnice
            // 
            txtIdProdavnice.Anchor = AnchorStyles.Bottom;
            txtIdProdavnice.Location = new Point(278, 312);
            txtIdProdavnice.Name = "txtIdProdavnice";
            txtIdProdavnice.Size = new Size(147, 30);
            txtIdProdavnice.TabIndex = 22;
            // 
            // label10
            // 
            label10.Anchor = AnchorStyles.Bottom;
            label10.AutoSize = true;
            label10.Location = new Point(278, 271);
            label10.Name = "label10";
            label10.Size = new Size(129, 25);
            label10.TabIndex = 21;
            label10.Text = "Id prodavnice";
            // 
            // txtKorisnickoIme
            // 
            txtKorisnickoIme.Anchor = AnchorStyles.Left;
            txtKorisnickoIme.Location = new Point(58, 202);
            txtKorisnickoIme.Name = "txtKorisnickoIme";
            txtKorisnickoIme.Size = new Size(147, 30);
            txtKorisnickoIme.TabIndex = 24;
            // 
            // label11
            // 
            label11.Anchor = AnchorStyles.Left;
            label11.AutoSize = true;
            label11.Location = new Point(58, 161);
            label11.Name = "label11";
            label11.Size = new Size(139, 25);
            label11.TabIndex = 23;
            label11.Text = "Korisnicko ime";
            // 
            // Staff
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Name = "Staff";
            Text = "Zaposleni";
            tab.ResumeLayout();
            tabPage1.ResumeLayout();
            tabPage2.ResumeLayout();
            tabPage2.PerformLayout();
            ResumeLayout();
        }

        #endregion
        private Label label6;
        private TextBox txtLozinka;
        private TextBox txtEmail;
        private Label label7;
        private TextBox txtPlata;
        private Label label9;
        private TextBox txtTelefon;
        private Label label8;
        private TextBox txtIdProdavnice;
        private Label label10;
        private TextBox txtKorisnickoIme;
        private Label label11;
    }
}