using CmmSimulatorLibrary;
using CmmSimulatorLibrary.Enums;
using CmmSimulatorLibrary.Models;

CmmSimulator CmmSimulator = new CmmSimulator();

double simulationTime = CmmSimulator.GetSimulationTime("C:\\Users\\Adam\\Downloads\\V5327470720000_REV_A00.dmi");

Console.WriteLine();
Console.WriteLine($"Total Measurement time: {simulationTime} seconds");