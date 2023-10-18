using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingManager : MonoBehaviour
{
    public static PostProcessingManager Instance { get; private set; }
    
    private Volume volume;
    private LensDistortion lensDistortion;
    private ChromaticAberration chromaticAberration;
    private Bloom bloom;
    
    public Volume GetVolume() => volume;

    private void Awake()
    {
        if (Instance != null) 
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        volume = GetComponent<Volume>();
        
        if(!volume.profile.TryGet(out lensDistortion))
            Debug.LogError("Did not find a lens distortion in PostEffects");
        if(!volume.profile.TryGet(out chromaticAberration))
            Debug.LogError("Did not find a chromatic aberration in PostEffects");
        if(!volume.profile.TryGet(out bloom))
            Debug.LogError("Did not find a bloom effect in PostEffects");
    }
    
    public void SetLensDistortionIntensity(float val) => lensDistortion.intensity.value = val;

    public void SetChromaticAberrationIntensity(float val) => chromaticAberration.intensity.value = val;

    public void SetBloomIntensity(float val) => bloom.intensity.value = val;

    public float GetLensDistortionIntensity() => lensDistortion.intensity.value;

    public float GetChromaticAberrationIntensity() => chromaticAberration.intensity.value;
    
    public float GetBloomIntensity() => bloom.intensity.value;
}
