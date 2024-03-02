using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RSCC_GEN.RebarAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSCC_GEN
{
    [TransactionAttribute(TransactionMode.Manual)]
    internal class RebarAnno : IExternalCommand
    {
        UIDocument uidoc;
        Document doc;
        List<Element> selectedSlabs;
        List<Level> levels;
        List<FamilySymbol> tags;
        List<AnnotationSymbolType> annoSymbols;
        AnnotationSymbolType famSymbol;
        List<ViewFamilyType> viewFamilyTypes;
        List<View> viewTemplates;
        List<Tuple<Element, View, double>> slabViews;
        List<View> CreatedViews;
        StringBuilder sb;
        RebarForm form;
        List<Tuple<string, List<string>>> paramsAndFilters;
        List<string> filters;
        ElementId viewTemplateId;
        List<List<Parameter>> parameters;
        List<Parameter> elementParam;
        ElementId tagId;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uidoc = commandData.Application.ActiveUIDocument;
            doc = uidoc.Document;
            sb = new StringBuilder();

            CreatedViews = new List<View>();
            parameters = new List<List<Parameter>>();

            //geting required data (levels, view templates, tags and view types)
            levels = new FilteredElementCollector(doc).OfClass(typeof(Level)).Cast<Level>().ToList();
            viewFamilyTypes = new FilteredElementCollector(doc).OfClass(typeof(ViewFamilyType)).Cast<ViewFamilyType>().ToList();
            viewTemplates = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views)
                .Cast<View>().Where(x => x.IsTemplate).ToList();
            tags = new FilteredElementCollector(doc)
                            .OfCategory(BuiltInCategory.OST_RebarTags).Where(x => x is FamilySymbol)
                            .Cast<FamilySymbol>().ToList();
            annoSymbols = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_GenericAnnotation)
                .WhereElementIsElementType()
                .Cast<AnnotationSymbolType>().ToList();
            //collecting all rebar parameters and its' values
            foreach (Element rebar in new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rebar).WhereElementIsNotElementType().ToElements())
            {
                elementParam = rebar.GetOrderedParameters().ToList();
                parameters.Add(elementParam.OrderBy(x => x.Definition.Name).ToList());
            }

            //initiate the form
            form = new RebarForm(parameters, tags, viewTemplates, annoSymbols);
            form.ShowDialog();

            if (form.DialogResult == System.Windows.Forms.DialogResult.Cancel) return Result.Cancelled;

            //pulling required info from the form
            viewTemplateId = viewTemplates.Where(x => x.Name == form.viewTemplatesCombo.SelectedItem.ToString()).First().Id;
            tagId = tags.First(x => x.Name.Contains(form.tagCombo.SelectedItem.ToString())).Id;
            paramsAndFilters = GetFilters();
            filters = createFilters(paramsAndFilters.Select(x => x.Item2).ToList(), 0, new List<string>());
            if (form.checkBox1.Checked)
            {
                famSymbol = annoSymbols.Where(x => x.Name == form.ballCombo.SelectedItem.ToString()).FirstOrDefault();
                famSymbol.Activate();
            }
            #region selection
            try
            {

                if (uidoc.Selection.GetElementIds().Any())
                {
                    selectedSlabs = uidoc.Selection.GetElementIds().Select(x => doc.GetElement(x)).Where(x => x is Floor).ToList();
                }
                else
                {
                    selectedSlabs = uidoc.Selection.PickObjects(ObjectType.Element, new FloorSelectionFilter(), "Pick Slabs Only").Select(x => doc.GetElement(x)).ToList();
                }
            }
            catch
            {
                return Result.Failed;
            }
            #endregion

            slabViews = new List<Tuple<Element, View, double>>();
            Level slabLevel = null;

            using (TransactionGroup Tg = new TransactionGroup(doc, "Create Views and annotations"))
            {
                Tg.Start();

                #region create views
                using (Transaction tr = new Transaction(doc, "Create the view"))
                {
                    tr.Start();
                    int type = 1;
                    foreach (Element slab in selectedSlabs)
                    {
                        double angle = getAngle(slab);
                        slabLevel = levels.Where(x => x.Id.IntegerValue == slab.LevelId.IntegerValue).First();

                        foreach (string filter in filters)
                        {
                            View slabView = ViewPlan.Create(doc, viewFamilyTypes.Where(x => x.ViewFamily == ViewFamily.StructuralPlan).First().Id, slabLevel.Id);
                            CreatedViews.Add(slabView);
                            //TODO change view template to be user choice. 
                            slabView.ViewTemplateId = viewTemplateId;
                            slabView.CropBox = slab.get_BoundingBox(null);
                            slabView.CropBoxActive = true;
                            slabView.CropBoxVisible = false;
                            slabView.Name = slab.Name + " - Type(" + type.ToString() + ") - " + filter;

                            // store a view in a list to rotate it later.
                            slabViews.Add(new Tuple<Element, View, double>(slab, slabView, angle));
                        }
                        type++;
                        break;
                    }
                    tr.Commit();
                }
                #endregion

                #region rotate the crop box
                foreach (var tuple in slabViews)
                {
                    Element slab = tuple.Item1;
                    View view = tuple.Item2;
                    Element cropBoxElement;
                    //getting the visible element without the hidden crop box element
                    List<ElementId> visibleElements = new FilteredElementCollector(doc, view.Id).ToElementIds().ToList();

                    using (Transaction t1 = new Transaction(doc, "get crop region"))
                    {
                        t1.Start();
                        view.CropBoxVisible = true;
                        t1.Commit();
                    }

                    //getting all element including the crop box element and then excluding the visible elements to get the crop box element
                    FilteredElementCollector allElements = new FilteredElementCollector(doc, view.Id).Excluding(visibleElements);
                    cropBoxElement = allElements.FirstElement();

                    using (Transaction t2 = new Transaction(doc, "Rotate the view"))
                    {
                        t2.Start();
                        BoundingBoxXYZ bbx = view.CropBox;
                        XYZ center = 0.5 * (bbx.Min + bbx.Max);
                        Line axis = Line.CreateBound(center, center + XYZ.BasisZ);
                        ElementTransformUtils.RotateElement(doc, cropBoxElement.Id, axis, -tuple.Item3);
                        t2.Commit();
                        t2.Dispose();
                    }

                    #endregion

                    #region presentation and annotation
                    List<Rebar> rebarSets = new FilteredElementCollector(doc, view.Id)
                        .OfCategory(BuiltInCategory.OST_Rebar)
                        .WhereElementIsNotElementType()
                    .Cast<Rebar>().ToList();

                    foreach (Rebar rebar in rebarSets)
                    {
                        List<Line> Curves = rebar.GetCenterlineCurves(false, true, true, MultiplanarOption.IncludeOnlyPlanarCurves, 0)
                            .Cast<Line>().Where(x => Math.Abs(x.Direction.Z) < 0.7).ToList();
                        Curve barCurve = Curves.OrderByDescending(x => x.Length).First();
                        BoundingBoxXYZ bx = rebar.get_BoundingBox(null);
                        XYZ m = 0.5 * (bx.Max + bx.Min);
                        XYZ finDir = XYZ.Zero;
                        double spacing = 0;
                        Dimension newDim = null;
                        if (!checkView(rebar, view) || rebar.GetHostId().IntegerValue != slab.Id.IntegerValue) //check if the rebar matches the view criteria eg. filter:top-long
                        {
                            //if not in view => hide rebar
                            using (Transaction t4 = new Transaction(doc, "presentation"))
                            {
                                t4.Start();
                                view.HideElements(new List<ElementId> { rebar.Id });
                                t4.Commit();
                                t4.Dispose();
                            }
                        }
                        else //if in view => start presentation and annotation
                        {
                            XYZ a = XYZ.Zero, b = XYZ.Zero;
                            if (rebar.Quantity > 1)
                            {
                                using (Transaction t4 = new Transaction(doc, "presentation"))
                                {
                                    t4.Start();
                                    try
                                    {
                                        rebar.SetPresentationMode(view, RebarPresentationMode.Middle);

                                    }
                                    catch (Exception ex)
                                    {
                                        sb.AppendLine(ex.Message);
                                        sb.AppendLine(ex.StackTrace);
                                        sb.AppendLine("");
                                    }

                                    t4.Commit();
                                    t4.Dispose();
                                }
                                XYZ dir;
                                using (Transaction t5 = new Transaction(doc, "dimension"))
                                {
                                    if (Curves.Count != 0)
                                    {

                                        Line firstCurve = Curves.OrderByDescending(x => x.Length).First();
                                        dir = view.ViewDirection.CrossProduct(new XYZ(firstCurve.Direction.X, firstCurve.Direction.Y, 0)).Normalize();
                                        MultiReferenceAnnotationType mraType = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_MultiReferenceAnnotations).Cast<MultiReferenceAnnotationType>().First();
                                        MultiReferenceAnnotationOptions options = new MultiReferenceAnnotationOptions(mraType);
                                        options.SetElementsToDimension(new List<ElementId> { rebar.Id });
                                        //TODO

                                        options.DimensionLineDirection = new XYZ(firstCurve.Direction.Y, -firstCurve.Direction.X, firstCurve.Direction.Z);
                                        options.DimensionPlaneNormal = new XYZ(0, 0, 1);
                                        options.TagHeadPosition = firstCurve.GetEndPoint(0);

                                        //to get the references for the dimension line
                                        //we create a multi reference tag to get the 1st and the last bars,
                                        //then we remove the tag and use it's dimension references to create new dim.
                                        try
                                        {

                                            t5.Start();
                                            MultiReferenceAnnotation MRA = MultiReferenceAnnotation.Create(doc, view.Id, options);
                                            Dimension dim = doc.GetElement(MRA.DimensionId) as Dimension;
                                            Reference first = dim.References.get_Item(0);
                                            Reference last = dim.References.get_Item(dim.References.Size - 1);

                                            t5.RollBack();
                                            ReferenceArray referenceArray = new ReferenceArray();
                                            referenceArray.Append(first);
                                            referenceArray.Append(last);
                                            XYZ p = firstCurve.Evaluate(0.5, true);
                                            t5.Start();
                                            newDim = doc.Create.NewDimension(view, Line.CreateBound(p, p.Add(10 * dir)), referenceArray);
                                            //newDim.TextPosition
                                            if (form.checkBox1.Checked)
                                            {
                                                //here
                                                XYZ Loc = newDim.TextPosition;
                                                spacing = newDim.Value.Value / (rebar.Quantity - 1);
                                                Curve dimCurve = newDim.Curve;
                                                IList<ClosestPointsPairBetweenTwoCurves> result = new List<ClosestPointsPairBetweenTwoCurves>();
                                                barCurve.ComputeClosestPoints(dimCurve, false, false, true, out result);
                                                //RebarInSystem ris = rebar as RebarInSystem;
                                                if (result.Count == 0) continue;
                                                XYZ startPt = result.OrderBy(x => x.Distance).First().XYZPointOnFirstCurve;
                                                XYZ dimDir = ((Line)dimCurve).Direction;
                                                Line l = Line.CreateUnbound(newDim.TextPosition, dimDir);
                                                XYZ pt2 = l.Project(startPt).XYZPoint;
                                                finDir = Line.CreateBound(pt2, newDim.TextPosition).Direction;
                                                if (rebar.Quantity % 2 == 0)
                                                {
                                                    Loc = startPt.Add(((rebar.Quantity / 2) - 1) * spacing * finDir);
                                                }
                                                newDim.TextPosition = newDim.TextPosition.Add(finDir * 2 * spacing);
                                                doc.Create.NewFamilyInstance(new XYZ(Loc.X, Loc.Y, 0), famSymbol, view);
                                            }
                                            t5.Commit();
                                        }
                                        catch (Exception ex)
                                        {
                                            doc.print(ex.Message + '\n' + ex.StackTrace);
                                        }
                                    }
                                }
                            }
                            using (Transaction t6 = new Transaction(doc, "Tag Rebars"))
                            {
                                List<Subelement> subelements = rebar.GetSubelements().ToList();
                                int index = 0;
                                if (rebar.NumberOfBarPositions % 2 == 0)
                                {
                                    index = (rebar.NumberOfBarPositions / 2) - 1;
                                }
                                else
                                {
                                    index = Convert.ToInt32(Math.Ceiling(rebar.NumberOfBarPositions / 2.0)) - 1;
                                }

                                Subelement middleBar = subelements.ElementAt(index);
                                t6.Start();
                                IndependentTag tag = IndependentTag.Create(doc,
                                    tagId,
                                    view.Id,
                                    middleBar.GetReference(),
                                    false, TagOrientation.AnyModelDirection,
                                    new XYZ(m.X + 1, m.Y + 1, 0).Add(-spacing * finDir));
                                try
                                {
                                    tag.RotationAngle = Math.PI / 2 - finDir.AngleTo(view.RightDirection);
                                    if (tag.RotationAngle > Math.PI / 4)
                                    {
                                        tag.TagHeadPosition = tag.TagHeadPosition.Add(-3 * spacing * finDir);
                                    }
                                }
                                catch (Exception ex) { }

                                t6.Commit();
                                t6.Dispose();

                            }
                        }
                    }
                }
                #endregion

                #region delete empty views
                using (Transaction t7 = new Transaction(doc, "Delete empty views"))
                {
                    List<ElementId> viewsToDelete = new List<ElementId>();
                    foreach (View rftView in CreatedViews)
                    {
                        List<Rebar> reb = new List<Rebar>();
                        reb = new FilteredElementCollector(doc, rftView.Id).OfCategory(BuiltInCategory.OST_Rebar).WhereElementIsNotElementType().Cast<Rebar>().ToList();
                        if (reb.Count == 0) viewsToDelete.Add(rftView.Id);
                    }
                    t7.Start();
                    doc.Delete(viewsToDelete);
                    t7.Commit();
                    t7.Dispose();
                }
                #endregion

                Tg.Assimilate();
                Tg.Dispose();
            }

            if (sb.Length > 0)
            {

                doc.print(sb);
            }
            return Result.Succeeded;
        }

        private bool checkView(Rebar rebar, View view)
        {
            //check if the rebar matches the view criteria. eg. filters: top-long
            StringBuilder strBuilder = new StringBuilder();
            foreach (string par in paramsAndFilters.Select(x => x.Item1))
            {
                if (rebar.LookupParameter(par) == null) return false;
                strBuilder.Append(rebar.LookupParameter(par).AsString() + "-");
            }
            string temp = strBuilder.ToString();
            return view.Name.Split(' ').Any(x => x == temp.Remove(temp.Length - 1));
        }

        private List<Tuple<string, List<string>>> GetFilters()
        {
            //gets all the used parameters and its' values
            List<Tuple<string, List<string>>> layers = new List<Tuple<string, List<string>>>();
            foreach (System.Windows.Forms.UserControl control in form.panel1.Controls)
            {
                Tuple<string, List<string>> tup;
                System.Windows.Forms.ComboBox paramCombo = control.Controls.Find("parametersCombo", true).First() as System.Windows.Forms.ComboBox;
                System.Windows.Forms.Label label = control.Controls.Find("valuesLabel", true).First() as System.Windows.Forms.Label;
                tup = new Tuple<string, List<string>>(paramCombo.SelectedItem.ToString(), label.Text.Split(',').Where(x => x.Trim() != "").ToList());
                layers.Add(tup);
            }
            return layers;
        }

        private List<string> createFilters(List<List<string>> layers, int index, List<string> sentCollection)
        {
            //gets all posible combinations for the parameters value filters 
            //eg. top-x-main, top-y-add .. etc
            if (index >= layers.Count()) return sentCollection;
            List<string> currentCollection = new List<string>();
            for (int i = 0; i < layers[index].Count; i++)
            {
                string paramValue = layers[index][i];
                if (sentCollection.Count() == 0)
                {
                    currentCollection.Add(paramValue);
                }
                foreach (string sentValue in sentCollection)
                {
                    string temp = sentValue + "-" + paramValue;
                    currentCollection.Add(temp);
                }
            }
            return createFilters(layers, index + 1, currentCollection);
        }

        private double getAngle(Element element)
        {
            //get the angle between the longest curve of the slab and the x-axis
            Solid solid = doc.getSolid(element);
            Face face = solid.Faces.get_Item(0);
            double angle = 0;
            double length = double.MinValue;
            Line longestCurve = null;
            foreach (EdgeArray ea in face.EdgeLoops)
            {
                foreach (Edge edge in ea)
                {
                    if (edge.AsCurve().Length > length)
                    {
                        length = edge.AsCurve().Length;
                        longestCurve = (Line)edge.AsCurve();
                    }
                }
            }
            angle = longestCurve.Direction.AngleTo(new XYZ(1, 0, 0));
            return Math.Asin(Math.Abs(Math.Sin(angle))) * -1;
        }
    }
}
