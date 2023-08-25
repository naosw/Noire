using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineMeshBuilder : MonoBehaviour
{
    public static void GenerateMeshFromSplines(Mesh mesh, List<Vector3> verts1, List<Vector3> verts2, int startingVerticeIndex = 1)
    {
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();

        int length = verts1.Count;
        for (int i = startingVerticeIndex; i < length; i++)
        { 
            verts.AddRange(new List<Vector3> {
                verts1[i - 1],
                verts2[i - 1],
                verts1[i],
                verts2[i]
            });

            int offset = 4 * (i - startingVerticeIndex);
            tris.AddRange(new List<int>{
                offset,
                offset + 2,
                offset + 3,
                offset + 3,
                offset + 1,
                offset + 0,
            });
        }

        mesh.Clear();
        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateNormals();
    }

    public static void GenerateMeshFromSplines(Mesh mesh, Vector3[] verts1, Vector3[] verts2, int startingVerticeIndex = 1)
    {
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();

        int length = verts1.Length;
        for (int i = startingVerticeIndex; i < length; i++)
        {
            verts.AddRange(new List<Vector3> {
                verts1[i - 1],
                verts2[i - 1],
                verts1[i],
                verts2[i]
            });

            int offset = 4 * (i - startingVerticeIndex);
            tris.AddRange(new List<int>{
                offset,
                offset + 2,
                offset + 3,
                offset + 3,
                offset + 1,
                offset + 0,
            });
        }

        mesh.Clear();
        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateNormals();
    }
}
