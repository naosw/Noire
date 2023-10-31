using UnityEngine;
using FlatKit;

public class ValleyOfSoluraController : SceneController
{
    [SerializeField] private FlatKitFog fogRendererFeature;
    [SerializeField] private FogSettings fogSettings;
    
    protected override void Init()
    {
        fogRendererFeature.SetActive(true);
        fogRendererFeature.settings = fogSettings;
    }
}