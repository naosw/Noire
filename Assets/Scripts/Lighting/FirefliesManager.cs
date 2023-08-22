using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirefliesManager : MonoBehaviour
{
    private ParticleSystem fireflies;
    private bool isPlaying;

    private void Awake()
    {
        isPlaying = false;
    }

    private void Start()
    {
        fireflies = GetComponent<ParticleSystem>();
        fireflies.Stop();
    }

    private void Update()
    {
        float timeOfDay = LightingManager.Instance.GetTimeOfDay();
        bool isNight = timeOfDay > .9f || timeOfDay < .1f;

        if (!isPlaying && isNight)
        {
            fireflies.Play();
            isPlaying = true;
        }
        else if (isPlaying && !isNight)
        {
            fireflies.Stop();
            isPlaying = false;
        }
    }
}
