using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class WeatherManager : MonoBehaviour
{
    [SerializeField] private Weather[] availableWeathers;
    [SerializeField] private Weather defaultWeather;
    [SerializeField] private float[] weatherWeights;

    private Weather currentWeather;
    private float tickTimer = 0;
    private List<int> probablisticWeathers = new List<int>();
    private int probabilityLen;

    private void Awake()
    {
        HandleWeatherProbabilities();
    }

    private void HandleWeatherProbabilities()
    {
        // normalize weather weights
        float minWeight = weatherWeights.Min();
        var weightsNormalized = weatherWeights.Select(x => x / minWeight).ToArray();

        // generate weather probability array
        for (int i = 0; i < weightsNormalized.Length; i++)
        {
            int weight = (int)weightsNormalized[i];
            var weights = Enumerable.Repeat(i, weight);

            probablisticWeathers.AddRange(weights);
        }
        probabilityLen = probablisticWeathers.Count;
    }

    private void Start()
    {
        foreach (Weather w in  availableWeathers)
            w.StopWeather();
        currentWeather = defaultWeather;
        ChangeWeather(defaultWeather);

        InvokeRepeating("Tick", 1, 1); // invokes every second
    }

    private Weather NextWeather()
    {
        // randomly select an element from weighted probability array
        return availableWeathers[probablisticWeathers[UnityEngine.Random.Range(0, probabilityLen)]];
    }

    private void Tick()
    {
        tickTimer--;
        if (tickTimer < 0)
        {
            Weather nextWeather = NextWeather();
            tickTimer = nextWeather.GetDuration();
            if (nextWeather != currentWeather)
                ChangeWeather(nextWeather);
        }
    }

    private void ChangeWeather(Weather weather)
    {
        currentWeather.StopWeather();
        currentWeather = weather;
        weather.PlayWeather();
    }
}
