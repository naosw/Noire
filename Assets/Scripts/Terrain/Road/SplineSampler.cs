using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.Splines;
using UnityEngine;
using UnityEngine.Splines;

[ExecuteInEditMode()]
public class SplineSampler : MonoBehaviour
{
    [SerializeField] private SplineContainer m_splineContainer;
    [SerializeField] public Terrain terrainSnapTo;
    [SerializeField] public float terrainSnappingOffset = 0.1f;

    public SplineContainer Container => m_splineContainer;
    public int NumSplines => m_splineContainer.Splines.Count;

    public void SampleSplineWidth(int splineIndex, float t, float width, out Vector3 p1, out Vector3 p2)
    {
        m_splineContainer.Evaluate(splineIndex, t, out float3 position, out float3 forward, out float3 upVector);

        float3 right = Vector3.Cross(forward, upVector).normalized;
        p1 = position + (right * width);
        p2 = position + (-right * width);
        p1.y = terrainSnapTo.SampleHeight(p1) + terrainSnappingOffset;
        p2.y = terrainSnapTo.SampleHeight(p2) + terrainSnappingOffset;
    }
}
