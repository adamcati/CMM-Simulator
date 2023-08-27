using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmmSimulatorLibrary.Models;
public class CircleModel : FeatureModel
{
    public double Diameter { get; set; }
    public int NumberOfDivisions { get; set; }

    public CircleModel() { }

    public CircleModel(double x, double y, double z, double i, double j, double k, double diameter, int numberOfDivisions) : base(x,y,z,i,j,k)
    {
        Diameter = diameter;
        NumberOfDivisions = numberOfDivisions;
    }

    public CircleModel(PointModel point, double diameter, int numberOfDivisions)
    {
        Diameter = diameter;
        NumberOfDivisions = numberOfDivisions;
        this.Coordinates.XAxis = point.Coordinates.XAxis;
        this.Coordinates.YAxis= point.Coordinates.YAxis;
        this.Coordinates.ZAxis = point.Coordinates.ZAxis;
        this.Vectors.XAxis = point.Vectors.XAxis;
        this.Vectors.YAxis = point.Vectors.YAxis;
        this.Vectors.ZAxis = point.Vectors.ZAxis;

    }

    internal int GetNumberOfCircleDivisions(string circleMeasurementBlock)
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
        double circleCoordinateOnDirectionAxis;
        double distanceToTravel = 0;
        double diagonalAcceleration = 0;
        (double x, double y) pointOnCircle2D;

        if (circle.Vectors.XAxis == 1)
        {
            circleCoordinateOnDirectionAxis = circle.Coordinates.XAxis;

            pointOnCircle2D = GetOnePointOnCircle2D(circle.Coordinates.YAxis,circle.Coordinates.ZAxis,circle.NumberOfDivisions,circle.Diameter / 2);

            diagonalAcceleration = Physics.GetDiagonalAcceleration(CMM.Acceleration.YAxis, CMM.Acceleration.ZAxis, 0);
            distanceToTravel = Library3D.GetDistanceBetweenTwoPoints(
                circle.Coordinates.XAxis, circleCoordinateOnDirectionAxis,
                circle.Coordinates.YAxis, pointOnCircle2D.x,
                circle.Coordinates.ZAxis, pointOnCircle2D.y);
        }
        else if(circle.Vectors.YAxis == 1)
        {
            circleCoordinateOnDirectionAxis = circle.Coordinates.YAxis;

            pointOnCircle2D = GetOnePointOnCircle2D(circle.Coordinates.XAxis,circle.Coordinates.ZAxis,circle.NumberOfDivisions,circle.Diameter / 2);

            diagonalAcceleration = Physics.GetDiagonalAcceleration(CMM.Acceleration.XAxis, CMM.Acceleration.ZAxis, 0);
            distanceToTravel = Library3D.GetDistanceBetweenTwoPoints(
                circle.Coordinates.XAxis, pointOnCircle2D.x,
                circle.Coordinates.YAxis, circleCoordinateOnDirectionAxis,
                circle.Coordinates.ZAxis, pointOnCircle2D.y);
        }
        else if (circle.Vectors.ZAxis == 1)
        {
            circleCoordinateOnDirectionAxis = circle.Coordinates.ZAxis;

            pointOnCircle2D = GetOnePointOnCircle2D(circle.Coordinates.XAxis,circle.Coordinates.YAxis,circle.NumberOfDivisions,circle.Diameter / 2);

            diagonalAcceleration = Physics.GetDiagonalAcceleration(CMM.Acceleration.XAxis, CMM.Acceleration.YAxis, 0);
            distanceToTravel = Library3D.GetDistanceBetweenTwoPoints(
                circle.Coordinates.XAxis, pointOnCircle2D.x,
                circle.Coordinates.YAxis, pointOnCircle2D.y,
                circle.Coordinates.ZAxis, circleCoordinateOnDirectionAxis);
        }
        else //circle is on an inclined plane
        {
            throw new Exception($"Circle on inclined plane not implemented");
        }

        output += circle.NumberOfDivisions * Physics.GetTimeToTravelDistance(distanceToTravel, CMM.TouchSpeed, diagonalAcceleration);
        output += circle.NumberOfDivisions * Physics.GetTimeToTravelDistance(distanceToTravel, CMM.RetractSpeed, CMM.RetractAcceleration);

        return output;
    }

    public (double x,double y) GetOnePointOnCircle2D(double firstCoordinate, double secondCoordinate, int numberOfDivisions, double radius)
    {
        double angle = 360 / numberOfDivisions;
        double xCoord = firstCoordinate + radius * Math.Cos((Math.PI / 180) * angle);
        double yCoord = secondCoordinate + radius * Math.Sin((Math.PI / 180) * angle);

        return (xCoord,yCoord);
    }
}
