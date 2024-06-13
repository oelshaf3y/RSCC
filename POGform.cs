using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.DB.Plumbing;
using System.Reflection.Emit;

namespace RSCC_GEN
{
    public partial class POGform : System.Windows.Forms.Form
    {
        public List<MEPSize> sizes = new List<MEPSize>();
        List<Element> pipeTypes, systems;
        Document doc;
        Pipe pipe = null;
        List<double> diameters = new List<double>();
        public POGform(Document doc, List<Element> pipeTypes, List<Element> systems)
        {
            InitializeComponent();
            this.doc = doc;
            this.pipeTypes = pipeTypes;
            this.systems = systems;
            FilteredElementCollector segments = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PipeSegments);
            foreach (PipeSegment segment in segments.Cast<PipeSegment>())
            {
                foreach (MEPSize size in segment.GetSizes())
                {
                    diameters.Add(size.NominalDiameter);
                }
            }
        }

        private void POGform_Load(object sender, EventArgs e)
        {
            comboBox1.Items.AddRange(pipeTypes.Select(x => x.Name).ToArray());
            comboBox2.Items.AddRange(systems.Select(x => x.Name).ToArray());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (double.TryParse(textBox1.Text, out double distance) &&
            double.TryParse(textBox2.Text, out double angle) &&
            comboBox3.SelectedIndex != -1 &&
            double.TryParse(textBox4.Text, out double offset))
            {
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Make sure you enter valid numbers not text.");
            }
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ElementId pipeTypeId = pipeTypes[comboBox1.SelectedIndex].Id;
            ElementId systemId = systems[0].Id;
            ElementId levelId = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType().First().Id;
            PipeSegment segment;
            using (Transaction tr = new Transaction(doc, "Create pipe"))
            {
                tr.Start();
                pipe = Pipe.Create(doc, systemId, pipeTypeId, levelId, XYZ.Zero, XYZ.Zero.Add(3 * XYZ.BasisX));
                segment = pipe.PipeSegment;
                pipe.LookupParameter("Diameter").Set(diameters[0]);
                int i = 0;
                while (segment == null && i < diameters.Count)
                {
                    i++;
                    pipe.LookupParameter("Diameter").Set(diameters[i]);
                    segment = pipe.PipeSegment;
                }
                sizes.Clear();
                sizes = segment.GetSizes().ToList();
                comboBox3.Items.AddRange(segment.GetSizes().Select(x => x.NominalDiameter.feetToMM().ToString()).ToArray());
                tr.RollBack();
            }

        }

    }
}
