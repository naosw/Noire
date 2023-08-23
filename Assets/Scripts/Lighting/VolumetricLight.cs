using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumetricLight : MonoBehaviour
{
    [SerializeField] private Gradient nightColor;
    [SerializeField] private Gradient dawnColor;
    [SerializeField] private ParticleSystem dustParticles;

    private ParticleSystem volumetricFogLight;
    private bool isVolumetricLightPlaying = false;
    private ParticleSystem.MinMaxGradient nightGradient;
    private ParticleSystem.MinMaxGradient dawnGradient;

    private void Awake()
    {
        volumetricFogLight = GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        volumetricFogLight.Stop();
        nightGradient = new ParticleSystem.MinMaxGradient(nightColor);
        dawnGradient = new ParticleSystem.MinMaxGradient(dawnColor);

        if(dustParticles != null )
            dustParticles.Stop();
    }

    private void Update()
    {
        UpdateVolumetricFogLight();
    }

    private void UpdateVolumetricFogLight()
    {
        if (!isVolumetricLightPlaying && LightingManager.Instance.IsNight())
        {
            var colorModule = volumetricFogLight.colorOverLifetime;
            colorModule.color = nightGradient;
            volumetricFogLight.Play();
            isVolumetricLightPlaying = true;
            if (dustParticles != null)
                dustParticles.Play();
        }
        else if (!isVolumetricLightPlaying && LightingManager.Instance.IsDawn())
        {
            var colorModule = volumetricFogLight.colorOverLifetime;
            colorModule.color = dawnGradient;
            volumetricFogLight.Play();
            isVolumetricLightPlaying = true;
            if (dustParticles != null)
                dustParticles.Stop();
        }
        else if(isVolumetricLightPlaying && LightingManager.Instance.IsDay())
        {
            volumetricFogLight.Stop();
            isVolumetricLightPlaying = false;
            if (dustParticles != null)
                dustParticles.Stop();
        }

    }
}
