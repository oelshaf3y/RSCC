using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            List<RevitLinkType> links = new FilteredElementCollector(doc).OfClass(typeof(RevitLinkType)).Cast<RevitLinkType>().ToList();
            StringBuilder sb = new StringBuilder();
            using (Transaction tr = new Transaction(doc, "Delete Links"))
            {
                tr.Start();
                try
                {
                    List<ElementId> ids = links.Select(x => x.GetRootId()).Distinct().ToList();
                    foreach (RevitLinkType linkType in ids.Select(x => doc.GetElement(x) as RevitLinkType))
                    {

                        doc.Delete(linkType.Id);
                    }
                }
                catch (Exception ex)
                {
                    sb.AppendLine(ex.ToString());
                    sb.AppendLine(ex.Message);
                    sb.AppendLine(ex.StackTrace);
                    doc.print(sb);
                }

                tr.Commit();
            }
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
