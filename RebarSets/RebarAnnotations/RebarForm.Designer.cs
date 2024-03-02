namespace RSCC_GEN.RebarAnnotations
{
    partial class RebarForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tagCombo = new System.Windows.Forms.ComboBox();
            this.viewTemplatesCombo = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.ballCombo = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(280, 57);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(673, 365);
            this.panel1.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(748, 439);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 35);
            this.button1.TabIndex = 1;
            this.button1.Text = "Select";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.RosyBrown;
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Location = new System.Drawing.Point(842, 439);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 35);
            this.button2.TabIndex = 2;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(284, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(204, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "Filter Rebar and Name Views By:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "Select Tag Type:";
            // 
            // tagCombo
            // 
            this.tagCombo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.tagCombo.FormattingEnabled = true;
            this.tagCombo.Location = new System.Drawing.Point(12, 65);
            this.tagCombo.Name = "tagCombo";
            this.tagCombo.Size = new System.Drawing.Size(258, 24);
            this.tagCombo.TabIndex = 5;
            // 
            // viewTemplatesCombo
            // 
            this.viewTemplatesCombo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.viewTemplatesCombo.FormattingEnabled = true;
            this.viewTemplatesCombo.Location = new System.Drawing.Point(12, 121);
            this.viewTemplatesCombo.Name = "viewTemplatesCombo";
            this.viewTemplatesCombo.Size = new System.Drawing.Size(258, 24);
            this.viewTemplatesCombo.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 101);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(140, 17);
            this.label3.TabIndex = 6;
            this.label3.Text = "Select View Template:";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(15, 162);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(164, 21);
            this.checkBox1.TabIndex = 8;
            this.checkBox1.Text = "Include Dimension Ball";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // ballCombo
            // 
            this.ballCombo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ballCombo.FormattingEnabled = true;
            this.ballCombo.Location = new System.Drawing.Point(12, 216);
            this.ballCombo.Name = "ballCombo";
            this.ballCombo.Size = new System.Drawing.Size(258, 24);
            this.ballCombo.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 194);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(107, 17);
            this.label4.TabIndex = 10;
            this.label4.Text = "Select Ball Type:";
            // 
            // RebarForm
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gray;
            this.CancelButton = this.button2;
            this.ClientSize = new System.Drawing.Size(964, 489);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.ballCombo);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.viewTemplatesCombo);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tagCombo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "RebarForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "RebarForm";
            this.Load += new System.EventHandler(this.RebarForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.Button button1;
        public System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.ComboBox tagCombo;
        public System.Windows.Forms.ComboBox viewTemplatesCombo;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.CheckBox checkBox1;
        public System.Windows.Forms.ComboBox ballCombo;
        private System.Windows.Forms.Label label4;
    }
}