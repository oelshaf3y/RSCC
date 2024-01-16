using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RSCC_GEN
{
    public static class Ninja
    {
        public static void print(this Document doc, object mes)
        {
            TaskDialog.Show("Message", mes.ToString());
        }
    }
}
