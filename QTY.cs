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
    internal class QTY : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            IList<Element> selection = uidoc.Selection.GetElementIds().Select(x => doc.GetElement(x)).ToList();
            uidoc.Selection.SetElementIds(new List<ElementId> { });

            double vols = 0;
            List<Solid> solids = new List<Solid>();
            foreach (Element element in selection)
            {
                try
                {
                    Solid s = doc.getSolid(element);
                    solids.Add(s);
                    double vol = s.Volume;
                    vols += vol;
                }
                catch (Exception ex)
                {
                    doc.print(element.Name);
                    IList<ElementId> ids = uidoc.Selection.GetElementIds().ToList();
                    ids.Add(element.Id);
                    uidoc.Selection.SetElementIds(ids);
                }
            }
            int count = 0;
            Solid solid = solids.First();
            while (solids.Count > 1)
            {
                for (int i = 1; i < solids.Count; i++)
                {
                    Solid other = solids[i];
                    try
                    {
                        BooleanOperationsUtils.ExecuteBooleanOperationModifyingOriginalSolid(solid, other, BooleanOperationsType.Union);
                        solids.RemoveAt(i);
                        break;
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
                count++;
                if (count == 500)
                {
                    doc.print(solids.Count);
                    break;
                }
            }
            double surfaceArea = 0;
            List<PlanarFace> faces = new List<PlanarFace>();
            foreach (Face face in solids[0].Faces)
            {
                PlanarFace planarFace = face as PlanarFace;
                if (planarFace != null)
                {

                    if (planarFace.FaceNormal.Z >= 0)
                    {
                        surfaceArea += planarFace.Area;
                        faces.Add(planarFace);
                    }
                }
            } 
            //using(Transaction tr = new Transaction(doc))
            //{
            //    tr.Start("Draw");
            //    DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel)).SetShape(new List<GeometryObject> { solids.First()});
                
            //    tr.Commit();
                
            //}
            vols *= 0.02831685;
            surfaceArea *= 0.09290304;
            QTYForm form = new QTYForm(vols,surfaceArea);
            form.ShowDialog();
            return Result.Succeeded;
        }
    }
}
