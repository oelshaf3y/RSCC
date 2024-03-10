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
    internal class Jubail : IExternalCommand
    {
        public UIDocument uidoc { get; set; }
        public Document doc { get; set; }
        public List<ViewFamilyType> viewFamilyTypes { get; set; }
        public List<View> viewTemplates { get; set; }
        public List<Level> levels { get; set; }
        public double topFaceAngle { get; set; }
        public double botFaceAngle { get; set; }
        ViewSection section, section2, section3, section4;
        Solid s;
        Face topFace, botFace;
        double height;
        ViewSheet sh;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uidoc = commandData.Application.ActiveUIDocument;
            doc = uidoc.Document;
            levels = new FilteredElementCollector(doc).OfClass(typeof(Level)).Cast<Level>().ToList();
            viewFamilyTypes = new FilteredElementCollector(doc).OfClass(typeof(ViewFamilyType)).Cast<ViewFamilyType>().ToList();
            viewTemplates = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views)
                .Cast<View>().Where(x => x.IsTemplate).ToList();
            Element selected;
            try
            {

            selected= doc.GetElement(uidoc.Selection.PickObject(ObjectType.Element, "Pick Block"));
            }
            catch
            {
                return Result.Cancelled;
            }
            s = doc.getSolid(selected);
            BoundingBoxXYZ bx = selected.get_BoundingBox(null);
            height = bx.Max.Z - bx.Min.Z;
            if (bx == null) doc.print("Null");
            topFace = s.getFace("top");
            botFace = s.getFace("bot");

            View blockTopPlan, blockBotPlan;
            topFaceAngle = getAngle(topFace);
            botFaceAngle = getAngle(botFace);
            FilteredElementCollector visibleColl;
            Element cropBox;
            using (TransactionGroup tg = new TransactionGroup(doc, "Create View"))
            {
                tg.Start();
                create1stSection();
                using (Transaction tr = new Transaction(doc, "Create View"))
                {
                    tr.Start();


                    blockTopPlan = ViewPlan.Create(doc, viewFamilyTypes.Where(x => x.ViewFamily == ViewFamily.StructuralPlan)
                        .First(x => !x.Name.ToLower().Contains("up")).Id, selected.LevelId);
                    blockTopPlan.Scale = 25;
                    blockTopPlan.DetailLevel = ViewDetailLevel.Fine;
                    blockTopPlan.DisplayStyle = DisplayStyle.Realistic;
                    blockTopPlan.CropBox = bx;
                    blockTopPlan.CropBoxActive = true;
                    //blockTopPlan.ViewTemplateId = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views)
                    //    .Cast<View>().Where(x => x.IsTemplate && x.Name.ToLower().Contains("top")).First().Id;
                    blockBotPlan = ViewPlan.Create(doc, viewFamilyTypes.Where(x => x.ViewFamily == ViewFamily.StructuralPlan)
                        .First(x => x.Name.ToLower().Contains("up")).Id, selected.LevelId);
                    blockBotPlan.Scale = 25;
                    blockBotPlan.DetailLevel = ViewDetailLevel.Fine;
                    blockBotPlan.DisplayStyle = DisplayStyle.Realistic;
                    blockBotPlan.CropBox = bx;
                    blockBotPlan.CropBoxActive = true;
                    tr.Commit();
                }
                using (TransactionGroup tg2 = new TransactionGroup(doc, "Get CropBox"))
                {
                    tg2.Start();
                    using (Transaction tr = new Transaction(doc, "Hide crop Region"))
                    {
                        tr.Start();
                        blockTopPlan.CropBoxVisible = false;
                        tr.Commit();
                        visibleColl = new FilteredElementCollector(doc, blockTopPlan.Id);
                        tr.Start();
                        blockTopPlan.CropBoxVisible = true;
                        tr.Commit();
                        FilteredElementCollector allColl = new FilteredElementCollector(doc, blockTopPlan.Id);
                        cropBox = allColl.Excluding(visibleColl.ToElementIds()).FirstElement();
                    }
                    tg2.RollBack();
                }

                using (Transaction tr = new Transaction(doc, "Rotate"))
                {
                    tr.Start();
                    blockTopPlan.CropBoxActive = true;
                    blockTopPlan.CropBoxVisible = true;
                    tr.Commit();
                }

                using (Transaction tr = new Transaction(doc, "Sheet"))
                {
                    tr.Start();
                    ElementId tb = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_TitleBlocks).WhereElementIsElementType().Where(x => x.Name == "Title Block - Jubail").First().Id;
                    sh = ViewSheet.Create(doc, tb);
                    double xcord = (blockTopPlan.CropBox.Max.X - blockTopPlan.CropBox.Min.X) / blockTopPlan.Scale + 50 / 304.8;
                    double ycord = (blockTopPlan.CropBox.Max.Y - blockTopPlan.CropBox.Min.Y) / blockTopPlan.Scale;
                    Viewport vp = Viewport.Create(doc, sh.Id, blockTopPlan.Id, new XYZ(xcord, ycord, 0));
                    xcord += 400 / 304.8 + (blockBotPlan.CropBox.Max.X - blockBotPlan.CropBox.Min.X) / blockBotPlan.Scale;
                    Viewport.Create(doc, sh.Id, blockBotPlan.Id, new XYZ(xcord, ycord, 0));
                    xcord = (blockTopPlan.CropBox.Max.X - blockTopPlan.CropBox.Min.X) / blockTopPlan.Scale + 50 / 304.8;
                    ycord += Math.Max((blockBotPlan.CropBox.Max.Y - blockBotPlan.CropBox.Min.Y) / blockBotPlan.Scale, (blockBotPlan.CropBox.Max.Y - blockBotPlan.CropBox.Min.Y) / blockBotPlan.Scale);
                    ycord += Math.Max(
                       Math.Max((section.CropBox.Max.Y - section.CropBox.Min.Y) / section.Scale,
                       (section2.CropBox.Max.Y - section2.CropBox.Min.Y) / section2.Scale),
                      Math.Max((section3.CropBox.Max.Y - section3.CropBox.Min.Y) / section3.Scale,
                       (section4.CropBox.Max.Y - section4.CropBox.Min.Y) / section4.Scale)
                        );
                    Viewport.Create(doc, sh.Id, section.Id, new XYZ(xcord, ycord, 0));
                    xcord += (section.CropBox.Max.Y - section.CropBox.Min.Y) / section.Scale;
                    xcord += 50 / 304.8 + (section2.CropBox.Max.Y - section2.CropBox.Min.Y) / section2.Scale;
                    Viewport.Create(doc, sh.Id, section2.Id, new XYZ(xcord, ycord, 0));
                    xcord += (section2.CropBox.Max.Y - section2.CropBox.Min.Y) / section2.Scale;

                    xcord += 50 / 304.8 + (section3.CropBox.Max.Y - section3.CropBox.Min.Y) / section3.Scale;
                    Viewport.Create(doc, sh.Id, section3.Id, new XYZ(xcord, ycord, 0));
                    xcord += (section3.CropBox.Max.Y - section3.CropBox.Min.Y) / section3.Scale;

                    xcord += 50 / 304.8 + (section4.CropBox.Max.Y - section4.CropBox.Min.Y) / section4.Scale;
                    Viewport.Create(doc, sh.Id, section4.Id, new XYZ(xcord, ycord, 0));
                    tr.Commit();
                }
                tg.Assimilate();
            }
            if (cropBox == null)
            {



                uidoc.RequestViewChange(sh as View);
                return Result.Succeeded;
            }
            using (Transaction tr = new Transaction(doc, "Rotate"))
            {
                tr.Start();
                ElementTransformUtils.RotateElement(doc, cropBox.Id, Line.CreateUnbound(0.5 * (bx.Max + bx.Min), XYZ.BasisZ), topFaceAngle);
                tr.Commit();
            }
            return Result.Succeeded;
        }

        private void create1stSection()
        {
            List<Line> lines = new List<Line>();
            XYZ origin = XYZ.Zero;
            int c = 0;
            foreach (EdgeArray ea in botFace.EdgeLoops)
            {
                foreach (Edge edge in ea)
                {
                    if (edge.AsCurve() as Line == null) continue;
                    lines.Add(edge.AsCurve() as Line);
                    origin += (edge.AsCurve() as Line).Origin;
                    c++;
                }
            }
            origin = origin / c;
            Line longest = lines.OrderByDescending(x => x.Length).FirstOrDefault();
            if (longest == null) return;
            XYZ lineDir = longest.Direction.Normalize();
            XYZ perpendicular = new XYZ(-lineDir.Y, lineDir.X, lineDir.Z);
            XYZ midPoint = longest.Evaluate(0.5, true);
            XYZ offset = midPoint.Add(perpendicular);
            Line temp = Line.CreateUnbound(offset, XYZ.BasisZ);
            if (botFace.Intersect(temp) == SetComparisonResult.Disjoint) perpendicular = perpendicular.Negate();
            double width = Math.Abs(midPoint.DistanceTo(origin)) * 2;
            double length = longest.Length;
            Line aligned = Line.CreateBound(longest.GetEndPoint(0), longest.GetEndPoint(0).Add(perpendicular * width));
            XYZ viewDir = lineDir.Normalize();
            XYZ bxOrg = longest.GetEndPoint(0).Add(perpendicular * width / 2);
            XYZ Xdir = aligned.Direction.Normalize();
            XYZ Ydir = Xdir.CrossProduct(XYZ.BasisZ);
            XYZ bbxMin = new XYZ(-width / 2, 0, 0);
            XYZ bbxMax = new XYZ(width / 2, height, length);
            Line temp2 = Line.CreateUnbound(bxOrg.Add(bbxMax), XYZ.BasisZ);
            if (botFace.Intersect(temp2) == SetComparisonResult.Disjoint)
            {
                bxOrg = longest.GetEndPoint(1).Add(perpendicular * width / 2);
                viewDir = viewDir.Negate();
            }
            Transform transform = Transform.Identity;
            transform.Origin = bxOrg;
            transform.BasisY = XYZ.BasisZ;
            transform.BasisZ = viewDir;
            transform.BasisX = XYZ.BasisZ.CrossProduct(viewDir);
            BoundingBoxXYZ bbx = new BoundingBoxXYZ();
            bbx.Min = bbxMin;
            bbx.Max = bbxMax;
            bbx.Transform = transform;
            using (Transaction tr = new Transaction(doc, "create section"))
            {
                tr.Start();
                section = ViewSection.CreateSection(doc,
                        viewFamilyTypes.Where(x => x.ViewFamily == ViewFamily.Section).First().Id, bbx);
                section.DetailLevel = ViewDetailLevel.Fine;
                section.DisplayStyle = DisplayStyle.Realistic;
                section.Scale = 25;
                tr.Commit();
            }

            Transform transform4 = Transform.Identity;
            transform4.Origin = bxOrg.Add(length * viewDir);
            transform4.BasisY = XYZ.BasisZ;
            transform4.BasisZ = viewDir.Negate();
            transform4.BasisX = XYZ.BasisZ.CrossProduct(viewDir.Negate());
            BoundingBoxXYZ bbx4 = new BoundingBoxXYZ();
            bbx4.Min = bbxMin;
            bbx4.Max = bbxMax;
            bbx4.Transform = transform4;

            using (Transaction tr = new Transaction(doc, "create section"))
            {
                tr.Start();
                section4 = ViewSection.CreateSection(doc,
                        viewFamilyTypes.Where(x => x.ViewFamily == ViewFamily.Section).First().Id, bbx4);
                section4.DetailLevel = ViewDetailLevel.Fine;
                section4.DisplayStyle = DisplayStyle.Realistic;
                section4.Scale = 25;
                tr.Commit();
            }
            BoundingBoxXYZ bbx2 = new BoundingBoxXYZ();
            Transform transform2 = Transform.Identity;
            transform2.Origin = midPoint;
            transform2.BasisY = XYZ.BasisZ;
            transform2.BasisZ = perpendicular;
            transform2.BasisX = XYZ.BasisZ.CrossProduct(perpendicular);
            //XYZ bbxMin = new XYZ(-width / 2, 0, 0);
            //XYZ bbxMax = new XYZ(width / 2, height, length);
            bbx2.Min = new XYZ(-length / 2, 0, 0);
            bbx2.Max = new XYZ(length / 2, height, width);
            bbx2.Transform = transform2;
            using (Transaction tr = new Transaction(doc, "create section"))
            {
                tr.Start();
                section2 = ViewSection.CreateSection(doc,
                        viewFamilyTypes.Where(x => x.ViewFamily == ViewFamily.Section).First().Id, bbx2);
                section2.DetailLevel = ViewDetailLevel.Fine;
                section2.DisplayStyle = DisplayStyle.Realistic;
                section2.Scale = 25;
                tr.Commit();
            }
            BoundingBoxXYZ bbx3 = new BoundingBoxXYZ();
            Transform transform3 = Transform.Identity;
            transform3.Origin = midPoint.Add(width * perpendicular);
            transform3.BasisY = XYZ.BasisZ;
            transform3.BasisZ = perpendicular.Negate();
            transform3.BasisX = XYZ.BasisZ.CrossProduct(perpendicular.Negate());
            //XYZ bbxMin = new XYZ(-width / 2, 0, 0);
            //XYZ bbxMax = new XYZ(width / 2, height, length);
            bbx3.Min = new XYZ(-length / 2, 0, 0);
            bbx3.Max = new XYZ(length / 2, height, width);
            bbx3.Transform = transform3;
            using (Transaction tr = new Transaction(doc, "create section"))
            {
                tr.Start();
                section3 = ViewSection.CreateSection(doc,
                        viewFamilyTypes.Where(x => x.ViewFamily == ViewFamily.Section).First().Id, bbx3);
                section3.DetailLevel = ViewDetailLevel.Fine;
                section3.DisplayStyle = DisplayStyle.Realistic;
                section3.Scale = 25;
                tr.Commit();
            }
        }

        private double getAngle(Face face)
        {

            double angle = 0;

            foreach (EdgeArray ea in face.EdgeLoops)
            {
                XYZ prevEedgeDir = null;
                foreach (Edge edge in ea)
                {
                    if (edge.AsCurve() as Line == null) continue;
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
