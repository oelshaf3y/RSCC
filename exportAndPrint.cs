using Autodesk.Revit.Attributes;
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
    [TransactionAttribute(TransactionMode.Manual)]
    internal class exportAndPrint : IExternalCommand
    {
        UIDocument uidoc;
        Document doc;
        List<ViewSheet> sheets;
        StringBuilder sb;
        List<ViewSheetSet> sheetSets;
        List<ViewSchedule> schedules;
        ViewScheduleExportOptions viewScheduleExportOptions;
        ExcelExport excelExporter;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uidoc = commandData.Application.ActiveUIDocument;
            doc = uidoc.Document;

            sb = new StringBuilder();
            PrintAndExportForm form = new PrintAndExportForm(commandData);
            form.ShowDialog();
            sheets = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Sheets).Cast<ViewSheet>().ToList();
            schedules = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Schedules).Cast<ViewSchedule>().ToList();
            PrintManager printManager = form.printManager;
            viewScheduleExportOptions = new ViewScheduleExportOptions();
            viewScheduleExportOptions.FieldDelimiter = ",";
            if (form.DialogResult == DialogResult.OK)
            {
                if (form.xlsxEx.Checked)
                {
                    excelExporter = new ExcelExport(commandData);
                    excelExporter.ShowDialog();

                }
                printManager.Apply();
                using (Transaction tr = new Transaction(doc, "Print and Export"))
                {
                    tr.Start();
                    if (form.xlsxEx.Checked)
                    {
                        if (excelExporter.DialogResult == DialogResult.OK)
                        {
                            foreach (DataGridViewRow row in excelExporter.dataGridView1.Rows)
                            {
                                if (Convert.ToBoolean((row.Cells[0] as DataGridViewCheckBoxCell).Value))
                                {
                                    ViewSchedule schedule = schedules.Where(x => x.Name == row.Cells[2].Value.ToString()).FirstOrDefault();
                                    schedule.Export(form.XLSLocation, excelExporter.getFileName(row) + ".csv", viewScheduleExportOptions);
                                }
                            }
                        }
                    }
                    if (form.pdfex.Checked)
                    {
                        try
                        {

                            printManager.SelectNewPrintDriver(form.comboBox1.SelectedItem.ToString());
                            printManager.PrintRange = PrintRange.Select;
                            printManager.PrintSetup.CurrentPrintSetting.PrintParameters.PaperSize = printManager.PaperSizes.Cast<PaperSize>().Where(x => x.Name == form.comboBox2.SelectedItem.ToString()).First();
                            printManager.PrintSetup.CurrentPrintSetting.PrintParameters.HiddenLineViews = HiddenLineViewsType.VectorProcessing;
                            printManager.PrintSetup.CurrentPrintSetting.PrintParameters.RasterQuality = RasterQualityType.Presentation;
                            printManager.PrintSetup.CurrentPrintSetting.PrintParameters.HideScopeBoxes = true;
                            printManager.PrintSetup.CurrentPrintSetting.PrintParameters.HideUnreferencedViewTags = true;
                            printManager.PrintSetup.CurrentPrintSetting.PrintParameters.HideCropBoundaries = true;
                            printManager.PrintSetup.CurrentPrintSetting.PrintParameters.HideReforWorkPlanes = true;
                            printManager.PrintSetup.CurrentPrintSetting.PrintParameters.ColorDepth = ColorDepthType.Color;
                            printManager.PrintToFile = true;
                            printManager.CombinedFile = true;
                            doc.PrintManager.PrintSetup.CurrentPrintSetting = form.printManager.PrintSetup.CurrentPrintSetting;
                        }
                        catch (Exception ex)
                        {
                            doc.print(ex.ToString());
                        }
                    }
                    foreach (var item in form.checkedListBox1.CheckedItems)
                    {
                        ViewSet viewSet = new ViewSet();
                        ViewSheet sheet = sheets.Where(x => x.SheetNumber + " - " + x.Name == item.ToString()).FirstOrDefault();
                        if (sheet == null) continue;
                        viewSet.Insert(sheet);
                        string name = form.getFileName(sheet);
                        printManager.PrintToFileName = Path.Combine(form.PDFLocation, name + ".pdf");
                        printManager.Apply();
                        if (form.pdfex.Checked) doc.Print(viewSet, true);
                        if (form.cadex.Checked) doc.Export(form.DWGLocation, name + ".dwg", new List<ElementId> { sheet.Id }, form.currentSettings);
                    }
                    tr.Commit();
                    tr.Dispose();
                }
            }
            return Result.Succeeded;

        }
    }
}
