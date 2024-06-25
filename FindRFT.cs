using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSCC_GEN
{
    [TransactionAttribute(TransactionMode.Manual)]
    internal class FindRFT : IExternalCommand
    {
        UIDocument uidoc;
        Document doc;
        Rebar rebar;
        List<Rebar> rebarSets;
        List<Rebar> found;
        List<View> views;
        List<ViewSheet> sheets;
        List<string> partitions;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uidoc = commandData.Application.ActiveUIDocument;
            doc = uidoc.Document;
            found = new List<Rebar>();
            partitions = new List<string>();
            rebarSets = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rebar)
                .WhereElementIsNotElementType()
                .Cast<Rebar>()
                .ToList();
            foreach (Rebar rebar in rebarSets)
            {
                if (rebar.LookupParameter("Partition").AsValueString() != null) partitions.Add(rebar.LookupParameter("Partition").AsValueString());
            }
            partitions.Sort();
            partitions = partitions.Distinct().ToList();
            FindRFTForm form = new FindRFTForm(partitions);
            form.ShowDialog();
            if (form.DialogResult == System.Windows.Forms.DialogResult.Cancel) return Result.Cancelled;
            views = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views)
                .ToElements().Cast<View>()
                .Where(x => x.ViewType == ViewType.CeilingPlan || x.ViewType == ViewType.AreaPlan || x.ViewType == ViewType.FloorPlan || x.ViewType == ViewType.Detail
                || x.ViewType == ViewType.DraftingView || x.ViewType == ViewType.Elevation || x.ViewType == ViewType.Section || x.ViewType == ViewType.ThreeD).ToList();
            sheets = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Sheets).WhereElementIsNotElementType()
                .ToElements().Where(x => x is ViewSheet).Cast<ViewSheet>().ToList();
            //foreach (Rebar bar in rebarSets)
            //{
            //    if (bar.LookupParameter("Rebar Number").AsString() == form.textBox1.Text.Trim())
            //    {
            //        found.Add(bar);
            //    }
            //}
            found = rebarSets.Where(x => x.LookupParameter("Rebar Number").AsString() == form.textBox1.Text.Trim()
            && x.LookupParameter("Partition").AsString() == partitions[form.comboBox1.SelectedIndex])?.ToList();
            Rebar bar;
            if (found.Count > 0) bar = found.First();
            else
            {
                doc.print("nothing found");
                return Result.Cancelled;
            }
            foreach (ViewSheet sheet in sheets)
            {
                List<ElementId> viewsOnSheet = sheet.GetAllViewports().ToList();
                //doc.print(viewsOnSheet.Count);
                foreach (ElementId id in viewsOnSheet)
                {
                    Viewport vp = doc.GetElement(id) as Viewport;
                    ElementId viewId = vp.ViewId;
                    View view = doc.GetElement(viewId) as View;
                    if (view.ViewType == ViewType.ThreeD) continue;
                    List<Rebar> setsInView = new FilteredElementCollector(doc, viewId).OfCategory(BuiltInCategory.OST_Rebar)
                        .Cast<Rebar>().ToList();
                    if (setsInView.Any(x => x.Id.IntegerValue == bar.Id.IntegerValue) && bar.IsUnobscuredInView(view))
                    {
                        uidoc.RequestViewChange(doc.GetElement(sheet.Id) as View);
                    }
                }
            }

            uidoc.Selection.SetElementIds(found.Select(found => found.Id).ToArray());
            return Result.Succeeded;
        }
    }
}
