using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BGMAudio : MonoBehaviour
{
    public FMODUnity.EventReference bgmAudioEvent;
    private FMOD.Studio.EventInstance bgmState;

    // Update is called once per frame
    private void Start()
    {
        bgmState = FMODUnity.RuntimeManager.CreateInstance(bgmAudioEvent);
    }

    public void PlayBgmAudio(){
        bgmState = FMODUnity.RuntimeManager.CreateInstance(bgmAudioEvent);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(bgmState, transform, false);
        bgmState.start();
    }
    public void StopBgmAudio(){
        bgmState.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        bgmState.release();
    }
}
