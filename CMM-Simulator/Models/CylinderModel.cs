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
}
