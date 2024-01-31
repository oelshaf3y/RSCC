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
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uidoc = commandData.Application.ActiveUIDocument;
            doc = uidoc.Document;
            sb = new StringBuilder();
            PrintAndExportForm form = new PrintAndExportForm(commandData);
            form.ShowDialog();
            sheets = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Sheets).Cast<ViewSheet>().ToList();
            PrintManager printManager = form.printManager;
            if (form.DialogResult == DialogResult.OK)
            {

                printManager.Apply();
                using (Transaction tr = new Transaction(doc, "Print and Export"))
                {
                    tr.Start();
                    try
                    {

                        printManager.SelectNewPrintDriver(form.comboBox1.SelectedItem.ToString());
                        printManager.PrintRange = PrintRange.Select;
                        printManager.PrintSetup.CurrentPrintSetting.PrintParameters.PaperSize = printManager.PaperSizes.Cast<PaperSize>().Where(x => x.Name == form.comboBox2.SelectedItem.ToString()).First();
                        printManager.PrintSetup.CurrentPrintSetting.PrintParameters.HiddenLineViews = HiddenLineViewsType.VectorProcessing;
                        printManager.PrintSetup.CurrentPrintSetting.PrintParameters.RasterQuality = form.rasterQualityTypes.ElementAt(form.comboBox4.SelectedIndex);
                        printManager.PrintSetup.CurrentPrintSetting.PrintParameters.HideScopeBoxes = true;
                        printManager.PrintSetup.CurrentPrintSetting.PrintParameters.HideUnreferencedViewTags = true;
                        printManager.PrintSetup.CurrentPrintSetting.PrintParameters.HideCropBoundaries = true;
                        printManager.PrintSetup.CurrentPrintSetting.PrintParameters.HideReforWorkPlanes = true;
                        printManager.PrintSetup.CurrentPrintSetting.PrintParameters.ColorDepth = form.colorDepthTypes.ElementAt(form.comboBox3.SelectedIndex);
                        printManager.PrintToFile = true;
                        printManager.CombinedFile = true;
                        doc.PrintManager.PrintSetup.CurrentPrintSetting = form.printManager.PrintSetup.CurrentPrintSetting;
                    }
                    catch (Exception ex)
                    {
                        doc.print(ex.ToString());
                    }
                    foreach (var item in form.checkedListBox1.CheckedItems)
                    {
                        ViewSet viewSet = new ViewSet();
                        ViewSheet sheet = sheets.Where(x => x.SheetNumber + " - " + x.Name == item.ToString()).FirstOrDefault();
                        if (sheet == null) continue;
                        viewSet.Insert(sheet);
                        string name = form.getFileName(sheet) + ".pdf";
                        printManager.PrintToFileName = Path.Combine(form.PDFLocation, name);
                        printManager.Apply();
                        doc.Print(viewSet, true);
                    }
                    tr.Commit();
                    tr.Dispose();
                }
            }
            return Result.Succeeded;

        }
    }
}
