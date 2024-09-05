using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RSCC_GEN
{
    [TransactionAttribute(TransactionMode.Manual)]
    internal class GetCoordinates : IExternalCommand
    {
        Document doc;
        UIDocument uidoc;
        string location;
        List<XYZ> BasePoints, SurveyPoints;
        List<List<XYZ>> Bases, Surveys;
        List<string> locationNames;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uidoc = commandData.Application.ActiveUIDocument;
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.ShowDialog();
            location = folderBrowser.SelectedPath;
            List<string> revitFiles = Directory.GetFiles(location, "*.rvt").ToList();
            //List<string> revitFiles = new List<string>();
            //revitFiles.Add("Test");
            locationNames = new List<string>();
            Bases = new List<List<XYZ>>();
            Surveys = new List<List<XYZ>>();
            doc = uidoc.Document;
            foreach (string revitFile in revitFiles)
            {
                BasePoints = new List<XYZ>();
                SurveyPoints = new List<XYZ>();
                // Open the Revit file in the background
                using (ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(revitFile))
                {
                    // Set up OpenOptions to ignore links, avoid saving as new central
                    OpenOptions openOptions = new OpenOptions
                    {
                        // Detach from central without saving as a new central model
                        DetachFromCentralOption = DetachFromCentralOption.DetachAndPreserveWorksets,

                        // Allow opening of models where local is not allowed by wrong user
                        AllowOpeningLocalByWrongUser = true
                    };

                    // Optional: Define workset configuration to control what is loaded
                    WorksetConfiguration worksetConfig = new WorksetConfiguration(WorksetConfigurationOption.CloseAllWorksets);
                    openOptions.SetOpenWorksetsConfiguration(worksetConfig);

                    using (Document doc = commandData.Application.Application.OpenDocumentFile(modelPath, openOptions))
                    {
                        // Collect all base points in the document
                        ProjectLocationSet AllSites = doc.ProjectLocations;
                        using (TransactionGroup tg = new TransactionGroup(doc, "set Location"))
                        {
                            tg.Start();
                            foreach (ProjectLocation projectLocation in AllSites)
                            {
                                using (Transaction tr = new Transaction(doc, "Change Location"))
                                {
                                    tr.Start();
                                    doc.ActiveProjectLocation = projectLocation;
                                    tr.Commit();
                                }
                                locationNames.Add(doc.ActiveProjectLocation.Name);
                                FilteredElementCollector collector = new FilteredElementCollector(doc)
                                    .OfClass(typeof(BasePoint));

                                // Find the project base point
                                foreach (BasePoint basePoint in collector)
                                {
                                    if (basePoint.IsShared == false)
                                    {
                                        BasePoints.Add(basePoint.SharedPosition);
                                    }
                                    else
                                    {
                                        SurveyPoints.Add(basePoint.SharedPosition);
                                    }
                                }

                                // Store the data in the list
                                //string data = $"{revitFile},{basePoint.Position.X},{basePoint.Position.Y},{basePoint.Position.Z}";
                                //basePointData.Add(data);
                            }
                            tg.RollBack();
                        }
                    }
                }
                Bases.Add(BasePoints);
                Surveys.Add(SurveyPoints);
            }
            ExportPointsToFile(revitFiles, locationNames, Bases, Surveys, Path.Combine(location, "data.csv"));
            return Result.Succeeded;
        }

        public static void ExportPointsToFile(List<string> fileNames, List<string> LocationNames, List<List<XYZ>> bases, List<List<XYZ>> surveys, string saveLocation)
        {
            using (StreamWriter writer = new StreamWriter(saveLocation))
            {
                // Write the header
                writer.WriteLine("Location,Base Points,,,Survey Points");
                writer.WriteLine("Name,East/West,North/South,Elevation,East/West,North/South,Elevation");
                for (int k = 0; k < fileNames.Count; k++)
                {
                    string fileName = fileNames[k].Split('\\').Last();
                    List<XYZ> basePoints = bases[k];
                    List<XYZ> surveyPoints = surveys[k];
                    // Write the file name
                    writer.WriteLine(fileName);
                    // Loop through the points and write them in the desired format
                    int count = Math.Max(basePoints.Count, surveyPoints.Count);
                    for (int i = 0; i < count; i++)
                    {
                        string Location = LocationNames[i];
                        string basePoint = i < basePoints.Count ? Math.Round(basePoints[i].X.feetToMM()).ToString() + "," + Math.Round(basePoints[i].Y.feetToMM()).ToString() + "," + Math.Round(basePoints[i].Z.feetToMM()).ToString() : "N/A,N/A,N/A";
                        string surveyPoint = i < surveyPoints.Count ? Math.Round(surveyPoints[i].X.feetToMM()).ToString() + "," + Math.Round(surveyPoints[i].Y.feetToMM()).ToString() + "," + Math.Round(surveyPoints[i].Z.feetToMM()).ToString() : "N/A,N/A,N/A";

                        writer.WriteLine($"{Location},{basePoint},{surveyPoint}");
                    }
                }
            }
        }


    }

}
