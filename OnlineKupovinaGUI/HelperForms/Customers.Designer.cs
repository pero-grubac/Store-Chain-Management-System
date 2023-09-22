namespace OnlineKupovinaGUI.HelperForms
{
    partial class Customers
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
            label7 = new Label();
            txtEmail = new TextBox();
            txtIdAdrese = new TextBox();

            tab.TabPages.Remove(tabPage3);
            tab.TabPages.Remove(tabPage4);
            // 
            // tabPage1
            // 
            tabPage1.Text = "Kupci";
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(txtIdAdrese);
            tabPage2.Controls.Add(txtEmail);
            tabPage2.Controls.Add(label7);
            tabPage2.Controls.Add(label6);
            tabPage2.Text = "Dodaj kupca";
            tabPage2.Controls.SetChildIndex(label1, 0);
            tabPage2.Controls.SetChildIndex(txt1, 0);
            tabPage2.Controls.SetChildIndex(btnSave, 0);
            tabPage2.Controls.SetChildIndex(label5, 0);
            tabPage2.Controls.SetChildIndex(txt2, 0);
            tabPage2.Controls.SetChildIndex(label6, 0);
            tabPage2.Controls.SetChildIndex(label7, 0);
            tabPage2.Controls.SetChildIndex(txtEmail, 0);
            tabPage2.Controls.SetChildIndex(txtIdAdrese, 0);
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
            btnSave.Click += btnSave_Click_1;
            // 
            // btnReadAll
            // 
            btnReadAll.Click += btnReadAll_Click_1;
            // 
            // txt2
            // 
            txt2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label5.Size = new Size(80, 25);
            label5.Text = "Lozinka";
            // 
            // txt5
            // 
            txt5.Size = new Size(147, 27);
            // 
            // txt4
            // 
            txt4.Size = new Size(147, 27);
            // 
            // txt3
            // 
            txt3.Size = new Size(147, 27);
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label6.AutoSize = true;
            label6.Location = new Point(58, 159);
            label6.Name = "label6";
            label6.Size = new Size(60, 25);
            label6.TabIndex = 13;
            label6.Text = "Email";
            // 
            // label7
            // 
            label7.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            label7.AutoSize = true;
            label7.Location = new Point(278, 159);
            label7.Name = "label7";
            label7.Size = new Size(93, 25);
            label7.TabIndex = 14;
            label7.Text = "Id adrese";
            // 
            // txtEmail
            // 
            txtEmail.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            txtEmail.Location = new Point(58, 200);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(147, 30);
            txtEmail.TabIndex = 15;
            // 
            // txtIdAdrese
            // 
            txtIdAdrese.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            txtIdAdrese.Location = new Point(278, 200);
            txtIdAdrese.Name = "txtIdAdrese";
            txtIdAdrese.Size = new Size(147, 30);
            txtIdAdrese.TabIndex = 16;
            // 
            // Customers
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Name = "Customers";
            Text = "Kupci";
            tab.ResumeLayout();
            tabPage1.ResumeLayout();
            tabPage2.ResumeLayout();
            tabPage2.PerformLayout();
            ResumeLayout();
        }
        #endregion

        private TextBox txtEmail;
        private Label label7;
        private Label label6;
        private TextBox txtIdAdrese;
    }
}