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
    internal class IsolateElements : IExternalCommand
    {
        UIDocument uidoc;
        Document doc;
        List<ElementId> ids;
        View activeView;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uidoc = commandData.Application.ActiveUIDocument;
            doc = uidoc.Document;
            activeView = doc.ActiveView;
            if (activeView is ViewSheet || activeView is ViewSchedule)
            {
                doc.print("Active view must be a view not a sheet or a schedule");
                return Result.Failed;
            }
            ids = new List<ElementId>();
            if (uidoc.Selection.GetElementIds().Count == 0)
            {
                ids = uidoc.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element, "Select Elements")
                    .Select(x => x.ElementId).ToList();
            }
            else
            {
                ids = uidoc.Selection.GetElementIds().ToList();
            }
            FilteredElementCollector collector = new FilteredElementCollector(doc, activeView.Id).WhereElementIsNotElementType();
            using (Transaction tr = new Transaction(doc, "Isolate Elements"))
            {
                tr.Start();
                foreach (ElementId id in collector.Select(x => x.Id).Except(ids.ToList()).ToList())
                {
                    try
                    {
                        activeView.HideElements(new List<ElementId>() { id });
                    }
                    catch (Exception ex)
                    {

                    }
                }
                activeView.UnhideElements(ids.ToList());
                tr.Commit();
                tr.Dispose();
            }

            return Result.Succeeded;
        }
    }
}
