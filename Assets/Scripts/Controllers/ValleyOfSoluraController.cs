using UnityEngine;
using FlatKit;
using UnityEngine.Rendering.Universal;

public class ValleyOfSoluraController : SceneController
{
    [SerializeField] private int fogIndex;
    
    protected override void Init()
    {
        ScriptableRendererFeatureManager.Instance.EnableOnlyOneFog(fogIndex);
    }
}