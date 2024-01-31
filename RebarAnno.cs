using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RSCC_GEN
{
    [TransactionAttribute(TransactionMode.Manual)]
    internal class RebarAnno : IExternalCommand
    {
        UIDocument uidoc;
        Document doc;
        List<Element> slabs;
        List<Level> levels;
        List<ViewFamilyType> viewFamilyTypes;
        List<View> viewTemplates;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uidoc = commandData.Application.ActiveUIDocument;
            doc = uidoc.Document;
            levels = new FilteredElementCollector(doc).OfClass(typeof(Level)).Cast<Level>().ToList();
            viewFamilyTypes = new FilteredElementCollector(doc).OfClass(typeof(ViewFamilyType)).Cast<ViewFamilyType>().ToList();
            viewTemplates = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views)
                .Cast<View>().Where(x => x.IsTemplate).ToList();
            #region selection
            try
            {

                if (uidoc.Selection.GetElementIds().Any())
                {
                    slabs = uidoc.Selection.GetElementIds().Select(x => doc.GetElement(x)).Where(x => x is Floor).ToList();
                }
                else
                {
                    slabs = uidoc.Selection.PickObjects(ObjectType.Element, new FloorSelectionFilter(), "Pick Slabs Only").Select(x => doc.GetElement(x)).ToList();
                }
            }
            catch
            {
                return Result.Failed;
            }
            #endregion

            Level slabLevel = null;
            using (Transaction tr = new Transaction(doc, "Create View"))
            {
                tr.Start();

                foreach (Element slab in slabs)
                {
                    double angle = getAngle(slab);
                    slabLevel = levels.Where(x => x.Id.IntegerValue == slab.LevelId.IntegerValue).First();
                    View slabView = ViewPlan.Create(doc, viewFamilyTypes.Where(x => x.ViewFamily == ViewFamily.StructuralPlan).First().Id, slabLevel.Id);
                    slabView.ViewTemplateId = viewTemplates.Where(x => x.Name == "Section - RFT").First().Id;
                    BoundingBoxXYZ slabBBX = slab.get_BoundingBox(null);

                    XYZ min = slabBBX.Min;
                    XYZ max = slabBBX.Max;
                    XYZ mid = Line.CreateBound(min, max).Evaluate(0.5, true);
                    Transform rotate = Transform.CreateRotation(XYZ.BasisX, angle);
                    slabBBX.Transform = slabBBX.Transform.Multiply(rotate);
                    slabView.CropBox = slabBBX;
                    slabView.CropBoxActive = true;
                    //doc.print(angle * 180 / Math.PI);

                }
                tr.Commit();
                tr.Dispose();
            }


            return Result.Succeeded;
        }
        private double getAngle(Element element)
        {
            Solid solid = doc.getSolid(element);
            Face face = solid.Faces.get_Item(0);
            double angle = 0;

            foreach (EdgeArray ea in face.EdgeLoops)
            {
                XYZ prevEedgeDir = null;
                foreach (Edge edge in ea)
                {
                    if (prevEedgeDir != null)
                    {
                        angle = (edge.AsCurve() as Line).Direction.AngleTo(prevEedgeDir);
                        if (angle >= 75 * Math.PI / 180 && angle <= 110 * Math.PI / 180) return (edge.AsCurve() as Line).Direction.AngleTo(new XYZ(1, 0, 0));
                    }
                    prevEedgeDir = (edge.AsCurve() as Line).Direction;
                }
            }

            return 0;
        }
    }
}
