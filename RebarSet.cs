using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSCC_GEN
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class RebarSet : IExternalCommand
    {
        UIDocument uidoc;
        Document doc;
        List<ElementId> rebarSets;
        StringBuilder sb;
        int count = 0;
        Consolidate con;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            con = new Consolidate();
            con.ShowDialog();
            if (con.DialogResult == System.Windows.Forms.DialogResult.Cancel) return Result.Cancelled;
            sb = new StringBuilder();
            uidoc = commandData.Application.ActiveUIDocument;
            doc = uidoc.Document;
            rebarSets = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rebar)
                .WhereElementIsNotElementType().ToElementIds().ToList();
            var t1 = doc.GetElement(rebarSets.First());
            Rebar r1 = t1 as Rebar;
            if (!checkParameters(r1))
            {
                doc.print(sb.ToString());
                return Result.Failed;
            }
            sb.Clear();
            using (TransactionGroup tg = new TransactionGroup(doc, "Rebar magic"))
            {
                tg.Start();
                checkConsolidation();
                //doc.print(sb.ToString());
                ConsolidateRebarNumbers();
                getBarNumbers();
                tg.Assimilate();
            }
            return Result.Succeeded;
        }
        bool checkParameters(Rebar r)
        {
            sb.AppendLine("THESE SHARED PARAMETERS ARE MISSING, OR NOT ASSOCIATED WITH STRUCTURAL REBAR!");
            Parameter p1 = r.LookupParameter("RSCC_SetRebNum1");
            Parameter p2 = r.LookupParameter("RSCC_SetRebNum2");
            Parameter p3 = r.LookupParameter("RSCC_MinLength");
            Parameter p4 = r.LookupParameter("RSCC_MaxLength");
            Parameter p5 = r.LookupParameter("RSCC_SetQTY");
            if (p1 == null) sb.AppendLine("RSCC_SetRebNum1");
            if (p2 == null) sb.AppendLine("RSCC_SetRebNum2");
            if (p3 == null) sb.AppendLine("RSCC_MinLength");
            if (p4 == null) sb.AppendLine("RSCC_MaxLength");
            if (p5 == null) sb.AppendLine("RSCC_SetQTY");
            if (p1 != null & p2 != null & p3 != null & p4 != null & p5 != null)
            {
                return true;
            }
            return false;
        }
        void checkConsolidation()
        {
            List<Rebar> barsToRenumber = new List<Rebar>();
            foreach (ElementId id in rebarSets)
            {
                Rebar r = doc.GetElement(id) as Rebar;
                ElementId rebNumberId = r.LookupParameter("Rebar Number").Id;
                ElementId barLengthId = r.LookupParameter("Bar Length").Id;
                List<int> rebarNumbers = new List<int>();
                for (int i = 0; i < r.Quantity; i++)
                {
                    try
                    {
                        ParameterValue rebNumPV = r.GetParameterValueAtIndex(rebNumberId, i);
                        StringParameterValue rebarNumber = rebNumPV as StringParameterValue;
                        if (rebarNumber == null) break;
                        rebarNumbers.Add(Convert.ToInt32(rebarNumber.Value));
                    }
                    catch (Exception ex)
                    {
                        doc.print(ex.StackTrace);
                    }
                }
                rebarNumbers = rebarNumbers.OrderBy(x => x).ToList();
                if (rebarNumbers.Distinct().Count() < rebarNumbers.Count()) continue;
                //foreach (int i in rebarNumbers)
                //{
                //    sb.Append(i.ToString() + ",");
                //}
                if (rebarNumbers.Count == 0) continue;
                if (rebarNumbers[0] + rebarNumbers.Count - 1 != rebarNumbers[rebarNumbers.Count - 1])
                {
                    //sb.Append("last:" + (rebarNumbers[0] + rebarNumbers.Count - 1).ToString() + "\n");
                    barsToRenumber.Add(r);
                }
                //else
                //{
                //    sb.Append("    Correct\n");
                //}
                correctBarNumbers(barsToRenumber);
            }
        }

        void getBarNumbers()
        {
            using (Transaction tr = new Transaction(doc, "getNumbers"))
            {
                tr.Start();
                foreach (ElementId id in rebarSets)
                {
                    var t = doc.GetElement(id);
                    Rebar r = t as Rebar;
                    ElementId rebNumberId = r.LookupParameter("Rebar Number").Id;
                    ElementId barLengthId = r.LookupParameter("Bar Length").Id;
                    StringParameterValue startNum = r.GetParameterValueAtIndex(rebNumberId, 0) as StringParameterValue;
                    StringParameterValue endNum = r.GetParameterValueAtIndex(rebNumberId, r.Quantity - 1) as StringParameterValue;
                    DoubleParameterValue minLength = r.GetParameterValueAtIndex(barLengthId, 0) as DoubleParameterValue;
                    DoubleParameterValue maxLength = r.GetParameterValueAtIndex(barLengthId, r.Quantity - 1) as DoubleParameterValue;
                    try
                    {
                        r.LookupParameter("RSCC_MaxLength").Set(Math.Max(minLength.Value, maxLength.Value));
                        r.LookupParameter("RSCC_MinLength").Set(Math.Min(minLength.Value, maxLength.Value));
                        r.LookupParameter("RSCC_SetRebNum1").Set(startNum.Value);
                        r.LookupParameter("RSCC_SetRebNum2").Set(endNum.Value);
                        r.LookupParameter("RSCC_SetQTY").Set(r.Quantity);
                    }
                    catch
                    {
                    }
                }
                tr.Commit();
                tr.Dispose();
            }
        }

        void correctBarNumbers(List<Rebar> rebarsToRenumber)
        {
            List<string> oldPartitions = new List<string>();
            using (Transaction tr = new Transaction(doc))
            {
                tr.Start("set");
                foreach (Rebar rebar in rebarsToRenumber)
                {
                    oldPartitions.Add(rebar.LookupParameter("Partition").AsString());
                    rebar.LookupParameter("Partition").Set("RSCC_TEMP_PART");
                }
                tr.Commit();
                tr.Dispose();
            }

            using (Transaction tr = new Transaction(doc))
            {
                tr.Start("reset");
                for (int i = 0; i < rebarsToRenumber.Count; i++)
                {
                    Rebar rebar = rebarsToRenumber[i];
                    rebar.LookupParameter("Partition").Set(oldPartitions[i]);
                }
                tr.Commit();
                tr.Dispose();
            }
        }

        void ConsolidateRebarNumbers()
        {
            // Obtain a schema object for a particular kind of elements 
            NumberingSchema schema = NumberingSchema.GetNumberingSchema(doc, NumberingSchemaTypes.StructuralNumberingSchemas.Rebar);

            // Collect the names of partitions of all the numbering sequences currently contained in the schema
            IList<string> sequences = schema.GetNumberingSequences();

            using (Transaction transaction = new Transaction(doc))
            {
                // Changes to numbers must be made inside a transaction
                transaction.Start("Consolidate Rebar Numbers");

                // First we make sure numbers in all sequences are consecutive
                // by removing possible gaps in numbers. Note: RemoveGaps does
                // nothing for a sequence where there are no gaps present.

                // We also want to find what the maximum range of numbers is
                // of all the sequences (the one the widest span of used numbers)
                int maxRange = 1;

                foreach (string name in sequences)
                {
                    schema.RemoveGaps(name);
                    schema.ShiftNumbers(name, maxRange);
                    IntegerRange range = schema.GetNumbers(name).First();
                    maxRange = range.High + 1;
                }
                if (con.radioButton2.Checked)
                {

                    foreach (string name in sequences)
                    {
                        schema.ShiftNumbers(name, 1);
                    }
                }

                transaction.Commit();
            }
            //doc.print("Please note that: rebar sets were not in series. please run again!");
        }
    }
}
