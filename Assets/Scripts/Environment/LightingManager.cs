using UnityEngine;

[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    [Header("Sun and Moon")]
    [SerializeField] private Light sun;
    [SerializeField] private Light moon;
    [SerializeField] private float dayLength = 10; 
    [SerializeField] private float yearLength = 40; // days
    [SerializeField, Range(0, 1)] private float timeOfDay;
    [SerializeField, Range(0, 1)] private float timeOfYear;
    [SerializeField] private AnimationCurve sunIntensityCurve;
    [SerializeField] private AnimationCurve moonIntensityCurveDay;
    [SerializeField] private AnimationCurve moonIntensityCurveYear;
    [SerializeField] private Gradient sunColor;
    [SerializeField] private Gradient moonColor;

    public static LightingManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Application.isPlaying)
        {
            timeOfDay += Time.deltaTime / dayLength;
            timeOfYear += Time.deltaTime / dayLength / yearLength;
            timeOfDay %= 1;
            timeOfYear %= 1;
        }
        UpdateLightingRotation();
        UpdateLightingIntensity();
        UpdateLightingColor();
    }

    private void UpdateLightingRotation()
    {
        float rotationX = timeOfDay * 360f;
        float rotationY = timeOfYear * 350f + timeOfDay * 10f;
        float rotationOffset = 90f;

        sun.transform.localRotation = Quaternion.Euler(new Vector3(rotationX - rotationOffset, rotationY, 0f));
        moon.transform.localRotation = Quaternion.Euler(new Vector3(rotationX + rotationOffset, rotationY, 0f));
    }

    private void UpdateLightingIntensity()
    {
        sun.intensity = sunIntensityCurve.Evaluate(timeOfDay);
        moon.intensity = moonIntensityCurveDay.Evaluate(timeOfDay) * moonIntensityCurveYear.Evaluate(timeOfYear);
    }

    private void UpdateLightingColor()
    {
        sun.color = sunColor.Evaluate(timeOfDay);
        moon.color = moonColor.Evaluate(timeOfDay);
    }

    public float GetTimeOfDay()
    {
        return timeOfDay;
    }

    public bool IsNight()
    {
        return timeOfDay > .9f || timeOfDay < .1f;
    }

    public bool IsDawn()
    {
        return timeOfDay >= .2f && timeOfDay <= .35f;
    }

    public bool IsDay()
    {
        return !IsNight() && !IsDawn();
    }
}