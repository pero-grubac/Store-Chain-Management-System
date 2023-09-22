namespace OnlineKupovinaGUI.HelperForms
{
    partial class StaffProducts
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
            tabPage5 = new TabPage();
            button1 = new Button();
            dgvAdrese = new DataGridView();
            label6 = new Label();
            txtBarKod = new TextBox();
            txtOpis = new TextBox();
            label7 = new Label();
            txtZalihe = new TextBox();
            label8 = new Label();
            txtIdProizvodjaca = new TextBox();
            label10 = new Label();
            txtCijena = new TextBox();
            label9 = new Label();
            tab.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            tabPage3.SuspendLayout();
            tabPage4.SuspendLayout();
            tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvAdrese).BeginInit();
            SuspendLayout();
            // 
            // tab
            // 
            tab.Controls.Add(tabPage5);
            tab.Size = new Size(732, 450);
            tab.Controls.SetChildIndex(tabPage5, 0);
            tab.Controls.SetChildIndex(tabPage4, 0);
            tab.Controls.SetChildIndex(tabPage3, 0);
            tab.Controls.SetChildIndex(tabPage2, 0);
            tab.Controls.SetChildIndex(tabPage1, 0);
            // 
            // tabPage1
            // 
            tabPage1.Size = new Size(724, 412);
            tabPage1.Text = "Proizvodi";
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(txtCijena);
            tabPage2.Controls.Add(label9);
            tabPage2.Controls.Add(txtIdProizvodjaca);
            tabPage2.Controls.Add(label10);
            tabPage2.Controls.Add(txtZalihe);
            tabPage2.Controls.Add(label8);
            tabPage2.Controls.Add(txtOpis);
            tabPage2.Controls.Add(label7);
            tabPage2.Controls.Add(txtBarKod);
            tabPage2.Controls.Add(label6);
            tabPage2.Text = "Dodaj proizvod";
            tabPage2.Controls.SetChildIndex(label1, 0);
            tabPage2.Controls.SetChildIndex(txt1, 0);
            tabPage2.Controls.SetChildIndex(btnSave, 0);
            tabPage2.Controls.SetChildIndex(label5, 0);
            tabPage2.Controls.SetChildIndex(txt2, 0);
            tabPage2.Controls.SetChildIndex(label6, 0);
            tabPage2.Controls.SetChildIndex(txtBarKod, 0);
            tabPage2.Controls.SetChildIndex(label7, 0);
            tabPage2.Controls.SetChildIndex(txtOpis, 0);
            tabPage2.Controls.SetChildIndex(label8, 0);
            tabPage2.Controls.SetChildIndex(txtZalihe, 0);
            tabPage2.Controls.SetChildIndex(label10, 0);
            tabPage2.Controls.SetChildIndex(txtIdProizvodjaca, 0);
            tabPage2.Controls.SetChildIndex(label9, 0);
            tabPage2.Controls.SetChildIndex(txtCijena, 0);
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(386, 349);
            btnDelete.Click += btnDelete_Click_1;
            // 
            // label1
            // 
            label1.Size = new Size(61, 25);
            label1.Text = "Naziv";
            // 
            // btnSave
            // 
            btnSave.Click += btnSave_Click_1;
            // 
            // btnReadAll
            // 
            btnReadAll.Click += btnReadAll_Click_1;
            // 
            // label5
            // 
            label5.Location = new Point(279, 61);
            label5.Size = new Size(125, 25);
            label5.Text = "Datum isteka";
            // 
            // tabPage3
            // 
            tabPage3.Text = "Proizvodjaci";
            // 
            // tabPage4
            // 
            tabPage4.Size = new Size(724, 412);
            tabPage4.Text = "Dodaj Proizvodjaca";
            // 
            // btnDeleteAdd
            // 
            btnDeleteAdd.Click += btnDeleteAdd_Click_1;
            // 
            // btnReadAllAdd
            // 
            btnReadAllAdd.Click += btnReadAllAdd_Click_1;
            // 
            // btnSaveAdd
            // 
            btnSaveAdd.Click += btnSaveAdd_Click_1;
            // 
            // txt5
            // 
            txt5.Location = new Point(399, 106);
            // 
            // label4
            // 
            label4.Location = new Point(399, 65);
            label4.Size = new Size(93, 25);
            label4.Text = "Id adresa";
            // 
            // txt4
            // 
            txt4.Location = new Point(226, 106);
            // 
            // label3
            // 
            label3.Location = new Point(226, 65);
            label3.Size = new Size(60, 25);
            label3.Text = "Email";
            // 
            // label2
            // 
            label2.Size = new Size(44, 25);
            label2.Text = "Ime";
            // 
            // tabPage5
            // 
            tabPage5.Controls.Add(button1);
            tabPage5.Controls.Add(dgvAdrese);
            tabPage5.Location = new Point(4, 34);
            tabPage5.Name = "tabPage5";
            tabPage5.Size = new Size(792, 412);
            tabPage5.TabIndex = 4;
            tabPage5.Text = "Adrese";
            tabPage5.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.BackColor = Color.FromArgb(0, 74, 173);
            button1.ForeColor = Color.White;
            button1.Location = new Point(29, 340);
            button1.Name = "button1";
            button1.Size = new Size(183, 44);
            button1.TabIndex = 1;
            button1.Text = "Osvjezi";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // dgvAdrese
            // 
            dgvAdrese.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dgvAdrese.BackgroundColor = Color.White;
            dgvAdrese.BorderStyle = BorderStyle.None;
            dgvAdrese.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvAdrese.Location = new Point(29, 31);
            dgvAdrese.Name = "dgvAdrese";
            dgvAdrese.ReadOnly = true;
            dgvAdrese.RowHeadersWidth = 51;
            dgvAdrese.RowTemplate.Height = 29;
            dgvAdrese.Size = new Size(730, 276);
            dgvAdrese.TabIndex = 0;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(494, 160);
            label6.Name = "label6";
            label6.Size = new Size(79, 25);
            label6.TabIndex = 13;
            label6.Text = "Bar kod";
            // 
            // txtBarKod
            // 
            txtBarKod.Location = new Point(494, 201);
            txtBarKod.Name = "txtBarKod";
            txtBarKod.Size = new Size(147, 30);
            txtBarKod.TabIndex = 14;
            // 
            // txtOpis
            // 
            txtOpis.Location = new Point(279, 201);
            txtOpis.Name = "txtOpis";
            txtOpis.Size = new Size(147, 30);
            txtOpis.TabIndex = 16;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(279, 160);
            label7.Name = "label7";
            label7.Size = new Size(53, 25);
            label7.TabIndex = 15;
            label7.Text = "Opis";
            // 
            // txtZalihe
            // 
            txtZalihe.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            txtZalihe.Location = new Point(58, 201);
            txtZalihe.Name = "txtZalihe";
            txtZalihe.Size = new Size(147, 30);
            txtZalihe.TabIndex = 18;
            // 
            // label8
            // 
            label8.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label8.AutoSize = true;
            label8.Location = new Point(58, 160);
            label8.Name = "label8";
            label8.Size = new Size(65, 25);
            label8.TabIndex = 17;
            label8.Text = "Zalihe";
            // 
            // txtIdProizvodjaca
            // 
            txtIdProizvodjaca.Location = new Point(494, 316);
            txtIdProizvodjaca.Name = "txtIdProizvodjaca";
            txtIdProizvodjaca.Size = new Size(147, 30);
            txtIdProizvodjaca.TabIndex = 22;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(494, 268);
            label10.Name = "label10";
            label10.Size = new Size(143, 25);
            label10.TabIndex = 21;
            label10.Text = "Id proizvodjaca";
            // 
            // txtCijena
            // 
            txtCijena.Location = new Point(494, 102);
            txtCijena.Name = "txtCijena";
            txtCijena.Size = new Size(147, 30);
            txtCijena.TabIndex = 24;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(494, 61);
            label9.Name = "label9";
            label9.Size = new Size(68, 25);
            label9.TabIndex = 23;
            label9.Text = "Cijena";
            // 
            // StaffProducts
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(732, 450);
            Name = "StaffProducts";
            Text = "Proizvodi";
            tab.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            tabPage3.ResumeLayout(false);
            tabPage4.ResumeLayout(false);
            tabPage4.PerformLayout();
            tabPage5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvAdrese).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TabPage tabPage5;
        private Button button1;
        private DataGridView dgvAdrese;
        private Label label6;
        private TextBox txtBarKod;
        private TextBox txtOpis;
        private Label label7;
        private TextBox txtZalihe;
        private Label label8;
        private TextBox txtIdProizvodjaca;
        private Label label10;
        private TextBox txtCijena;
        private Label label9;
    }
}