using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace RevitAddin
{
    [TransactionAttribute(TransactionMode.Manual)]
    internal class NotOnSheets : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication application = commandData.Application;
            Application app = application.Application;
            UIDocument uidoc = application.ActiveUIDocument;
            Document document = uidoc.Document;
            FilteredElementCollector views = new FilteredElementCollector(document);
            views.OfClass(typeof(View));
            views.WhereElementIsViewIndependent();
            views.Where(v => v.LookupParameter("View Template") != null);
            Parameter sheetNumber;
            IList<ElementId> viewIds = new List<ElementId>();
            using (TransactionGroup tg = new TransactionGroup(document, "Delete not on sheets"))
            {

                tg.Start();
                foreach (View view in views)
                {
                    if (view != null && view.GetType() != typeof(ViewSheet) && view.GetType() != typeof(ViewSchedule)
                        && view.ViewType != ViewType.ProjectBrowser && view.ViewType != ViewType.SystemBrowser)
                    {
                        sheetNumber = view.LookupParameter("Sheet Name");
                        if (sheetNumber == null || sheetNumber.AsValueString() == "---")
                        {
                            if (view.LookupParameter("Dependency") != null && view.LookupParameter("Dependency").AsString() != "Primary")
                            {
                                using (Transaction tx = new Transaction(document, "Delete sheet"))
                                {

                                    tx.Start();

                                    document.Delete(view.Id);
                                    tx.Commit();
                                    tx.Dispose();
                                }
                            }
                        }
                    }
                }
                tg.Assimilate();
            }
            return Result.Succeeded;

        }
    }
}