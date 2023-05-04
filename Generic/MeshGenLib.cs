using Unity.Entities;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using System;


public class MeshGenLib
{

    // Keeping for now in case player changes to use RayCasts- needed to form mesh colllider
    public NativeArray<float3> VectorArrayToNativeFloatArray(Vector3[] points)
    {
        NativeArray<float3> verticesAsFloat3Array = new NativeArray<float3>(points.Length, Allocator.Temp);
        for (int i = 0; i < points.Length; i++)
        {
            verticesAsFloat3Array[i] = new float3(points[i].x, points[i].y, points[i].z);
        }
        return verticesAsFloat3Array;
    }

    public NativeArray<int3> VectorArrayToNativeIntArray(int[] points)
    {
        NativeArray<int3> verticesAsInt3Array = new NativeArray<int3>(points.Length, Allocator.Temp);
        for (int i = 0; i < points.Length; i += 3)
        {
            verticesAsInt3Array[i] = new int3(points[i], points[i + 1], points[i + 2]);
        }
        return verticesAsInt3Array;
    }


    public static Mesh FlipNormals(Mesh mesh)
    {
        int[] triangles = mesh.triangles;

        // Reverse the winding order of each triangle
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int temp = triangles[i];
            triangles[i] = triangles[i + 2];
            triangles[i + 2] = temp;
        }

        // Assign the modified triangles array back to the mesh
        mesh.triangles = triangles;
        return mesh;
    }

    public static Vector3[] ReOrintateMesh(Vector3[] planeVertices, Vector3 direction)
    {

        Vector3 planeNormal = Vector3.Cross(planeVertices[1] - planeVertices[0], planeVertices[2] - planeVertices[0]).normalized;
        Vector3 targetDirection = direction.normalized;

        // Calculate the rotation needed to align the plane with the target direction vector
        Quaternion rotation = Quaternion.FromToRotation(planeNormal, targetDirection);

        // Rotate the plane vertices around the center of the plane
        Vector3 center = (planeVertices[0] + planeVertices[1] + planeVertices[2] + planeVertices[3]) / 4f;
        for (int i = 0; i < planeVertices.Length; i++)
        {
            planeVertices[i] = rotation * (planeVertices[i] - center) + center;
        }

        return planeVertices;
    }

    public static Mesh CombineMeshes(Vector3[] newVertices, int[] faceVertices, Mesh mesh)
    {
        Vector3[] combinedVertices = new Vector3[mesh.vertices.Length + newVertices.Length];
        mesh.vertices.CopyTo(combinedVertices, 0);
        newVertices.CopyTo(combinedVertices, mesh.vertices.Length);

        int[] combinedTriangles = new int[mesh.triangles.Length + 8];
        mesh.triangles.CopyTo(combinedTriangles, 0);

        int baseIndex = mesh.vertices.Length;
        int[] newTriangles = GenerateNewTriangles(new int[] { faceVertices[0], faceVertices[1], faceVertices[2], faceVertices[3] }, new int[] { baseIndex, baseIndex + 1, baseIndex + 2, baseIndex + 3 });

        Array.Resize(ref combinedTriangles, mesh.triangles.Length + newTriangles.Length);

        newTriangles.CopyTo(combinedTriangles, mesh.triangles.Length);
        mesh.vertices = combinedVertices;
        mesh.triangles = combinedTriangles;

        return mesh;
    }

    public static int[] GenerateNewTriangles(int[] vertices1, int[] vertices2)
    {
        // Format of vertices are [x, y, z], [x, y, z1], [x, y1, z], [x, y1, z1]
        int[] newTriangles = new int[]
        {
            vertices1[0], vertices2[0], vertices1[1], // New triangle 1
            vertices1[1], vertices2[0], vertices2[1], // New triangle 2
            vertices1[1], vertices2[1], vertices1[3], // New triangle 3
            vertices1[3], vertices2[1], vertices2[3], // New triangle 4
            vertices1[0], vertices2[2], vertices2[0],  // New triangle 5
            vertices1[2], vertices2[3], vertices2[2], // New triangle 6
            vertices1[2], vertices2[2], vertices1[0], // New triangle 7
            vertices1[3], vertices2[3], vertices1[2], // New triangle 8
        };
        return newTriangles;
    }

    public static float GetBezierCurveLength(Vector3 startPoint, Vector3 controlPoint1, Vector3 controlPoint2, Vector3 endPoint, int numSamples = 100)
    {
        float length = 0.0f;
        Vector3 prevPoint = startPoint;
        for (int i = 1; i <= numSamples; i++)
        {
            float t = (float)i / numSamples;
            Vector3 currPoint = Formulas.CalculateBezierPoint(startPoint, controlPoint1, controlPoint2, endPoint, t);
            length += Vector3.Distance(prevPoint, currPoint);
            prevPoint = currPoint;
        }
        return length;
    }

    public static Mesh DefineCubeMesh(float3 position, float scaleX, float scaleY, float scaleZ)
    {
        var mesh = new Mesh();

        var adjustedScaleX = 0.2f * scaleX;
        var adjustedScaleY = 0.2f * scaleY;
        var adjustedScaleZ = 0.2f * scaleZ;

        // Set the vertex positions
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(position.x - adjustedScaleX, position.y - adjustedScaleY, position.z - adjustedScaleZ), // 0
            new Vector3(position.x - adjustedScaleX, position.y - adjustedScaleY, position.z + adjustedScaleZ), // 1
            new Vector3(position.x - adjustedScaleX, position.y + adjustedScaleY, position.z - adjustedScaleZ), // 2
            new Vector3(position.x - adjustedScaleX, position.y + adjustedScaleY, position.z + adjustedScaleZ), // 3
            new Vector3(position.x + adjustedScaleX, position.y - adjustedScaleY, position.z - adjustedScaleZ), // 4
            new Vector3(position.x + adjustedScaleX, position.y - adjustedScaleY, position.z + adjustedScaleZ), // 5
            new Vector3(position.x + adjustedScaleX, position.y + adjustedScaleY, position.z - adjustedScaleZ), // 6
            new Vector3(position.x + adjustedScaleX, position.y + adjustedScaleY, position.z + adjustedScaleZ), // 7
        };

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].y -= 0.3f;
        }
        mesh.vertices = vertices;

        // Set the triangle indices
        int[] triangles = new int[]
        {
            0, 2, 1, // Front face
            1, 2, 3,
            4, 5, 6, // Back face
            5, 7, 6,
            0, 1, 4, // Left face
            1, 5, 4,
            2, 6, 3, // Right face
            3, 6, 7,
            0, 4, 2, // Bottom face
            2, 4, 6,
            1, 3, 5, // Top face
            3, 7, 5,
        };
        mesh.triangles = triangles;

        return mesh;
    }
}



