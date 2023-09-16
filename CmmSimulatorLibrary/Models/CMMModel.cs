using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmmSimulatorLibrary.Enums;

namespace CmmSimulatorLibrary.Models;
public class CMMModel
{
    public CmmSettingsModel Settings { get; }
    public CoordinatesModel Acceleration { get; }
    public CoordinatesModel Velocity { get; }
    public double TouchSpeed { get; set; }
    public double RetractSpeed { get; set; }
    public double RetractAcceleration { get; set; }
    public double SearchSpeed { get; set; } // only for relative to surface auto mode

    public CMMModel()
    {
        //Acceleration in mm/sec^2
        Acceleration = new CoordinatesModel()
        {
            XAxis = 297.778,
            YAxis = 616.666,
            ZAxis = 500
        };

        //Velocity in mm/sec
        Velocity = new CoordinatesModel()
        {
            XAxis = 233.333,
            YAxis = 383.333,
            ZAxis = 300
        };

        //touch speed in mm / sec and distancies
        TouchSpeed = 1.6667;
        RetractSpeed = 33.3333;
        RetractAcceleration = 69.4445;
        SearchSpeed = 8.3333;

        //in mm 
        Settings = new CmmSettingsModel();
        Settings.Approach = 5; //approach distance
        Settings.Retract = 1; //retract distance
        Settings.Clearance = 15; //clearance distabce
        Settings.Depth = 2; //depth
    }

    public CMMModel(Units measurementUnits)
    {
        if (measurementUnits == Units.Inch)
        {
            //Acceleration in inch/sec^2
            Acceleration = new CoordinatesModel()
            {
                XAxis = 11.7235,
                YAxis = 24.2782,
                ZAxis = 19.6850
            };

            //Velocity in inch/sec
            Velocity = new CoordinatesModel()
            {
                XAxis = 9.1863,
                YAxis = 15.0918,
                ZAxis = 11.811
            };

            //touch speed in inch / sec and distancies
            TouchSpeed = 0.0656;
            RetractSpeed = 1.3123;
            RetractAcceleration = 2.7340;
            SearchSpeed = 0.3281;

            //in inch 
            Settings = new CmmSettingsModel();
            Settings.Approach = 0.08; //approach distance
            Settings.Retract = 0.04; //retract distance
            Settings.Clearance = 0.2; //clearance distabce
            Settings.Depth = 0.08; //depth
        }
        else
        {
            //Acceleration in mm/sec^2
            Acceleration = new CoordinatesModel()
            {
                XAxis = 297.778,
                YAxis = 616.666,
                ZAxis = 500
            };

            //Velocity in mm/sec
            Velocity = new CoordinatesModel()
            {
                XAxis = 233.333,
                YAxis = 383.333,
                ZAxis = 300
            };

            //touch speed in mm / sec and distancies
            TouchSpeed = 1.6667;
            RetractSpeed = 33.3333;
            RetractAcceleration = 69.4445;
            SearchSpeed = 8.3333;

            //in mm 
            Settings = new CmmSettingsModel();
            Settings.Approach = 5; //approach distance
            Settings.Retract = 1; //retract distance
            Settings.Clearance = 15; //clearance distabce
            Settings.Depth = 2; //depth
        }
    }
}
