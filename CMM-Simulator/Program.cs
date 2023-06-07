using CMM_Simulator;
using CMM_Simulator.Controllers;
using CMM_Simulator.Enums;
using CMM_Simulator.Models;

CMMModel CMM1 = new CMMModel();
PointModel startPoint = new PointModel(0, 0, 0, 0, 0, 0);
CMMController controller = new CMMController();

//measurement time is in seconds
double measurementTime = 0;

List<string> fileLines = FileHandler.ReadAllNonEmptyLines("C:\\Users\\Adam\\Documents\\CMM-Simulator\\V5311076420500_REV_B00.dmi");
fileLines.RemoveTextOutfilFromFileLines();
fileLines.RemoveOutputsFromFileLines();
fileLines.RemoveCommentsFromFileLines();
fileLines.RemoveConstructedFeaturesFromFileLines();
fileLines.RemoveMultipleLineCode();

List<string> measurementBlock = new List<string>();

foreach (string line in fileLines)
{

    if (line.Contains('/'))
    {
        if (CMM1.Settings.ContainsKey(line.Split('/')[1].Split(',')[0]))
        {
            CMM1.Settings[(line.Split('/')[1].Split(',')[0])] = double.Parse(line.Split('/')[1].Split(',')[1]);
        }
    }
}

int i = 0;
while(i < fileLines.Count)
{
    if (fileLines[i].Contains("FEAT"))
    {
        while (fileLines[i] != "ENDMES")
        {
            measurementBlock.Add(fileLines[i]);
            i++;
        }
        measurementTime += controller.GetTimeOfBlockfExecution(Operations.Measurement, measurementBlock);
        ClearMeasurementBock();
    }
    else if (fileLines[i].Contains("GOTO"))
    {
        measurementBlock.Add((string)fileLines[i]);
        measurementTime += controller.GetTimeOfBlockfExecution(Operations.GoTo, measurementBlock);
        ClearMeasurementBock();
    }
    else if(fileLines[i].Contains("SNSLCT"))
    {
        measurementTime += controller.GetTimeOfBlockfExecution(Operations.SensorSelect, null);
    }
    else
    {
        Console.WriteLine($"Other operations type: {fileLines[i]}");
    }

    i++;
}

void ClearMeasurementBock()
{
    measurementBlock.Clear();
}

Console.WriteLine();
Console.WriteLine($"Total Measurement time: {measurementTime} seconds");