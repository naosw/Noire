using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [SerializeField] private float flickerIntensity = 8f;
    [SerializeField] private float flickerPerSecond = 3.0f;
    [SerializeField] private float speedRandomness = 10f;
    [SerializeField] private float radiusShiftsIntensity = 1f;

    private float time;
    private float startingIntensity;
    private float startingAngle;
    private Light light;

    void Start()
    {
        light = GetComponent<Light>();
        startingIntensity = light.intensity;
        startingAngle = light.spotAngle;
    }

    void Update()
    {
        time += Time.deltaTime * (1 - Random.Range(-speedRandomness, speedRandomness)) * Mathf.PI;
        light.intensity = startingIntensity + Mathf.Sin(time * flickerPerSecond) * flickerIntensity;
        light.spotAngle = startingAngle + Mathf.Sin(time * flickerPerSecond) * radiusShiftsIntensity;
    }
}
