using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class TerrainMapping : MonoBehaviour
{
    [SerializeField] private Camera camToDrawWith;
    [SerializeField] private RenderTexture tempTex;

    private void Start()
    {
        camToDrawWith.orthographicSize = 12.5f;
        camToDrawWith.aspect = 1.2f;
        camToDrawWith.Render();

        Shader.SetGlobalTexture("_TerrainDiffuse", tempTex);
    }
}
