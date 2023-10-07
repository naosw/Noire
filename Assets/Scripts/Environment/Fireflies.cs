using UnityEngine;

public class Fireflies : MonoBehaviour
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

    public void Play()
    {
        if (!isPlaying)
        {
            fireflies.Play();
            isPlaying = true;
        }
    }

    public void Stop()
    {
        if (isPlaying)
        {
            fireflies.Stop();
            isPlaying = false;
        }
    }
    
    // private void Update()
    // {
    //     bool isNight = LightingManager.Instance.IsNight();
    //
    //     if (!isPlaying && isNight)
    //     {
    //         fireflies.Play();
    //         isPlaying = true;
    //     }
    //     else if (isPlaying && !isNight)
    //     {
    //         fireflies.Stop();
    //         isPlaying = false;
    //     }
    // }
}
