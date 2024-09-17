using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Excel = Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Application = Autodesk.Revit.ApplicationServices.Application;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Group = Autodesk.Revit.DB.Group;
using Autodesk.Revit.DB.Structure;

namespace RSCC_GEN
{
    [TransactionAttribute(TransactionMode.Manual)]
    internal class OnGoing : IExternalCommand
    {
        public UIDocument uidoc { get; set; }
        public Document doc { get; set; }
        public UIApplication uiapp { get; set; }
        public Application application { get; set; }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uiapp = commandData.Application;
            uidoc = commandData.Application.ActiveUIDocument;
            doc = uidoc.Document;
            application = uiapp.Application;

            #region delete rooms
            //FilteredElementCollector coll = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms);
            //int count = coll.Count();
            //using (Transaction tr = new Transaction(doc, "delete rooms"))
            //{
            //    tr.Start();
            //    doc.Delete(coll.Select(x => x.Id).ToList());

            //    tr.Commit();
            //}
            //doc.print("total of " + count + " Rooms Deleted");
            #endregion

            #region delete hvac zones
            //FilteredElementCollector hvac = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_HVAC_Zones);
            //using (Transaction tr = new Transaction(doc, "Mena beysaba7"))
            //{
            //    tr.Start();
            //    foreach (var t in hvac.ToElements())
            //    {
            //        try
            //        {

            //            if (t != null)
            //                doc.Delete(t.Id);
            //        }
            //        catch (Exception ex)
            //        {
            //            doc.print(ex.StackTrace);
            //        }
            //    }
            //    tr.Commit();
            //    tr.Dispose();
            //}
            #endregion

            #region define parameters

            //string spfile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "SPF.txt");
            //File.Create(spfile).Dispose();

            //application.SharedParametersFilename = spfile;
            //DefinitionFile defFile = application.OpenSharedParameterFile();
            //if (defFile == null)
            //{
            //    TaskDialog.Show("Error", "Failed to open shared parameter file.");
            //    return Result.Failed;
            //}

            //DefinitionGroup defGroup = defFile.Groups.Create("Omar");
            //ExternalDefinitionCreationOptions options = new ExternalDefinitionCreationOptions("OMAR", SpecTypeId.String.Text);
            //Definition definition = defGroup.Definitions.Create(options);
            //Category strFound = doc.Settings.Categories.get_Item(BuiltInCategory.OST_StructuralFoundation);

            //using (Transaction tr = new Transaction(doc, "Create Shared Parameter"))
            //{
            //    tr.Start();
            //    {
            //        CategorySet catSet = application.Create.NewCategorySet();
            //        foreach (Category cat in doc.Settings.Categories)
            //        {
            //            if (!cat.AllowsBoundParameters) continue;
            //            catSet.Insert(cat);
            //        }
            //        try
            //        {

            //            InstanceBinding binding = application.Create.NewInstanceBinding(catSet);
            //            BindingMap bm = doc.ParameterBindings;
            //            bool res = bm.Insert(definition, binding, BuiltInParameterGroup.PG_TEXT);
            //        }
            //        catch (Exception ex)
            //        {
            //        }

            //        tr.Commit();
            //    }

            //}
            #endregion

            #region get pile lengths

            //List<Element> piles = uidoc.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element, "select Piles")
            //    .Select(x => doc.GetElement(x)).ToList();
            //StringBuilder sb = new StringBuilder();
            //double totalLength = 0;
            //foreach (Element e in piles)
            //{
            //    double depth = e.LookupParameter("Sea Bed Depth").AsDouble();
            //    double pipeDepth = e.LookupParameter("Pile Depth").AsDouble();
            //    sb.AppendLine("Pipe length=" + Math.Round(Ninja.feetToMeter(depth + pipeDepth)));
            //    totalLength += Math.Round(Ninja.feetToMeter(depth + pipeDepth));
            //} 
            //sb.AppendLine("total length= "+totalLength.ToString());
            //System.Windows.Forms.Clipboard.SetText(sb.ToString());

            #endregion

            #region ungroup

            //FilteredElementCollector FEC = new FilteredElementCollector(doc).WhereElementIsNotElementType();
            //using (Transaction tr = new Transaction(doc, "UnGroup Elements"))
            //{
            //    tr.Start();

            //    foreach (Element elem in FEC)
            //    {
            //        if (elem is Group)
            //        {
            //            Group group = (Group)elem;
            //            group.UngroupMembers();
            //        }
            //    }
            //    tr.Commit();
            //    tr.Dispose();
            //}

            #endregion

            #region RFT Weight
            double vol = 0;
            List<Element> selection = uidoc.Selection.GetElementIds().Select(x=>doc.GetElement(x)).ToList();
            if (selection.Count > 0)
            {
                selection = selection.Where(x=>x is Rebar).ToList();

                foreach (Element element in selection)
                {
                    Rebar rebar = element as Rebar;
                    vol += rebar.Volume;
                }

            }
            Element Host = doc.GetElement(((Rebar)selection.First()).GetHostId());
            uidoc.Selection.SetElementIds(new List<ElementId> { Host.Id });
            Solid s = doc.getSolid(Host);
            double rebWeight = vol * 0.0283168 * 7850;
            double hostVolume = s.Volume * 0.0283168;
            doc.print("W= "+rebWeight+"\nHost Volume ="+hostVolume+"\nW/m3="+rebWeight/hostVolume);


            #endregion

            return Result.Succeeded;
        }


    }

}
