using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace RSCC_GEN.RebarAnnotations
{
    public partial class RebarForm : System.Windows.Forms.Form
    {
        List<List<Parameter>> parameters;
        List<FamilySymbol> tags;
        List<Autodesk.Revit.DB.View> viewTemplates;
        List<AnnotationSymbolType> annoSymbols;
        public RebarForm(List<List<Parameter>> parameters, List<FamilySymbol> tags,
            List<Autodesk.Revit.DB.View> viewteplates, List<AnnotationSymbolType> annoSymbols)
        {
            this.parameters = parameters;
            this.tags = tags;
            this.annoSymbols = annoSymbols;
            viewTemplates = viewteplates;
            InitializeComponent();
        }

        private void RebarForm_Load(object sender, EventArgs e)
        {
            tagCombo.Items.AddRange(tags.Select(x => x.Name).ToArray());
            viewTemplatesCombo.Items.AddRange(viewTemplates.Select(x => x.Name).ToArray());
            panel1.Controls.Add(new ParameterFilter(this, parameters));
            panel1.Controls[0].Location = new System.Drawing.Point(3, 0);
            label4.Visible = checkBox1.Checked;
            ballCombo.Visible = checkBox1.Checked;
            ballCombo.Items.AddRange(annoSymbols.Select(x => x.Name).ToArray());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (UserControl control in panel1.Controls)
            {
                ComboBox combo = control.Controls.Find("parametersCombo", true).First() as ComboBox;
                if (combo.SelectedIndex == -1)
                {
                    MessageBox.Show("You have to select a parameter for every filter.");
                    return;
                }
            }
            if (tagCombo.SelectedIndex == -1 || viewTemplatesCombo.SelectedIndex == -1)
            {
                MessageBox.Show("You have to select View template and tag type.");
                return;
            }
            if (checkBox1.Checked)
            {
                if (ballCombo.SelectedIndex == -1)
                {
                    MessageBox.Show("Please Select the ball type!");
                    return;
                }
            }
            DialogResult = DialogResult.OK;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

            label4.Visible = checkBox1.Checked;
            ballCombo.Visible = checkBox1.Checked;

        }

        //private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    comboBox2.Items.Clear();
        //    comboBox2.Items.AddRange(
        //        parameters
        //        .Where(x => x.ElementAt(comboBox1.SelectedIndex).AsString() != null)
        //        .Select(x => x.ElementAt(comboBox1.SelectedIndex).AsString()).Distinct().ToArray());
        //}
    }
}
