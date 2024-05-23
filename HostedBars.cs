using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSCC_GEN
{
    [TransactionAttribute(TransactionMode.Manual)]
    internal class HostedBars : IExternalCommand
    {
        public UIDocument uidoc { get; set; }
        public Document doc { get; set; }
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uidoc = commandData.Application.ActiveUIDocument;
            doc = uidoc.Document;

            Element selected = doc.GetElement(uidoc.Selection.PickObject(ObjectType.Element));
            List<Rebar> bars = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rebar)
                .Where(x=>x is Rebar).Cast<Rebar>()
                .Where(x=>x.GetHostId().IntegerValue == selected.Id.IntegerValue).ToList();

            uidoc.Selection.SetElementIds(bars.Select(x=>x.Id).Concat(new List<ElementId> { selected.Id}).ToList());

            return Result.Succeeded;
        }
    }
}
