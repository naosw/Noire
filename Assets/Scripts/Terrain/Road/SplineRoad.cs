using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineSampler))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode()]
public class SplineRoad : MonoBehaviour
{
    private SplineSampler m_splineSampler;
    private MeshFilter m_meshFilter;

    private List<Vector3> m_vertsP1;
    private List<Vector3> m_vertsP2;

    [SerializeField, Min(5)] private int resolution = 10;
    [SerializeField, Range(0.01f,1f)] private float m_curveStep = 0.1f;
    [SerializeField] private float m_width;

    [SerializeField] private List<Intersection> intersections;

    public List<Intersection> Intersections => intersections;

    //Draw mesh
    private void Awake()
    {
        m_splineSampler = gameObject.GetComponent<SplineSampler>();
        m_meshFilter = gameObject.GetComponent<MeshFilter>();
    }

    private void OnEnable()
    {
        Spline.Changed += OnSplineChanged;
        GetSplineVerts();
    }

    private void OnSplineChanged(Spline arg1, int arg2, SplineModification arg3)
    {
        Rebuild();
    }

    public void Rebuild()
    {
        GetSplineVerts();
        BuildMesh();
    }

    private void Update()
    {
        Rebuild();
    }

    private void BuildMesh()
    {
        Mesh m = new Mesh();

        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        int offset = 0;

        float uvOffset = 0f;

        for (int currentSplineIndex = 0; currentSplineIndex < m_splineSampler.NumSplines; currentSplineIndex++)
        {
            int splineOffset = resolution * currentSplineIndex;
            splineOffset += currentSplineIndex;
            //Iterate verts and build a face
            for (int currentSplinePoint = 1; currentSplinePoint < resolution+1; currentSplinePoint++)
            {
                int vertoffset = splineOffset + currentSplinePoint;
                Vector3 p1 = m_vertsP1[vertoffset - 1];
                Vector3 p2 = m_vertsP2[vertoffset - 1];
                Vector3 p3 = m_vertsP1[vertoffset];
                Vector3 p4 = m_vertsP2[vertoffset];

                offset = 4 * resolution * currentSplineIndex;
                offset += 4 * (currentSplinePoint - 1);

                int t1 = offset + 0;
                int t2 = offset + 2;
                int t3 = offset + 3;

                int t4 = offset + 3;
                int t5 = offset + 1;
                int t6 = offset + 0;

                verts.AddRange(new List<Vector3> { p1, p2, p3, p4 });
                tris.AddRange(new List<int> { t1, t2, t3, t4, t5, t6 });

                float distance = Vector3.Distance(p1, p3) / 4f;
                float uvDistance = uvOffset + distance;
                uvs.AddRange(new List<Vector2> { new Vector2(uvOffset, 0), new Vector2(uvOffset, 1), new Vector2(uvDistance, 0), new Vector2(uvDistance, 1) });

                uvOffset += distance;
            }
        }

        List<int> trisB = new List<int>();

        GetIntersectionVerts(verts, trisB, uvs);

        m.subMeshCount = 2;
        m.SetVertices(verts);
        m.SetTriangles(tris, 0);
        m.SetTriangles(trisB, 1);
        m.SetUVs(0, uvs);
        m.RecalculateNormals();
        m_meshFilter.mesh = m;
    }

    private void GetSplineVerts()
    {
        m_vertsP1 = new List<Vector3>();
        m_vertsP2 = new List<Vector3>();

        float step = 1f / (float)resolution;
        Vector3 p1;
        Vector3 p2;
        for (int j = 0; j < m_splineSampler.NumSplines; j++)
        {
            for (int i = 0; i < resolution; i++)
            {
                float t = step * i;
                m_splineSampler.SampleSplineWidth(j, t, m_width, out p1, out p2);
                m_vertsP1.Add(p1);
                m_vertsP2.Add(p2);
            }

            m_splineSampler.SampleSplineWidth(j, 1f, m_width, out p1, out p2);
            m_vertsP1.Add(p1);
            m_vertsP2.Add(p2);
        }
    }

    struct JunctionEdge
    {
        public Vector3 left;
        public Vector3 right;

        public Vector3 Center => (left + right)/2;

        public JunctionEdge (Vector3 p1, Vector3 p2)
        {
            this.left = p1;
            this.right = p2;
        }
    }

