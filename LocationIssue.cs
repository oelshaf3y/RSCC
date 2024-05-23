using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RSCC_GEN
{
    [TransactionAttribute(TransactionMode.Manual)]
    internal class LocationIssue : IExternalCommand
    {
        public UIDocument uidoc { get; set; }
        public Document doc { get; set; }
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uidoc = commandData.Application.ActiveUIDocument;
            doc = uidoc.Document;

            ProjectLocationSet locations = doc.ProjectLocations;
            ProjectLocation activeLocation = doc.ActiveProjectLocation;
            List<string> locationNames = new List<string>();
            foreach (ProjectLocation location in locations)
            {
                if (doc.ActiveProjectLocation.Name == location.Name)
                {
                    locationNames.Add(location.Name + " (current)");
                }
                else
                {

                    locationNames.Add(location.Name);
                }

            }
            locationForm lform = new locationForm(locationNames);
            lform.ShowDialog();
            if (lform.DialogResult == DialogResult.Cancel) return Result.Cancelled;
            foreach (ProjectLocation location in locations)
            {
                if (location.Name == locationNames[lform.comboBox1.SelectedIndex])
                {
                    using (Transaction tr = new Transaction(doc, "change location"))
                    {
                        tr.Start();
                        doc.ActiveProjectLocation = location;
                        tr.Commit();
                    }
                    break;
                }
            }
            return Result.Succeeded;
        }
    }
}
