using CmmSimulatorLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CmmSimulatorLibrary.Models;
public class SlotModel : FeatureModel
{
    public double Length { get; }
    public double Width { get; }
    public int NumberOfDivisons { get; }
    public bool IsRound { get; }

    public SlotModel() { } 

    public SlotModel(double x, double y, double z, double i, double j, double k, double length, double width, int numberOfDivisions, bool isInner,bool isRound) : base(x, y, z, i, j, k, isInner)
    {
        Length = length;
        Width = width;
        NumberOfDivisons = numberOfDivisions;
        IsRound = isRound;
    }

    public SlotModel GetSlotFromMeasurementBlock(List<string> measurementBlock)
    {
        double[] slotData = GetFeatureData(measurementBlock[0]);
        int numberOfDivisions = GetNumberOfMeasurementPoints(measurementBlock[1]);
        bool isInner = measurementBlock[0].Contains("INNER");
        bool isRound = measurementBlock[0].Contains("ROUND");
        //only for INNER
        SlotModel slot = new SlotModel(slotData[0], slotData[1], slotData[2], slotData[3], slotData[4], slotData[5], slotData[9], slotData[10],numberOfDivisions,isInner,isRound);

        return slot;
    }

    public double GetSlotMeasurementTime(SlotModel slot, CMMModel CMM)
    {
        double output = 0;

        if(slot.IsInner)
        {
            if(slot.IsRound)
            {
                double slotArcCenterX1 = slot.Coordinates.XAxis + (slot.Length - slot.Width) / 2;
                double slotArcCenterX2 = slot.Coordinates.XAxis - (slot.Length - slot.Width) / 2;
                CircleModel arc1 = new CircleModel(
                    slotArcCenterX1,
                    slot.Coordinates.YAxis,
                    slot.Coordinates.ZAxis,
                    slot.Vectors.XAxis,
                    slot.Vectors.YAxis,
                    slot.Vectors.ZAxis,
                    slot.Width,
                    slot.NumberOfDivisons / 2);
                CircleModel arc2 = new CircleModel(
                    slotArcCenterX2,
                    slot.Coordinates.YAxis,
                    slot.Coordinates.ZAxis,
                    slot.Vectors.XAxis,
                    slot.Vectors.YAxis,
                    slot.Vectors.ZAxis,
                    slot.Width,
                    slot.NumberOfDivisons / 2);

                output += arc1.GetCircleMeasurementTime(arc1, CMM);
                output += GetMoveToSecondArcTime(arc1,arc2,CMM);
                output += arc2.GetCircleMeasurementTime(arc2, CMM);
            }
        }

        return output;
    }

    private double GetMoveToSecondArcTime(CircleModel arc1, CircleModel arc2, CMMModel CMM)
    {
        double output = 0;

        double distanceToTravel;
        double diagonalAcceleration = Physics.GetDiagonalAcceleration(
            arc2.Vectors.XAxis != 0 ? CMM.Acceleration.XAxis : 0,
            arc2.Vectors.YAxis != 0 ? CMM.Acceleration.YAxis : 0,
            arc2.Vectors.ZAxis != 0 ? CMM.Acceleration.ZAxis : 0);
        double diagonalVelocity = Physics.GetDiagonalAcceleration(
            arc2.Vectors.XAxis != 0 ? CMM.Velocity.XAxis : 0,
            arc2.Vectors.YAxis != 0 ? CMM.Velocity.YAxis : 0,
            arc2.Vectors.ZAxis != 0 ? CMM.Velocity.ZAxis : 0);

        distanceToTravel = Library3D.GetDistanceBetweenTwoPoints(
            arc1.Coordinates.XAxis, arc2.Coordinates.XAxis,
            arc1.Coordinates.YAxis, arc2.Coordinates.YAxis,
            arc1.Coordinates.ZAxis, arc2.Coordinates.ZAxis);

        output += Physics.GetTimeToTravelDistance(distanceToTravel, diagonalVelocity, diagonalAcceleration);

        return output;
    }
}
