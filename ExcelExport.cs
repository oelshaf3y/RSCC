using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RSCC_GEN
{
    public partial class ExcelExport : System.Windows.Forms.Form
    {
        Document doc;
        public List<ViewSheetSet> sheetSets;
        public List<ViewSchedule> schedules;

        public ExcelExport(ExternalCommandData commandData)
        {
            doc = commandData.Application.ActiveUIDocument.Document;
            sheetSets = new FilteredElementCollector(doc).OfClass(typeof(ViewSheetSet)).Cast<ViewSheetSet>().ToList();
            schedules = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Schedules).Cast<ViewSchedule>().ToList();
            InitializeComponent();
        }

        private void ExcelExport_Load(object sender, EventArgs e)
        {
            comboBox1.Items.AddRange(sheetSets.Select(s => s.Name).ToArray());
            comboBox1.Items.Add("All Schedules in the Model");
            DataGridViewCheckBoxColumn cbCol = new DataGridViewCheckBoxColumn();
            cbCol.Name = "selected";
            cbCol.HeaderText = "Selected";
            cbCol.ValueType = typeof(bool);
            dataGridView1.Columns.Add(cbCol);
            dataGridView1.Columns.Add("no", "No.");
            dataGridView1.Columns.Add("name", "Name");
            dataGridView1.Columns[2].Width = 250;
            comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
            exampleLabel.Text = getFileName(dataGridView1.Rows[0]);

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            if (comboBox1.SelectedItem.ToString() == "All Schedules in the Model")
            {
                foreach (var s in schedules)
                {
                    dataGridView1.Rows.Add(false, "", s.Name);
                    //dgvData.Add(new DGVData(false, "", s.Name));
                }
                dataGridView1.Refresh();
                return;
            }
            List<Autodesk.Revit.DB.ViewSchedule> scheds = sheetSets.ElementAt(comboBox1.SelectedIndex).OrderedViewList.Where(x => x is ViewSchedule)?.Cast<Autodesk.Revit.DB.ViewSchedule>().ToList();
            foreach (Autodesk.Revit.DB.ViewSchedule v in scheds)
            {
                dataGridView1.Rows.Add(false, "", v.Name);
            }
            dataGridView1.Refresh();

        }

        private void button4_Click(object sender, EventArgs e)
        {

            for (var i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Cells[0].Value = true;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Cells[0].Value = false;
            }
        }

        private void includeNo_CheckedChanged(object sender, EventArgs e)
        {
            checkExample(sender, e);
        }

        private void includeName_CheckedChanged(object sender, EventArgs e)
        {
            checkExample(sender, e);

        }

        private void prefixBox_TextChanged(object sender, EventArgs e)
        {
            checkExample(sender, e);
        }

        private void suffixBox_TextChanged(object sender, EventArgs e)
        {
            checkExample(sender, e);
        }
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            checkExample(sender, e);
        }

        private void checkExample(object sender, EventArgs e)
        {
            if (!includeName.Checked & !includeNo.Checked)
            {
                MessageBox.Show("There's no uniqe name for sheets to be printed.\nPlease include Sheet Number or Sheeet Name.");
                CheckBox cb = sender as CheckBox;
                cb.Checked = true;
                return;
            }

            exampleLabel.Text = getFileName(dataGridView1.Rows[0]);
        }

        public string getFileName(DataGridViewRow row)
        {
            StringBuilder name = new StringBuilder();
            if (prefixBox.Text.Trim().Length > 0) name.Append(prefixBox.Text);
            if (includeNo.Checked && includeName.Checked) name.Append(row.Cells[1].Value.ToString() + " - " + row.Cells[2].Value.ToString());
            else if (includeNo.Checked) name.Append(row.Cells[1].Value.ToString());
            else if (includeName.Checked) name.Append(row.Cells[2].Value.ToString());
            if (suffixBox.Text.Trim().Length > 0) name.Append(suffixBox.Text);
            return name.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (includeNo.Checked)
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells[1].Value.ToString().Trim().Length == 0)
                    {
                        MessageBox.Show("you included numbers.yet, you didn't specify any numbers");
                        return;
                    }
                }
            }
            DialogResult = DialogResult.OK;
        }
    }
}
