using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SobelPass : ScriptableRenderPass
{
    private SobelFeature.CustomPassSettings settings;

    private RenderTargetIdentifier colorBuffer, pixelBuffer;
    private int pixelBufferID = Shader.PropertyToID("_PixelBuffer");

    //private RenderTargetIdentifier pointBuffer;
    //private int pointBufferID = Shader.PropertyToID("_PointBuffer");

    private Material material;
    private float depthThreshold, normalThreshold;
    private Color depthColor, normalColor;

    public SobelPass(SobelFeature.CustomPassSettings settings)
    {
        this.settings = settings;
        this.renderPassEvent = settings.renderPassEvent;
        if (material == null) material = CoreUtils.CreateEngineMaterial("Hidden/Sobel");
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        ConfigureInput(ScriptableRenderPassInput.Normal);
        colorBuffer = renderingData.cameraData.renderer.cameraColorTarget;
        RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;

        //cmd.GetTemporaryRT(pointBufferID, descriptor.width, descriptor.height, 0, FilterMode.Point);
        //pointBuffer = new RenderTargetIdentifier(pointBufferID);

        depthColor = settings.DepthColor;
        normalColor = settings.NormalColor;
        depthThreshold = settings.DepthThreshold;
        normalThreshold = settings.NormalThreshold;

        material.SetFloat("_DepthThreshold", depthThreshold);
        material.SetFloat("_NormalThreshold", normalThreshold);
        material.SetVector("_DepthColor", depthColor);
        material.SetVector("_NormalColor", normalColor);

        cmd.GetTemporaryRT(pixelBufferID, descriptor, FilterMode.Point);
        pixelBuffer = new RenderTargetIdentifier(pixelBufferID);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get();
        using (new ProfilingScope(cmd, new ProfilingSampler("Sobel Pass")))
        {
            cmd.Blit(colorBuffer, pixelBuffer, material);
            cmd.Blit(pixelBuffer, colorBuffer);
        }

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        if (cmd == null) 
            throw new System.ArgumentNullException("cmd");
        cmd.ReleaseTemporaryRT(pixelBufferID);
    }

}