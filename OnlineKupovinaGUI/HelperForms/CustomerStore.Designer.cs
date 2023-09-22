namespace OnlineKupovinaGUI.HelperForms
{
    partial class CustomerStore
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
            btnSelectStore = new Button();
            dgvStore = new DataGridView();
            btnReadStore = new Button();
            tab = new TabControl();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            btnBuy = new Button();
            label1 = new Label();
            txt = new TextBox();
            btnOrder = new Button();
            dgvProducts = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)dgvStore).BeginInit();
            tab.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvProducts).BeginInit();
            SuspendLayout();
            // 
            // btnSelectStore
            // 
            btnSelectStore.BackColor = Color.FromArgb(0, 74, 173);
            btnSelectStore.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            btnSelectStore.ForeColor = Color.White;
            btnSelectStore.Location = new Point(601, 331);
            btnSelectStore.Name = "btnSelectStore";
            btnSelectStore.Size = new Size(183, 44);
            btnSelectStore.TabIndex = 0;
            btnSelectStore.Text = "Izaberi";
            btnSelectStore.UseVisualStyleBackColor = false;
            btnSelectStore.Click += btnSelectStore_Click;
            // 
            // dgvStore
            // 
            dgvStore.BackgroundColor = Color.White;
            dgvStore.BorderStyle = BorderStyle.None;
            dgvStore.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvStore.Location = new Point(25, 6);
            dgvStore.Name = "dgvStore";
            dgvStore.ReadOnly = true;
            dgvStore.RowHeadersWidth = 51;
            dgvStore.RowTemplate.Height = 29;
            dgvStore.Size = new Size(759, 280);
            dgvStore.TabIndex = 1;
            // 
            // btnReadStore
            // 
            btnReadStore.BackColor = Color.FromArgb(0, 74, 173);
            btnReadStore.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            btnReadStore.ForeColor = Color.White;
            btnReadStore.Location = new Point(25, 331);
            btnReadStore.Name = "btnReadStore";
            btnReadStore.Size = new Size(183, 44);
            btnReadStore.TabIndex = 2;
            btnReadStore.Text = "Osvjezi";
            btnReadStore.UseVisualStyleBackColor = false;
            btnReadStore.Click += btnReadStore_Click;
            // 
            // tab
            // 
            tab.Controls.Add(tabPage1);
            tab.Controls.Add(tabPage2);
            tab.Dock = DockStyle.Fill;
            tab.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            tab.Location = new Point(0, 0);
            tab.Name = "tab";
            tab.SelectedIndex = 0;
            tab.Size = new Size(800, 450);
            tab.TabIndex = 3;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(btnReadStore);
            tabPage1.Controls.Add(dgvStore);
            tabPage1.Controls.Add(btnSelectStore);
            tabPage1.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            tabPage1.Location = new Point(4, 34);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(792, 412);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Prodavnice";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(btnBuy);
            tabPage2.Controls.Add(label1);
            tabPage2.Controls.Add(txt);
            tabPage2.Controls.Add(btnOrder);
            tabPage2.Controls.Add(dgvProducts);
            tabPage2.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            tabPage2.Location = new Point(4, 34);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(792, 412);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Proizvodi";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnBuy
            // 
            btnBuy.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBuy.BackColor = Color.FromArgb(0, 74, 173);
            btnBuy.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            btnBuy.ForeColor = Color.White;
            btnBuy.Location = new Point(28, 334);
            btnBuy.Name = "btnBuy";
            btnBuy.Size = new Size(183, 44);
            btnBuy.TabIndex = 6;
            btnBuy.Text = "Kupi";
            btnBuy.UseVisualStyleBackColor = false;
            btnBuy.Click += btnBuy_Click;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new Point(308, 344);
            label1.Name = "label1";
            label1.Size = new Size(81, 25);
            label1.TabIndex = 5;
            label1.Text = "Kolicina";
            // 
            // txt
            // 
            txt.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txt.Location = new Point(400, 344);
            txt.Name = "txt";
            txt.Size = new Size(183, 30);
            txt.TabIndex = 4;
            // 
            // btnOrder
            // 
            btnOrder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOrder.BackColor = Color.FromArgb(0, 74, 173);
            btnOrder.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            btnOrder.ForeColor = Color.White;
            btnOrder.Location = new Point(601, 337);
            btnOrder.Name = "btnOrder";
            btnOrder.Size = new Size(183, 44);
            btnOrder.TabIndex = 3;
            btnOrder.Text = "Stavi u korpu";
            btnOrder.UseVisualStyleBackColor = false;
            btnOrder.Click += btnOrder_Click;
            // 
            // dgvProducts
            // 
            dgvProducts.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvProducts.BackgroundColor = Color.White;
            dgvProducts.BorderStyle = BorderStyle.None;
            dgvProducts.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvProducts.Location = new Point(8, 16);
            dgvProducts.Name = "dgvProducts";
            dgvProducts.ReadOnly = true;
            dgvProducts.RowHeadersWidth = 51;
            dgvProducts.RowTemplate.Height = 29;
            dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProducts.Size = new Size(759, 280);
            dgvProducts.TabIndex = 2;
            // 
            // CustomerStore
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(800, 450);
            Controls.Add(tab);
            Name = "CustomerStore";
            Text = "CustomerStore";
            ((System.ComponentModel.ISupportInitialize)dgvStore).EndInit();
            tab.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvProducts).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button btnSelectStore;
        private DataGridView dgvStore;
        private Button btnReadStore;
        private TabControl tab;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TextBox txt;
        private Button btnOrder;
        private DataGridView dgvProducts;
        private Label label1;
        private Button btnBuy;
    }
}