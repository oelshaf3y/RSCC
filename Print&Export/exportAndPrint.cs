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
        public DWGExportOptions currentSettings;
        PDFExportOptions pdfOptions;

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
                                    try
                                    {

                                        schedule.Export(form.XLSLocation, excelExporter.getFileName(row) + ".csv", viewScheduleExportOptions);
                                    }
                                    catch
                                    {
                                        sb.AppendLine(schedule.Name + " failed to be exported");
                                    }
                                }
                            }
                        }
                    }
                    if (form.pdfex.Checked)
                    {
                        try
                        {
                            pdfOptions = new PDFExportOptions();
                            pdfOptions.ColorDepth = ColorDepthType.Color;
                            pdfOptions.ExportQuality = PDFExportQualityType.DPI1200;
                            pdfOptions.PaperFormat = ExportPaperFormat.Default;
                            pdfOptions.StopOnError = true;
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

                        if (form.pdfex.Checked)
                        {
                            pdfOptions.FileName = name;
                            doc.Export(form.PDFLocation, new List<ElementId> { sheet.Id }, pdfOptions);
                            //doc.Print(viewSet, true);
                            //doc.Export()
                        }
                        if (form.cadex.Checked) doc.Export(form.DWGLocation, name + ".dwg", new List<ElementId> { sheet.Id }, form.currentDWGSettings);
                    }
                    tr.Commit();
                    tr.Dispose();
                }
            }
            if (sb.Length > 0) doc.print(sb.ToString());
            return Result.Succeeded;

        }
    }
}
