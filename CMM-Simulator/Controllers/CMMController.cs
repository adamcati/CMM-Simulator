using CMM_Simulator.Enums;
using CMM_Simulator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CMM_Simulator.Controllers;
public class CMMController
{
    PointModel startPoint = new PointModel(0, 0, 0, 0, 0, 0);

    public double GetTimeOfBlockfExecution(Operations actionType, List<string> measurementBlock,CMMModel CMM1)
    {
        double output = 0;

        switch (actionType)
        {
            case Operations.Approach:
                //output += GetTimeToApproach(GetAxisOfMovement(measurementBlock));
                break;
            case Operations.Measurement:
                Features featureType = FeatureModel.GetFeatureType(measurementBlock.First());
                ProgramModes measurementMode = GetMeasurementMode(measurementBlock);
                output += GetMeasurementTime(featureType, measurementMode, measurementBlock, CMM1);
                break;
            case Operations.GoTo:
                GoTo goToType = GetGoToType(measurementBlock.First());
                double[] goToData = FeatureModel.GetFeatureData(measurementBlock.First());
                output += GetGoToMovementTime(goToType, goToData, CMM1);
                break;
            case Operations.SensorSelect:
                output += 4;
                break;
        };

        return output;
    }

    private double GetFeatureMeasurementTime(Features featureType, FeatureModel feature,CMMModel CMM)
    {
        double output = 0;

        switch(featureType)
        {
            case Features.CIRCLE:
                CircleModel circle = (CircleModel)feature;
                output += GetClearanceMoveTime(circle, CMM);
                output += GetMoveToFeatureTime(circle, CMM);
                output += circle.GetCircleMeasurementTime(circle, CMM);
                startPoint = new PointModel(circle.Coordinates["x-axis"], circle.Coordinates["y-axis"], circle.Coordinates["z-axis"],
                    circle.Vectors["x-axis"], circle.Vectors["x-axis"], circle.Vectors["x-axis"]);
                break;
        }

        return output;
    }

    private double GetMeasurementTime(Features featureType, ProgramModes measurementMode, List<string> measurementBlock, CMMModel CMM1)
    {
        double output = 0;

        if(measurementMode == ProgramModes.AUTO)
        {
            switch(featureType)
            {
                case Features.CIRCLE:
                    CircleModel circle = new CircleModel(0,0,0,0,0,0,0,0);
                    circle = circle.GetCircleFromMeasurementBlock(measurementBlock);
                    output += GetClearanceMoveTime(circle, CMM1);
                    output += GetMoveToFeatureTime(circle, CMM1);
                    output += circle.GetCircleMeasurementTime(circle, CMM1);
                    startPoint = new PointModel(circle.Coordinates["x-axis"], circle.Coordinates["y-axis"], circle.Coordinates["z-axis"],
                        circle.Vectors["x-axis"], circle.Vectors["x-axis"], circle.Vectors["x-axis"]);
                    break;
                case Features.POINT:
                    double[] data = FeatureModel.GetFeatureData(measurementBlock.First());
                    PointModel pointToMeasure = new PointModel(data[0], data[1], data[2], data[3], data[4], data[5]);
                    output += GetClearanceMoveTime(pointToMeasure, CMM1);
                    output += GetMoveToFeatureTime(pointToMeasure, CMM1);
                    PointModel pointAtApproachDistance = GetPointAtDistanceFrom(pointToMeasure, CMM1.Settings["APPRCH"]);
                    output += GetDiagonalMoveToPointTime(startPoint, pointAtApproachDistance, CMM1);
                    output += GetSinglePointMeasurementTime(pointAtApproachDistance, pointToMeasure, CMM1);
                    PointModel pointAtRetractDistance = GetPointAtDistanceFrom(pointToMeasure, CMM1.Settings["RETRCT"]);
                    output += GetRetractFromPointTime(pointToMeasure, pointAtRetractDistance, CMM1);
                    startPoint = pointToMeasure;
                    break;
                case Features.ARC:
                    CircleModel arc = new CircleModel(0, 0, 0, 0, 0, 0, 0, 0);
                    arc = arc.GetCircleFromMeasurementBlock(measurementBlock);
                    arc.Diameter = arc.Diameter * 2;
                    output += GetClearanceMoveTime(arc,CMM1);
                    output += GetMoveToFeatureTime(arc, CMM1);
                    output += arc.GetCircleMeasurementTime(arc, CMM1);
                    startPoint = new PointModel(arc.Coordinates["x-axis"], arc.Coordinates["y-axis"], arc.Coordinates["z-axis"],
                        arc.Vectors["x-axis"], arc.Vectors["x-axis"], arc.Vectors["x-axis"]);
                    break;
                case Features.CYLNDR:
                    CylinderModel cylinder = new CylinderModel(0, 0, 0, 0, 0, 0, 0, 0, 0);
                    cylinder = cylinder.GetCylinderFromMeasurementBlock(measurementBlock);
                    PointModel circleCenter = GetPointAtDistanceFrom(new PointModel(
                        cylinder.Coordinates["x-axis"],
                        cylinder.Coordinates["y-axis"],
                        cylinder.Coordinates["z-axis"],
                        cylinder.Vectors["x-axis"],
                        cylinder.Vectors["y-axis"],
                        cylinder.Vectors["z-axis"]),
                        CMM1.Settings["DEPTH"]);
                    CircleModel circleFromCylinder = new CircleModel(circleCenter, cylinder.Diameter, cylinder.NumberOfDivisions / 2);
                    output += GetFeatureMeasurementTime(Features.CIRCLE, circleFromCylinder, CMM1);
                    circleCenter = GetPointAtDistanceFrom(new PointModel(
                        cylinder.Coordinates["x-axis"],
                        cylinder.Coordinates["y-axis"],
                        cylinder.Coordinates["z-axis"],
                        cylinder.Vectors["x-axis"],
                        cylinder.Vectors["y-axis"],
                        cylinder.Vectors["z-axis"]),
                        cylinder.Length < 0 ? cylinder.Length + CMM1.Settings["DEPTH"] : cylinder.Length - CMM1.Settings["DEPTH"]);
                    circleFromCylinder = new CircleModel(circleCenter, cylinder.Diameter, cylinder.NumberOfDivisions / 2);
                    output += GetFeatureMeasurementTime(Features.CIRCLE, circleFromCylinder, CMM1);
                    break;
                default:
                    throw new Exception($"Feature simulation not implemented {featureType}");
                    break;
            };
        }
        else if(measurementMode == ProgramModes.PROGRAM)
        {
           foreach(string block in measurementBlock)
            {
                if(block.Contains("PTMEAS"))
                {
                    double[] data = FeatureModel.GetFeatureData(block);
                    PointModel pointToMeasure = new PointModel(data[0], data[1], data[2], data[3], data[4], data[5]);
                    PointModel endPoint = GetPointAtDistanceFrom(pointToMeasure, CMM1.Settings["APPRCH"]);
                    output += GetDiagonalMoveToPointTime(startPoint, endPoint, CMM1);
                    output += GetSinglePointMeasurementTime(endPoint, pointToMeasure, CMM1);
                    endPoint = GetPointAtDistanceFrom(pointToMeasure, CMM1.Settings["RETRCT"]);
                    output += GetRetractFromPointTime(pointToMeasure, endPoint, CMM1);
                    startPoint = pointToMeasure;
                }
            }
        }

        return output;
    }

