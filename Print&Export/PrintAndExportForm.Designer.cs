namespace RSCC_GEN
{
    partial class PrintAndExportForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.prefixBox = new System.Windows.Forms.TextBox();
            this.exampleLabel = new System.Windows.Forms.Label();
            this.suffixBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.includeSheetNo = new System.Windows.Forms.CheckBox();
            this.includeSheetName = new System.Windows.Forms.CheckBox();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.location = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.pdfex = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cadex = new System.Windows.Forms.CheckBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.comboBox5 = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.merge = new System.Windows.Forms.CheckBox();
            this.xlsxEx = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Prefix";
            // 
            // prefixBox
            // 
            this.prefixBox.Location = new System.Drawing.Point(38, 34);
            this.prefixBox.Name = "prefixBox";
            this.prefixBox.Size = new System.Drawing.Size(428, 22);
            this.prefixBox.TabIndex = 1;
            this.prefixBox.TextChanged += new System.EventHandler(this.prefixBox_TextChanged);
            // 
            // exampleLabel
            // 
            this.exampleLabel.AutoSize = true;
            this.exampleLabel.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Italic);
            this.exampleLabel.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.exampleLabel.Location = new System.Drawing.Point(14, 182);
            this.exampleLabel.Name = "exampleLabel";
            this.exampleLabel.Size = new System.Drawing.Size(60, 14);
            this.exampleLabel.TabIndex = 3;
            this.exampleLabel.Text = "Example: ";
            // 
            // suffixBox
            // 
            this.suffixBox.Location = new System.Drawing.Point(38, 82);
            this.suffixBox.Name = "suffixBox";
            this.suffixBox.Size = new System.Drawing.Size(428, 22);
            this.suffixBox.TabIndex = 5;
            this.suffixBox.TextChanged += new System.EventHandler(this.suffixBox_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(34, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Suffix";
            // 
            // includeSheetNo
            // 
            this.includeSheetNo.AutoSize = true;
            this.includeSheetNo.Checked = true;
            this.includeSheetNo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.includeSheetNo.Location = new System.Drawing.Point(38, 120);
            this.includeSheetNo.Name = "includeSheetNo";
            this.includeSheetNo.Size = new System.Drawing.Size(134, 20);
            this.includeSheetNo.TabIndex = 6;
            this.includeSheetNo.Text = "Include Sheet No.";
            this.includeSheetNo.UseVisualStyleBackColor = true;
            this.includeSheetNo.CheckedChanged += new System.EventHandler(this.includeSheetNo_CheckedChanged);
            // 
            // includeSheetName
            // 
            this.includeSheetName.AutoSize = true;
            this.includeSheetName.Checked = true;
            this.includeSheetName.CheckState = System.Windows.Forms.CheckState.Checked;
            this.includeSheetName.Location = new System.Drawing.Point(38, 147);
            this.includeSheetName.Name = "includeSheetName";
            this.includeSheetName.Size = new System.Drawing.Size(150, 20);
            this.includeSheetName.TabIndex = 7;
            this.includeSheetName.Text = "Include Sheet Name";
            this.includeSheetName.UseVisualStyleBackColor = true;
            this.includeSheetName.CheckedChanged += new System.EventHandler(this.includeSheetName_CheckedChanged);
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(509, 98);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(546, 242);
            this.checkedListBox1.Sorted = true;
            this.checkedListBox1.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(506, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 16);
            this.label2.TabIndex = 9;
            this.label2.Text = "Select Sheets";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(34, 318);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(93, 16);
            this.label8.TabIndex = 20;
            this.label8.Text = "Save Location";
            // 
            // location
            // 
            this.location.Location = new System.Drawing.Point(38, 338);
            this.location.Name = "location";
            this.location.Size = new System.Drawing.Size(324, 22);
            this.location.TabIndex = 21;
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(392, 334);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(86, 31);
            this.button1.TabIndex = 22;
            this.button1.Text = "Browse";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // pdfex
            // 
            this.pdfex.AutoSize = true;
            this.pdfex.Checked = true;
            this.pdfex.CheckState = System.Windows.Forms.CheckState.Checked;
            this.pdfex.Location = new System.Drawing.Point(102, 372);
            this.pdfex.Name = "pdfex";
            this.pdfex.Size = new System.Drawing.Size(56, 20);
            this.pdfex.TabIndex = 23;
            this.pdfex.Text = "PDF";
            this.pdfex.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(34, 373);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(45, 16);
            this.label9.TabIndex = 24;
            this.label9.Text = "Export";
            // 
            // cadex
            // 
            this.cadex.AutoSize = true;
            this.cadex.Checked = true;
            this.cadex.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cadex.Location = new System.Drawing.Point(187, 372);
            this.cadex.Name = "cadex";
            this.cadex.Size = new System.Drawing.Size(57, 20);
            this.cadex.TabIndex = 25;
            this.cadex.Text = "CAD";
            this.cadex.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.button2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Location = new System.Drawing.Point(47, 415);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(86, 35);
            this.button2.TabIndex = 26;
            this.button2.Text = "OK";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.button3.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button3.FlatAppearance.BorderSize = 0;
            this.button3.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Red;
            this.button3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Location = new System.Drawing.Point(368, 415);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(86, 35);
            this.button3.TabIndex = 27;
            this.button3.Text = "Cancel";
            this.button3.UseVisualStyleBackColor = false;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(619, 407);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(277, 48);
            this.label10.TabIndex = 28;
            this.label10.Text = "© Omar Elshafey | 2024\r\n@ RSCC Rawabi Specialized Contracting Co.\r\nDo not distrib" +
    "ute without permision.";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button4
            // 
            this.button4.FlatAppearance.BorderSize = 0;
            this.button4.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.button4.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Location = new System.Drawing.Point(685, 355);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(86, 35);
            this.button4.TabIndex = 29;
            this.button4.Text = "Check All";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.FlatAppearance.BorderSize = 0;
            this.button5.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.button5.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button5.Location = new System.Drawing.Point(785, 355);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(106, 35);
            this.button5.TabIndex = 30;
            this.button5.Text = "Check None";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // comboBox5
            // 
            this.comboBox5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBox5.FormattingEnabled = true;
            this.comboBox5.Location = new System.Drawing.Point(509, 34);
            this.comboBox5.Name = "comboBox5";
            this.comboBox5.Size = new System.Drawing.Size(182, 24);
            this.comboBox5.TabIndex = 32;
            this.comboBox5.SelectedIndexChanged += new System.EventHandler(this.comboBox5_SelectedIndexChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(505, 14);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(59, 16);
            this.label11.TabIndex = 31;
            this.label11.Text = "View Set";
            // 
            // comboBox3
            // 
            this.comboBox3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Location = new System.Drawing.Point(38, 227);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(182, 24);
            this.comboBox3.TabIndex = 34;
            this.comboBox3.SelectedIndexChanged += new System.EventHandler(this.comboBox3_SelectedIndexChanged_1);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(34, 206);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(143, 16);
            this.label6.TabIndex = 33;
            this.label6.Text = "* Export Settings (CAD)";
            // 
            // merge
            // 
            this.merge.AutoSize = true;
            this.merge.Checked = true;
            this.merge.CheckState = System.Windows.Forms.CheckState.Checked;
            this.merge.Location = new System.Drawing.Point(272, 227);
            this.merge.Name = "merge";
            this.merge.Size = new System.Drawing.Size(117, 20);
            this.merge.TabIndex = 35;
            this.merge.Text = "Merge in DWG";
            this.merge.UseVisualStyleBackColor = true;
            this.merge.CheckedChanged += new System.EventHandler(this.merge_CheckedChanged);
            // 
            // xlsxEx
            // 
            this.xlsxEx.AutoSize = true;
            this.xlsxEx.Checked = true;
            this.xlsxEx.CheckState = System.Windows.Forms.CheckState.Checked;
            this.xlsxEx.Location = new System.Drawing.Point(272, 369);
            this.xlsxEx.Name = "xlsxEx";
            this.xlsxEx.Size = new System.Drawing.Size(62, 20);
            this.xlsxEx.TabIndex = 36;
            this.xlsxEx.Text = "Excel";
            this.xlsxEx.UseVisualStyleBackColor = true;
            // 
            // PrintAndExportForm
            // 
            this.AcceptButton = this.button2;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.CancelButton = this.button3;
            this.ClientSize = new System.Drawing.Size(1087, 474);
            this.Controls.Add(this.xlsxEx);
            this.Controls.Add(this.merge);
            this.Controls.Add(this.comboBox3);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.comboBox5);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.cadex);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.pdfex);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.location);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkedListBox1);
            this.Controls.Add(this.includeSheetName);
            this.Controls.Add(this.includeSheetNo);
            this.Controls.Add(this.suffixBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.exampleLabel);
            this.Controls.Add(this.prefixBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PrintAndExportForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Print & Export CAD";
            this.Load += new System.EventHandler(this.PrintAndExportForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label exampleLabel;
        public System.Windows.Forms.TextBox suffixBox;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.CheckBox includeSheetNo;
        public System.Windows.Forms.CheckBox includeSheetName;
        public System.Windows.Forms.CheckedListBox checkedListBox1;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.Label label8;
        public System.Windows.Forms.TextBox location;
        public System.Windows.Forms.Button button1;
        public System.Windows.Forms.CheckBox pdfex;
        public System.Windows.Forms.Label label9;
        public System.Windows.Forms.CheckBox cadex;
        public System.Windows.Forms.Button button2;
        public System.Windows.Forms.Button button3;
        public System.Windows.Forms.Label label10;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox prefixBox;
        public System.Windows.Forms.Button button4;
        public System.Windows.Forms.Button button5;
        public System.Windows.Forms.ComboBox comboBox5;
        public System.Windows.Forms.Label label11;
        public System.Windows.Forms.ComboBox comboBox3;
        public System.Windows.Forms.Label label6;
        public System.Windows.Forms.CheckBox merge;
        public System.Windows.Forms.CheckBox xlsxEx;
    }
}