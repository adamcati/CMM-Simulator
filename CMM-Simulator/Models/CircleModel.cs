using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMM_Simulator.Models;
public class CircleModel : FeatureModel
{
    public double Diameter { get; set; }
    public double Length { get; set; }
    public int NumberOfDivisions { get; set; }
    public CircleModel(double x, double y, double z, double i, double j, double k, double diameter, int numberOfDivisions, double length = 0) : base(x,y,z,i,j,k)
    {
        Diameter = diameter;
        Length = length;
        NumberOfDivisions = numberOfDivisions;
    }

    int GetNumberOfCircleDivisions(string circleMeasurementBlock)
    {
        int output = int.Parse(circleMeasurementBlock.Split(',')[2]);

        return output;
    }

    public CircleModel GetCircleFromMeasurementBlock(List<string> measurementBlock)
    {
        double[] circleData = GetFeatureData(measurementBlock[0]);
        int numberOfDivisions = GetNumberOfCircleDivisions(measurementBlock[1]);
        //only for INNER
        CircleModel circle = new CircleModel(circleData[0], circleData[1], circleData[2], circleData[3], circleData[4], circleData[5], circleData[6], numberOfDivisions);

        return circle;
    }

    public double GetCircleMeasurementTime(CircleModel circle, CMMModel CMM)
    {
        double output = 0;

        string[] circleAxis = circle.Vectors.Where(x => x.Value != 1).ToDictionary(x => x.Key, x => x.Value).Keys.ToArray();
        var orderedVectors = circle.Vectors.OrderBy(x => x.Value).ToList();
        string circleDirection = orderedVectors[2].Key;
        //circleDirection = circle.Vectors.Where(x => x.Value == 1).ToList().First().Key;

        //circle.Coordinates[circleDirection] = 0;
        PointModel pointOnCircle = GetOnePointOnCircle(circle.Coordinates[circleAxis[0]],
            circle.Coordinates[circleAxis[1]],
            circle.Coordinates[circleDirection],
            circle.NumberOfDivisions,
            circle.Diameter / 2,
            circleAxis[0],
            circleAxis[1],
            circleDirection);
        double distanceToTravel;
        double diagonalAcceleration = Physics.GetDiagonalAcceleration(CMM.Acceleration[circleAxis[0]], CMM.Acceleration[circleAxis[1]],0);
        double diagonalVelocity = Physics.GetDiagonalVelocity(CMM.Velocity[circleAxis[0]], CMM.Velocity[circleAxis[1]],0);
        distanceToTravel = Library3D.GetDistanceBetweenTwoPoints(
            circle.Coordinates["x-axis"], pointOnCircle.Coordinates["x-axis"],
            circle.Coordinates["y-axis"], pointOnCircle.Coordinates["y-axis"],
            circle.Coordinates["z-axis"], pointOnCircle.Coordinates["z-axis"]);

        output += circle.NumberOfDivisions * Physics.GetTimeToTravelDistance(distanceToTravel, CMM.TouchSpeed, diagonalAcceleration);
        output += circle.NumberOfDivisions * Physics.GetTimeToTravelDistance(distanceToTravel, CMM.RetractSpeed, CMM.RetractAcceleration);

        return output;
    }

    public PointModel GetOnePointOnCircle(double firstCoordinate, double secondCoordinate, double thirdCoordinate,int numberOfDivisions, double radius, string firstAxis, string secondAxis, string circleDirection)
    {
        PointModel pointOnCircle = new PointModel(0, 0, 0, 0, 0, 0);
        double angle = 360 / numberOfDivisions;
        double x = firstCoordinate + radius * Math.Cos((Math.PI / 180) * angle);
        double y = secondCoordinate + radius * Math.Sin((Math.PI / 180) * angle);
        pointOnCircle.Coordinates[firstAxis] = x;
        pointOnCircle.Coordinates[secondAxis] = y;
        pointOnCircle.Coordinates[circleDirection] = thirdCoordinate;

        return pointOnCircle;
    }
}
