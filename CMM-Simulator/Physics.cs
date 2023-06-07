using CMM_Simulator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMM_Simulator;
public class Physics
{
    //distance in meters

    static double GetTimeToReachMaxVelocity(double maxVelocity, double acceleration)
    {
        return maxVelocity / acceleration;
    }
    public static double GetDistanceToReachMaxVelocity(double maxVelocity, double acceleration, double timeToReachMaxVelocity)
    {
        return 0.5 * acceleration * timeToReachMaxVelocity * timeToReachMaxVelocity;
        //return (velocity * velocity) / (2 * acceleration);
    }

    //deceleration can be equal with the acceleration
    double GetDistanceToStop(double velocity, double deceleration)
    {
        return (velocity * velocity) / (2 * deceleration);
    }

    static double GetTimeTraveledAtMaxVelocity(double distance, double velocity)
    {
        return distance / velocity;
    }

    static double GetTimeTraveledAccelerating(double distance, double acceleration)
    {
        return Math.Sqrt(2 * distance / acceleration);
    }

    // deceleration can be equal with the acceleration
    static double GetTimeItTakesToStop(double distance, double deceleration)
    {
        return Math.Sqrt(2 * distance / deceleration);
    }

    public static double GetTimeToTravelDistance(double distanceToTravel, double maxVelocity,double acceleration)
    {
        double output = 0;

        double timeToReachMaxVelocity = GetTimeToReachMaxVelocity(maxVelocity, acceleration);

        double distanceToReachMaxVelocity = GetDistanceToReachMaxVelocity(maxVelocity, acceleration,timeToReachMaxVelocity);

        if (distanceToTravel <= 2 * distanceToReachMaxVelocity && distanceToTravel > 0)
        {
            output += GetTimeTraveledAccelerating(distanceToTravel, acceleration);
        }
        else if (distanceToTravel > 2 * distanceToReachMaxVelocity)
        {
            distanceToTravel -= distanceToReachMaxVelocity * 2;
            output += GetTimeTraveledAccelerating(distanceToReachMaxVelocity, acceleration);
            output += GetTimeTraveledAtMaxVelocity(distanceToTravel, maxVelocity);
            output += GetTimeItTakesToStop(distanceToReachMaxVelocity, acceleration);
        }

        return output;
    }

    internal static double GetDiagonalVelocity(double v1, double v2, double v3)
    {
        return Math.Sqrt(v1 * v1 + v2 * v2 + v3 * v3);
    }

    internal static double GetDiagonalAcceleration(double a1, double a2, double a3)
    {
        return Math.Sqrt(a1 * a1 + a2 * a2 + a3 * a3);
    }

    internal static double GetDiagonalMovementTimeToReachMaxVelocity(double[] velocities, double[] accelerations)
    {
        double output = 0;

        for(int i = 0; i < velocities.Length; i++)
        {
            double timeToReachMaxVelocity = GetTimeToReachMaxVelocity(velocities[i], accelerations[i]);
            if(timeToReachMaxVelocity > output)
            {
                output = timeToReachMaxVelocity;
            }
        }

        return output;
    }
}