    private double GetMoveToFeatureTime(FeatureModel feature, CMMModel CMM1)
    {
        double output = 0;

        foreach(string axis in feature.Coordinates.Keys)
        {
            double distanceToTravel = Library3D.GetLinearDistance(startPoint.Coordinates[axis], feature.Coordinates[axis]);
            output += Physics.GetTimeToTravelDistance(distanceToTravel, CMM1.Velocity[axis], CMM1.Acceleration[axis]);
            startPoint.Coordinates[axis] = feature.Coordinates[axis];
        }

        return output;
    }

    private double GetTimeToApproach(string axis, CMMModel CMM1)
    {
        double output;

        output = Physics.GetTimeToTravelDistance(CMM1.Settings["APPRCH"], CMM1.TouchSpeed, CMM1.Acceleration[axis]);

        return output;
    }

    PointModel GetPointAtDistanceFrom(PointModel point, double distance)
    {
        PointModel output = new PointModel(0,0,0,0,0,0);
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

    GoTo GetGoToType(string fileLine)
    {
        if (fileLine.Contains("INCR"))
            return GoTo.INCR;

        return GoTo.CART;
    }

    ProgramModes GetMeasurementMode(List<string> measurementBlock)
    {
        foreach (string block in measurementBlock)
        {
            if (block.Contains("PTMEAS"))
            {
                return ProgramModes.PROGRAM;
            }
        }

        return ProgramModes.AUTO;
    }

    double GetGoToMovementTime(GoTo goToType, double[] goToData, CMMModel CMM1)
    {
        double output = 0;

        if(goToType == GoTo.CART)
        {
            PointModel goTo = new PointModel(goToData[0], goToData[1], goToData[2], 0, 0, 0);
            double distanceToTravel = Library3D.GetDistanceBetweenTwoPoints(
                0, goTo.Coordinates["x-axis"],
                0, goTo.Coordinates["y-axis"],
                0, goTo.Coordinates["z-axis"]);
            double diagonalVelocity = Physics.GetDiagonalVelocity(CMM1.Velocity["x-axis"], CMM1.Velocity["y-axis"], CMM1.Velocity["z-axis"]);
            double diagonalAcceleration = Physics.GetDiagonalVelocity(CMM1.Acceleration["x-axis"], CMM1.Acceleration["y-axis"], CMM1.Acceleration["z-axis"]);

            output += Physics.GetTimeToTravelDistance(distanceToTravel, diagonalVelocity, diagonalAcceleration);
        }
        else if(goToType == GoTo.INCR)
        {
            PointModel goTo = new PointModel(goToData[0], goToData[1], goToData[2], 0,0,0);
            foreach(string axis in goTo.Vectors.Keys)
            {
                if (goTo.Vectors[axis] == 1)
                {
                    output += Physics.GetTimeToTravelDistance(goTo.Coordinates[axis], CMM1.Velocity[axis], CMM1.Acceleration[axis]);
                }
            }
        }   
        else
        {
            throw new NotImplementedException($"GoTo type: {goToType} not recognized");
        }

        return output;
    }
    double GetSinglePointMeasurementTime(PointModel startPoint, PointModel endPoint, CMMModel CMM1)
    {
        double output = 0;
        double distanceToTravel;

        double diagonalAcceleration = Physics.GetDiagonalAcceleration(
            endPoint.Vectors["x-axis"] != 0 ? CMM1.Acceleration["x-axis"] : 0,
            endPoint.Vectors["y-axis"] != 0 ? CMM1.Acceleration["y-axis"] : 0,
            endPoint.Vectors["z-axis"] != 0 ? CMM1.Acceleration["z-axis"] : 0);

        distanceToTravel = Library3D.GetDistanceBetweenTwoPoints(
            startPoint.Coordinates["x-axis"], endPoint.Coordinates["x-axis"],
            startPoint.Coordinates["y-axis"], endPoint.Coordinates["y-axis"],
            startPoint.Coordinates["z-axis"], endPoint.Coordinates["z-axis"]);

        output += Physics.GetTimeToTravelDistance(distanceToTravel, CMM1.TouchSpeed, diagonalAcceleration);

        return output;
    }

    double GetDiagonalMoveToPointTime(PointModel startpoint, PointModel endPoint, CMMModel CMM1)
    {
        double output = 0;

        double distanceToTravel;
        double diagonalAcceleration = Physics.GetDiagonalAcceleration(
            endPoint.Vectors["x-axis"] != 0 ? CMM1.Acceleration["x-axis"] : 0,
            endPoint.Vectors["y-axis"] != 0 ? CMM1.Acceleration["y-axis"] : 0,
            endPoint.Vectors["z-axis"] != 0 ? CMM1.Acceleration["z-axis"] : 0);
        double diagonalVelocity = Physics.GetDiagonalAcceleration(
            endPoint.Vectors["x-axis"] != 0 ? CMM1.Velocity["x-axis"] : 0,
            endPoint.Vectors["y-axis"] != 0 ? CMM1.Velocity["y-axis"] : 0,
            endPoint.Vectors["z-axis"] != 0 ? CMM1.Velocity["z-axis"] : 0);

        distanceToTravel = Library3D.GetDistanceBetweenTwoPoints(
            startPoint.Coordinates["x-axis"], endPoint.Coordinates["x-axis"],
            startPoint.Coordinates["y-axis"], endPoint.Coordinates["y-axis"],
            startPoint.Coordinates["z-axis"], endPoint.Coordinates["z-axis"]);

        output += Physics.GetTimeToTravelDistance(distanceToTravel, diagonalVelocity, diagonalAcceleration);

        return output;
    }

    private double GetRetractFromPointTime(PointModel pointToMeasure, PointModel endPoint, CMMModel CMM1)
    {
        double distanceToTravel = Library3D.GetDistanceBetweenTwoPoints(
            startPoint.Coordinates["x-axis"], endPoint.Coordinates["x-axis"],
            startPoint.Coordinates["y-axis"], endPoint.Coordinates["y-axis"],
            startPoint.Coordinates["z-axis"], endPoint.Coordinates["z-axis"]);

        return Physics.GetTimeToTravelDistance(distanceToTravel, CMM1.RetractSpeed, CMM1.RetractAcceleration);
    }

    private double GetClearanceMoveTime(FeatureModel featureToMeasure, CMMModel CMM1)
    {
        double output = 0;

        double distanceToTravel;
        double diagonalAcceleration = Physics.GetDiagonalAcceleration(
            featureToMeasure.Vectors["x-axis"] != 0 ? CMM1.Acceleration["x-axis"] : 0,
            featureToMeasure.Vectors["y-axis"] != 0 ? CMM1.Acceleration["y-axis"] : 0,
            featureToMeasure.Vectors["z-axis"] != 0 ? CMM1.Acceleration["z-axis"] : 0);
        double diagonalVelocity = Physics.GetDiagonalAcceleration(
            featureToMeasure.Vectors["x-axis"] != 0 ? CMM1.Velocity["x-axis"] : 0,
            featureToMeasure.Vectors["y-axis"] != 0 ? CMM1.Velocity["y-axis"] : 0,
            featureToMeasure.Vectors["z-axis"] != 0 ? CMM1.Velocity["z-axis"] : 0);

        PointModel endpoint = GetPointAtDistanceFrom(startPoint, CMM1.Settings["CLRSRF"]);
        distanceToTravel = Library3D.GetDistanceBetweenTwoPoints(
            startPoint.Coordinates["x-axis"], endpoint.Coordinates["x-axis"],
            startPoint.Coordinates["y-axis"], endpoint.Coordinates["y-axis"],
            startPoint.Coordinates["z-axis"], endpoint.Coordinates["z-axis"]);

        output += Physics.GetTimeToTravelDistance(distanceToTravel, diagonalVelocity, diagonalAcceleration);

        return output;
    }
}
