namespace OnlineKupovinaGUI.HelperForms
{
    public partial class AdminStore : Stores
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
        protected void InitializeComponent()
        {
            tab.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            tabPage3.SuspendLayout();
            tabPage4.SuspendLayout();
            SuspendLayout();
            // 
            // tabPage1
            // 
            tabPage1.Text = "Prodavnice";
            // 
            // tabPage2
            // 
            tabPage2.Text = "Kreiranje prodavnice";
            // 
            // label1
            // 
            label1.Size = new Size(145, 25);
            label1.Text = "Ime prodavnice";
            // 
            // label5
            // 
            label5.Size = new Size(93, 25);
            label5.Text = "Id adrese";
            // 
            // tabPage3
            // 
            tabPage3.Text = "Adrese";
            // 
            // tabPage4
            // 
            tabPage4.Text = "Kreiranje adrese";
            // 
            // label4
            // 
            label4.Size = new Size(55, 25);
            label4.Text = "Ulica";
            // 
            // label3
            // 
            label3.Size = new Size(55, 25);
            label3.Text = "Grad";
            // 
            // label2
            // 
            label2.Size = new Size(74, 25);
            label2.Text = "Drzava";
            // 
            // AdminStore
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Name = "AdminStore";
            Text = "AdminStore";
            tab.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            tabPage3.ResumeLayout(false);
            tabPage4.ResumeLayout(false);
            tabPage4.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
    }
}