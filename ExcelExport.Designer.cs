namespace RSCC_GEN
{
    partial class ExcelExport
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
            this.components = new System.ComponentModel.Container();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.suffixBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.exampleLabel = new System.Windows.Forms.Label();
            this.prefixBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.includeName = new System.Windows.Forms.CheckBox();
            this.includeNo = new System.Windows.Forms.CheckBox();
            this.dGVDataBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dGVDataBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(246, 39);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(160, 24);
            this.comboBox1.TabIndex = 34;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(243, 19);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(59, 17);
            this.label11.TabIndex = 33;
            this.label11.Text = "View Set";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(243, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 17);
            this.label2.TabIndex = 35;
            this.label2.Text = "Select Sheets";
            // 
            // button5
            // 
            this.button5.FlatAppearance.BorderSize = 0;
            this.button5.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.button5.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button5.Location = new System.Drawing.Point(524, 402);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(93, 35);
            this.button5.TabIndex = 37;
            this.button5.Text = "Check None";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button4
            // 
            this.button4.FlatAppearance.BorderSize = 0;
            this.button4.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.button4.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Location = new System.Drawing.Point(436, 402);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 35);
            this.button4.TabIndex = 36;
            this.button4.Text = "Check All";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.button3.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button3.FlatAppearance.BorderSize = 0;
            this.button3.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Red;
            this.button3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Location = new System.Drawing.Point(165, 361);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 35);
            this.button3.TabIndex = 39;
            this.button3.Text = "Cancel";
            this.button3.UseVisualStyleBackColor = false;
            // 
            // button2
            // 
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.button2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Location = new System.Drawing.Point(13, 361);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 35);
            this.button2.TabIndex = 38;
            this.button2.Text = "OK";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(246, 104);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 26;
            this.dataGridView1.Size = new System.Drawing.Size(570, 292);
            this.dataGridView1.TabIndex = 40;
            this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellValueChanged);
            // 
            // suffixBox
            // 
            this.suffixBox.Location = new System.Drawing.Point(13, 167);
            this.suffixBox.Name = "suffixBox";
            this.suffixBox.Size = new System.Drawing.Size(195, 24);
            this.suffixBox.TabIndex = 45;
            this.suffixBox.TextChanged += new System.EventHandler(this.suffixBox_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 146);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 17);
            this.label3.TabIndex = 44;
            this.label3.Text = "Suffix";
            // 
            // exampleLabel
            // 
            this.exampleLabel.AutoSize = true;
            this.exampleLabel.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Italic);
            this.exampleLabel.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.exampleLabel.Location = new System.Drawing.Point(10, 296);
            this.exampleLabel.Name = "exampleLabel";
            this.exampleLabel.Size = new System.Drawing.Size(60, 14);
            this.exampleLabel.TabIndex = 43;
            this.exampleLabel.Text = "Example: ";
            // 
            // prefixBox
            // 
            this.prefixBox.Location = new System.Drawing.Point(13, 119);
            this.prefixBox.Name = "prefixBox";
            this.prefixBox.Size = new System.Drawing.Size(195, 24);
            this.prefixBox.TabIndex = 42;
            this.prefixBox.TextChanged += new System.EventHandler(this.prefixBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 98);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 17);
            this.label1.TabIndex = 41;
            this.label1.Text = "Prefix";
            // 
            // includeName
            // 
            this.includeName.AutoSize = true;
            this.includeName.Checked = true;
            this.includeName.CheckState = System.Windows.Forms.CheckState.Checked;
            this.includeName.Location = new System.Drawing.Point(13, 236);
            this.includeName.Name = "includeName";
            this.includeName.Size = new System.Drawing.Size(113, 21);
            this.includeName.TabIndex = 47;
            this.includeName.Text = "Include Name";
            this.includeName.UseVisualStyleBackColor = true;
            this.includeName.CheckedChanged += new System.EventHandler(this.includeName_CheckedChanged);
            // 
            // includeNo
            // 
            this.includeNo.AutoSize = true;
            this.includeNo.Checked = true;
            this.includeNo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.includeNo.Location = new System.Drawing.Point(13, 209);
            this.includeNo.Name = "includeNo";
            this.includeNo.Size = new System.Drawing.Size(99, 21);
            this.includeNo.TabIndex = 46;
            this.includeNo.Text = "Include No.";
            this.includeNo.UseVisualStyleBackColor = true;
            this.includeNo.CheckedChanged += new System.EventHandler(this.includeNo_CheckedChanged);
            // 
            // dGVDataBindingSource
            // 
            this.dGVDataBindingSource.DataSource = typeof(RSCC_GEN.DGVData);
            // 
            // ExcelExport
            // 
            this.AcceptButton = this.button2;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.CancelButton = this.button3;
            this.ClientSize = new System.Drawing.Size(883, 451);
            this.Controls.Add(this.includeName);
            this.Controls.Add(this.includeNo);
            this.Controls.Add(this.suffixBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.exampleLabel);
            this.Controls.Add(this.prefixBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label11);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ExcelExport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ExcelExport";
            this.Load += new System.EventHandler(this.ExcelExport_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dGVDataBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.ComboBox comboBox1;
        public System.Windows.Forms.Label label11;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.Button button5;
        public System.Windows.Forms.Button button4;
        public System.Windows.Forms.Button button3;
        public System.Windows.Forms.Button button2;
        public System.Windows.Forms.TextBox suffixBox;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.Label exampleLabel;
        public System.Windows.Forms.TextBox prefixBox;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.CheckBox includeName;
        public System.Windows.Forms.CheckBox includeNo;
        public System.Windows.Forms.DataGridView dataGridView1;
        public System.Windows.Forms.BindingSource dGVDataBindingSource;
    }
}