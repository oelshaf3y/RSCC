using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;

namespace RSCC_GEN
{
    [TransactionAttribute(TransactionMode.Manual)]
    internal class WPCCopy : IExternalCommand
    {
        UIDocument uidoc;
        Document doc;
        XYZ dir = XYZ.Zero;
        XYZ lastDir = XYZ.Zero;
        XYZ start = XYZ.Zero;
        double rotation = 0;
        double otherRotation = 0;
        double tilt = 0;
        double otherTilt= 0;   
        List<Element> otherUnits;
        List<ElementId> selectedWPC;
        Element hostUnit;
        string selectionCheck;
        StringBuilder sb = new StringBuilder();
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uidoc = commandData.Application.ActiveUIDocument;
            doc = uidoc.Document;
            #region selection

            using (TransactionGroup tg = new TransactionGroup(doc, "Copy wpc"))
            {
                tg.Start();
                try
                {
                    WPCForm form = new WPCForm();
                    form.ShowDialog();
                    if (form.DialogResult == DialogResult.Cancel) return Result.Cancelled;
                    if (form.radioButton1.Checked)
                    {
                        selectionCheck = "HS";
                    }
                    else
                    {
                        selectionCheck = "SS";
                    }
                    otherUnits = new List<Element>();
                    selectedWPC = uidoc.Selection.PickObjects(ObjectType.Element, new SelectionFilter(BuiltInCategory.OST_IOSModelGroups), "Pick Model Group")
                        .Select(x => doc.GetElement(x).Id).ToList();
                    hostUnit = doc.GetElement(uidoc.Selection.PickObject(ObjectType.Element, new SelectionFilter(BuiltInCategory.OST_StructuralFoundation, selectionCheck), "Select Host Element"));
                    otherUnits.Add(hostUnit);
                    hideAll(hostUnit);
                    otherUnits.AddRange(
                        uidoc.Selection.PickObjects(ObjectType.Element, new SelectionFilter(BuiltInCategory.OST_StructuralFoundation, selectionCheck), "Select other units")
                        .Select(x => doc.GetElement(x))
                        .OrderBy(x => ((LocationPoint)x.Location).Point.DistanceTo(((LocationPoint)hostUnit.Location).Point)).ToList());
                    otherUnits = otherUnits.Distinct().ToList();
                }
                catch
                {
                    doc.print("Either you canceled or something went wrong!");
                    return Result.Cancelled;
                }
                #endregion
                List<Line> lines = new List<Line>();

                for (int i = 0; i < otherUnits.Count; i++)
                {
                    selectedWPC = copyElements(selectedWPC, otherUnits, i);
                }
                tg.Assimilate();
            }
            //doc.print(sb);
            return Result.Succeeded;
        }

