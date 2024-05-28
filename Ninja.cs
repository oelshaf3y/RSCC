using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace RSCC_GEN
{
    public static class Ninja
    {
        public static void print(this Document doc, object mes)
        {
            MessageBox.Show(mes.ToString());
        }

        public static TaskDialogResult YesNoMessage(this Document doc,object mes)
        {
            TaskDialog dialog = new TaskDialog("Question?")
            {
                MainInstruction = mes.ToString(),
                CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No
            };
            return dialog.Show();
        } 

        public static Solid getSolid(this Document doc, Element elem)
        {
            Options options = new Options();
            options.ComputeReferences = true;
            IList<Solid> solids = new List<Solid>();
            try
            {

                GeometryElement geo = elem.get_Geometry(options);
                if (geo.FirstOrDefault() is Solid)
                {
                    Solid solid = (Solid)geo.FirstOrDefault();
                    return SolidUtils.Clone(solid);
                }
                foreach (GeometryObject geometryObject in geo)
                {
                    if (geometryObject != null)
                    {
                        Solid solid = geometryObject as Solid;
                        if (solid != null && solid.Volume > 0)
                        {
                            solids.Add(solid);

                        }
                    }
                }
            }
            catch
            {
            }
            if (solids.Count == 0)
            {
                try
                {
                    GeometryElement geo = elem.get_Geometry(options);
                    GeometryInstance geoIns = geo.FirstOrDefault() as GeometryInstance;
                    if (geoIns != null)
                    {
                        GeometryElement geoElem = geoIns.GetInstanceGeometry();
                        if (geoElem != null)
                        {
                            foreach (GeometryObject geometryObject in geoElem)
                            {
                                Solid solid = geometryObject as Solid;
                                if (solid != null && solid.Volume > 0)
                                {
                                    solids.Add(solid);
                                }
                            }
                        }
                    }
                }
                catch
                {
                    //throw new InvalidOperationException();
                }
            }
            if (solids.Count > 0)
            {
                try
                {

                    return SolidUtils.Clone(solids.OrderByDescending(x => x.Volume).ElementAt(0));
                }
                catch
                {
                    return solids.OrderByDescending(x => x.Volume).ElementAt(0);
                }
            }
            else
            {
                return null;
            }
        }

        public static Face getFace(this Solid s, string location)
        {
            if (location.ToLower() == "top")
            {

                List<PlanarFace> faces = new List<PlanarFace>();
                foreach (Face face in s.Faces)
                {
                    PlanarFace pf = face as PlanarFace;
                    if (pf == null) continue;
                    if (Math.Abs(pf.FaceNormal.AngleTo(new XYZ(0, 0, 1))) < Math.PI / 18)
                    {
                        faces.Add(pf);
                    }
                }
                if (faces.Count == 0) return null;
                return faces.OrderByDescending(x => x.Origin.Z)?.First();
            }
            else if (location.ToLower() == "bot")
            {
                List<PlanarFace> faces = new List<PlanarFace>();
                foreach (Face face in s.Faces)
                {
                    PlanarFace pf = face as PlanarFace;
                    if (pf == null) continue;
                    if (Math.Abs(pf.FaceNormal.AngleTo(new XYZ(0, 0, -1))) < Math.PI / 18)
                    {
                        faces.Add(pf);
                    }
                }
                if (faces.Count == 0) return null;
                return faces.OrderBy(x => x.Origin.Z)?.First();
            }
            else
            {
                return null;
            }
        }
    }
}
