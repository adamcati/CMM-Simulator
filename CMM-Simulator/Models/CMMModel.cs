using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMM_Simulator.Models;
public class CMMModel
{
    public Dictionary<string, double> Settings = new Dictionary<string, double>();
    public Dictionary<string, double> Acceleration { get; }
    public Dictionary<string, double> Velocity { get; }
    public double TouchSpeed { get; set; }
    public double RetractSpeed { get; set; }
    public double RetractAcceleration { get; set; }
    public double SearchSpeed { get; set; } // only for relative to surface auto mode
    public string MeasurementUnits { get; set; }

    public CMMModel()
    {
        //Acceleration in mm/sec^2
        Acceleration = new Dictionary<string, double>()
        {
            { "x-axis", 297.778 },
            { "y-axis", 616.666 },
            { "z-axis", 500 }
        };

        //Velocity in mm/sec
        Velocity = new Dictionary<string, double>()
        {
            { "x-axis", 233.333 },
            { "y-axis", 383.333 },
            { "z-axis", 300 }
        };

        //touch speed in mm / sec and distancies
        TouchSpeed = 1.6667;
        RetractSpeed = 33.3333;
        RetractAcceleration = 69.4445;
        SearchSpeed = 8.3333;

        //in mm 
        Settings.Add("APPRCH", 5); //approach distance
        Settings.Add("RETRCT", 5); //retract distance
        Settings.Add("CLRSRF", 15); //clearance distabce
        Settings.Add("DEPTH", 2); //depth
    }

    public CMMModel(string measurementUnits)
    {
        MeasurementUnits = measurementUnits;

        if (MeasurementUnits == "INCH")
        {
            //Acceleration in inch/sec^2
            Acceleration = new Dictionary<string, double>()
            {
                { "x-axis", 11.7235 },
                { "y-axis", 24.2782 },
                { "z-axis", 19.6850 }
            };
            //Velocity in inch/sec
            Velocity = new Dictionary<string, double>()
            {
                { "x-axis", 9.1863 },
                { "y-axis", 15.0918 },
                { "z-axis", 11.811 }
            };

            //touch speed in inch / sec and distancies
            TouchSpeed = 0.0656;
            RetractSpeed = 1.3123;
            RetractAcceleration = 2.7340;
            SearchSpeed = 0.3281;

            //in inch 
            Settings.Add("APPRCH", 0.08); //approach distance
            Settings.Add("RETRCT", 0.04); //retract distance
            Settings.Add("CLRSRF", 0.2); //clearance distabce
            Settings.Add("DEPTH", 0.04); //depth
        }
        else
        {
            //Acceleration in mm/sec^2
            Acceleration = new Dictionary<string, double>()
            {
                { "x-axis", 297.778 },
                { "y-axis", 616.666 },
                { "z-axis", 500 }
            };

            //Velocity in mm/sec
            Velocity = new Dictionary<string, double>()
            {
                { "x-axis", 233.333 },
                { "y-axis", 383.333 },
                { "z-axis", 300 }
            };

            //touch speed in mm / sec and distancies
            TouchSpeed = 1.6667;
            RetractSpeed = 33.3333;
            RetractAcceleration = 69.4445;
            SearchSpeed = 8.3333;

            //in mm 
            Settings.Add("APPRCH", 5); //approach distance
            Settings.Add("RETRCT", 5); //retract distance
            Settings.Add("CLRSRF", 15); //clearance distabce
            Settings.Add("DEPTH", 2); //depth
        }
    }
}
