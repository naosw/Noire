using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weather : MonoBehaviour
{
    [SerializeField] private string weatherName;
    [SerializeField] private bool FollowPlayer;
    [SerializeField] private float weatherDurationMax; // in seconds
	[SerializeField] private WeatherAudio weatherAudio;

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
        weatherAudio.PlayWthAudio();
    }

    public void StopWeather()
    {
        if (hasParticleSystem)
            weatherParticleSystem.Stop();
        weatherAudio.StopWthAudio();
    }

    // get randomized duration of weather
    public float GetDuration()
    {
        return Random.Range(weatherDurationMax / 2, weatherDurationMax);
    }
}
