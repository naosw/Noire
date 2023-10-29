using UnityEngine;

public partial class Player
{
    [Header("Audio")]
    [SerializeField] private AudioManager audioManager;
    
    public void PlaySteps(string path)
    {
        FMODUnity.RuntimeManager.PlayOneShot(path, transform.position);
    }
    public void PlaySwoosh(string path)
    {
        FMODUnity.RuntimeManager.PlayOneShot(path, transform.position);
    }
}