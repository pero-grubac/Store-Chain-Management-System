namespace OnlineKupovinaGUI.HelperForms
{
    partial class Stores
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
            tab = new TabControl();
            tabPage1 = new TabPage();
            btnReadAll = new Button();
            btnDelete = new Button();
            dataGridView = new DataGridView();
            tabPage2 = new TabPage();
            txt2 = new TextBox();
            label5 = new Label();
            btnSave = new Button();
            txt1 = new TextBox();
            label1 = new Label();
            tabPage3 = new TabPage();
            btnDeleteAdd = new Button();
            dgvAdd = new DataGridView();
            btnReadAllAdd = new Button();
            tabPage4 = new TabPage();
            btnSaveAdd = new Button();
            txt5 = new TextBox();
            label4 = new Label();
            txt4 = new TextBox();
            label3 = new Label();
            txt3 = new TextBox();
            label2 = new Label();
            tab.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView).BeginInit();
            tabPage2.SuspendLayout();
            tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvAdd).BeginInit();
            tabPage4.SuspendLayout();
            SuspendLayout();
            // 
            // tab
            // 
            tab.Controls.Add(tabPage1);
            tab.Controls.Add(tabPage2);
            tab.Controls.Add(tabPage3);
            tab.Controls.Add(tabPage4);
            tab.Dock = DockStyle.Fill;
            tab.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            tab.Location = new Point(0, 0);
            tab.Name = "tab";
            tab.SelectedIndex = 0;
            tab.Size = new Size(800, 450);
            tab.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(btnReadAll);
            tabPage1.Controls.Add(btnDelete);
            tabPage1.Controls.Add(dataGridView);
            tabPage1.Location = new Point(4, 34);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(792, 412);
            tabPage1.TabIndex = 0;
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnReadAll
            // 
            btnReadAll.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnReadAll.BackColor = Color.FromArgb(0, 74, 173);
            btnReadAll.FlatStyle = FlatStyle.Flat;
            btnReadAll.Location = new Point(24, 349);
            btnReadAll.Name = "btnReadAll";
            btnReadAll.Size = new Size(183, 44);
            btnReadAll.TabIndex = 5;
            btnReadAll.Text = "Osvjezi";
            btnReadAll.UseVisualStyleBackColor = false;
            btnReadAll.Click += btnReadAll_Click;
            // 
            // btnDelete
            // 
            btnDelete.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnDelete.BackColor = Color.FromArgb(0, 74, 173);
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.Location = new Point(590, 349);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(183, 44);
            btnDelete.TabIndex = 4;
            btnDelete.Text = "Brisanje";
            btnDelete.UseVisualStyleBackColor = false;
            btnDelete.Click += btnDelete_Click;
            // 
            // dataGridView
            // 
            dataGridView.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dataGridView.BackgroundColor = Color.White;
            dataGridView.BorderStyle = BorderStyle.None;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Location = new Point(24, 18);
            dataGridView.Name = "dataGridView";
            dataGridView.RowHeadersWidth = 51;
            dataGridView.RowTemplate.Height = 29;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.Size = new Size(749, 313);
            dataGridView.TabIndex = 1;
            dataGridView.CellBeginEdit += dataGridView_CellBeginEdit;
            dataGridView.CellEndEdit += dataGridView_CellEndEdit;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(txt2);
            tabPage2.Controls.Add(label5);
            tabPage2.Controls.Add(btnSave);
            tabPage2.Controls.Add(txt1);
            tabPage2.Controls.Add(label1);
            tabPage2.Location = new Point(4, 34);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(792, 412);
            tabPage2.TabIndex = 1;
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // txt2
            // 
            txt2.Location = new Point(278, 102);
            txt2.Name = "txt2";
            txt2.Size = new Size(147, 30);
            txt2.TabIndex = 12;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label5.Location = new Point(278, 61);
            label5.Name = "label5";
            label5.Size = new Size(0, 25);
            label5.TabIndex = 11;
            // 
            // btnSave
            // 
            btnSave.BackColor = Color.FromArgb(0, 74, 173);
            btnSave.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            btnSave.Location = new Point(127, 277);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(183, 44);
            btnSave.TabIndex = 9;
            btnSave.Text = "Sacuvaj";
            btnSave.UseVisualStyleBackColor = false;
            btnSave.Click += btnSave_Click;
            // 
            // txt1
            // 
            txt1.Location = new Point(58, 102);
            txt1.Name = "txt1";
            txt1.Size = new Size(147, 30);
            txt1.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(58, 61);
            label1.Name = "label1";
            label1.Size = new Size(0, 25);
            label1.TabIndex = 0;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(btnDeleteAdd);
            tabPage3.Controls.Add(dgvAdd);
            tabPage3.Controls.Add(btnReadAllAdd);
            tabPage3.Location = new Point(4, 34);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(792, 412);
            tabPage3.TabIndex = 2;
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // btnDeleteAdd
            // 
            btnDeleteAdd.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnDeleteAdd.BackColor = Color.FromArgb(0, 74, 173);
            btnDeleteAdd.Location = new Point(587, 342);
            btnDeleteAdd.Name = "btnDeleteAdd";
            btnDeleteAdd.Size = new Size(183, 44);
            btnDeleteAdd.TabIndex = 8;
            btnDeleteAdd.Text = "Brisanje";
            btnDeleteAdd.UseVisualStyleBackColor = false;
            btnDeleteAdd.Click += btnDeleteAdd_Click;
            // 
            // dgvAdd
            // 
            dgvAdd.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dgvAdd.BackgroundColor = Color.White;
            dgvAdd.BorderStyle = BorderStyle.None;
            dgvAdd.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvAdd.Location = new Point(21, 23);
            dgvAdd.Name = "dgvAdd";
            dgvAdd.RowHeadersWidth = 51;
            dgvAdd.RowTemplate.Height = 29;
            dgvAdd.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAdd.Size = new Size(749, 313);
            dgvAdd.TabIndex = 7;
            dgvAdd.CellBeginEdit += dgvAdd_CellBeginEdit;
            dgvAdd.CellEndEdit += dgvAdd_CellEndEdit;
            // 
            // btnReadAllAdd
            // 
            btnReadAllAdd.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnReadAllAdd.BackColor = Color.FromArgb(0, 74, 173);
            btnReadAllAdd.Location = new Point(21, 342);
            btnReadAllAdd.Name = "btnReadAllAdd";
            btnReadAllAdd.Size = new Size(183, 44);
            btnReadAllAdd.TabIndex = 6;
            btnReadAllAdd.Text = "Osvjezi";
            btnReadAllAdd.UseVisualStyleBackColor = false;
            btnReadAllAdd.Click += btnReadAllAdd_Click;
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(btnSaveAdd);
            tabPage4.Controls.Add(txt5);
            tabPage4.Controls.Add(label4);
            tabPage4.Controls.Add(txt4);
            tabPage4.Controls.Add(label3);
            tabPage4.Controls.Add(txt3);
            tabPage4.Controls.Add(label2);
            tabPage4.Location = new Point(4, 34);
            tabPage4.Name = "tabPage4";
            tabPage4.Size = new Size(792, 412);
            tabPage4.TabIndex = 3;
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // btnSaveAdd
            // 
            btnSaveAdd.BackColor = Color.FromArgb(0, 74, 173);
            btnSaveAdd.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            btnSaveAdd.Location = new Point(226, 240);
            btnSaveAdd.Name = "btnSaveAdd";
            btnSaveAdd.Size = new Size(183, 44);
            btnSaveAdd.TabIndex = 10;
            btnSaveAdd.Text = "Sacuvaj";
            btnSaveAdd.UseVisualStyleBackColor = false;
            btnSaveAdd.Click += btnSaveAdd_Click;
            // 
            // txt5
            // 
            txt5.Location = new Point(437, 106);
            txt5.Name = "txt5";
            txt5.Size = new Size(147, 30);
            txt5.TabIndex = 7;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label4.Location = new Point(437, 65);
            label4.Name = "label4";
            label4.Size = new Size(0, 25);
            label4.TabIndex = 6;
            // 
            // txt4
            // 
            txt4.Location = new Point(243, 106);
            txt4.Name = "txt4";
            txt4.Size = new Size(147, 30);
            txt4.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label3.Location = new Point(243, 65);
            label3.Name = "label3";
            label3.Size = new Size(0, 25);
            label3.TabIndex = 4;
            // 
            // txt3
            // 
            txt3.Location = new Point(46, 106);
            txt3.Name = "txt3";
            txt3.Size = new Size(147, 30);
            txt3.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label2.Location = new Point(46, 65);
            label2.Name = "label2";
            label2.Size = new Size(0, 25);
            label2.TabIndex = 2;
            // 
            // Stores
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(tab);
            Name = "Stores";
            tab.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView).EndInit();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvAdd).EndInit();
            tabPage4.ResumeLayout(false);
            tabPage4.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        public TabControl tab;
        public TabPage tabPage1;
        public TabPage tabPage2;
        public DataGridView dataGridView;
        public Button btnDelete;
        public Label label1;
        public TextBox txt1;
        public Button btnSave;
        public Button btnReadAll;
        public TextBox txt2;
        public Label label5;
        public TabPage? tabPage3;
        public TabPage? tabPage4;
        public Button btnDeleteAdd;
        public DataGridView dgvAdd;
        public Button btnReadAllAdd;
        public Button btnSaveAdd;
        public TextBox txt5;
        public Label label4;
        public TextBox txt4;
        public Label label3;
        public TextBox txt3;
        public Label label2;
    }
}