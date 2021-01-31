using UnityEngine;

public class TrajectoryScript
{
    public static float[] CalculateTrajectory(Vector2 point1, Vector2 point3, float angle, float arcPeakXPercent)
    {
        //Math credit to: https://www.desmos.com/calculator/lac2i0bgum

        bool shotRightOfGoal = (point3.x < point1.x) ? true : false;

        float distToGoal = Mathf.Abs(point3.x - point1.x);
        if (shotRightOfGoal == true) distToGoal *= -1;

        float point2X = point1.x + distToGoal * arcPeakXPercent;
        float point2Y = point3.y + (Mathf.Tan(angle) * Mathf.Abs(point3.x - point2X));
        Vector2 point2 = new Vector2(point2X, point2Y);

        //Algorithm
        float A1 = -1 * (point1.x * point1.x) + (point2.x * point2.x);
        float B1 = -1 * point1.x + point2.x;
        float D1 = -1 * point1.y + point2.y;

        float A2 = -1 * (point2.x * point2.x) + (point3.x * point3.x);
        float B2 = -1 * point2.x + point3.x;
        float D2 = -1 * point2.y + point3.y;

        float Bmultiplier = -1 * (B2 / B1);
        float A3 = Bmultiplier * A1 + A2;
        float D3 = Bmultiplier * D1 + D2;

        float shotA = D3 / A3;
        float shotB = (D1 - A1 * shotA) / B1;
        float shotC = point1.y - (shotA * (point1.x * point1.x)) - (shotB * point1.x);

        return new float[] { shotA, shotB, shotC };
    }

    public static float[] CalculateStraightTrajectory(Vector2 point1, Vector2 point2)
    {
        float slope = (point1.y - point2.y) / (point1.x - point2.x);
        float mx = slope * point1.x;
        float b = point1.y - mx;

        return new float[] { 0, slope, b };
    }
}
