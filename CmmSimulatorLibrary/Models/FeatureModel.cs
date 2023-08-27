using CmmSimulatorLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmmSimulatorLibrary.Models;
public class FeatureModel
{
    public CoordinatesModel Coordinates { get; }
    public VectorsModel Vectors { get; }
    public bool IsInner { get; }

    public FeatureModel() 
    {
        Coordinates = new CoordinatesModel();
        Vectors = new VectorsModel();
    }

    public FeatureModel(double x, double y, double z, double i, double j, double k)
    {
        Coordinates = new CoordinatesModel()
        {
            XAxis = x,
            YAxis = y,
            ZAxis = z,
        };
        Vectors = new VectorsModel()
        {
            XAxis = i,
            YAxis = j,
            ZAxis = k
        };
    }

    public FeatureModel(double x, double y, double z, double i, double j, double k,bool isInner)
    {
        Coordinates = new CoordinatesModel()
        {
            XAxis = x,
            YAxis = y,
            ZAxis = z,
        };
        Vectors = new VectorsModel()
        {
            XAxis = i,
            YAxis = j,
            ZAxis = k
        };
        IsInner = isInner;
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

    public int GetNumberOfMeasurementPoints(string measurementLine)
    {
        //number of measurement points is always the third element from the measurement line code
        int output = int.Parse(measurementLine.Split(',')[2]);

        return output;
    }
}
