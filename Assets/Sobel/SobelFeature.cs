using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SobelFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class CustomPassSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public float DepthThreshold = 0.1f;
        public float NormalThreshold = 0.8f;
        public Color DepthColor = Color.black;
        public Color NormalColor = Color.white;
    }

    [SerializeField] private CustomPassSettings settings;
    private SobelPass customPass;

    public override void Create()
    {
        customPass = new SobelPass(settings);
    }
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        
        renderer.EnqueuePass(customPass);
    }
}
