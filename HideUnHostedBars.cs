using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;

namespace RSCC_GEN
{
    [TransactionAttribute(TransactionMode.Manual)]
    internal class HideUnHostedBars : IExternalCommand
    {
        UIDocument uidoc;
        Document doc;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uidoc = commandData.Application.ActiveUIDocument;
            doc = uidoc.Document;

            Element Selected = doc.GetElement(uidoc.Selection.PickObject(ObjectType.Element, "pick host"));
            FilteredElementCollector rebar = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_Rebar);
            List<ElementId> ids = new List<ElementId>();
            foreach (Rebar bar in rebar)
            {
                if (bar.GetHostId().IntegerValue != Selected.Id.IntegerValue)
                {
                    ids.Add(bar.Id);
                }
            }
            if (ids.Count == 0)
            {
                doc.print("every thing is fine");
                return Result.Cancelled;
            }
            using (Transaction tr = new Transaction(doc, "Fix View"))
            {
                tr.Start();
                doc.ActiveView.HideElements(ids.ToArray());
                tr.Commit();
                tr.Dispose();
            }
            doc.print("every thing is fine");
            return Result.Succeeded;
        }


    }
}









































