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
}
