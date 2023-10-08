using System;
using System.Collections;
using UnityEngine;

public class BedRockPlainsController : MonoBehaviour
{
    [Header("Opening Lights Animation (Lantern Interact)")]
    [SerializeField] private Light mainLight;
    [SerializeField] private float finalIntensity;
    [SerializeField] private AnimationCurve intensityCurve;
    [SerializeField] private float animationTime = 3;
    [SerializeField] private Fireflies fireflies;
    private bool lightsOpened = false;
    
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
        if (lightsOpened)
            return;
        
        lightsOpened = true;
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
        
        fireflies.Play();
    }
}