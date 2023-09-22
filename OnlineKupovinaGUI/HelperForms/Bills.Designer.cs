namespace OnlineKupovinaGUI.HelperForms
{
    partial class Bills
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            dgvBill = new DataGridView();
            btnReadBill = new Button();
            dgvUkupnePare = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)dgvBill).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvUkupnePare).BeginInit();
            SuspendLayout();
            // 
            // dgvBill
            // 
            dgvBill.AllowUserToAddRows = false;
            dgvBill.AllowUserToDeleteRows = false;
            dgvBill.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dgvBill.BackgroundColor = Color.White;
            dgvBill.BorderStyle = BorderStyle.None;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dgvBill.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dgvBill.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = SystemColors.Window;
            dataGridViewCellStyle2.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            dgvBill.DefaultCellStyle = dataGridViewCellStyle2;
            dgvBill.Location = new Point(12, 12);
            dgvBill.Name = "dgvBill";
            dgvBill.ReadOnly = true;
            dgvBill.RowHeadersWidth = 51;
            dgvBill.RowTemplate.Height = 29;
            dgvBill.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBill.Size = new Size(837, 179);
            dgvBill.TabIndex = 2;
            // 
            // btnReadBill
            // 
            btnReadBill.BackColor = Color.FromArgb(0, 74, 173);
            btnReadBill.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            btnReadBill.ForeColor = Color.White;
            btnReadBill.Location = new Point(538, 363);
            btnReadBill.Name = "btnReadBill";
            btnReadBill.Size = new Size(183, 44);
            btnReadBill.TabIndex = 3;
            btnReadBill.Text = "Osvjezi";
            btnReadBill.UseVisualStyleBackColor = false;
            btnReadBill.Click += btnReadBill_Click;
            // 
            // dgvUkupnePare
            // 
            dgvUkupnePare.AllowUserToAddRows = false;
            dgvUkupnePare.AllowUserToDeleteRows = false;
            dgvUkupnePare.BackgroundColor = Color.White;
            dgvUkupnePare.BorderStyle = BorderStyle.None;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = SystemColors.Control;
            dataGridViewCellStyle3.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle3.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.True;
            dgvUkupnePare.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            dgvUkupnePare.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = SystemColors.Window;
            dataGridViewCellStyle4.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle4.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.False;
            dgvUkupnePare.DefaultCellStyle = dataGridViewCellStyle4;
            dgvUkupnePare.Location = new Point(22, 279);
            dgvUkupnePare.Name = "dgvUkupnePare";
            dgvUkupnePare.ReadOnly = true;
            dgvUkupnePare.RowHeadersWidth = 51;
            dgvUkupnePare.RowTemplate.Height = 29;
            dgvUkupnePare.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUkupnePare.Size = new Size(423, 148);
            dgvUkupnePare.TabIndex = 4;
            // 
            // Bills
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(878, 450);
            Controls.Add(dgvUkupnePare);
            Controls.Add(btnReadBill);
            Controls.Add(dgvBill);
            Name = "Bills";
            Text = "Racuni";
            ((System.ComponentModel.ISupportInitialize)dgvBill).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvUkupnePare).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dgvBill;
        private Button btnReadBill;
        private DataGridView dgvUkupnePare;
    }
}