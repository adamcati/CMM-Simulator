using CmmSimulatorLibrary;
using CmmSimulatorLibrary.Enums;
using CmmSimulatorLibrary.Models;

CmmSimulator CmmSimulator = new CmmSimulator();

List<string> fileLines = FileHandler.ReadAllNonEmptyLines("C:\\Users\\Adam\\Downloads\\19197_REV_A1A_Rev6.dmi");
double simulationTime = CmmSimulator.GetSimulationTime(fileLines);

Console.WriteLine();
Console.WriteLine($"Total Measurement time: {simulationTime} seconds");