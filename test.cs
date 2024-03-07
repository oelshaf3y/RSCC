using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace RSCC_GEN
{
    [TransactionAttribute(TransactionMode.Manual)]
    internal class test : IExternalCommand
    {
        public UIDocument uidoc { get; set; }
        public Document doc { get; set; }
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uidoc = commandData.Application.ActiveUIDocument;
            doc = uidoc.Document;
            List<Double> Xs = new List<Double>();
            List<Double> Ys = new List<Double>();
            List<Double> Zs = new List<Double>();
            List<Element> selected = uidoc.Selection.PickObjects(ObjectType.Element).Select(x => doc.GetElement(x)).ToList();

            foreach (Element el in selected)
            {
                BoundingBoxXYZ bx = el.get_BoundingBox(null);
                Xs.Add(bx.Min.X);
                Xs.Add(bx.Max.X);
                Ys.Add(bx.Min.Y);
                Ys.Add(bx.Max.Y);
                Zs.Add(bx.Min.Z);
                Zs.Add(bx.Max.Z);

            }
            XYZ min, max;
            min = new XYZ(Xs.OrderBy(x => x).First(), Ys.OrderBy(x => x).First(), Zs.OrderBy(x => x).First());
            max = new XYZ(Xs.OrderByDescending(x => x).First(), Ys.OrderByDescending(x => x).First(), Zs.OrderByDescending(x => x).First());
            Element elem = doc.GetElement(uidoc.Selection.PickObject(ObjectType.Element).ElementId);
            Solid s = doc.getSolid(elem);
            Face f = getTopFace(s);
            drawTopFace(f);
            XYZ avg = 0.5 * (min + max);
            Line l = Line.CreateBound(avg.Add(10 * XYZ.BasisZ), avg.Add(-10 * XYZ.BasisZ));
            using (Transaction tr = new Transaction(doc, "Draw Line"))
            {
                tr.Start();
                DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel)).SetShape(new List<GeometryObject> { l });
                tr.Commit();
                tr.Dispose();
            }
            if (f.Intersect(l) == SetComparisonResult.Disjoint) doc.print("no intersection");
            return Result.Succeeded;
        }
        public void drawBBX()
        {
            List<Element> selected = uidoc.Selection.PickObjects(ObjectType.Element).Select(x => doc.GetElement(x)).ToList();
            List<Double> Xs = new List<Double>();
            List<Double> Ys = new List<Double>();
            List<Double> Zs = new List<Double>();

            foreach (Element el in selected)
            {
                BoundingBoxXYZ bx = el.get_BoundingBox(null);
                Xs.Add(bx.Min.X);
                Xs.Add(bx.Max.X);
                Ys.Add(bx.Min.Y);
                Ys.Add(bx.Max.Y);
                Zs.Add(bx.Min.Z);
                Zs.Add(bx.Max.Z);

            }
            XYZ p2, p3, p4, p5, p6, p8;
            XYZ min, max;
            min = new XYZ(Xs.OrderBy(x => x).First(), Ys.OrderBy(x => x).First(), Zs.OrderBy(x => x).First());
            max = new XYZ(Xs.OrderByDescending(x => x).First(), Ys.OrderByDescending(x => x).First(), Zs.OrderByDescending(x => x).First());
            p2 = new XYZ(max.X, min.Y, min.Z);
            p3 = new XYZ(max.X, max.Y, min.Z);
            p4 = new XYZ(min.X, max.Y, min.Z);
            p5 = new XYZ(min.X, min.Y, max.Z);
            p6 = new XYZ(max.X, min.Y, max.Z);
            p8 = new XYZ(min.X, max.Y, max.Z);
            Line l1 = Line.CreateBound(min, p2);
            Line l2 = Line.CreateBound(p2, p3);
            Line l3 = Line.CreateBound(p3, p4);
            Line l4 = Line.CreateBound(p4, min);
            Line l5 = Line.CreateBound(p5, p6);
            Line l6 = Line.CreateBound(p6, max);
            Line l7 = Line.CreateBound(max, p8);
            Line l8 = Line.CreateBound(p8, p5);
            Line l9 = Line.CreateBound(min, p5);
            Line l10 = Line.CreateBound(p2, p6);
            Line l11 = Line.CreateBound(p3, max);
            Line l12 = Line.CreateBound(p4, p8);
            using (Transaction tr = new Transaction(doc, "Draw bbx"))
            {
                tr.Start();
                DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel)).SetShape(new List<GeometryObject> { l1, l2, l3, l4, l5, l6, l7, l8, l9, l10, l11, l12 });
                tr.Commit();
                tr.Dispose();
            }
        }
        public Face getTopFace(Solid s)
        {
            List<PlanarFace> faces = new List<PlanarFace>();
            foreach (Face face in s.Faces)
            {
                PlanarFace pf = face as PlanarFace;
                if (pf == null) continue;
                if (Math.Abs(pf.FaceNormal.AngleTo(new XYZ(0, 0, 1))) < Math.PI / 18)
                {
                    faces.Add(pf);
                }
            }
            if (faces.Count == 0) return null;
            return faces.OrderByDescending(x => x.Origin.Z)?.First();
        }
        public void drawTopFace(Face face)
        {
            //Element selected = doc.GetElement(uidoc.Selection.PickObject(ObjectType.Element).ElementId);
            //Solid solid = doc.getSolid(selected);
            //Face face = getTopFace(solid);
            if (face == null) return;
            List<Line> lines = new List<Line>();
            foreach (EdgeArray ea in face.EdgeLoops)
            {
                foreach (Edge edge in ea)
                {
                    if (edge.AsCurve() as Line == null) continue;
                    lines.Add(edge.AsCurve() as Line);
                }
            }
            using (Transaction tr = new Transaction(doc, "Draw top face"))
            {
                tr.Start();
                if (lines.Count > 0)
                {

                    DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel)).SetShape(lines.ToArray());
                }
                tr.Commit();
                tr.Dispose();
            }
        }
    }

}
