using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;

namespace RSCC_GEN
{
    internal class RSCCSelectionFilter : ISelectionFilter
    {
        public Document doc;
        Func<Element, bool> filter;
        bool isNative;
        public RSCCSelectionFilter(Func<Element, bool> filter, bool isNative)
        {
            this.filter = filter;
            this.isNative = isNative;
        }
        public bool AllowElement(Element elem)
        {
            if (isNative)
            {
                return filter(elem);
            }
            doc = ((RevitLinkInstance)elem).GetLinkDocument();
            return true;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            Element elem = doc.GetElement(reference.LinkedElementId);
            return filter(elem);
        }
    }
}
