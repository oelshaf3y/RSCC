using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSCC_GEN
{
    [TransactionAttribute(TransactionMode.Manual)]
    internal class Insulation : IExternalCommand
    {
        UIDocument uidoc;
        Document doc;
        FloorType screedFloorType, insulationFoorType, gravelFloorType;
        WallType wallType;
        RoofType screedRoofType, insulationRoofType, gravelRoofType;
        List<ElementId> ids;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uidoc = commandData.Application.ActiveUIDocument;
            doc = uidoc.Document;
            List<Element> elems = new List<Element>();
            try
            {

                wallType = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsElementType()
                    .Where(x => x.Name == "RSCC_UltraPly TPO Roofing Membrane_firestone").Cast<WallType>().First();
                screedRoofType = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Roofs).WhereElementIsElementType()
                    .Where(x => x.Name == "RSCC_Screed_50mm").Cast<RoofType>().First();
                insulationRoofType = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Roofs)
                            .WhereElementIsElementType().Where(x => x.Name == "RSCC_Non Accessible Roof_Waterproof&Insulation_155.5mm").Cast<RoofType>().First();
                gravelRoofType = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Roofs)
                    .WhereElementIsElementType().Where(x => x.Name == "RSCC_Gravel_50mm").Cast<RoofType>().First();
                screedFloorType = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Floors)
                            .WhereElementIsElementType().Where(x => x.Name == "RSCC_Screed_50mm").Cast<FloorType>().First();
                insulationFoorType = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Floors)
                    .WhereElementIsElementType().Where(x => x.Name == "RSCC_Non Accessible Roof_Waterproof&Insulation_155.5mm").Cast<FloorType>().First();
                gravelFloorType = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Floors)
                    .WhereElementIsElementType().Where(x => x.Name == "RSCC_Gravel_50mm").Cast<FloorType>().First();
            }
            catch
            {
                doc.print("Please Load necessary Family Types and Profile");
                return Result.Cancelled;
            }
            if (uidoc.Selection.GetElementIds().Count > 0)
            {
                elems = uidoc.Selection.GetElementIds().Select(x => doc.GetElement(x)).ToList();
                ids = uidoc.Selection.GetElementIds().ToList();
            }
            else
            {
                elems = uidoc.Selection.PickObjects(ObjectType.Element, "Pick Floor or Roof!").Select(x => doc.GetElement(x)).ToList();
                ids = elems.Select(x => x.Id).ToList();
            }
            using (TransactionGroup tg = new TransactionGroup(doc, "Insulation"))
            {
                tg.Start();
                foreach (Element elem in elems)
                {
                    if (elem is Floor) floorInsulation(elem as Floor);
                    if (elem is FootPrintRoof) roofInsulation(elem as FootPrintRoof);
                }
                deleteViewsAndLinks();
                tg.Assimilate();
            }



            return Result.Succeeded;
        }

        private void deleteViewsAndLinks()
        {
            List<View> views = new FilteredElementCollector(doc)
                   .OfCategory(BuiltInCategory.OST_Views).WhereElementIsNotElementType().Cast<View>()
                   .Where(x => x.ViewType != ViewType.Legend)
                   .ToList();
            using (Transaction tr = new Transaction(doc, "Delete Views"))
            {
                tr.Start();
                foreach (View view in views)
                {
                    try
                    {
                        if (checkView(view))
                        {
                            doc.Delete(view.Id);
                        }
                    }
                    catch { }
                }
                uidoc.Selection.SetElementIds(ids.Distinct().ToArray());
                doc.ActiveView.HideElementsTemporary(ids.Distinct().ToArray());
                List<RevitLinkType> links = new FilteredElementCollector(doc).OfClass(typeof(RevitLinkType)).Cast<RevitLinkType>().ToList();
                StringBuilder sb = new StringBuilder();
                try
                {
                    List<ElementId> ids = links.Select(x => x.GetRootId()).Distinct().ToList();
                    foreach (RevitLinkType linkType in ids.Select(x => doc.GetElement(x) as RevitLinkType))
                    {

                        doc.Delete(linkType.Id);
                    }
                }
                catch (Exception ex)
                {
                    sb.AppendLine(ex.ToString());
                    sb.AppendLine(ex.Message);
                    sb.AppendLine(ex.StackTrace);
                    doc.print(sb);
                }

                tr.Commit();
            }
        }

        private void roofInsulation(FootPrintRoof roof)
        {
            double thick = 0;
            XYZ mid = 0.5 * (roof.get_BoundingBox(null).Max + roof.get_BoundingBox(null).Min);
            double baseOffset = roof.LookupParameter("Base Offset From Level").AsDouble();

            RoofType selectedRoofType = doc.GetElement(roof.GetTypeId()) as RoofType;
            if (selectedRoofType.GetCompoundStructure().VariableLayerIndex != -1)
            {
                thick = selectedRoofType.GetCompoundStructure().GetLayerWidth(selectedRoofType.GetCompoundStructure().VariableLayerIndex);
            }
            else
            {
                thick = 50 / 304.8;
            }
            using (Transaction tr = new Transaction(doc, "Insulation"))
            {
                tr.Start();
                CompoundStructureLayer layer = screedRoofType.GetCompoundStructure().GetLayers().First();
                CompoundStructure CS = screedRoofType.GetCompoundStructure();
                CS.SetLayerWidth(layer.LayerId, thick);
                screedRoofType.SetCompoundStructure(CS);
                screedRoofType.Name = screedRoofType.Name.Replace("50mm", (thick * 304.8).ToString() + "mm");
                roof.ChangeTypeId(screedRoofType.Id);
                roof.LookupParameter("Base Offset From Level").Set(baseOffset);
                FootPrintRoof Insu = doc.GetElement(ElementTransformUtils.CopyElement(doc, roof.Id, new XYZ(0, 0, thick)).First()) as FootPrintRoof;
                ids.Add(Insu.Id);
                Insu.ChangeTypeId(insulationRoofType.Id);
                FootPrintRoof gravel = doc.GetElement(ElementTransformUtils.CopyElement(doc, Insu.Id, new XYZ(0, 0, 155.5 / 304.8)).First()) as FootPrintRoof;
                ids.Add(gravel.Id);
                gravel.ChangeTypeId(gravelRoofType.Id);
                foreach (ModelCurveArray curveArr in roof.GetProfiles())
                {
                    foreach (CurveElement modLine in curveArr)
                    {
                        if (modLine.GeometryCurve is Arc)
                        {
                            Arc arc = modLine.GeometryCurve as Arc;
                            XYZ center = arc.Center;
                            double r = arc.Radius;
                            XYZ p1 = arc.GetEndPoint(0);
                            XYZ p2 = arc.GetEndPoint(1);
                            Line line1 = Line.CreateBound(p1, center);
                            Line line2 = Line.CreateBound(p2, center);
                            Line temp1 = Line.CreateBound(p1, mid);
                            Line temp2 = Line.CreateBound(p2, mid);
                            XYZ np1, np2, np3;
                            double nr = 0;
                            if (line1.Direction.AngleTo(temp1.Direction) <= Math.PI / 2)
                            {
                                np1 = p1.Add(0.75 / 304.8 * line1.Direction);
                                nr = r - 0.75 / 304.8;
                            }
                            else
                            {
                                np1 = p1.Add(0.75 / 304.8 * line1.Direction.Negate());
                                nr = r + 0.75 / 304.8;
                            }
                            if (line2.Direction.AngleTo(temp2.Direction) <= Math.PI / 2)
                            {
                                np2 = p2.Add(0.75 / 304.8 * line2.Direction);
                            }
                            else
                            {
                                np2 = p2.Add(0.75 / 304.8 * line2.Direction.Negate());
                            }
                            double angle = line1.Direction.Negate().AngleTo(line2.Direction.Negate());
                            XYZ temp = new XYZ(-line1.Direction.Y, line1.Direction.X, line1.Direction.Z);
                            np3 = center.Add(nr * Math.Cos(angle / 2) * line1.Direction.Negate()).Add(nr * Math.Sin(angle / 2) * temp);
                            Line tempLine = Line.CreateBound(center, np3.Add((np3 - center)));
                            if (tempLine.Intersect(arc) != SetComparisonResult.Disjoint)
                            {

                                Arc a = Arc.Create(np1, np2, np3);
                                if (a == null) continue;
                                Wall wall = Wall.Create(doc, a, wallType.Id, roof.LevelId, 455.5 / 304.8, Insu.LookupParameter("Base Offset From Level").AsDouble(), false, false);
                                ids.Add(wall.Id);
                            }
                            else
                            {
                                np3 = center.Add(nr * Math.Cos(angle / 2) * line1.Direction.Negate()).Add(nr * Math.Sin(angle / 2) * temp.Negate());
                                Arc a = Arc.Create(np1, np2, np3);
                                if (a == null) continue;
                                Wall wall = Wall.Create(doc, a, wallType.Id, roof.LevelId, 455.5 / 304.8, Insu.LookupParameter("Base Offset From Level").AsDouble(), false, false);
                                ids.Add(wall.Id);
                            }
                        }
                        else if (modLine.GeometryCurve is Curve)
                        {

                            Curve curve = modLine.GeometryCurve;
                            Line c = curve as Line;
                            if (c == null) continue;
                            XYZ perpendicular = new XYZ(-c.Direction.Y, c.Direction.X, c.Direction.Z);
                            Line temp = Line.CreateBound(c.Evaluate(0.5, true), mid);
                            Line nc;
                            if (perpendicular.AngleTo(temp.Direction) > Math.PI / 2)
                            {
                                nc = Line.CreateBound(curve.GetEndPoint(0).Add(perpendicular.Negate() * 0.75 / 304.8), curve.GetEndPoint(1).Add(perpendicular.Negate() * 0.75 / 304.8));
                            }
                            else
                            {
                                nc = Line.CreateBound(curve.GetEndPoint(0).Add(perpendicular * 0.75 / 304.8), curve.GetEndPoint(1).Add(perpendicular * 0.75 / 304.8));

                            }
                            Wall wall = Wall.Create(doc, nc, wallType.Id, roof.LevelId, 455.5 / 304.8, Insu.LookupParameter("Base Offset From Level").AsDouble(), false, false);
                            ids.Add(wall.Id);
                        }

                    }
                }
                tr.Commit();
                tr.Dispose();
            }
        }

        void floorInsulation(Floor floor)
        {
            double thick = 0;
            double up = 0;
            XYZ mid = 0.5 * (floor.get_BoundingBox(null).Max + floor.get_BoundingBox(null).Min);
            Solid solid = doc.getSolid(floor);
            double prevZ = (solid.Faces.get_Item(1) as PlanarFace).Origin.Z;
            FloorType selectedFloorType = doc.GetElement(floor.GetTypeId()) as FloorType;
            if (selectedFloorType.GetCompoundStructure().VariableLayerIndex != -1)
            {
                thick = selectedFloorType.GetCompoundStructure().GetLayerWidth(selectedFloorType.GetCompoundStructure().VariableLayerIndex);
            }
            else
            {
                thick = 50 / 304.8;
            }

            using (Transaction tr = new Transaction(doc, "Change Type"))
            {
                tr.Start();

                CompoundStructureLayer layer = screedFloorType.GetCompoundStructure().GetLayers().First();
                CompoundStructure CS = screedFloorType.GetCompoundStructure();
                CS.SetLayerWidth(layer.LayerId, thick);
                screedFloorType.SetCompoundStructure(CS);
                screedFloorType.Name = screedFloorType.Name.Replace("50mm", (thick * 304.8).ToString() + "mm");
                ElementId nId = floor.ChangeTypeId(screedFloorType.Id);
                Solid solid2 = doc.getSolid(floor);
                double newZ = (solid2.Faces.get_Item(1) as PlanarFace).Origin.Z;
                up = newZ - prevZ;
                floor.LookupParameter("Height Offset From Level").Set(floor.LookupParameter("Height Offset From Level").AsDouble() - up);
                Floor Insu = doc.GetElement(ElementTransformUtils.CopyElement(doc, floor.Id, new XYZ(0, 0, 155.5 / 304.8)).First()) as Floor;
                ids.Add(Insu.Id);
                Insu.ChangeTypeId(insulationFoorType.Id);
                Floor gravel = doc.GetElement(ElementTransformUtils.CopyElement(doc, Insu.Id, new XYZ(0, 0, 50 / 304.8)).First()) as Floor;
                ids.Add(gravel.Id);
                gravel.ChangeTypeId(gravelFloorType.Id);
                Sketch sketch = doc.GetElement(floor.SketchId) as Sketch;
                foreach (CurveArray curveArray in sketch.Profile)
                {
                    foreach (Curve curve in curveArray)
                    {
                        if (curve is Arc)
                        {
                            Arc arc = curve as Arc;
                            XYZ center = arc.Center;
                            double r = arc.Radius;
                            XYZ p1 = arc.GetEndPoint(0);
                            XYZ p2 = arc.GetEndPoint(1);
                            Line line1 = Line.CreateBound(p1, center);
                            Line line2 = Line.CreateBound(p2, center);
                            Line temp1 = Line.CreateBound(p1, mid);
                            Line temp2 = Line.CreateBound(p2, mid);
                            XYZ np1, np2, np3;
                            double nr = 0;
                            if (line1.Direction.AngleTo(temp1.Direction) <= Math.PI / 2)
                            {
                                np1 = p1.Add(0.75 / 304.8 * line1.Direction);
                                nr = r - 0.75 / 304.8;
                            }
                            else
                            {
                                np1 = p1.Add(0.75 / 304.8 * line1.Direction.Negate());
                                nr = r + 0.75 / 304.8;
                            }
                            if (line2.Direction.AngleTo(temp2.Direction) <= Math.PI / 2)
                            {
                                np2 = p2.Add(0.75 / 304.8 * line2.Direction);
                            }
                            else
                            {
                                np2 = p2.Add(0.75 / 304.8 * line2.Direction.Negate());
                            }
                            double angle = line1.Direction.Negate().AngleTo(line2.Direction.Negate());
                            XYZ temp = new XYZ(-line1.Direction.Y, line1.Direction.X, line1.Direction.Z);
                            np3 = center.Add(nr * Math.Cos(angle / 2) * line1.Direction.Negate()).Add(nr * Math.Sin(angle / 2) * temp);
                            Line tempLine = Line.CreateBound(center, np3.Add((np3 - center)));
                            if (tempLine.Intersect(arc) != SetComparisonResult.Disjoint)
                            {

                                Arc a = Arc.Create(np1, np2, np3);
                                if (a == null) continue;
                                Wall wall = Wall.Create(doc, a, wallType.Id, floor.LevelId, 455.5 / 304.8, floor.LookupParameter("Height Offset From Level").AsDouble(), false, false);
                                ids.Add(wall.Id);
                            }
                            else
                            {
                                np3 = center.Add(nr * Math.Cos(angle / 2) * line1.Direction.Negate()).Add(nr * Math.Sin(angle / 2) * temp.Negate());
                                Arc a = Arc.Create(np1, np2, np3);
                                if (a == null) continue;
                                Wall wall = Wall.Create(doc, a, wallType.Id, floor.LevelId, 455.5 / 304.8, floor.LookupParameter("Height Offset From Level").AsDouble(), false, false);
                                ids.Add(wall.Id);
                            }

                        }
                        else
                        {

                            Line c = curve as Line;
                            if (c == null) continue;
                            XYZ perpendicular = new XYZ(-c.Direction.Y, c.Direction.X, c.Direction.Z);
                            Line temp = Line.CreateBound(c.Evaluate(0.5, true), mid);
                            Line nc;
                            if (perpendicular.AngleTo(temp.Direction) > Math.PI / 2)
                            {
                                nc = Line.CreateBound(curve.GetEndPoint(0).Add(perpendicular.Negate() * 0.75 / 304.8), curve.GetEndPoint(1).Add(perpendicular.Negate() * 0.75 / 304.8));
                            }
                            else
                            {
                                nc = Line.CreateBound(curve.GetEndPoint(0).Add(perpendicular * 0.75 / 304.8), curve.GetEndPoint(1).Add(perpendicular * 0.75 / 304.8));

                            }
                            Wall wall = Wall.Create(doc, nc, wallType.Id, floor.LevelId, 455.5 / 304.8, floor.LookupParameter("Height Offset From Level").AsDouble(), false, false);
                            ids.Add(wall.Id);
                        }
                    }
                }
                //doc.print(sb.ToString());
                tr.Commit();
            }
        }

        bool checkView(View view)
        {
            if (view.Id.IntegerValue == doc.ActiveView.Id.IntegerValue) return false;
            string name = view.Name.ToLower();
            if (name.Contains("export") || name.Contains("bim") || name.Contains("navis")) return false;
            else return true;
        }
    }


}
