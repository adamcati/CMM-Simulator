using CMM_Simulator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMM_Simulator.Models;
public class CylinderModel : CircleModel
{
    public double Length { get; set; }
    public CylinderModel(double x, double y, double z, double i, double j, double k, double diameter, int numberOfDivisions, double length) : base(x, y, z, i, j, k, diameter, numberOfDivisions)
    {
        Length = length;
    }

    public CylinderModel GetCylinderFromMeasurementBlock(List<string> measurementBlock)
    {
        double[] circleData = GetFeatureData(measurementBlock[0]);
        int numberOfDivisions = GetNumberOfCircleDivisions(measurementBlock[1]);
        //only for INNER
        CylinderModel cylinder = new CylinderModel(circleData[0], circleData[1], circleData[2], circleData[3], circleData[4], circleData[5], circleData[6], numberOfDivisions, circleData[7]);

        return cylinder;
    }

    public double GetCylinderMeasurementTime(CylinderModel cylinder, CMMModel CMM)
    {
        double output = 0;

        PointModel circleCenter = Library3D.GetPointAtDistanceFrom(new PointModel(
            cylinder.Coordinates.XAxis,
            cylinder.Coordinates.YAxis,
            cylinder.Coordinates.ZAxis,
            cylinder.Vectors.XAxis,
            cylinder.Vectors.YAxis,
            cylinder.Vectors.ZAxis),
            CMM.Settings["DEPTH"]);
        CircleModel circleFromCylinder = new CircleModel(circleCenter, cylinder.Diameter, cylinder.NumberOfDivisions / 2);
        output += GetCircleMeasurementTime(circleFromCylinder, CMM);
        circleCenter = Library3D.GetPointAtDistanceFrom(new PointModel(
            cylinder.Coordinates.XAxis,
            cylinder.Coordinates.YAxis,
            cylinder.Coordinates.ZAxis,
            cylinder.Vectors.XAxis,
            cylinder.Vectors.YAxis,
            cylinder.Vectors.ZAxis),
            cylinder.Length < 0 ? cylinder.Length + CMM.Settings["DEPTH"] : cylinder.Length - CMM.Settings["DEPTH"]);
        circleFromCylinder = new CircleModel(circleCenter, cylinder.Diameter, cylinder.NumberOfDivisions / 2);
        output += GetCircleMeasurementTime(circleFromCylinder, CMM);

        return output;
    }
}
