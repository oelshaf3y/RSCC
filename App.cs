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

                panel = application.CreateRibbonPanel("Sheets");
            }
            catch
            {
                MessageBox.Show("error");
                return Result.Cancelled;
            }
            PushButtonData SaveState = new PushButtonData("Save View State", "Save State", assemblyName, "RSCC_GEN.SaveViewState")
            {
                LargeImage = Properties.Resources.capture.ToImageSource(),
                //Image = Properties.Resources.similar_s.ToImageSource(),
                ToolTip = "Save visible elements in the current view to be reset later"
            };
            PushButtonData restoreState = new PushButtonData("Reset View State", "Reset State", assemblyName, "RSCC_GEN.ResetViewState")
            {
                LargeImage = Properties.Resources.restore.ToImageSource(),
                //Image = Properties.Resources.similar_s.ToImageSource(),
                ToolTip = "reset stored elements to be the only visible elements."
            };
            PushButtonData ResetSheets = new PushButtonData("Reset Sheets State", "Reset Sheets", assemblyName, "RSCC_GEN.ResetSheet")
            {
                LargeImage = Properties.Resources.sheets.ToImageSource(),
                //Image = Properties.Resources.similar_s.ToImageSource(),
                ToolTip = "reset stored elements to be the only visible elements in all sheets."
            };
            PushButtonData HideUnhosted = new PushButtonData("Hide Unhosted Rebar", "Hide Unhosted", assemblyName, "RSCC_GEN.HideUnHostedBars")
            {
                LargeImage = Properties.Resources.rft.ToImageSource(),
                //Image = Properties.Resources.similar_s.ToImageSource(),
                ToolTip = "Hide rebar bars which are not hosted by selected element."
            };
            panel.AddItem(HideUnhosted);
            panel.AddItem(SaveState);
            panel.AddItem(restoreState);
            panel.AddItem(ResetSheets);

            return Result.Succeeded;
        }
    }
}
