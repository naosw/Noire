using UnityEngine;

public class TerrainMapping : MonoBehaviour
{
    [SerializeField] private Camera camToDrawWith;
    [SerializeField] private RenderTexture tempTex;
    [SerializeField] private float orthographicSize;
    [SerializeField] private float aspect;

    private void Start()
    {
        camToDrawWith.orthographicSize = orthographicSize;
        camToDrawWith.aspect = aspect;
        camToDrawWith.Render();

        Shader.SetGlobalTexture("_TerrainDiffuse", tempTex);
    }
}
