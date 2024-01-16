using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSCC_GEN
{
    [TransactionAttribute(TransactionMode.Manual)]
    internal class RebarSet : IExternalCommand
    {
        UIDocument uidoc;
        Document doc;
        List<ElementId> rebarSets;
        StringBuilder sb;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            sb = new StringBuilder();
            uidoc = commandData.Application.ActiveUIDocument;
            doc = uidoc.Document;
            rebarSets = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rebar)
                .WhereElementIsNotElementType().ToElementIds().ToList();
            using (Transaction tr = new Transaction(doc, "Rebar magic"))
            {
                tr.Start();
                foreach (ElementId id in rebarSets)
                {
                    try
                    {

                        var t = doc.GetElement(id);
                        Rebar r = t as Rebar;
                        ElementId rebNumberId = r.LookupParameter("Rebar Number").Id;
                        ElementId barLengthId = r.LookupParameter("Bar Length").Id;
                        StringParameterValue startNum = r.GetParameterValueAtIndex(rebNumberId, 0) as StringParameterValue;
                        StringParameterValue endNum = r.GetParameterValueAtIndex(rebNumberId, r.Quantity - 1) as StringParameterValue;
                        DoubleParameterValue minLength = r.GetParameterValueAtIndex(barLengthId, 0) as DoubleParameterValue;
                        DoubleParameterValue maxLength = r.GetParameterValueAtIndex(barLengthId, r.Quantity - 1) as DoubleParameterValue;
                        r.LookupParameter("RSCC_MaxLength").Set(Math.Max(minLength.Value, maxLength.Value));
                        r.LookupParameter("RSCC_MinLength").Set(Math.Min(minLength.Value, maxLength.Value));
                        r.LookupParameter("RSCC_SetRebNum1").Set(startNum.Value);
                        r.LookupParameter("RSCC_SetRebNum2").Set(endNum.Value);
                        r.LookupParameter("RSCC_SetQTY").Set(r.Quantity);
                    }
                    catch
                    {

                    }
                }
                tr.Commit();
                tr.Dispose();
            }
            return Result.Succeeded;

        }
        void td(string mes)
        {
            TaskDialog.Show("info", mes);
        }
    }
}
