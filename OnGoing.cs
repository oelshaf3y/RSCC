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

            return Result.Succeeded;
        }
    }
}
