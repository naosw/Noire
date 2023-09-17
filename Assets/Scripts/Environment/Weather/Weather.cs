using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weather : MonoBehaviour
{
    [SerializeField] private string weatherName;
    [SerializeField] private bool FollowPlayer;
    [SerializeField] private float weatherDurationMax; // in seconds

    private ParticleSystem weatherParticleSystem;
    private bool hasParticleSystem;
    private Vector3 weatherHeight = new Vector3(0,30,0);

    private void Awake()
    {
        hasParticleSystem = TryGetComponent(out weatherParticleSystem);
    }

    private void Update()
    {
        if(FollowPlayer)
            transform.position = Player.Instance.transform.position + weatherHeight;
    }

    public string GetWeatherName() => weatherName;

    public void PlayWeather()
    {
        if (hasParticleSystem && !weatherParticleSystem.isPlaying)
            weatherParticleSystem.Play();

        // ****************** PLAY WEATHER SOUNDS HERE ****************************

        // fmod.play(weatherName) or sth

        // ****************** PLAY WEATHER SOUNDS HERE ****************************
    }

    public void StopWeather()
    {
        if (hasParticleSystem)
            weatherParticleSystem.Stop();

        // ****************** END WEATHER SOUNDS HERE ****************************

        // fmod.stop(weatherName) or sth

        // ****************** END WEATHER SOUNDS HERE ****************************
    }

    // get randomized duration of weather
    public float GetDuration()
    {
        return Random.Range(weatherDurationMax / 2, weatherDurationMax);
    }
}
