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

    private Mesh mesh;
    private LinkedList<Tuple<Vector3, Vector3>> transformsBuffer;
    private float smearDurationCounter;
    private List<Vector3> spline1;
    private List<Vector3> spline2;

    private void Awake()
    {
        activate = false;
        mesh = GetComponent<MeshFilter>().mesh;
        transformsBuffer = new LinkedList<Tuple<Vector3, Vector3>>();
        smearDurationCounter = smearDuration;
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
        transformsBuffer = new LinkedList<Tuple<Vector3, Vector3>>();
    }

    private Tuple<Vector3, Vector3> GetTransformInfo(Weapon obj)
    {
        return new Tuple<Vector3, Vector3>(obj.p1_bot.position, obj.p2_top.position);
    }

    private void UpdateTransformBuffer()
    {
        transformsBuffer.AddFirst(GetTransformInfo(weapon));
        if (smearDurationCounter < fadeThreshold)
            transformsBuffer.RemoveLast();
    }

    private void UpdateSmear()
    {
        spline1 = new List<Vector3>();
        spline2 = new List<Vector3>();

        foreach ((Vector3 p1, Vector3 p2) in transformsBuffer)
        {
            spline1.Add(p1);
            spline2.Add(p2);
        }

        SplineMeshBuilder.GenerateMeshFromSplines(mesh, spline1, spline2);
    }

    public void SetActivate(bool value)
    {
        activate = value;
    }
}