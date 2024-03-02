using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSCC_GEN
{
    [TransactionAttribute(TransactionMode.Manual)]
    internal class rebarTag : IExternalCommand
    {
        UIDocument uidoc;
        Document doc;
        List<Reference> sets;
        StringBuilder sb;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uidoc = commandData.Application.ActiveUIDocument;
            doc = uidoc.Document;
            //XYZ point = uidoc.Selection.PickPoint();
            sets = uidoc.Selection.PickObjects(ObjectType.Element, new RebarSelectionFilter()).ToList();
            sb = new StringBuilder();
            foreach (Reference set in sets)
            {

                using (Transaction t5 = new Transaction(doc, "Tag Rebars"))
                {
                    t5.Start();
                    Rebar rebar = doc.GetElement(set) as Rebar;
                    Line firstCurve = rebar.GetCenterlineCurves(false, true, true, MultiplanarOption.IncludeOnlyPlanarCurves, 0).Cast<Line>().OrderByDescending(x => x.Length).First();
                    Line lastCurve = rebar.GetCenterlineCurves(false, true, true, MultiplanarOption.IncludeOnlyPlanarCurves, 2).Cast<Line>().OrderByDescending(x => x.Length).First();
                    XYZ dir = XYZ.BasisZ.CrossProduct(firstCurve.Direction).Normalize();
                    MultiReferenceAnnotationType mraType = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_MultiReferenceAnnotations).Cast<MultiReferenceAnnotationType>().First();
                    MultiReferenceAnnotationOptions options = new MultiReferenceAnnotationOptions(mraType);
                    options.SetElementsToDimension(new List<ElementId> { rebar.Id });
                    options.DimensionLineDirection = dir;
                    options.DimensionPlaneNormal = XYZ.BasisZ;
                    //options.DimensionLineOrigin = firstCurve.GetEndPoint(0);
                    options.TagHeadPosition = lastCurve.GetEndPoint(0);
                    MultiReferenceAnnotation MRA = MultiReferenceAnnotation.Create(doc, doc.ActiveView.Id, options);
                    Dimension dim = doc.GetElement(MRA.DimensionId) as Dimension;
                    Reference first = dim.References.get_Item(0);
                    Reference last = dim.References.get_Item(dim.References.Size - 1);

                    t5.RollBack();
                    ReferenceArray referenceArray = new ReferenceArray();
                    referenceArray.Append(first);
                    referenceArray.Append(last);
                    t5.Start();
                    XYZ p = firstCurve.Evaluate(0.5, true);
                    doc.Create.NewDimension(doc.ActiveView, Line.CreateBound(p, p.Add(10 * dir)), referenceArray);
                    t5.Commit();
                }
                if (sb.Length > 0) doc.print(sb);
            }
            return Result.Succeeded;
        }
    }
}
