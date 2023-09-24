using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeatherAudio : MonoBehaviour
{
    public FMODUnity.EventReference wthAudioEvent;
    private FMOD.Studio.EventInstance wthState;

    // Update is called once per frame
    private void Start()
    {
        wthState = FMODUnity.RuntimeManager.CreateInstance(wthAudioEvent);
    }

    public void PlayWthAudio(){
        wthState = FMODUnity.RuntimeManager.CreateInstance(wthAudioEvent);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(wthState, transform, false);
        wthState.start();
    }
    public void StopWthAudio(){
        wthState.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        wthState.release();
    }
}
