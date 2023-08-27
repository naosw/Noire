using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineMeshBuilder : MonoBehaviour
{
    public static void GenerateMeshFromSplines(Mesh mesh, List<Vector3> verts1, List<Vector3> verts2)
    {
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();

        int length = verts1.Count;
        for (int i = 1; i < length; i++)
        { 
            verts.AddRange(new List<Vector3> {
                verts1[i - 1],
                verts2[i - 1],
                verts1[i],
                verts2[i]
            });

            int offset = 4 * (i - 1);
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
