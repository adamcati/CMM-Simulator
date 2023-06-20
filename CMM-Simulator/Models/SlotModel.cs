using CMM_Simulator.Controllers;
using CMM_Simulator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CMM_Simulator.Models;
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
                double slotArcCenterX1 = slot.Coordinates["x-axis"] + (slot.Length - slot.Width) / 2;
                double slotArcCenterX2 = slot.Coordinates["x-axis"] - (slot.Length - slot.Width) / 2;
                CircleModel arc1 = new CircleModel(
                    slotArcCenterX1,
                    slot.Coordinates["y-axis"],
                    slot.Coordinates["z-axis"],
                    slot.Vectors["x-axis"],
                    slot.Vectors["y-axis"],
                    slot.Vectors["z-axis"],
                    slot.Width,
                    slot.NumberOfDivisons / 2);
                CircleModel arc2 = new CircleModel(
                    slotArcCenterX2,
                    slot.Coordinates["y-axis"],
                    slot.Coordinates["z-axis"],
                    slot.Vectors["x-axis"],
                    slot.Vectors["y-axis"],
                    slot.Vectors["z-axis"],
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
            arc2.Vectors["x-axis"] != 0 ? CMM.Acceleration["x-axis"] : 0,
            arc2.Vectors["y-axis"] != 0 ? CMM.Acceleration["y-axis"] : 0,
            arc2.Vectors["z-axis"] != 0 ? CMM.Acceleration["z-axis"] : 0);
        double diagonalVelocity = Physics.GetDiagonalAcceleration(
            arc2.Vectors["x-axis"] != 0 ? CMM.Velocity["x-axis"] : 0,
            arc2.Vectors["y-axis"] != 0 ? CMM.Velocity["y-axis"] : 0,
            arc2.Vectors["z-axis"] != 0 ? CMM.Velocity["z-axis"] : 0);

        distanceToTravel = Library3D.GetDistanceBetweenTwoPoints(
            arc1.Coordinates["x-axis"], arc2.Coordinates["x-axis"],
            arc1.Coordinates["y-axis"], arc2.Coordinates["y-axis"],
            arc1.Coordinates["z-axis"], arc2.Coordinates["z-axis"]);

        output += Physics.GetTimeToTravelDistance(distanceToTravel, diagonalVelocity, diagonalAcceleration);

        return output;
    }
}
