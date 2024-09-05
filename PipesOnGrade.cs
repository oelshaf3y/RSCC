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
        List<CurveElement> detialLines;
        List<Element> pipeTypes, systems, fittings;
        ElementId pipeTypeId, systemId;
        Level level;
        Element toposolid;
        StringBuilder sb;
        double distAcc, angleAcc, offset;
        MEPSize pipeSize;
        bool isLinked;
        //FilteredElementCollector
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uidoc = commandData.Application.ActiveUIDocument;
            doc = uidoc.Document;
            if (!(doc.ActiveView.ViewType is ViewType.FloorPlan || doc.ActiveView.ViewType is ViewType.EngineeringPlan))
            {
                doc.print("active view must be a plan view");
                return Result.Failed;
            }
            pipeTypes = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PipeCurves).WhereElementIsElementType().Where(x => !x.Name.Contains("Default")).ToList();
            systems = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PipingSystem).WhereElementIsElementType().ToList();
            fittings = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PipeFitting).WhereElementIsElementType().ToList();

            POGform pogform = new POGform(doc, pipeTypes, systems);

            pogform.ShowDialog();
            if (pogform.DialogResult == System.Windows.Forms.DialogResult.Cancel) return Result.Cancelled;
            distAcc = Convert.ToDouble(pogform.textBox1.Text).meterToFeet();
            angleAcc = Convert.ToDouble(pogform.textBox2.Text).toRad();
            pipeSize = pogform.sizes[pogform.comboBox3.SelectedIndex];
            offset = Convert.ToDouble(pogform.textBox4.Text).mmToFeet();
            pipeTypeId = pipeTypes[pogform.comboBox1.SelectedIndex].Id;
            sb = new StringBuilder();
            level = doc.ActiveView.LevelId.IntegerValue != -1 ? doc.GetElement(doc.ActiveView.LevelId) as Level : new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType().First() as Level;
            systemId = systems[pogform.comboBox2.SelectedIndex].Id;
            List<ElementId> ids = new List<ElementId>();
            try
            {
                detialLines = uidoc.Selection.PickObjects(ObjectType.Element, new RSCCSelectionFilter(x => x.GetType() == typeof(DetailLine) || x.GetType() == typeof(DetailArc), true), "pick detial line")
                    .Select(x => doc.GetElement(x)).Cast<CurveElement>().ToList();
                if (pogform.checkBox1.Checked)
                {

                    Reference reference = uidoc.Selection.PickObjects(ObjectType.LinkedElement,
                        new RSCCSelectionFilter(x => x.GetType() == typeof(Toposolid), false), "Pick Toposolid").FirstOrDefault();
                    RevitLinkInstance rli = doc.GetElement(reference.ElementId) as RevitLinkInstance;
                    linkedDoc = rli.GetLinkDocument();
                    //doc.print(linkedDoc.PathName);
                    toposolid = linkedDoc.GetElement(reference.LinkedElementId);
                }
                else
                {
                    toposolid = doc.GetElement(uidoc.Selection.PickObjects(ObjectType.Element,
                        new RSCCSelectionFilter((x) => x.GetType() == typeof(Toposolid), true), "Pick Toposolid").FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                return Result.Cancelled;
            }
            List<XYZ> points = new List<XYZ>();
            foreach (CurveElement line in detialLines)
            {
                List<XYZ> projPts = new List<XYZ>();
                if (line.GetType() == typeof(DetailArc)) projPts = getProjectionPoints(toposolid, points: line.GeometryCurve.Tessellate().ToList());
                else projPts = getProjectionPoints(toposolid, line.GeometryCurve);
                if (projPts.Count == 0) continue;
                points.AddRange(projPts);
            }
            using (Transaction tr = new Transaction(doc, "create pipe"))
            {

                tr.Start();
                int b = 0;
                int a = 0;
                XYZ dir = XYZ.Zero;
                XYZ stPt = XYZ.Zero;
                XYZ endPt = XYZ.Zero;
                for (int i = 0; i < points.Count - 1; i++)
                {
                    if (i == 0)
                    {
                        dir = (points[i + 1] - points[i]).Normalize();
                        a = i;
                    }
                    else
                    {
                        if (
                            ((points[i + 1] - points[i]).Normalize().AngleTo(dir) >= angleAcc &&
                            (points[i + 1] - points[i]).Normalize().AngleTo(dir) <= Math.PI - angleAcc) || i == points.Count - 2)
                        {
                            sb.AppendLine((points[i + 1] - points[i]).Normalize().AngleTo(dir).toDegree().ToString());
                            b = i;
                            stPt = new XYZ(points[a].X, points[a].Y, points[a].Z + (pipeSize.OuterDiameter / 2) + offset);
                            endPt = new XYZ(points[b].X, points[b].Y, points[b].Z + (pipeSize.OuterDiameter / 2) + offset);

                            Element pipe = Pipe.Create(doc, systemId, pipeTypeId, level.Id, stPt, endPt);
                            pipe.LookupParameter("Diameter").Set(pipeSize.NominalDiameter);
                            ids.Add(pipe.Id);
                            dir = (points[i + 1] - points[i]).Normalize();
                            a = b;
                        }
                    }
                }
                for (int i = 0; i < ids.Count - 1; i++)
                {
                    Pipe pipe = doc.GetElement(ids[i]) as Pipe;
                    Pipe otherPipe = doc.GetElement(ids[i + 1]) as Pipe;
                    ConnectorSet connectorSet = pipe.ConnectorManager.Connectors;
                    ConnectorSet otherConnectorSet = otherPipe.ConnectorManager.Connectors;
                    foreach (Connector connector in connectorSet)
                    {
                        if (
                            (connector.CoordinateSystem.Origin.IsAlmostEqualTo(points[0].Add(((pipeSize.OuterDiameter / 2) + offset) * XYZ.BasisZ))) ||
                            connector.IsConnected
                            ) continue;
                        foreach (Connector otherConnector in otherConnectorSet)
                        {
                            if (otherConnector.IsConnected) continue;
                            if (otherConnector.CoordinateSystem.Origin.DistanceTo(connector.CoordinateSystem.Origin) <= distAcc)
                            {
                                try
                                {

                                    doc.Create.NewElbowFitting(connector, otherConnector);
                                }
                                catch
                                {

                                }
                            }
                        }

                    }
                }
                tr.Commit();
                tr.Dispose();
            }
            uidoc.Selection.SetElementIds(ids);
            return Result.Succeeded;
        }

        List<XYZ> getProjectionPoints(Element toposolid, Curve curve = null, List<XYZ> points = null)
        {
            List<XYZ> intersectionPts = new List<XYZ>();
            Solid solid;
            if (linkedDoc != null)
            {
                solid = linkedDoc.getSolid(toposolid);
            }
            else
            {
                solid = doc.getSolid(toposolid);
            }

            if (curve == null)
            {
                foreach (XYZ temp in points)
                {
                    SolidCurveIntersectionOptions tempOptions = new SolidCurveIntersectionOptions();
                    tempOptions.ResultType = SolidCurveIntersectionMode.CurveSegmentsInside;
                    SolidCurveIntersection tempSCI = solid.IntersectWithCurve(Line.CreateBound(temp.Add(500 * XYZ.BasisZ), temp.Add(-500 * XYZ.BasisZ)), tempOptions);
                    if (tempSCI == null || tempSCI.SegmentCount == 0)
                    {
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
                return intersectionPts;
            }
            for (double i = 0; i < curve.Length; i += distAcc)
            {
                XYZ temp = curve.Evaluate(i, false);
                SolidCurveIntersectionOptions tempOptions = new SolidCurveIntersectionOptions();
                tempOptions.ResultType = SolidCurveIntersectionMode.CurveSegmentsInside;
                SolidCurveIntersection tempSCI = solid.IntersectWithCurve(Line.CreateBound(temp.Add(500 * XYZ.BasisZ), temp.Add(-500 * XYZ.BasisZ)), tempOptions);
                if (tempSCI == null || tempSCI.SegmentCount == 0)
                {
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
            return intersectionPts;

        }

    }
}
