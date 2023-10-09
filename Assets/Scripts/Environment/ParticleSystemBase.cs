using UnityEngine;

public class ParticleSystemBase : MonoBehaviour
{
    private ParticleSystem particles;
    private bool isPlaying;

    private void Awake()
    {
        isPlaying = false;
    }

    private void Start()
    {
        particles = GetComponent<ParticleSystem>();
        particles.Stop();
    }

    public void Play()
    {
        if (!isPlaying)
        {
            particles.Play();
            isPlaying = true;
        }
    }

    public void Stop()
    {
        if (isPlaying)
        {
            particles.Stop();
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
