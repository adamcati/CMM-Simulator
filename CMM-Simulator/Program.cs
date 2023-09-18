using CmmSimulatorLibrary;
using CmmSimulatorLibrary.Enums;
using CmmSimulatorLibrary.Models;

CmmSimulator CmmSimulator = new CmmSimulator();

List<string> fileLines = FileHandler.ReadAllNonEmptyLines("C:\\Users\\Adam\\Downloads\\V5327470720000_REV_A00.dmi");
double simulationTime = CmmSimulator.GetSimulationTime(fileLines);

Console.WriteLine();
Console.WriteLine($"Total Measurement time: {simulationTime} seconds");