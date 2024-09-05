using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
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
    internal class PilesToSeaBed : IExternalCommand
    {
        public Document doc { get; set; }
        public UIDocument uidoc { get; set; }
        Element toposolid;
        List<Element> piles;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uidoc = commandData.Application.ActiveUIDocument;
            doc = uidoc.Document;
            try
            {

                toposolid = doc.GetElement(uidoc.Selection.PickObjects(ObjectType.Element,
                          new RSCCSelectionFilter((x) => x.GetType() == typeof(Toposolid), true), "Pick Toposolid").FirstOrDefault());

                piles = uidoc.Selection.PickObjects(ObjectType.Element,
                    new RSCCSelectionFilter((x) => x.Category.Name == "Structural Foundations", true),
                    "Select Piles").Select(x => doc.GetElement(x)).ToList();

            }
            catch
            {
                return Result.Failed;
            }
            using (Transaction tr = new Transaction(doc, "Edit Piles"))
            {
                tr.Start();
                foreach (Element pile in piles)
                {
                    XYZ locationPoint = ((LocationPoint)pile.Location).Point;
                    Level level = doc.GetElement(pile.LevelId) as Level;
                    if (getIntersection(toposolid, locationPoint).First() is XYZ point && point != null)
                    {
                        double heightOffset = pile.LookupParameter("Height Offset From Level").AsDouble();
                        double depth = level.Elevation+Ninja.mmToFeet(2300) - point.Z + heightOffset;
                        //doc.print(Ninja.feetToMeter(depth));
                        doc.print(Ninja.feetToMM(point.Z));
                        if (depth > 0)
                        {
                            pile.LookupParameter("Sea Bed Depth").Set(depth);
                        }
                        else
                        {
                            pile.LookupParameter("Sea Bed Depth").Set(heightOffset + Ninja.mmToFeet(10));

                        }
                    }
                    else
                    {
                        doc.print("Null");
                    }
                }
                tr.Commit();
                tr.Dispose();
            }
            return Result.Succeeded;

        }

        List<XYZ> getIntersection(Element toposolid, XYZ temp)
        {
            Solid solid = doc.getSolid(toposolid);
            List<XYZ> intersectionPts = new List<XYZ>();
            SolidCurveIntersectionOptions tempOptions = new SolidCurveIntersectionOptions();
            tempOptions.ResultType = SolidCurveIntersectionMode.CurveSegmentsInside;
            SolidCurveIntersection tempSCI = solid.IntersectWithCurve(Line.CreateBound(temp.Add(500 * XYZ.BasisZ), temp.Add(-500 * XYZ.BasisZ)), tempOptions);
            if (tempSCI == null || tempSCI.SegmentCount == 0)
            {
                return null;
            }
            Curve tempC = tempSCI.GetCurveSegment(0);
            XYZ p1 = tempC.GetEndPoint(0);
            XYZ p2 = tempC.GetEndPoint(1);
            if (p1.Z > p2.Z)
            {
                intersectionPts.Add(p1);
            }
            else
            {
                intersectionPts.Add(p2);
            }
            return intersectionPts;
        }
    }
}
