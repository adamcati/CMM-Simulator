using CMM_Simulator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CMM_Simulator;

public class Library3D
{
    public static double GetLinearDistance(double a, double b)
    {
        return Math.Abs(a - b);
    }

    public static double GetDistanceBetweenTwoPoints(double x1, double x2, double y1, double y2, double z1, double z2)
    {
        return Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1) + (z2 - z1) * (z2 - z1));
    }

    public static PointModel GetPointAtDistanceFrom(PointModel point, double distance)
    {
        PointModel output = new PointModel(0, 0, 0, 0, 0, 0);
        double magnitude = Math.Sqrt(point.Vectors.XAxis * point.Vectors.XAxis +
            point.Vectors.YAxis * point.Vectors.YAxis +
            point.Vectors.ZAxis * point.Vectors.ZAxis);
        double unitVector = 0;
        double displacementVector = 0;

        //X-axis
        unitVector = point.Vectors.XAxis / magnitude;
        displacementVector = distance * unitVector;
        output.Vectors.XAxis = displacementVector;
        output.Coordinates.XAxis = point.Coordinates.XAxis + displacementVector;

        //Y-axis
        unitVector = point.Vectors.YAxis / magnitude;
        displacementVector = distance * unitVector;
        output.Vectors.YAxis = displacementVector;
        output.Coordinates.YAxis = point.Coordinates.YAxis + displacementVector;

        //Z-axis
        unitVector = point.Vectors.ZAxis / magnitude;
        displacementVector = distance * unitVector;
        output.Vectors.ZAxis = displacementVector;
        output.Coordinates.ZAxis = point.Coordinates.ZAxis + displacementVector;

        return output;
    }
}
