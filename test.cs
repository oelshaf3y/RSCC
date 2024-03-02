using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSCC_GEN
{
    [TransactionAttribute(TransactionMode.Manual)]
    internal class test : IExternalCommand
    {
        UIDocument uidoc;
        Document doc;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uidoc = commandData.Application.ActiveUIDocument;
            doc = uidoc.Document;
            StringBuilder sb = new StringBuilder();
            try
            {
                List<View> views = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Views).WhereElementIsNotElementType().Cast<View>()
                    .Where(x => x.ViewType != ViewType.Legend)
                    .ToList();
                foreach (View view in views)
                {
                    sb.AppendLine(view.Name);
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine(ex.ToString());
                sb.AppendLine(ex.Message);
                sb.AppendLine(ex.StackTrace);
                doc.print(sb);
            }
            doc.print(sb);
            //using (Transaction tr = new Transaction(doc, "Delete Links"))
            //{
            //tr.Start();
            //tr.Commit();
            //}
            return Result.Succeeded;
        }
    }
    public class saveCords : ISaveSharedCoordinatesCallback
    {
        public SaveModifiedLinksOptions GetSaveModifiedLinksOption(RevitLinkType link)
        {

            return SaveModifiedLinksOptions.SaveLinks;
        }
    }
}
