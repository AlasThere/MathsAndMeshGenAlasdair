using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class DataBasedMeshGen
{

    public static Mesh GenerateMeshAlongCurve(Mesh mesh, float3 lastPoint, DynamicBuffer<BezierCurve> bezierCurve, float increment)
    {
        int[] faceVertices = new int[] { 4, 5, 6, 7 };
        var t = 0.02f; // setting above 0 to avoid 0 division for first point

        foreach (var curve in bezierCurve)
        {
            while (t < 1)
            {
                var nextPoint = Formulas.DeCasteljau(curve, t);
                Vector3 direction = math.normalize((Vector3)nextPoint - (Vector3)lastPoint);

                float extrusionAmount = math.abs(math.distance(nextPoint, lastPoint));

                Vector3[] newVertices = new Vector3[4];
                for (int i = 0; i < faceVertices.Length; i++)
                {
                    int vertexIndex = faceVertices[i];
                    Vector3 vertex = mesh.vertices[vertexIndex];
                    vertex += direction * extrusionAmount;
                    newVertices[i] = vertex;
                }

                var rotatedVertices = MeshGenLib.ReOrintateMesh(new Vector3[] { newVertices[0], newVertices[1], newVertices[2], newVertices[3] }, direction);
                newVertices[0] = rotatedVertices[0];
                newVertices[1] = rotatedVertices[1];
                newVertices[2] = rotatedVertices[2];
                newVertices[3] = rotatedVertices[3];

                MeshGenLib.CombineMeshes(newVertices, faceVertices, mesh);

                faceVertices = new int[] { mesh.vertices.Length - 4, mesh.vertices.Length - 3, mesh.vertices.Length - 2, mesh.vertices.Length - 1 };
                lastPoint = nextPoint;
                t += increment;
            }
            t = 0f;

        }

        return mesh;
    }
}
