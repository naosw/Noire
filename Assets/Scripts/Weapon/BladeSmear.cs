using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class BladeSmear : MonoBehaviour
{
    [SerializeField] private Weapon weapon;
    [SerializeField] private bool activate = true;
    [SerializeField] private float smearDuration = .25f;
    [SerializeField] private float fadeThreshold = .12f;

    private const float MINIMUM_DISTANCE_TO_RENDER = 0.2f;

    private Mesh mesh;
    private LinkedList<Tuple<Vector3, Vector3, Vector3>> transformsBuffer;
    private float smearDurationCounter;
    private Vector3[] spline1;
    private Vector3[] spline2;
    private float[] bezierControlPoints;

    private void Awake()
    {
        activate = false;
        mesh = GetComponent<MeshFilter>().mesh;
        transformsBuffer = new LinkedList<Tuple<Vector3, Vector3, Vector3>>();
        smearDurationCounter = smearDuration;
        bezierControlPoints = new float[6];
    }

    private void Start()
    {
        bezierControlPoints[0] = weapon.pos1y;
        bezierControlPoints[1] = weapon.pos2x;
        bezierControlPoints[2] = weapon.pos2y;
        bezierControlPoints[3] = weapon.pos3y;
        bezierControlPoints[4] = weapon.pos4x;
        bezierControlPoints[5] = weapon.pos4y;
    }

    private void Update()
    {
        if (activate)
        {
            smearDurationCounter -= Time.deltaTime;
            if (smearDurationCounter < 0)
            {
                activate = false;
                ResetSmear();
                return;
            }
            UpdateTransformBuffer();
            UpdateSmear();
        }
    }

    public void ResetSmear() 
    {
        mesh.Clear();
        smearDurationCounter = smearDuration;
        transformsBuffer = new LinkedList<Tuple<Vector3, Vector3, Vector3>>();
    }

    private Tuple<Vector3, Vector3, Vector3> CalculatePositionAndDirection(Weapon obj)
    {
        return new Tuple<Vector3, Vector3, Vector3>(obj.transform.position, obj.transform.TransformDirection(Vector3.right), obj.transform.TransformDirection(Vector3.up));
    }

    private void UpdateTransformBuffer()
    {
        transformsBuffer.AddFirst(CalculatePositionAndDirection(weapon));
        if (smearDurationCounter < fadeThreshold)
            transformsBuffer.RemoveLast();
    }

    private void UpdateSmear()
    {
        int bufferLength = transformsBuffer.Count;

        Vector3 first = transformsBuffer.First.Value.Item1;
        Vector3 last = transformsBuffer.Last.Value.Item1;

        if (bufferLength < 4 || Vector3.Distance(first, last) < MINIMUM_DISTANCE_TO_RENDER)
            return;

        GenerateCurve();
        GenerateMesh();
    }

    private void GenerateCurve()
    {
        int bufferLength = transformsBuffer.Count;
        spline1 = new Vector3[bufferLength - 1];
        spline2 = new Vector3[bufferLength - 1];

        int j = 0;
        for (var node = transformsBuffer.First; node != null && node.Next != null; node = node.Next)
        {
            (Vector3 p1Pos, Vector3 p1Right, Vector3 p1Up) = node.Value;
            (Vector3 p2Pos, Vector3 p2Right, Vector3 p2Up) = node.Next.Value;

            Vector3 p11 = p1Pos + p1Up * bezierControlPoints[0];
            Vector3 p12 = p1Pos + p1Right * bezierControlPoints[1] + p1Up * bezierControlPoints[2];
            Vector3 p13 = p1Pos + p1Up * bezierControlPoints[3];
            Vector3 p14 = p1Pos + p1Right * bezierControlPoints[4] + p1Up * bezierControlPoints[5];

            Vector3 p21 = p2Pos + p2Up * bezierControlPoints[0];
            Vector3 p22 = p2Pos + p2Right * bezierControlPoints[1] + p2Up * bezierControlPoints[2];
            Vector3 p23 = p2Pos + p2Up * bezierControlPoints[3];
            Vector3 p24 = p2Pos + p2Right * bezierControlPoints[4] + p2Up * bezierControlPoints[5];

            float t = .5f;
            spline1[j] = CalculateCubicBezierPoint(t, p11, p12, p21, p22);
            spline2[j] = CalculateCubicBezierPoint(t, p13, p14, p23, p24);
            j++;
        }
    }


    private void GenerateMesh()
    {
        int splineLength = spline1.Length;

        // vertices and uvs
        Vector3[] vertices = new Vector3[(splineLength - 1) * 4];
        Vector2[] uvs = new Vector2[(splineLength - 1) * 4];
        for (int i = 0, v = 0; i < splineLength - 1; i++, v += 4)
        {
            vertices[v] = transform.InverseTransformPoint(spline1[i]);
            vertices[v + 1] = transform.InverseTransformPoint(spline2[i]);
            vertices[v + 2] = transform.InverseTransformPoint(spline1[i + 1]);
            vertices[v + 3] = transform.InverseTransformPoint(spline2[i + 1]);

            uvs[v] = new Vector2(0, 0);
            uvs[v + 1] = new Vector2(0, 1);
            uvs[v + 2] = new Vector2(1, 1);
            uvs[v + 3] = new Vector2(1, 0);
        }

        // triangles
        int[] triangles = new int[(splineLength - 1) * 6];
        for (int i = 0, ti = 0, vi = 0; i < splineLength - 1; i++, ti += 6, vi += 4)
        {
            //first tri
            triangles[ti] = vi;
            triangles[ti + 1] = vi + 3;
            triangles[ti + 2] = vi + 1;

            //second matching tri
            triangles[ti + 3] = vi + 2;
            triangles[ti + 4] = vi + 3;
            triangles[ti + 5] = vi + 0;
        }

        // clean up, generate mesh
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        //Debug.Log("Generated!");
    }

    private Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;
        return p;
    }

    public void SetActivate(bool value)
    {
        activate = value;
    }
}
