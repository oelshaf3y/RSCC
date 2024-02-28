using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

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
            wallType = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsElementType()
                .Where(x => x.Name == "RSCC_UltraPly TPO Roofing Membrane_firestone").Cast<WallType>().First();
            screedRoofType = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Roofs).WhereElementIsElementType()
                .Where(x => x.Name == "RSCC_Screed_50mm").Cast<RoofType>().First();
            insulationRoofType = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Roofs)
                        .WhereElementIsElementType().Where(x => x.Name == "RSCC_Non Accessible Roof_Waterproof&Insulation_255.5mm").Cast<RoofType>().First();
            gravelRoofType = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Roofs)
                .WhereElementIsElementType().Where(x => x.Name == "RSCC_Gravel_50mm").Cast<RoofType>().First();
            screedFloorType = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Floors)
                        .WhereElementIsElementType().Where(x => x.Name == "RSCC_Screed_50mm").Cast<FloorType>().First();
            insulationFoorType = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Floors)
                .WhereElementIsElementType().Where(x => x.Name == "RSCC_Non Accessible Roof_Waterproof&Insulation_255.5mm").Cast<FloorType>().First();
            gravelFloorType = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Floors)
                .WhereElementIsElementType().Where(x => x.Name == "RSCC_Gravel_50mm").Cast<FloorType>().First();
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
                List<View> views = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views).WhereElementIsNotElementType().Cast<View>().ToList();
                using (Transaction tr = new Transaction(doc, "Delete Views"))
                {
                    tr.Start();
                    foreach (View view in views)
                    {
                        if (checkView(view))
                        {
                            try
                            {

                                doc.Delete(view.Id);
                            }
                            catch { }
                        }
                    }
                    tr.Commit();
                }
                tg.Assimilate();
            }


            uidoc.Selection.SetElementIds(ids.Distinct().ToArray());
            return Result.Succeeded;
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
                    foreach (ModelLine modLine in curveArr)
                    {
                        Curve curve = modLine.GeometryCurve;
                        Line c = curve as Line;
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
                        //sb.AppendLine(curve.ToString());
                        Line c = curve as Line;
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
