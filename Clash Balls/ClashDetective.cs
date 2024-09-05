using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.Office.Interop.Excel;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RSCC_GEN.Clash_Balls
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class ClashDetective : IExternalCommand
    {
        UIApplication app;
        ClashesUI uiForm;
        UIDocument uidoc;
        Document doc;
        string XMLFilePath;
        StringBuilder sb;
        FilteredElementCollector families;
        FamilySymbol clashBallFamSymb;
        ProjectPosition position;
        bool isCombined;
        int index, idi, testnamei, testtypei, toli, lefti, righti, clashnamei, xi, yi, zi, convIndex;
        string elementId, testName, testType, tolerance, left, right, clashName, x, y, z, outPath;
        List<double> converters;
        double ballRadius, converter;
        List<string[]> csvData;
        Autodesk.Revit.DB.View activeView;
        Microsoft.Office.Interop.Excel.Application xlapp;
        Workbook wb;
        Worksheet sheet;
        Loading floading;
        IList<ClashPoint> clashes = null;
        ClashPoint otherPoint;
        IList<Element> clashBalls;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            converters = new List<double> { 304.8, 30.48, 0.3048, 1, 12 };
            app = commandData.Application;
            uidoc = app.ActiveUIDocument;
            doc = uidoc.Document;
            activeView = doc.ActiveView;
            sb = new StringBuilder();
            families = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol));
            clashBallFamSymb = families.Where(x => x != null && x.Name.ToLower().Contains("clash"))?.FirstOrDefault() as FamilySymbol;
            position = doc.ActiveProjectLocation.GetProjectPosition(XYZ.Zero);


            #region checker
            if (!(activeView is View3D))
            {
                doc.print("Active View must be 3D view");
                return Result.Failed;
            }

            if (clashBallFamSymb == null)
            {
                using (Transaction t = new Transaction(doc))
                {

                    t.Start("loading clash Ball");
                    try
                    {
                        doc.LoadFamily(@"H:\01-Eng\8-Others\IBIMS Addins\Omar\Families\ClashReports\ClashBall.rfa");
                        doc.Regenerate();
                        families = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol));
                        clashBallFamSymb = families.Where(x => x != null && x.Name.ToLower().Contains("clash"))?.FirstOrDefault() as FamilySymbol;
                        t.Commit();
                        t.Dispose();
                    }
                    catch (Exception ex)
                    {
                        doc.print(ex.Message);
                        t.RollBack();
                    }
                }
            }
            #endregion

            #region collectBalls
            FilteredElementCollector clashBallsFEC = new FilteredElementCollector(doc);
            if (clashBallFamSymb == null)
            {
                return readAgain(true, false);
            }
            else
            {

                IList<Element> clashBalls = clashBallsFEC.WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_GenericModel)?.ToElements()?.Where(x => x.Name == clashBallFamSymb.Name)?.ToList();
                if (clashBalls.Count > 0)
                {
                    ClashBalls cb = Collect();
                    if (cb != null)
                    {

                        updateOnEdit(cb.points);
                    }
                    else { return Result.Failed; }
                }
                #endregion
                else
                {
                    return readAgain(true, false);
                }
            }
            return Result.Succeeded;
        }

        private void updateOnEdit(IList<ClashPoint> points)
        {
            using (Transaction t = new Transaction(doc))
            {
                t.Start("update");
                foreach (ClashPoint point in points)
                {
                    Element ball = clashBalls.Where(x => x.LookupParameter("Element ID (1)").AsString() == point.elementId1)?.FirstOrDefault();
                    if (ball == null) { doc.print("null"); continue; }
                    ball.LookupParameter("Clash Name")?.Set(point.clashName);
                    ball.LookupParameter("Test Name")?.Set(point.testName);
                    ball.LookupParameter("Test Type")?.Set(point.testType);
                    ball.LookupParameter("Tolerance")?.Set(point.tolerance);
                    ball.LookupParameter("Group A")?.Set(point.left);
                    ball.LookupParameter("Groub B")?.Set(point.right);
                    ball.LookupParameter("Clash Comment").Set(point.comment);
                    int a = 0;
                    if (point.resolved)
                    {
                        a = 1;
                    }
                    else
                    {
                        a = 0;
                    }
                    ball.LookupParameter("Status (Resolved/Not)").Set(a);
                }
                t.Commit();
                t.Dispose();
            }
        }

        public ClashBalls Collect()
        {
            ClashBalls cbForm;
            FilteredElementCollector clashBallsFEC = new FilteredElementCollector(doc);
            clashBalls = clashBallsFEC.WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_GenericModel)?.ToElements()?.Where(x => x.Name == clashBallFamSymb.Name)?.ToList();
            if (clashBalls.Count > 0)
            {
                cbForm = new ClashBalls(this, app, clashBalls, true);
                cbForm.ShowDialog();
                if (cbForm.DialogResult == DialogResult.Cancel) { return null; }
                else if (cbForm.DialogResult == DialogResult.OK) { return cbForm; }
                return cbForm;
            }
            else
            {
                return null;
            }
        }

        public Result readAgain(bool ins, bool coll)
        {
            uiForm = new ClashesUI(ins, coll);
            uiForm.ShowDialog();
            if (uiForm.DialogResult == DialogResult.Cancel) { return Result.Cancelled; }


            XMLFilePath = uiForm.textBox1.Text;
            if (uiForm.clashBallCheckBox.Checked)
            {
                convIndex = uiForm.comboBox1.SelectedIndex;
                ballRadius = Convert.ToDouble(uiForm.textBox2.Text);
            }
            converter = converters.ElementAt(convIndex);
            if (clashes == null) { clashes = new List<ClashPoint>(); }
            csvData = null;
            bool isRead = readData(XMLFilePath);
            if (!isRead)
            {
                return Result.Failed;
            }
            if (uiForm.inspectCheckBox.Checked)
            {
                ClashBalls dr = inspect();
                return Result.Succeeded;
            }
            else if (uiForm.clashBallCheckBox.Checked || uiForm.create3DCheckBox.Checked)
            {
                return create3D();
            }
            if (uiForm.collectBallCheckBox.Checked)
            {
                Collect();
            }
            return Result.Succeeded;
        }
        private bool readData(string path)
        {
            #region convert file

            try
            {
                floading = new Loading();
                floading.Show();
                xlapp = new Microsoft.Office.Interop.Excel.Application();
                wb = xlapp.Workbooks.Open(path);
                sheet = wb.Worksheets[1];
                outPath = Path.GetTempFileName() + ".csv";
                wb.SaveAs(outPath, Microsoft.Office.Interop.Excel.XlFileFormat.xlCSV);
                wb.Close();
                xlapp.Quit();
                floading.Close();
            }
            catch
            {
                doc.print("error occurred");
                wb.Close();
                xlapp.Quit();
                floading.Close();
                return false;
            }
            #endregion

            #region read data

            #region get indices
            try
            {
                csvData = ReadCsvFile(outPath);
                File.Delete(outPath);
                for (int i = 0; i < csvData[1].Length; i++)
                {
                    string s = csvData[1][i];
                    if (s.Trim() == "/batchtest/clashtests/clashtest/@name")
                    {
                        testnamei = i;
                    }
                    if (s.Trim() == "/batchtest/clashtests/clashtest/clashresults/clashresult/clashobjects/clashobject/objectattribute/value/#agg")
                    {
                        idi = i;
                    }
                    if (s.Split('/').Last().Contains("@test_type"))
                    {
                        testtypei = i;
                    }
                    if (s.Split('/').Last().Contains("@tolerance"))
                    {
                        toli = i;
                    }
                    if (s.Trim() == "/batchtest/clashtests/clashtest/left/clashselection/locator")
                    {
                        lefti = i;
                    }
                    if (s.Trim() == "/batchtest/clashtests/clashtest/right/clashselection/locator")
                    {
                        righti = i;
                    }
                    if (s.Trim() == "/batchtest/clashtests/clashtest/clashresults/clashresult/@name")
                    {
                        clashnamei = i;
                    }
                    if (s.Trim().Contains("@x/#agg"))
                    {
                        xi = i;
                    }
                    if (s.Trim().Contains("@y/#agg"))
                    {
                        yi = i;
                    }
                    if (s.Trim().Contains("@z/#agg"))
                    {
                        zi = i;
                    }
                    if (testnamei > 0 && idi > 0 && testtypei > 0 && toli > 0 && lefti > 0 && righti > 0 && clashnamei > 0 && xi > 0) { break; }
                    if (i > 500) { break; }
                }
            }
            catch
            {
                doc.print("this file is currupted.");
            }
            #endregion

            #region get clashes
            for (int i = 2; i < csvData.Count; i++)
            {
                try
                {
                    elementId = csvData[i][idi];
                    testName = csvData[i][testnamei];
                    if (testName.Trim() == "" || testName == null) { break; }
                    if (elementId.Trim() == "" || elementId == null) { continue; }
                    testType = csvData[i][testtypei];
                    tolerance = csvData[i][toli];
                    left = csvData[i][lefti];
                    left = left.Split('/')?.LastOrDefault();
                    right = csvData[i][righti];
                    right = right.Split('/')?.LastOrDefault();
                    clashName = csvData[i][clashnamei];
                    x = csvData[i][xi];
                    y = csvData[i][yi];
                    z = csvData[i][zi];
                    double dx = 0;
                    bool test = double.TryParse(x, out dx);
                    if (!clashName.ToLower().Contains("clash"))
                    {
                        doc.print("Maybe this xml file is combined and you didn't check.! or maybe it's separated and you checked combined file!\n\ndouble check your export and try again.\nif you are still facing issues please contact for support.");
                        return false;
                    }

                    if (test && dx != 0)
                    {
                        XYZ clashPoint = null;
                        try
                        {
                            clashPoint = new XYZ(Convert.ToDouble(x) / converter, Convert.ToDouble(y) / converter, Convert.ToDouble(z) / converter);
                        }
                        catch
                        {
                            clashPoint = null;
                        }
                        otherPoint = new ClashPoint(testName, testType, tolerance, left, right, clashName, clashPoint, position, elementId);
                        clashes.Add(otherPoint);
                    }
                    else
                    {
                        if (clashes.Count > 0)
                        {
                            otherPoint = clashes.ElementAt(clashes.Count - 1);
                            otherPoint.setElementId(elementId);
                        }
                    }
                    sb.AppendLine(otherPoint.clashName);
                }
                catch (Exception ex)
                {
                    doc.print("Please contact for support.");
                    return false;
                }
            }
            #endregion

            #endregion

            #region getting document element
            foreach (ClashPoint clash in clashes)
            {
                int idx = 0, idy = 0;
                if (clash.elementId1 != null)
                {
                    try
                    {
                        idx = int.Parse(clash.elementId1);
                        idy = int.Parse(clash.elementId2);
                        Element e = doc.GetElement(new ElementId(idy));
                        if (e != null)
                        {
                            //if (!(e is RevitLinkInstance))
                            //{
                            clash.setId(e.Id);
                            //}

                        }
                        else
                        {
                            FamilyInstance fs = doc.GetElement(new ElementId(idy)) as FamilyInstance;
                            if (fs != null)
                            {
                                //if (!(e is RevitLinkInstance))
                                //{
                                clash.setId(fs.Id);
                                //}
                            }
                        }
                        if (clash.elementId == null)
                        {
                            Element el = doc.GetElement(new ElementId(idx));
                            if (el != null)
                            {
                                //if (!(el is RevitLinkInstance))
                                //{
                                clash.setId(el.Id);
                                //}
                            }
                            else
                            {
                                FamilyInstance fs2 = doc.GetElement(new ElementId(idx)) as FamilyInstance;
                                //if (fs2 != null)
                                //{

                                clash.setId(fs2.Id);
                                //}
                            }
                        }
                    }
                    catch
                    {

                    }
                }
                else if (clash.elementId2 != null)
                {
                    idy = int.Parse(clash.elementId2);
                    var e = doc.GetElement(new ElementId(idy));
                    if (e is Element || e is FamilyInstance)
                    {
                        clash.setId(e.Id);
                    }
                }
            }
            #endregion

            return true;
        }

        private ClashBalls inspect()
        {
            #region inspect clashes
            ClashBalls clashViewer;
            //clashViewerUi cv;
            clashViewer = new ClashBalls(this, app, clashes, false);
            clashViewer.ShowDialog();
            //cv = new clashViewerUi(uidoc, clashes.OrderBy(x => x.testName.Split(' ')[0]).ThenBy(x => Convert.ToInt32(x.clashName.Split('h')[1])).Where(x => x.elementId != null).Where(x => doc.GetElement(x.elementId) != null).ToList());
            //cv.ShowDialog();
            return clashViewer;
            #endregion
        }

        private Result create3D()
        {
            #region clash 3D views or clash balls

            using (TransactionGroup tx = new TransactionGroup(doc))
            {
                tx.Start("Clash Points");
                using (Transaction t = new Transaction(doc))
                {
                    t.Start("3D Views");
                    View3D view3D = this.activeView as View3D;

                    foreach (ClashPoint clash in clashes)
                    {
                        Element id1 = null, id2 = null, id = null;
                        int idx = 0, idy = 0;
                        if (int.TryParse(clash.elementId1, out idx))
                        {
                            id1 = doc.GetElement(new ElementId(idx));
                        }
                        if (int.TryParse(clash.elementId2, out idy))
                        {
                            id2 = doc.GetElement(new ElementId(idy));
                        }
                        BoundingBoxXYZ bx = new BoundingBoxXYZ();
                        if (id1 != null)
                        {
                            bx = id1.get_BoundingBox(view3D);
                            id = id1;
                        }
                        else if (id2 != null)
                        {
                            bx = id2.get_BoundingBox(view3D);
                            id = id2;
                        }
                        if (view3D != null)
                        {
                            #region create clash balls
                            if (uiForm.clashBallCheckBox.Checked)
                            {
                                if (clashBallFamSymb != null)
                                {
                                    clashBallFamSymb.Activate();
                                    XYZ clashPoint = new XYZ(clash.clashPoint.X - position.EastWest, clash.clashPoint.Y - position.NorthSouth, clash.clashPoint.Z - position.Elevation);
                                    if (clashPoint == null)
                                    {
                                        if (id != null) { clashPoint = getLocation(id); }
                                    }
                                    if (clashPoint != null)
                                    {
                                        FamilyInstance fi = doc.Create.NewFamilyInstance(clashPoint, clashBallFamSymb, StructuralType.NonStructural);
                                        Element ball = doc.GetElement(fi.Id);
                                        ball.LookupParameter("Clash Name").Set(clash.testName.Split('-')[0] + " " + clash.clashName);
                                        ball.LookupParameter("Element ID (1)").Set(clash.elementId1);
                                        ball.LookupParameter("Element ID (2)").Set(clash.elementId2);
                                        ball.LookupParameter("Test Name").Set(clash.testName);
                                        ball.LookupParameter("Test Type").Set(clash.testType);
                                        ball.LookupParameter("Tolerance").Set(clash.tolerance.ToString());
                                        ball.LookupParameter("Group A").Set(clash.left.Split('/').LastOrDefault());
                                        ball.LookupParameter("Group B").Set(clash.right.Split('/').LastOrDefault());
                                        ball.LookupParameter("radius").Set(ballRadius / 304.8);
                                        ball.LookupParameter("X").Set(clash.x);
                                        ball.LookupParameter("Y").Set(clash.y);
                                        ball.LookupParameter("Z").Set(clash.z);
                                    }
                                }
                            }
                            #endregion

                            #region create 3D views
                            if (uiForm.create3DCheckBox.Checked)
                            {

                                View3D v = doc.GetElement(view3D.Duplicate(ViewDuplicateOption.Duplicate)) as View3D;
                                Random rnd = new Random();
                                try
                                {

                                    v.Name = "Test no." + clash.testName.Split('-')[0] + " - " + clash.clashName;
                                }
                                catch
                                {
                                    v.Name = "Test no." + clash.testName.Split('-')[0] + " - " + clash.clashName + " " + rnd.NextDouble().ToString();

                                }
                                v.get_Parameter(BuiltInParameter.MODEL_GRAPHICS_STYLE).Set(3);
                                BoundingBoxXYZ bx3 = new BoundingBoxXYZ();
                                bx3.Min = new XYZ(bx.Min.X - 100 / 304.8, bx.Min.Y - 100 / 304.8, bx.Min.Z - 100 / 304.8);
                                bx3.Max = new XYZ(bx.Max.X + 100 / 304.8, bx.Max.Y + 100 / 304.8, bx.Max.Z + 100 / 304.8);
                                bx3.Transform = Transform.Identity;
                                v.SetSectionBox(bx3);
                            }
                            #endregion
                        }
                    }
                    t.Commit();
                    t.Dispose();
                }
                tx.Assimilate();
                tx.Dispose();
            }
            //IList<ElementId> ids = clashes.Select(x => new ElementId).ToList();

            if (uiForm.clashBallCheckBox.Checked && uiForm.create3DCheckBox.Checked) { doc.print($"Clash Balls and 3D Views created successfully for {clashes.Count} clashes."); }
            else if (uiForm.clashBallCheckBox.Checked) { doc.print($"Clash Balls created successfully for {clashes.Count} clashes."); }
            else if (uiForm.create3DCheckBox.Checked) { doc.print($"3D Views created successfully for {clashes.Count} clashes."); }
            #endregion
            return Result.Succeeded;
        }

        private XYZ getLocation(Element elem)
        {
            Location loc = elem.Location;
            if (loc is LocationPoint)
            {
                return (loc as LocationPoint).Point;
            }
            else if (loc is LocationCurve)
            {
                return (loc as LocationCurve).Curve.GetEndPoint(0);
            }
            return null;
        }
        private List<string[]> ReadCsvFile(string filePath)
        {
            List<string[]> csvData = new List<string[]>();

            using (TextFieldParser parser = new TextFieldParser(filePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    csvData.Add(fields);
                }
            }

            return csvData;
        }
    }
}
