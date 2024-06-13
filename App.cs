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
                LargeImage = Properties.Resources.capture.ToImageSource(),
                Image = Properties.Resources.capture.ToImageSource(),
                ToolTip = "Save visible elements in the current view to be reset later"
            };
            PushButtonData restoreState = new PushButtonData("Reset View State", "Reset State", assemblyName, "RSCC_GEN.ResetViewState")
            {
                LargeImage = Properties.Resources.ResetView.ToImageSource(),
                Image = Properties.Resources.resetViewS.ToImageSource(),
                ToolTip = "reset stored elements to be the only visible elements."
            };
            PushButtonData ResetSheets = new PushButtonData("Reset Sheets State", "Reset Sheets", assemblyName, "RSCC_GEN.ResetSheet")
            {
                LargeImage = Properties.Resources.resetSheets.ToImageSource(),
                Image = Properties.Resources.ResetSheetsS.ToImageSource(),
                ToolTip = "reset stored elements to be the only visible elements in all sheets."
            };
            PushButtonData HideUnhosted = new PushButtonData("Hide Unhosted Rebar", "Hide Unhosted", assemblyName, "RSCC_GEN.HideUnHostedBars")
            {
                LargeImage = Properties.Resources.hideUnhosted.ToImageSource(),
                Image = Properties.Resources.hideUnhostedS.ToImageSource(),
                ToolTip = "Hide rebar bars which are not hosted by selected element."
            };
            PushButtonData NOS = new PushButtonData("Delete Not on sheets", "Delete N.O.S", assemblyName, "RSCC_GEN.NotOnSheet")
            {
                LargeImage = Properties.Resources.NOS.ToImageSource(),
                Image = Properties.Resources.NOS.ToImageSource(),
                ToolTip = "Delete Unused Views which are not hosted in sheets."
            };
            try
            {

                panel.AddItem(NOS);
                panel.AddItem(HideUnhosted);
                panel.AddItem(SaveState);
                panel.AddItem(restoreState);
                panel.AddItem(ResetSheets);
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
