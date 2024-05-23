using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSCC_GEN
{
    [TransactionAttribute(TransactionMode.Manual)]
    internal class GridsWS : IExternalCommand
    {
        UIDocument uidoc;
        Document doc;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uidoc = commandData.Application.ActiveUIDocument;
            doc = uidoc.Document;

            FilteredElementCollector Grids = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Grids);
            FilteredElementCollector Levels = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Levels);
            FilteredWorksetCollector worksets = new FilteredWorksetCollector(doc).OfKind(WorksetKind.UserWorkset);
            List<String> worksetNames = worksets.Select(x => x.Name).ToList();
            WSForm form = new WSForm(worksetNames);
            form.ShowDialog();
            if (form.DialogResult == System.Windows.Forms.DialogResult.Cancel) return Result.Cancelled;
            Workset WS = worksets.Where(x => x.Name == worksetNames[form.comboBox1.SelectedIndex]).First();
            List<ElementId> ids = new List<ElementId>();
            using (Transaction tr = new Transaction(doc, "Change workset"))
            {
                tr.Start();
                foreach (Element grid in Grids)
                {
                    try
                    {

                    grid.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM).Set(WS.Id.IntegerValue);
                    }
                    catch
                    {
                        ids.Add(grid.Id);
                    }
                }
                foreach (Element level in Levels)
                {
                    try
                    {

                        level.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM).Set(WS.Id.IntegerValue);
                    }
                    catch
                    {
                        ids.Add(level.Id);
                    }
                }
                tr.Commit();
            }
            uidoc.Selection.SetElementIds(ids);
            return Result.Succeeded;
        }
    }
}
