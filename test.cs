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
    internal class test : IExternalCommand
    {
        UIDocument uidoc;
        Document doc;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uidoc = commandData.Application.ActiveUIDocument;
            doc=uidoc.Document;

            Element selected = doc.GetElement(uidoc.Selection.GetElementIds().First());
            Solid s = doc.getSolid(selected);
            doc.print(s.Volume);
            return Result.Succeeded;
        }
    }
}
