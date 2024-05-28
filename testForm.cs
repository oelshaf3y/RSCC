using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace RSCC_GEN
{
    public partial class testForm : System.Windows.Forms.Form
    {
        public XYZ point;
        public UIDocument uidoc;
        public Document doc;
        public test parent;
        public testForm(UIDocument uidoc, test parent)
        {
            this.uidoc = uidoc;
            doc = uidoc.Document;
            this.parent = parent;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            point = uidoc.Selection.PickPoint();
            doc.print(point);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            XYZ p1, p2, p3, p4;
            p1 = point.Add(-1 * XYZ.BasisX).Add(-1 * XYZ.BasisY);
            p2 = point.Add(-1 * XYZ.BasisX).Add(1 * XYZ.BasisY);
            p3 = point.Add(1 * XYZ.BasisX).Add(-1 * XYZ.BasisY);
            p4 = point.Add(1 * XYZ.BasisX).Add(1 * XYZ.BasisY);

            Line l1, l2, l3, l4;
            l1 = Line.CreateBound(p1, p2);
            l2 = Line.CreateBound(p2, p3);
            l3 = Line.CreateBound(p3, p4);
            l4 = Line.CreateBound(p4, p1);
            parent.drawLines(l1, l2, l3, l4);
        }

        private void button4_Click(object sender, EventArgs e)
        {
        }
    }

}
