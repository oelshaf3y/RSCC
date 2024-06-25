using Autodesk.Revit.UI;
using System.Reflection;
using System.Windows.Forms;

namespace RSCC_GEN
{
    internal class App : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            string assemblyName = Assembly.GetExecutingAssembly().Location;
            string asPath = System.IO.Path.GetDirectoryName(assemblyName);
            RibbonPanel panel;
            try
            {

                panel = application.CreateRibbonPanel("RSCC - Views & Sheets");
            }
            catch
            {
                MessageBox.Show("error");
                return Result.Cancelled;
            }
            PushButtonData SaveState = new PushButtonData("Save View State", "Save State", assemblyName, "RSCC_GEN.SaveViewState")
            {
                Image = Properties.Resources.captures.ToImageSource(),
                LargeImage = Properties.Resources.capture.ToImageSource(),
                ToolTip = "Save visible elements in the current view to be reset later"
            };
            PushButtonData restoreState = new PushButtonData("Reset View State", "Reset State", assemblyName, "RSCC_GEN.ResetViewState")
            {
                Image = Properties.Resources.resetViewS.ToImageSource(),
                LargeImage = Properties.Resources.ResetView.ToImageSource(),
                ToolTip = "reset stored elements to be the only visible elements."
            };
            PushButtonData ResetSheets = new PushButtonData("Reset Sheets State", "Reset Sheets", assemblyName, "RSCC_GEN.ResetSheet")
            {
                Image = Properties.Resources.ResetSheetsS.ToImageSource(),
                LargeImage = Properties.Resources.resetSheets.ToImageSource(),
                ToolTip = "reset stored elements to be the only visible elements in all sheets."
            };
            PushButtonData HideUnhosted = new PushButtonData("Hide Unhosted Rebar", "Hide Unhosted", assemblyName, "RSCC_GEN.HideUnHostedBars")
            {
                Image = Properties.Resources.hideUnhostedS.ToImageSource(),
                LargeImage = Properties.Resources.hideUnhosted.ToImageSource(),
                ToolTip = "Hide rebar bars which are not hosted by selected element."
            };
            PushButtonData NOS = new PushButtonData("Delete Not on sheets", "Delete N.O.S", assemblyName, "RSCC_GEN.NotOnSheet")
            {
                Image = Properties.Resources.NOSsmall.ToImageSource(),
                LargeImage = Properties.Resources.NOS.ToImageSource(),
                ToolTip = "Delete Unused Views which are not hosted in sheets."
            };
            PushButtonData BatchPrint = new PushButtonData("Batch Export", "Print/Export", assemblyName, "RSCC_GEN.exportAndPrint")
            {
                Image = Properties.Resources.pdfSmall.ToImageSource(),
                LargeImage = Properties.Resources.pdfLarge.ToImageSource(),
                ToolTip = "Print or export multiple sheets, views or schedules at once."
            };
            try
            {
                panel.AddItem(BatchPrint);
                panel.AddStackedItems(NOS,HideUnhosted);
                panel.AddStackedItems(SaveState, restoreState, ResetSheets);
                //panel.AddItem(SaveState);
                //panel.AddItem(restoreState);
                //panel.AddItem(ResetSheets);
            }
            catch (System.Exception ex)
            {
                TaskDialog.Show("exception",ex.StackTrace);
                TaskDialog.Show("exception", ex.Message);

            }
            //panel.AddItem(SaveState);
            //panel.AddItem(restoreState);
            //panel.AddItem(ResetSheets);

            return Result.Succeeded;
        }
    }
}
