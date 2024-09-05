using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;
using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace RSCC_GEN.Clash_Balls
{
    
    public class ClashPoint
    {
        public string testName { get; set; }
        public string testType { get; set; }
        public string left { get; set; }
        public string right { get; set; }
        public string clashName { get; set; }
        public string elementId1 { get; private set; }
        public string elementId2 { get; private set; }
        public string tolerance { get; set; }
        public XYZ clashPoint;
        public double x { get; private set; }
        public double y { get; private set; }
        public double z { get; private set; }
        public bool resolved { get; set; }
        public ElementId elementId;
        public string comment { get; set; }
        //creator
        public ClashPoint(string testName, string testType, string tolerance, string left, string right, string clashName, XYZ clashPoint, ProjectPosition position, string elementId1, string comment = null)
        {
            this.testName = testName;
            this.testType = testType;
            this.tolerance = tolerance;
            this.left = left;
            this.right = right;
            this.clashName = clashName;
            this.elementId1 = elementId1;
            this.clashPoint = clashPoint;
            this.elementId = null;
            this.comment = comment;
            this.x = Math.Round((clashPoint.X) * 304.8, 2);
            this.y = Math.Round((clashPoint.Y) * 304.8, 2);
            this.z = Math.Round((clashPoint.Z) * 304.8, 2);
        }

        //collector
        public ClashPoint(string testName, string testType, string tolerance, string left, string right, string clashName, XYZ clashPoint, ProjectPosition position, string elementId1, string elementId2, bool resolved, string comment)
        {
            this.testName = testName;
            this.testType = testType;
            this.tolerance = tolerance;
            this.left = left;
            this.right = right;
            this.clashName = clashName;
            this.elementId1 = elementId1;
            this.elementId2 = elementId2;
            this.clashPoint = clashPoint;
            this.resolved = resolved;
            this.comment = comment;
            this.x = Math.Round(clashPoint.X);
            this.y = Math.Round(clashPoint.Y);
            this.z = Math.Round(clashPoint.Z);
        }

        public void setElementId(string elementId)
        {
            this.elementId2 = elementId;
        }
        public void setId(ElementId elementId)
        {
            this.elementId = elementId;
        }
    }
}
