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
    private Light mainLight;

    void Start()
    {
        mainLight = GetComponent<Light>();
        startingIntensity = mainLight.intensity;
        startingAngle = mainLight.spotAngle;
    }

    void Update()
    {
        time += Time.deltaTime * (1 - Random.Range(-speedRandomness, speedRandomness)) * Mathf.PI;
        mainLight.intensity = startingIntensity + Mathf.Sin(time * flickerPerSecond) * flickerIntensity;
        mainLight.spotAngle = startingAngle + Mathf.Sin(time * flickerPerSecond) * radiusShiftsIntensity;
    }
}
