using UnityEngine;

public class TerrainMapping : MonoBehaviour
{
    [SerializeField] private Camera camToDrawWith;
    [SerializeField] private RenderTexture tempTex;

    private void Start()
    {
        camToDrawWith.orthographicSize = 125f;
        camToDrawWith.aspect = 1.2f;
        camToDrawWith.Render();

        Shader.SetGlobalTexture("_TerrainDiffuse", tempTex);
    }
}
