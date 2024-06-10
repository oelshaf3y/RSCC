using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI.Selection;
using System.Windows.Media;

namespace RSCC_GEN
{
    [TransactionAttribute(TransactionMode.Manual)]
    internal class PipesOnGrade : IExternalCommand
    {
        UIDocument uidoc;
        Document doc;
        List<DetailLine> detialLines;
        ElementId pipeTypeId;
        Level level;
        ElementId systemId;
        Element toposolid;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uidoc = commandData.Application.ActiveUIDocument;
            doc = uidoc.Document;
            pipeTypeId = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PipeCurves).WhereElementIsElementType().First().Id;
            level = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType().First() as Level;
            systemId = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PipingSystem).WhereElementIsElementType().First().Id;
            //doc.print(systemId);
            //doc.print(pipeTypeId);
            //doc.print(level.Id);
            try
            {

                detialLines = uidoc.Selection.PickObjects(ObjectType.Element, new RSCCSelectionFilter(x => x.GetType() == typeof(DetailLine), true), "pick detial line")
                    .Select(x => doc.GetElement(x)).Cast<DetailLine>().ToList();
                toposolid = doc.GetElement(
                  uidoc.Selection.PickObjects(ObjectType.LinkedElement, new RSCCSelectionFilter(x => x.GetType() == typeof(Toposolid), false), "Pick Toposolid").FirstOrDefault().ElementId
                    );

            }
            catch (Exception ex)
            {
                doc.print(ex.Message);
            }
            List<XYZ> points = new List<XYZ>();
            using (TransactionGroup tg = new TransactionGroup(doc, "Create Solid"))
            {
                tg.Start();
                foreach (DetailLine line in detialLines)
                {
                    points.Concat(getProjectionPoints(line.GeometryCurve, toposolid));
                }
                tg.RollBack();
            }
            using (Transaction tr = new Transaction(doc, "create pipe"))
            {
                tr.Start();
                for (int i = 0; i < points.Count-1; i++)
                {

                    Pipe.Create(doc, systemId, pipeTypeId, level.Id, points[i], points[i+1]);
                }
                tr.Commit();
                tr.Dispose();
            }

            return Result.Succeeded;
        }

        List<XYZ> getProjectionPoints(Curve curve, Element toposolid)
        {
            List<XYZ> intersectionPts = new List<XYZ>();
            Solid solid = doc.getSolid(toposolid);
            XYZ startPt = curve.GetEndPoint(0);
            XYZ endPt = curve.GetEndPoint(1);
            double offset = 500;
            XYZ p1 = new XYZ(startPt.X, startPt.Y, startPt.Z + offset);
            XYZ p2 = new XYZ(startPt.X, startPt.Y, startPt.Z - offset);
            XYZ p3 = new XYZ(endPt.X, endPt.Y, endPt.Z + offset);
            XYZ p4 = new XYZ(endPt.X, endPt.Y, endPt.Z - offset);
            XYZ normal = (endPt - startPt).CrossProduct(new XYZ(0, 0, -1)).Normalize();
            XYZ origin = curve.Evaluate(0.5, true);
            CurveLoop loop = new CurveLoop();
            loop.Append(Line.CreateBound(p1, p2));
            loop.Append(Line.CreateBound(p2, p3));
            loop.Append(Line.CreateBound(p3, p4));
            loop.Append(Line.CreateBound(p4, p1));
            Solid thinSolid;
            using (Transaction tr = new Transaction(doc, "create solid"))
            {
                tr.Start();
                thinSolid = GeometryCreationUtilities.CreateExtrusionGeometry(new List<CurveLoop> { loop }, normal, 1);
                tr.Commit();
                tr.Dispose();
            }
            BooleanOperationsUtils.ExecuteBooleanOperationModifyingOriginalSolid(thinSolid, solid, BooleanOperationsType.Difference);
            foreach (Face face in thinSolid.Faces)
            {
                PlanarFace pf = face as PlanarFace;
                if (pf != null)
                {
                    if (pf.Origin.Z > origin.Z)
                    {
                        foreach (Edge edge in pf.EdgeLoops)
                        {
                            if (edge.AsCurve().Length >= curve.Length) continue;
                            if (((Line)edge.AsCurve()).Direction.Z == 1 || ((Line)edge.AsCurve()).Direction.Z == -1) continue;
                            intersectionPts.Add(edge.AsCurve().GetEndPoint(0));
                            intersectionPts.Add(edge.AsCurve().GetEndPoint(1));
                        }
                    }
                }
            }
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
