using System;
using System.Windows.Forms;

namespace RSCC_GEN
{
    public partial class Consolidate : Form
    {
        public Consolidate()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
