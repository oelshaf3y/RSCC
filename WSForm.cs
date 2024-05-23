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
    public partial class WSForm : Form
    {
        public WSForm(List<string>worksetnames)
        {
            InitializeComponent();
            comboBox1.Items.AddRange(worksetnames.ToArray());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult=DialogResult.OK;
            this.Close();
        }
    }
}
