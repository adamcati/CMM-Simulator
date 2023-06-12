﻿using CMM_Simulator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMM_Simulator.Models;
public class FeatureModel
{
    public Dictionary<string, double> Coordinates { get; }
    public Dictionary<string, double> Vectors { get; }

    public FeatureModel() { }

    public FeatureModel(double x, double y, double z, double i, double j, double k)
    {
        Coordinates = new Dictionary<string, double>
        {
            { "x-axis", x },
            { "y-axis", y },
            { "z-axis", z }
        };
        Vectors = new Dictionary<string, double>
        {
            { "x-axis", i },
            { "y-axis", j },
            { "z-axis", k }
        };
    }

    public static Features GetFeatureType(string measurementLine)
    {
        Features output;
        string featureType = measurementLine.Split('/')[1].Split(',')[0];

        if (Enum.TryParse<Features>(featureType, out output) == false)
        {
            throw new Exception($"Wrong feature type {featureType}");
        }

        return output;
    }

    public static double[] GetFeatureData(string featureData)
    {
        string[] dataString = featureData.Split(',');

        List<double> output = new List<double>();

        foreach (string data in dataString)
        {
            if (Double.TryParse(data, out double number))
            {
                output.Add(number);
            }
        }

        return output.ToArray();
    }
}