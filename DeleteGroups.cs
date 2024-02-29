using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSCC_GEN
{
    [TransactionAttribute(TransactionMode.Manual)]

    internal class DeleteGroups : IExternalCommand
    {
        UIDocument uidoc;
        Document doc;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uidoc = commandData.Application.ActiveUIDocument;
            doc = uidoc.Document;


            List<Group> selected = uidoc.Selection.PickObjects(ObjectType.Element, "Pick Elements").Select(x => doc.GetElement(x))
                .Where(x => x is Group).Cast<Group>().ToList();

            using (Transaction tr = new Transaction(doc, "Delete Elements"))
            {
                tr.Start();
                foreach (Group group in selected)
                {
                    doc.Delete(group.UngroupMembers());
                }
                tr.Commit();
                tr.Dispose();
            }
            return Result.Succeeded;
        }
    }
}
