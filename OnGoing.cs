using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSCC_GEN
{
    [TransactionAttribute(TransactionMode.Manual)]
    internal class OnGoing : IExternalCommand
    {
        public UIDocument uidoc { get; set; }
        public Document doc { get; set; }
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uidoc = commandData.Application.ActiveUIDocument;
            doc = uidoc.Document;

            FilteredElementCollector coll = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms);
            int count = coll.Count();
            using (Transaction tr = new Transaction(doc, "delete rooms"))
            {
                tr.Start();
                doc.Delete(coll.Select(x => x.Id).ToList());

                tr.Commit();
            }
            doc.print("total of " + count + " Rooms Deleted");
            return Result.Succeeded;
        }
    }
}
