namespace OnlineKupovinaGUI.HelperForms
{
    partial class Settings
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
            txtNewPass = new TextBox();
            label2 = new Label();
            txtNewPass2 = new TextBox();
            label3 = new Label();
            btnSave = new Button();
            SuspendLayout();
            // 
            // txtNewPass
            // 
            txtNewPass.Location = new Point(196, 125);
            txtNewPass.Name = "txtNewPass";
            txtNewPass.PasswordChar = '*';
            txtNewPass.Size = new Size(125, 27);
            txtNewPass.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label2.Location = new Point(50, 124);
            label2.Name = "label2";
            label2.Size = new Size(124, 25);
            label2.TabIndex = 2;
            label2.Text = "Nova lozinka";
            // 
            // txtNewPass2
            // 
            txtNewPass2.Location = new Point(196, 178);
            txtNewPass2.Name = "txtNewPass2";
            txtNewPass2.PasswordChar = '*';
            txtNewPass2.Size = new Size(125, 27);
            txtNewPass2.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label3.Location = new Point(50, 177);
            label3.Name = "label3";
            label3.Size = new Size(124, 25);
            label3.TabIndex = 4;
            label3.Text = "Nova lozinka";
            // 
            // btnSave
            // 
            btnSave.BackColor = Color.FromArgb(0, 74, 173);
            btnSave.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            btnSave.Location = new Point(138, 254);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(183, 44);
            btnSave.TabIndex = 6;
            btnSave.Text = "Sacuvaj";
            btnSave.UseVisualStyleBackColor = false;
            btnSave.Click += btnSave_Click;
            // 
            // Settings
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnSave);
            Controls.Add(txtNewPass2);
            Controls.Add(label3);
            Controls.Add(txtNewPass);
            Controls.Add(label2);
            Name = "Settings";
            Text = "Podesavanja";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox txtNewPass;
        private Label label2;
        private TextBox txtNewPass2;
        private Label label3;
        private Button btnSave;
    }
}