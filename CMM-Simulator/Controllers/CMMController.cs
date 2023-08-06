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
    PointModel StartPoint = new PointModel(0, 0, 0, 0, 0, 0);

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

    public double GetFeatureMeasurementTime(Features featureType, FeatureModel feature,CMMModel CMM)
    {
        double output = 0;

        switch(featureType)
        {
            case Features.CIRCLE:
                CircleModel circle = (CircleModel)feature;
                output += GetClearanceMoveTime(circle, CMM);
                output += GetMoveToFeatureTime(circle, CMM);
                SetStartPoint(circle);
                output += circle.GetCircleMeasurementTime(circle, CMM);
                break;
            case Features.CYLNDR:
                CylinderModel cylinder = (CylinderModel)feature;
                output += GetClearanceMoveTime(cylinder, CMM);
                output += GetMoveToFeatureTime(cylinder, CMM);
                SetStartPoint(cylinder);
                output += cylinder.GetCylinderMeasurementTime(cylinder, CMM);
                break;
            case Features.POINT:
                PointModel pointToMeasure = (PointModel)feature;
                output += GetClearanceMoveTime(pointToMeasure, CMM);
                output += GetMoveToFeatureTime(pointToMeasure, CMM);
                SetStartPoint(pointToMeasure);
                PointModel pointAtApproachDistance = Library3D.GetPointAtDistanceFrom(pointToMeasure, CMM.Settings["APPRCH"]);
                output += GetDiagonalMoveToPointTime(StartPoint, pointAtApproachDistance, CMM);
                output += GetSinglePointMeasurementTime(pointAtApproachDistance, pointToMeasure, CMM);
                PointModel pointAtRetractDistance = Library3D.GetPointAtDistanceFrom(pointToMeasure, CMM.Settings["RETRCT"]);
                output += GetRetractFromPointTime(pointToMeasure, pointAtRetractDistance, CMM);
                SetStartPoint(pointAtRetractDistance);
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
                    output += GetFeatureMeasurementTime(Features.CIRCLE, circle, CMM1);
                    SetStartPoint(circle);
                    break;
                case Features.ARC:
                    CircleModel arc = new CircleModel(0, 0, 0, 0, 0, 0, 0, 0);
                    arc = arc.GetCircleFromMeasurementBlock(measurementBlock);
                    arc.Diameter = arc.Diameter * 2;
                    output += GetFeatureMeasurementTime(Features.CIRCLE, arc, CMM1);
                    SetStartPoint(arc);
                    break;
                case Features.CYLNDR:
                    CylinderModel cylinder = new CylinderModel(0, 0, 0, 0, 0, 0, 0, 0, 0);
                    cylinder = cylinder.GetCylinderFromMeasurementBlock(measurementBlock);
                    output += GetFeatureMeasurementTime(Features.CYLNDR, cylinder,CMM1);
                    break;
                case Features.POINT:
                    double[] data = FeatureModel.GetFeatureData(measurementBlock.First());
                    PointModel pointToMeasure = new PointModel(data[0], data[1], data[2], data[3], data[4], data[5]);
                    output += GetFeatureMeasurementTime(Features.POINT, pointToMeasure, CMM1);
                    break;
                case Features.EDGEPT:
                    double[] edgePointData = FeatureModel.GetFeatureData(measurementBlock.First());
                    PointModel edgePoint = new PointModel(edgePointData[0], edgePointData[1], edgePointData[2], edgePointData[3], edgePointData[4], edgePointData[5]);
                    output += GetFeatureMeasurementTime(Features.POINT, edgePoint, CMM1);
                    break;
                case Features.CPARLN:
                    double[] slotData = FeatureModel.GetFeatureData(measurementBlock.First());
                    SlotModel slot = new SlotModel();
                    slot = slot.GetSlotFromMeasurementBlock(measurementBlock);
                    output += GetMoveToFeatureTime(slot, CMM1);
                    output += slot.GetSlotMeasurementTime(slot, CMM1);
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
                    PointModel endPoint = Library3D.GetPointAtDistanceFrom(pointToMeasure, CMM1.Settings["APPRCH"]);
                    output += GetDiagonalMoveToPointTime(StartPoint, endPoint, CMM1);
                    output += GetSinglePointMeasurementTime(endPoint, pointToMeasure, CMM1);
                    endPoint = Library3D.GetPointAtDistanceFrom(pointToMeasure, CMM1.Settings["RETRCT"]);
                    output += GetRetractFromPointTime(pointToMeasure, endPoint, CMM1);
                    SetStartPoint(pointToMeasure);
                }
            }
        }

        return output;
    }

    private void SetStartPoint(FeatureModel currentLocationPoint)
    {
        StartPoint = new PointModel(currentLocationPoint.Coordinates.XAxis, currentLocationPoint.Coordinates.YAxis, currentLocationPoint.Coordinates.ZAxis,
                        currentLocationPoint.Vectors.XAxis, currentLocationPoint.Vectors.YAxis, currentLocationPoint.Vectors.ZAxis);
    }

    private double GetMoveToFeatureTime(FeatureModel feature, CMMModel CMM1)
    {
        double output = 0;

        //X-axis
        double distanceToTravel = Library3D.GetLinearDistance(StartPoint.Coordinates.XAxis, feature.Coordinates.XAxis);
        output += Physics.GetTimeToTravelDistance(distanceToTravel, CMM1.Velocity.XAxis, CMM1.Acceleration.XAxis);

        //Y-axis
        distanceToTravel = Library3D.GetLinearDistance(StartPoint.Coordinates.YAxis, feature.Coordinates.YAxis);
        output += Physics.GetTimeToTravelDistance(distanceToTravel, CMM1.Velocity.YAxis, CMM1.Acceleration.YAxis);

        //Z-axis
        distanceToTravel = Library3D.GetLinearDistance(StartPoint.Coordinates.ZAxis, feature.Coordinates.ZAxis);
        output += Physics.GetTimeToTravelDistance(distanceToTravel, CMM1.Velocity.ZAxis, CMM1.Acceleration.ZAxis);

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
                0, goTo.Coordinates.XAxis,
                0, goTo.Coordinates.YAxis,
                0, goTo.Coordinates.XAxis);
            double diagonalVelocity = Physics.GetDiagonalVelocity(CMM1.Velocity.ZAxis, CMM1.Velocity.YAxis, CMM1.Velocity.ZAxis);
            double diagonalAcceleration = Physics.GetDiagonalVelocity(CMM1.Acceleration.XAxis, CMM1.Acceleration.YAxis, CMM1.Acceleration.ZAxis);

            output += Physics.GetTimeToTravelDistance(distanceToTravel, diagonalVelocity, diagonalAcceleration);
            SetStartPoint(goTo);
        }
        else if(goToType == GoTo.INCR)
        {
            double distanceToTravel = goToData[0];
            PointModel goToPoint =
                new PointModel(
                StartPoint.Coordinates.XAxis,
                StartPoint.Coordinates.YAxis,
                StartPoint.Coordinates.ZAxis,
                0,
                0,
                0);

            if (goToData[1] != 0)
            {
                goToPoint.Vectors.XAxis = goToData[1];
            }
            if (goToData[2] != 0)
            {
                goToPoint.Vectors.YAxis = goToData[2];
            }
            if (goToData[3] != 0)
            {
                goToPoint.Vectors.ZAxis = goToData[3];
            }
            PointModel endPoint = Library3D.GetPointAtDistanceFrom(goToPoint, distanceToTravel);
            output += GetDiagonalMoveToPointTime(StartPoint, endPoint, CMM1);
            SetStartPoint(endPoint);
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
            endPoint.Vectors.XAxis != 0 ? CMM1.Acceleration.XAxis : 0,
            endPoint.Vectors.YAxis != 0 ? CMM1.Acceleration.YAxis : 0,
            endPoint.Vectors.ZAxis != 0 ? CMM1.Acceleration.ZAxis : 0);

        distanceToTravel = Library3D.GetDistanceBetweenTwoPoints(
            startPoint.Coordinates.XAxis, endPoint.Coordinates.XAxis,
            startPoint.Coordinates.YAxis, endPoint.Coordinates.YAxis,
            startPoint.Coordinates.ZAxis, endPoint.Coordinates.ZAxis);

        output += Physics.GetTimeToTravelDistance(distanceToTravel, CMM1.TouchSpeed, diagonalAcceleration);

        return output;
    }

    double GetDiagonalMoveToPointTime(PointModel startPoint, PointModel endPoint, CMMModel CMM1)
    {
        double output = 0;

        double distanceToTravel;
        double diagonalAcceleration = Physics.GetDiagonalAcceleration(
            endPoint.Vectors.XAxis != 0 ? CMM1.Acceleration.XAxis : 0,
            endPoint.Vectors.YAxis != 0 ? CMM1.Acceleration.YAxis : 0,
            endPoint.Vectors.ZAxis != 0 ? CMM1.Acceleration.ZAxis : 0);
        double diagonalVelocity = Physics.GetDiagonalAcceleration(
            endPoint.Vectors.XAxis != 0 ? CMM1.Velocity.XAxis : 0,
            endPoint.Vectors.YAxis != 0 ? CMM1.Velocity.YAxis : 0,
            endPoint.Vectors.ZAxis != 0 ? CMM1.Velocity.ZAxis : 0);

        distanceToTravel = Library3D.GetDistanceBetweenTwoPoints(
            startPoint.Coordinates.XAxis, endPoint.Coordinates.XAxis,
            startPoint.Coordinates.YAxis, endPoint.Coordinates.YAxis,
            startPoint.Coordinates.ZAxis, endPoint.Coordinates.ZAxis);

        output += Physics.GetTimeToTravelDistance(distanceToTravel, diagonalVelocity, diagonalAcceleration);

        return output;
    }

    private double GetRetractFromPointTime(PointModel pointToMeasure, PointModel endPoint, CMMModel CMM1)
    {
        double distanceToTravel = Library3D.GetDistanceBetweenTwoPoints(
            pointToMeasure.Coordinates.XAxis, endPoint.Coordinates.XAxis,
            pointToMeasure.Coordinates.YAxis, endPoint.Coordinates.YAxis,
            pointToMeasure.Coordinates.ZAxis, endPoint.Coordinates.ZAxis);

        return Physics.GetTimeToTravelDistance(distanceToTravel, CMM1.RetractSpeed, CMM1.RetractAcceleration);
    }

    private double GetClearanceMoveTime(FeatureModel featureToMeasure, CMMModel CMM1)
    {
        double output = 0;

        double distanceToTravel;
        double diagonalAcceleration = Physics.GetDiagonalAcceleration(
            featureToMeasure.Vectors.XAxis != 0 ? CMM1.Acceleration.XAxis : 0,
            featureToMeasure.Vectors.YAxis != 0 ? CMM1.Acceleration.YAxis : 0,
            featureToMeasure.Vectors.ZAxis != 0 ? CMM1.Acceleration.ZAxis : 0);
        double diagonalVelocity = Physics.GetDiagonalAcceleration(
            featureToMeasure.Vectors.XAxis != 0 ? CMM1.Velocity.XAxis : 0,
            featureToMeasure.Vectors.YAxis != 0 ? CMM1.Velocity.YAxis : 0,
            featureToMeasure.Vectors.ZAxis != 0 ? CMM1.Velocity.ZAxis : 0);

        PointModel endpoint = Library3D.GetPointAtDistanceFrom(StartPoint, CMM1.Settings["CLRSRF"]);
        distanceToTravel = Library3D.GetDistanceBetweenTwoPoints(
            StartPoint.Coordinates.XAxis, endpoint.Coordinates.XAxis,
            StartPoint.Coordinates.YAxis, endpoint.Coordinates.YAxis,
            StartPoint.Coordinates.ZAxis, endpoint.Coordinates.ZAxis);

        output += Physics.GetTimeToTravelDistance(distanceToTravel, diagonalVelocity, diagonalAcceleration);

        return output;
    }
}
