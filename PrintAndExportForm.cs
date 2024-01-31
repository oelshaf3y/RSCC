using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RSCC_GEN
{
    public partial class PrintAndExportForm : System.Windows.Forms.Form
    {
        public PrintManager printManager;
        public List<string> printers;
        public List<ViewSheet> sheets;
        public List<ViewSheetSet> sheetSets;
        public ViewSheetSetting viewSheetSetting;
        List<ExportDWGSettings> exportSettings;
        public DWGExportOptions currentSettings;
        UIDocument uidoc;
        Document doc;
        public string savelocation, PDFLocation, DWGLocation, XLSLocation;
        public PrintAndExportForm(ExternalCommandData commandData)
        {
            currentSettings = null;
            savelocation = "";
            uidoc = commandData.Application.ActiveUIDocument;
            doc = uidoc.Document;
            sheetSets = new FilteredElementCollector(doc).OfClass(typeof(ViewSheetSet)).Cast<ViewSheetSet>().ToList();
            sheets = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Sheets).OfClass(typeof(ViewSheet)).Cast<ViewSheet>().ToList();
            printManager = doc.PrintManager;
            printManager.PrintRange = PrintRange.Select;
            printers = System.Drawing.Printing.PrinterSettings.InstalledPrinters.Cast<string>().ToList(); ;
            viewSheetSetting = printManager.ViewSheetSetting;
            exportSettings = new FilteredElementCollector(doc).OfClass(typeof(ExportDWGSettings)).Cast<ExportDWGSettings>().ToList();
            InitializeComponent();

        }

        private void PrintAndExportForm_Load(object sender, EventArgs e)
        {
            comboBox1.Items.AddRange(printers.ToArray());
            comboBox3.Items.AddRange(exportSettings.Select(x => x.Name).ToArray());
            if (exportSettings.Count() > 0) comboBox3.SelectedIndex = 0;
            comboBox5.Items.AddRange(sheetSets.Select(x => x.Name).ToArray());
            comboBox5.Items.Add("All sheets in the Model");
            comboBox5.SelectedIndex = comboBox5.Items.Count - 1;
            exampleLabel.Text = sheets.First().SheetNumber + " - " + sheets.First().Name.ToString();
            location.Text = "C:\\Users\\Omar\\Desktop\\Out";
            checkedListBox1.SetItemChecked(1, true);
            checkedListBox1.SetItemChecked(2, true);


        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            printManager.SelectNewPrintDriver(printers.ElementAt(comboBox1.SelectedIndex));
            comboBox2.Items.AddRange(printManager.PaperSizes.Cast<PaperSize>().OrderBy(x => x.Name).Select(x => x.Name).ToArray());
        }
        private void checkExampleLabel(object sender, EventArgs e)
        {
            if (!includeSheetName.Checked & !includeSheetNo.Checked)
            {
                MessageBox.Show("There's no uniqe name for sheets to be printed.\nPlease include Sheet Number or Sheeet Name.");
                CheckBox cb = sender as CheckBox;
                cb.Checked = true;
                return;
            }

            exampleLabel.Text = getFileName(sheets.First());
        }

        private void includeSheetName_CheckedChanged(object sender, EventArgs e)
        {
            checkExampleLabel(sender, e);
        }

        private void includeSheetNo_CheckedChanged(object sender, EventArgs e)
        {
            checkExampleLabel(sender, e);
        }

        private void suffixBox_TextChanged(object sender, EventArgs e)
        {
            checkExampleLabel(sender, e);
        }

        private void prefixBox_TextChanged(object sender, EventArgs e)
        {
            checkExampleLabel(sender, e);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            savelocation = location.Text;
            if (!Directory.Exists(savelocation))
            {
                MessageBox.Show("Please Select a valid location.");
                return;
            }
            if (cadex.Checked || pdfex.Checked)
            {
                if (checkedListBox1.CheckedItems.Count == 0)
                {
                    MessageBox.Show("Please select at least one sheet!");
                    return;
                }
                if (location.Text.Trim().Length == 0)
                {
                    MessageBox.Show("Please Select a Path to save the files!");
                    return;
                }
            }
            if (cadex.Checked && comboBox3.SelectedItem.ToString().Trim().Length == 0)
            {
                currentSettings = new DWGExportOptions();
                currentSettings.MergedViews = merge.Checked;
            }
            PDFLocation = Path.Combine(savelocation, "PDF");
            DWGLocation = Path.Combine(savelocation, "CAD");
            XLSLocation = Path.Combine(savelocation, "Excel");
            if (!Directory.Exists(Path.Combine(savelocation, "PDF")))
            {
                Directory.CreateDirectory(PDFLocation);
            }
            if (!Directory.Exists(DWGLocation))
            {
                Directory.CreateDirectory(DWGLocation);
            }
            if (!Directory.Exists(XLSLocation))
            {
                Directory.CreateDirectory(XLSLocation);
            }

            DialogResult = DialogResult.OK;
        }

        public string getFileName(ViewSheet sheet)
        {
            StringBuilder name = new StringBuilder();
            if (prefixBox.Text.Trim().Length > 0) name.Append(prefixBox.Text);
            if (includeSheetNo.Checked && includeSheetName.Checked) name.Append(sheet.SheetNumber + " - " + sheet.Name);
            else if (includeSheetNo.Checked) name.Append(sheet.SheetNumber);
            else if (includeSheetName.Checked) name.Append(sheet.Name);
            if (suffixBox.Text.Trim().Length > 0) name.Append(suffixBox.Text);
            return name.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.ShowDialog();
            location.Text = folderBrowser.SelectedPath;
        }


        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            currentSettings = exportSettings.ElementAt(comboBox3.SelectedIndex).GetDWGExportOptions();
            currentSettings.MergedViews = merge.Checked;
        }

        private void merge_CheckedChanged(object sender, EventArgs e)
        {
            currentSettings.MergedViews = merge.Checked;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, true);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, false);
            }
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkedListBox1.Items.Clear();
            if (comboBox5.SelectedItem.ToString() == "All sheets in the Model")
            {
                foreach (var s in this.sheets)
                {
                    checkedListBox1.Items.Add(s.SheetNumber + " - " + s.Name);
                }
                return;
            }
            List<Autodesk.Revit.DB.View> sheets = sheetSets.ElementAt(comboBox5.SelectedIndex).OrderedViewList.Cast<Autodesk.Revit.DB.View>().ToList();
            checkedListBox1.Items.AddRange(sheets.Select(x => x.Name).ToArray());
        }
    }
}