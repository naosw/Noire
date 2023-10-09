using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class BedRockPlainsController : MonoBehaviour
{
    [Header("Opening Lights Animation (Lantern Interact)")]
    [SerializeField] private Light mainLight;
    [SerializeField] private float finalIntensity;
    [SerializeField] private AnimationCurve intensityCurve;
    [SerializeField] private float animationTime = 3;
    [SerializeField] private ParticleSystemBase fireFlies;
    [SerializeField] private ParticleSystemBase dustParticles;  
    
    private void Awake()
    {
        mainLight.intensity = 0;
    }

    private void Start()
    {
        GameEventsManager.Instance.BedrockPlainsEvents.OnLampInteract += OpenLights;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.BedrockPlainsEvents.OnLampInteract -= OpenLights;
    }

    private void OpenLights()
    {
        StartCoroutine(PlayOpeningLightsAnimation());
    }

    private IEnumerator PlayOpeningLightsAnimation()
    {
        yield return new WaitForSeconds(.5f);
        float time = 0;
        while (time < 1)
        {
            mainLight.intensity = Mathf.Lerp(
                0, 
                finalIntensity, 
                intensityCurve.Evaluate(time)
            );
            yield return null;
            time += Time.deltaTime / animationTime;
        }
        
        dustParticles.Play();
        fireFlies.Play();
    }
}