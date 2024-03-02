using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RSCC_GEN.RebarAnnotations
{
    public partial class ParameterFilter : UserControl
    {
        List<List<Parameter>> parameters;
        RebarForm parent;
        public ParameterFilter(RebarForm parent, List<List<Parameter>> parameters)
        {
            this.parent = parent;
            this.parameters = parameters;
            InitializeComponent();
        }

        private void ParameterFilter_Load(object sender, EventArgs e)
        {
            parametersCombo.Items.AddRange(parameters.First().Select(x => x.Definition.Name).ToArray());
        }

        private void parametersCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string par in parameters
                .Where(x => x.ElementAt(parametersCombo.SelectedIndex).AsString() != null)
                .Select(x => x.ElementAt(parametersCombo.SelectedIndex).AsString()).Distinct())
            {
                sb.Append(par);
                sb.Append(',');
            }
            valuesLabel.Text = sb.ToString();
            //    //comboBox2.Items.AddRange(
            //    parameters
            //    .Where(x => x.ElementAt(comboBox1.SelectedIndex).AsString() != null)
            //    .Select(x => x.ElementAt(comboBox1.SelectedIndex).AsString()).Distinct().ToArray());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (parent.panel1.Controls.Count == 4)
            {
                MessageBox.Show("more than 4 filters will cause the program to crash.");
                return;
            }
            parent.panel1.Controls.Add(new ParameterFilter(parent, parameters));
            int index = parent.panel1.Controls.Count - 1;
            parent.panel1.Controls[index].Location = new System.Drawing.Point(3, index * 90);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (parent.panel1.Controls.Count == 1)
            {
                MessageBox.Show("You can't remove the only filter");
                return;
            }
            parent.panel1.Controls.Remove(this);
            for (int i = 0; i < parent.panel1.Controls.Count; i++)
            {
                UserControl control = parent.panel1.Controls[i] as UserControl;
                control.Location = new System.Drawing.Point(3, i * 90);
            }
        }
    }
}
