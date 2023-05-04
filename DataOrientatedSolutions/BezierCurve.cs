// An Icomponent data component used to hold together multiple bezier curves as a spline
using Unity.Entities;
using Unity.Mathematics;

public struct BezierManager : IComponentData { }


// The data used to define a Bezier curve
public struct BezierCurve : IBufferElementData
{
    public float3 ControlPoint0;
    public float3 ControlPoint1;
    public float3 ControlPoint2;
    public float3 ControlPoint3;
}