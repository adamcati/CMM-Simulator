using CMM_Simulator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
        double magnitude = Math.Sqrt(point.Vectors["x-axis"] * point.Vectors["x-axis"] +
            point.Vectors["y-axis"] * point.Vectors["y-axis"] +
            point.Vectors["z-axis"] * point.Vectors["z-axis"]);
        double unitVector = 0;
        double displacementVector = 0;

        foreach (string axis in point.Vectors.Keys)
        {
            unitVector = point.Vectors[axis] / magnitude;
            displacementVector = distance * unitVector;
            output.Vectors[axis] = displacementVector;
            output.Coordinates[axis] = point.Coordinates[axis] + displacementVector;
        }

        return output;
    }
}
