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
        bool isNight = LightingManager.Instance.IsNight();

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