        private List<ElementId> copyElements(List<ElementId> selectedWPC, List<Element> hostUnits, int index)
        {
            Solid solid = doc.getSolid(hostUnits[index]);
            Face hostFace = getTopFace(doc.getSolid(hostUnits[index]));
            bool inBound = true;
            bool lastUnit = false;
            while (inBound)
            {
                List<Double> Xs = new List<Double>();
                List<Double> Ys = new List<Double>();
                List<Double> Zs = new List<Double>();

                foreach (Element el in selectedWPC.Select(x => doc.GetElement(x)))
                {
                    BoundingBoxXYZ bx = el.get_BoundingBox(null);
                    Xs.Add(bx.Min.X);
                    Xs.Add(bx.Max.X);
                    Ys.Add(bx.Min.Y);
                    Ys.Add(bx.Max.Y);
                    Zs.Add(bx.Min.Z);
                    Zs.Add(bx.Max.Z);

                }
                XYZ min, max, mid;
                min = new XYZ(Xs.OrderBy(x => x).First(), Ys.OrderBy(x => x).First(), Zs.OrderBy(x => x).First());
                max = new XYZ(Xs.OrderByDescending(x => x).First(), Ys.OrderByDescending(x => x).First(), Zs.OrderByDescending(x => x).First());
                mid = 0.5 * (min + max);
                if (start == XYZ.Zero) start = mid;
                Line temp = Line.CreateUnbound(mid, XYZ.BasisZ);
                //if (MessageBox.Show("continue ?", "Title", MessageBoxButtons.YesNo) == DialogResult.No)
                //{
                //    break;
                //}
                if (hostFace.Intersect(temp) == SetComparisonResult.Disjoint)
                {
                    inBound = false;
                }
                else
                {

                    Line l = null;

                    foreach (EdgeArray ea in hostFace.EdgeLoops)
                    {
                        foreach (Edge edge in ea)
                        {
                            if (edge.AsCurve() as Line == null) continue;
                            if (edge.AsCurve().Length > 1500 / 304.8)
                            {
                                l = edge.AsCurve() as Line;
                                break;
                            }
                        }
                        if (l != null) break;
                    }
                    if (lastUnit)
                    {
                        dir = lastDir;
                    }
                    else
                    {
                        dir = l.Direction;
                        if (index != hostUnits.Count - 1)
                        {


                            Element nextUnit = hostUnits[index + 1];
                            XYZ location = ((LocationPoint)hostUnits[index].Location).Point;
                            XYZ otherLocation = ((LocationPoint)nextUnit.Location).Point;
                            XYZ tempDir = otherLocation - location;
                            if (dir.AngleTo(tempDir) > Math.PI && dir.AngleTo(tempDir) < 3 / 2 * Math.PI)
                            {
                                dir = l.Direction.Negate();
                                tilt = Math.Asin((l.GetEndPoint(0).Z - l.GetEndPoint(1).Z) / l.Length);
                            }
                            else
                            {
                                tilt = Math.Asin((l.GetEndPoint(1).Z - l.GetEndPoint(0).Z) / l.Length);

                            }
                        }
                        else
                        {
                            doc.print("The script can't seem to find the proper direction to copy!\nplease pick a point away from the precast unit which will host the elements towards the right direction.");
                            XYZ p = uidoc.Selection.PickPoint();
                            XYZ location = ((LocationPoint)hostUnits[index].Location).Point;
                            XYZ tempDir = p - location;
                            if (dir.AngleTo(tempDir) > Math.PI && dir.AngleTo(tempDir) < 3 / 2 * Math.PI)
                            {
                                dir = l.Direction.Negate();
                                tilt = Math.Asin((l.GetEndPoint(0).Z - l.GetEndPoint(1).Z) / l.Length);
                            }
                            else
                            {
                                tilt = Math.Asin((l.GetEndPoint(1).Z - l.GetEndPoint(0).Z) / l.Length);
                            }
                            lastDir = dir;
                            lastUnit = true;
                        }
                    }
                    //doc.print("got here");
                    using (Transaction tr = new Transaction(doc, "copy"))
                    {
                        tr.Start();
                        int mod = 1;
                        //DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel)).SetShape(new List<GeometryObject> { Line.CreateBound(mid, mid.Add(dir * 1000 / 304.8)) });
                        if (Math.Abs(start.DistanceTo(mid.Add(3 * dir))) > Math.Abs(start.DistanceTo(mid.Add(3 * dir.Negate()))))
                        {
                            dir = dir.Negate();
                            mod = -1;
                        }
                        selectedWPC = ElementTransformUtils.CopyElements(doc, selectedWPC, 1000 / 304.8 * dir).ToList();
                        rotation = ((LocationPoint)hostUnits[index].Location).Rotation;
                        if (index < hostUnits.Count - 1 && hostFace.Intersect(Line.CreateUnbound(mid.Add(1000 / 304.8 * dir), XYZ.BasisZ)) == SetComparisonResult.Disjoint)
                        {
                            rotation = ((LocationPoint)hostUnits[index + 1].Location).Rotation;
                            ElementTransformUtils.RotateElements(doc, selectedWPC, Line.CreateUnbound(mid, XYZ.BasisZ), rotation - otherRotation);
                            ElementTransformUtils.RotateElements(doc,selectedWPC, Line.CreateUnbound(mid,new XYZ(-dir.Y,dir.X,dir.Z)),mod *(tilt - otherTilt));
                        }
                        else
                        {

                            if (otherRotation == 0 || rotation - otherRotation == 0)
                            {
                                otherRotation = rotation;
                            }
                            else
                            {

                                ElementTransformUtils.RotateElements(doc, selectedWPC, Line.CreateUnbound(mid, XYZ.BasisZ), rotation - otherRotation);
                                ElementTransformUtils.RotateElements(doc, selectedWPC, Line.CreateUnbound(mid, new XYZ(-dir.Y, dir.X, dir.Z)), mod * (tilt - otherTilt));

                            }
                        }
                        otherRotation = rotation;
                        otherTilt = tilt;
                        lastDir = dir;
                        sb.AppendLine((tilt * 180 / Math.PI).ToString());
                        tr.Commit();
                        tr.Dispose();
                    }

                }
            }
            return selectedWPC;
        }
        private Face getTopFace(Solid s)
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
        private void hideAll(Element hostUnit)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc, doc.ActiveView.Id);
            using (Transaction tr = new Transaction(doc, "Isolate Elements"))
            {
                tr.Start();
                foreach (var v in collector)
                {
                    if (!v.Name.ToLower().Contains(selectionCheck.ToLower().Trim()) && v.Name.ToLower() != selectionCheck.ToLower().Trim())
                    {
                        doc.ActiveView.HideElementTemporary(v.Id);
                    }
                }
                doc.ActiveView.HideElementTemporary(hostUnit.Id);
                tr.Commit();
                tr.Dispose();
            }
        }
    }

    class SelectionFilter : ISelectionFilter
    {
        BuiltInCategory category;
        string selectionCheck;
        public SelectionFilter(BuiltInCategory category, string selectionCheck = null)
        {
            this.category = category;
            this.selectionCheck = selectionCheck;
        }
        public bool AllowElement(Element elem)
        {
            if (selectionCheck != null) return elem.Category.BuiltInCategory == category && elem.Name.Contains(selectionCheck);
            return elem.Category.BuiltInCategory == category;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
