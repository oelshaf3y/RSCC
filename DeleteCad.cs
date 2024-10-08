﻿using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Linq;
using System.Windows.Forms;

namespace RSCC_GEN
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class DeleteCad : IExternalCommand
    {
        UIDocument uidoc;
        Document doc;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uidoc = commandData.Application.ActiveUIDocument;
            doc = uidoc.Document;


            FilteredElementCollector fec = new FilteredElementCollector(doc).OfClass(typeof(CADLinkType));
            int count = fec.Count();
            if (count == 0) { TaskDialog.Show("Info", "No more DWG Imports In The Project."); return Result.Succeeded; }
            else
            {
                DialogResult dia = MessageBox.Show($"Are You Sure You Want To Delete {count} CAD Files?\nThis CAN NOT BE UNDONE!", "Are You Sure?", MessageBoxButtons.YesNo);
                if (dia == DialogResult.No) return Result.Cancelled;
            }

            Transaction tr = new Transaction(doc, "Delete CAD Imports");
            tr.Start();
            foreach (Element element in fec)
            {
                element.Pinned = false;
            }
            doc.Delete(fec.Select(x => x.Id).ToArray());
            TaskDialog.Show("Done", $"Successfully deleted {count} CAD Files.");
            tr.Commit();
            tr.Dispose();
            return Result.Succeeded;
        }
    }
}
