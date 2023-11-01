using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class ScriptableRendererFeatureManager : MonoBehaviour
{
    public static ScriptableRendererFeatureManager Instance { get; private set; }
    
    [Header("Scriptable Render Features")] 
    [SerializeField] private UniversalRendererData rendererData;
    [SerializeField] private int totalFogs;
    private List<ScriptableRendererFeature> rendererFeatures;

    private void Awake()
    {
        if (Instance != null) 
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        rendererFeatures = rendererData.rendererFeatures;
    }

    public void ToggleAll(bool active)
    {
        foreach (var rf in rendererFeatures)
            rf.SetActive(active);
    }

    public void ToggleOne(int index, bool active)
    {
        rendererFeatures[index].SetActive(active);
    }

    public void ToggleAllFog(bool active)
    {
        for (int i = 0; i < totalFogs; i++)
        {
            rendererFeatures[i].SetActive(active);
        }
    }

    public void EnableOnlyOneFog(int index)
    {
        for (int i = 0; i < totalFogs; i++)
        {
            rendererFeatures[i].SetActive(false);
        }
        rendererFeatures[index].SetActive(true);
    }
}