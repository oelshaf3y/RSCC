using System;
using System.Windows.Forms;

namespace RSCC_GEN.RebarSets.FindRFT
{
    public partial class FindRFTForm : Form
    {
        int result = 0;
        public FindRFTForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
            {
                MessageBox.Show("You have to enter a rebar number to search for!!");
                return;
            }
            if (int.TryParse(textBox1.Text.Trim(), out result))
            {

                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("You should enter only numbers!!");
                return;
            }
        }
    }
}
