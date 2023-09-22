namespace OnlineKupovinaGUI.HelperForms
{
    partial class Admin
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
            txtIme = new TextBox();
            txtPrezime = new TextBox();
            label7 = new Label();
            txtEmail = new TextBox();
            label8 = new Label();
            tab.TabPages.Remove(tabPage3);
            tab.TabPages.Remove(tabPage4);
            // 
            // tabPage1
            // 
            tabPage1.Text = "Administratori";
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(txtEmail);
            tabPage2.Controls.Add(label8);
            tabPage2.Controls.Add(txtPrezime);
            tabPage2.Controls.Add(label7);
            tabPage2.Controls.Add(txtIme);
            tabPage2.Controls.Add(label6);
            tabPage2.Text = "Dodaj administratora";
            tabPage2.Controls.SetChildIndex(label1, 0);
            tabPage2.Controls.SetChildIndex(txt1, 0);
            tabPage2.Controls.SetChildIndex(btnSave, 0);
            tabPage2.Controls.SetChildIndex(label5, 0);
            tabPage2.Controls.SetChildIndex(txt2, 0);
            tabPage2.Controls.SetChildIndex(label6, 0);
            tabPage2.Controls.SetChildIndex(txtIme, 0);
            tabPage2.Controls.SetChildIndex(label7, 0);
            tabPage2.Controls.SetChildIndex(txtPrezime, 0);
            tabPage2.Controls.SetChildIndex(label8, 0);
            tabPage2.Controls.SetChildIndex(txtEmail, 0);
            // 
            // btnDelete
            // 
            btnDelete.Click += btnDelete_Click_1;
            // 
            // label1
            // 
            label1.Size = new Size(139, 25);
            label1.Text = "Korisnicko ime";
            // 
            // btnSave
            // 
            btnSave.Anchor = AnchorStyles.Bottom;
            btnSave.Click += btnSave_Click_1;
            // 
            // btnReadAll
            // 
            btnReadAll.Click += btnReadAll_Click_1;
            // 
            // txt2
            // 
            txt2.Anchor = AnchorStyles.Top;
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Top;
            label5.Size = new Size(80, 25);
            label5.Text = "Lozinka";
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label6.AutoSize = true;
            label6.Location = new Point(58, 154);
            label6.Name = "label6";
            label6.Size = new Size(44, 25);
            label6.TabIndex = 13;
            label6.Text = "Ime";
            // 
            // txtIme
            // 
            txtIme.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            txtIme.Location = new Point(58, 195);
            txtIme.Name = "txtIme";
            txtIme.Size = new Size(147, 30);
            txtIme.TabIndex = 14;
            // 
            // txtPrezime
            // 
            txtPrezime.Anchor = AnchorStyles.Bottom;
            txtPrezime.Location = new Point(278, 195);
            txtPrezime.Name = "txtPrezime";
            txtPrezime.Size = new Size(147, 30);
            txtPrezime.TabIndex = 16;
            // 
            // label7
            // 
            label7.Anchor = AnchorStyles.Bottom;
            label7.AutoSize = true;
            label7.Location = new Point(278, 154);
            label7.Name = "label7";
            label7.Size = new Size(83, 25);
            label7.TabIndex = 15;
            label7.Text = "Prezime";
            // 
            // txtEmail
            // 
            txtEmail.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txtEmail.Location = new Point(473, 102);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(147, 30);
            txtEmail.TabIndex = 18;
            // 
            // label8
            // 
            label8.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label8.AutoSize = true;
            label8.Location = new Point(473, 61);
            label8.Name = "label8";
            label8.Size = new Size(60, 25);
            label8.TabIndex = 17;
            label8.Text = "Email";
            // 
            // Admin
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Name = "Admin";
            Text = "Administrator";
            tab.ResumeLayout();
            tabPage2.ResumeLayout();
            tabPage2.PerformLayout();
            ResumeLayout();
        }

        #endregion

        private TextBox txtIme;
        private Label label6;
        private TextBox txtEmail;
        private Label label8;
        private TextBox txtPrezime;
        private Label label7;
    }
}