    private void GetIntersectionVerts(List<Vector3> verts, List<int> tris, List<Vector2> uvs)
    {
        int offset = verts.Count;

        //Get intersection verts
        for (int i = 0; i < intersections.Count; i++)
        {
            Intersection intersection = intersections[i];
            int count = 0;
            
            List<JunctionEdge> junctionEdges = new List<JunctionEdge>();

            Vector3 center = new Vector3();
            foreach (JunctionInfo junction in intersection.GetJunctions())
            {
                int splineIndex = junction.splineIndex;
                float t = junction.knotIndex == 0 ? 0f : 1f;
                m_splineSampler.SampleSplineWidth(splineIndex, t, m_width, out Vector3 p1, out Vector3 p2);
                //If knot index is 0 we're facing away from the junction
                //If we're more than zero we're facing the junction
                if (junction.knotIndex == 0)
                {
                    junctionEdges.Add(new JunctionEdge(p1, p2));
                }
                else
                {
                    junctionEdges.Add(new JunctionEdge(p2, p1));

                }
                center += p1;
                center += p2;
                count++;
            }
            center = center / (junctionEdges.Count * 2);

            //Sort the junctions based on their direction from the center
            junctionEdges.Sort((x, y) => SortPoints(center, x.Center, y.Center));


            List<Vector3> curvePoints = new List<Vector3>();
            //Add additional points
            Vector3 mid;
            Vector3 c;
            Vector3 b;
            Vector3 a;
            BezierCurve curve;
            for (int j = 1; j <= junctionEdges.Count; j++)
            {
                a = junctionEdges[j - 1].left;
                curvePoints.Add(a);
                b = (j < junctionEdges.Count) ? junctionEdges[j].right : junctionEdges[0].right;
                mid = Vector3.Lerp(a, b, 0.5f);
                Vector3 dir = center - mid;
                mid = mid - dir;
                c = Vector3.Lerp(mid, center, intersection.curves[j-1]);

                curve = new BezierCurve(a, c, b);
                for (float t = 0f; t < 1f; t += m_curveStep)
                {
                    Vector3 pos = CurveUtility.EvaluatePosition(curve, t);
                    curvePoints.Add(pos);
                }

                curvePoints.Add(b);
            }

            curvePoints.Reverse();

            int pointsOffset = verts.Count;

            for (int j = 1; j <= curvePoints.Count; j++)
            {

                Vector3 pointA = curvePoints[j - 1];
                Vector3 pointB;
                if (j == curvePoints.Count)
                {
                    pointB = curvePoints[0];
                }
                else
                {
                    pointB = curvePoints[j];
                }

                center.y = m_splineSampler.terrainSnapTo.SampleHeight(center) + m_splineSampler.terrainSnappingOffset;
                pointA.y = m_splineSampler.terrainSnapTo.SampleHeight(pointA) + m_splineSampler.terrainSnappingOffset;
                pointB.y = m_splineSampler.terrainSnapTo.SampleHeight(pointB) + m_splineSampler.terrainSnappingOffset;

                verts.Add(center);
                verts.Add(pointA);
                verts.Add(pointB);

                tris.Add(pointsOffset + ((j - 1) * 3) + 0);
                tris.Add(pointsOffset + ((j - 1) * 3) + 1);
                tris.Add(pointsOffset + ((j - 1) * 3) + 2);

                uvs.Add(new Vector2(center.z, center.x));
                uvs.Add(new Vector2(pointA.z, pointA.x));
                uvs.Add(new Vector2(pointB.z, pointB.x));

            }
        }
    }

    private int SortPoints(Vector3 center, Vector3 x, Vector3 y)
    {
        Vector3 xDir = x - center;
        Vector3 yDir = y - center;

        float angleA = Vector3.SignedAngle(center.normalized, xDir.normalized, Vector3.up);
        float angleB = Vector3.SignedAngle(center.normalized, yDir.normalized, Vector3.up);

        if (angleA > angleB)
        {
            return -1;
        }
        if (angleA < angleB)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    private void OnDisable()
    {
        Spline.Changed -= OnSplineChanged;
    }

    public void AddJunction(Intersection junction)
    {
        if(intersections == null)
        {
            intersections = new List<Intersection>();
        }
        
        intersections.Add(junction);
    }

    public void RemoveJunction(Intersection junction)
    {
        intersections.Remove(junction);
    }

    public void RemoveAllJunctions()
    {
        intersections.Clear();
    }

    public void SaveMesh(string name)
    {
        Mesh mesh = m_meshFilter.mesh;
        string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/", name, "asset");
        if (string.IsNullOrEmpty(path)) return;

        path = FileUtil.GetProjectRelativePath(path);

        Mesh meshToSave = UnityEngine.Object.Instantiate(mesh) as Mesh;
        MeshUtility.Optimize(meshToSave);

        AssetDatabase.CreateAsset(meshToSave, path);
        AssetDatabase.SaveAssets();
    }

}
