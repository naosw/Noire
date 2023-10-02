using System.Collections;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;
    
    public void PlaySteps(string path)
    {
        FMODUnity.RuntimeManager.PlayOneShot(path, GetComponent<Transform>().position);
    }
    public void PlaySwoosh(string path)
    {
        FMODUnity.RuntimeManager.PlayOneShot(path, GetComponent<Transform>().position);
    }
}