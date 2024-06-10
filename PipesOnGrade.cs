using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSCC_GEN
{
    [TransactionAttribute(TransactionMode.Manual)]
    internal class PipesOnGrade : IExternalCommand
    {
        UIDocument uidoc;
        Document doc, linkedDoc;
        List<DetailLine> detialLines;
        ElementId pipeTypeId;
        Level level;
        ElementId systemId;
        Element toposolid;
        StringBuilder sb;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uidoc = commandData.Application.ActiveUIDocument;
            doc = uidoc.Document;
            pipeTypeId = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PipeCurves).WhereElementIsElementType().First().Id;
            level = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType().First() as Level;
            systemId = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PipingSystem).WhereElementIsElementType().First().Id;
            sb = new StringBuilder();
            List<ElementId> ids = new List<ElementId>();
            //doc.print(systemId);
            //doc.print(pipeTypeId);
            //doc.print(level.Id);
            try
            {

                detialLines = uidoc.Selection.PickObjects(ObjectType.Element, new RSCCSelectionFilter(x => x.GetType() == typeof(DetailLine), true), "pick detial line")
                    .Select(x => doc.GetElement(x)).Cast<DetailLine>().ToList();
                Reference reference = uidoc.Selection.PickObjects(ObjectType.LinkedElement,
                    new RSCCSelectionFilter(x => x.GetType() == typeof(Toposolid), false), "Pick Toposolid").FirstOrDefault();
                RevitLinkInstance rli = doc.GetElement(reference.ElementId) as RevitLinkInstance;
                linkedDoc = rli.GetLinkDocument();
                //doc.print(linkedDoc.PathName);
                toposolid = linkedDoc.GetElement(reference.LinkedElementId);
            }
            catch (Exception ex)
            {
                doc.print(ex.StackTrace);
            }
            List<XYZ> points = new List<XYZ>();
            foreach (DetailLine line in detialLines)
            {
                List<XYZ> projPts = getProjectionPoints(line.GeometryCurve, toposolid);
                if (projPts.Count == 0) continue;
                points.AddRange(projPts);
            }
            doc.print(points.Count.ToString());
            using (Transaction tr = new Transaction(doc, "create pipe"))
            {
                tr.Start();
                for (int i = 0; i < points.Count - 1; i++)
                {

                    ids.Add(Pipe.Create(doc, systemId, pipeTypeId, level.Id, points[i], points[i + 1]).Id);
                }
                tr.Commit();
                tr.Dispose();
            }
            doc.print(sb);
            uidoc.Selection.SetElementIds(ids);
            return Result.Succeeded;
        }

        List<XYZ> getProjectionPoints(Curve curve, Element toposolid)
        {
            List<XYZ> intersectionPts = new List<XYZ>();
            Solid solid = doc.getSolid(toposolid);
            //if (solid == null) doc.print("Null");
            //doc.print(sci.ResultType);
            for (int i = 0; i < curve.Length; i++)
            {
                XYZ temp = curve.Evaluate(i, false);
                SolidCurveIntersectionOptions tempOptions = new SolidCurveIntersectionOptions();
                tempOptions.ResultType = SolidCurveIntersectionMode.CurveSegmentsInside;
                SolidCurveIntersection tempSCI = solid.IntersectWithCurve(Line.CreateBound(temp, new XYZ(temp.X, temp.Y, temp.Z - 500)), tempOptions);
                if (tempSCI == null || tempSCI.SegmentCount == 0)
                {
                    sb.AppendLine(i + " (" + temp.ToString() + ")");
                    continue;
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

            }
            sb.AppendLine("Count= " + intersectionPts.Count.ToString());
            return intersectionPts;
        }

    }
    internal class RSCCSelectionFilter : ISelectionFilter
    {
        public Document doc;
        Func<Element, bool> filter;
        bool isNative;
        public RSCCSelectionFilter(Func<Element, bool> filter, bool isNative)
        {
            this.filter = filter;
            this.isNative = isNative;
        }
        public bool AllowElement(Element elem)
        {
            if (isNative)
            {
                return filter(elem);
            }
            doc = ((RevitLinkInstance)elem).GetLinkDocument();
            return true;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            Element elem = doc.GetElement(reference.LinkedElementId);
            return filter(elem);
        }
    }
}
