using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class Formulas
{
    public static float3 DeCasteljau([ReadOnly] BezierCurve controlPoints, float t)
    {
        // Use De Casteljau algorithm to calculate point on curve for given value of t
        NativeList<float3> points = new NativeList<float3>(Allocator.Temp);

        points.Add(new float3() { x = controlPoints.ControlPoint0.x, y = controlPoints.ControlPoint0.y, z = controlPoints.ControlPoint0.z });
        points.Add(new float3() { x = controlPoints.ControlPoint1.x, y = controlPoints.ControlPoint1.y, z = controlPoints.ControlPoint1.z });
        points.Add(new float3() { x = controlPoints.ControlPoint2.x, y = controlPoints.ControlPoint2.y, z = controlPoints.ControlPoint2.z });
        points.Add(new float3() { x = controlPoints.ControlPoint3.x, y = controlPoints.ControlPoint3.y, z = controlPoints.ControlPoint3.z });

        for (int j = 3; j > 0; j--)
        {
            for (int i = 0; i < j; i++)
            {
                points[i] = math.lerp(points[i], points[i + 1], t);
            }
        }

        return points[0];
    }

    public static Vector3 CalculateBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float oneMinusT = 1f - t;
        return oneMinusT * oneMinusT * oneMinusT * p0 +
               3f * oneMinusT * oneMinusT * t * p1 +
               3f * oneMinusT * t * t * p2 +
               t * t * t * p3;
    }
}
