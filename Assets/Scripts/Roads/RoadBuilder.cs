using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RoadBuilder : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private int splineIndex = 0;
    [SerializeField] private float width;
    [SerializeField] private float resolution;

    float3 position;
    float3 tangent;
    float3 normal;

    private List<Vector3> verts1;
    private List<Vector3> verts2;
    private Mesh mesh;

    private void OnEnable()
    {
        mesh = GetComponent<MeshFilter>().mesh;
    }

    private void Update()
    {
        GetVerts();
        SplineMeshBuilder.GenerateMeshFromSplines(mesh, verts1, verts2);
    }

    private void SampleSplineWidth(float t, out Vector3 p1, out Vector3 p2)
    {
        splineContainer.Evaluate(splineIndex, t, out position, out tangent, out normal);

        float3 right = Vector3.Cross(tangent, normal).normalized;
        p1 = position + right * width;
        p2 = position - right * width;
    }

    private void GetVerts()
    {
        verts1 = new List<Vector3>();
        verts2 = new List<Vector3>();
        float step = 1f / resolution;
        for (int i = 0;i < resolution;i++)
        {
            float t = step * i;
            SampleSplineWidth(t, out Vector3 p1, out Vector3 p2);
            verts1.Add(p1);
            verts2.Add(p2);
        }
    }

    private void OnDrawGizmosSelected()
    {
        foreach (var v in verts1)
        {
            Gizmos.DrawSphere(v, .1f);
        }
        foreach (var v in verts2)
        {
            Gizmos.DrawSphere(v, .1f);
        }
    }
}
