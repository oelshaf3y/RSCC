using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RSCC_GEN
{
    public partial class QTYForm : Form
    {
        public double volume { get; set; }
        public double surfaceArea { get; set; }

        public QTYForm(double volume, double surfaceArea)
        {
            this.volume = volume;
            this.surfaceArea = surfaceArea;
            InitializeComponent();
            textBox1.Text = Math.Round(volume, 2).ToString();
            textBox2.Text = Math.Ceiling(volume * 1.1).ToString();
            textBox4.Text = Math.Round(surfaceArea, 2).ToString();
            textBox3.Text = Math.Ceiling(surfaceArea * 1.1).ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBox2.Text);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            ChangeCheck();
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            ChangeCheck();
        }

        private void ChangeCheck()
        {
            if (radioButton1.Checked)
            {
                textBox1.Text = Math.Round(volume, 2).ToString();
                textBox2.Text = Math.Ceiling(volume * 1.1).ToString();
                label3.Text = "m3";
                label4.Text = "m3";
                textBox3.Visible = true;
                button4.Visible = true;
                textBox4.Visible = true;
                button5.Visible = true;
                label5.Visible = true;
                label6.Visible = true;
                label7.Visible = true;
                label8.Visible = true;
            }
            else
            {
                textBox1.Text = Math.Round(volume * 7.85, 2).ToString();
                textBox2.Text = Math.Ceiling(volume * 7.85 * 1.1).ToString();
                label3.Text = "Ton";
                label4.Text = "Ton";
                textBox3.Visible = false;
                button4.Visible = false;
                textBox4.Visible = false;
                button5.Visible = false;
                label5.Visible = false;
                label6.Visible = false;
                label7.Visible = false;
                label8.Visible = false;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBox3.Text);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBox4.Text);
        }
    }
}
