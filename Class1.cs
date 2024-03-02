using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSCC_GEN
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class Class1 : IExternalCommand
    {
        UIDocument uidoc;
        Document doc;
        StringBuilder sb = new StringBuilder();
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uidoc = commandData.Application.ActiveUIDocument;
            doc = uidoc.Document;
            Dimension dim = uidoc.Selection.GetElementIds().Select(x => doc.GetElement(x)).FirstOrDefault() as Dimension;
            dim.Curve.MakeBound(0.5, 0.6);
            Transaction t = new Transaction(doc, "draw");
            t.Start();
            doc.draw(new List<Line> { dim.Curve.Clone() as Line }.Cast<GeometryObject>().ToList());
            t.Commit();
            return Result.Succeeded;
        }
    }
}

