using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Volume Controller")]
    [SerializeField] [Range(0,1.25f)] private float sfxVolume = 1f;
    [SerializeField] [Range(0,1.25f)] private float ostVolume = 1f;
    private FMOD.Studio.VCA sfxVCA;
    private FMOD.Studio.VCA ostVCA;

    public float GetVcaVolume(string vca)
    {
        float result;
        if (vca == "Sfx")
        {
            sfxVCA = FMODUnity.RuntimeManager.GetVCA("vca:/SfxVCA");
            sfxVCA.getVolume(out result);
            return result;
        }
        else
        {
            ostVCA = FMODUnity.RuntimeManager.GetVCA("vca:/OstVCA");
            ostVCA.getVolume(out result);
            return result;
        }
    }
    public void SetSfxVolume(float desVolume)
    {
        sfxVCA = FMODUnity.RuntimeManager.GetVCA("vca:/SfxVCA");
        if (!(sfxVCA.isValid()))
        {
            Debug.LogError("sfxVca is not Valid");
        }
        else
        {
            if (0 <= desVolume & desVolume <= 1.25)
            {
                sfxVCA.setVolume(desVolume);
                sfxVolume = desVolume;
                float vol;
                sfxVCA.getVolume(out vol);
                //Debug.Log("sfx" + vol);
            }
            else
            {
                Debug.LogError("sfxVca desvolume is not valid");
            }
        }
    }
    public void SetOstVolume(float desVolume)
    {
        ostVCA = FMODUnity.RuntimeManager.GetVCA("vca:/OstVCA");
        if (!(ostVCA.isValid()))
        {
            Debug.LogError("OstVca is not Valid");
        }
        else
        {
            if (0 <= desVolume & desVolume <= 1.25)
            {
                ostVCA.setVolume(desVolume);
                ostVolume = desVolume;
                float vol;
                ostVCA.getVolume(out vol);
                //Debug.Log("ost:" +vol);
            }
            else
            {
                Debug.LogError("OstVca desvolume is not valid");
            }
        }
    }
    public void ChangeGlobalParaByName(string name, float value)
    {
        FMOD.Studio.PARAMETER_DESCRIPTION parameterDescription;
        var result =
            FMODUnity.RuntimeManager.StudioSystem.getParameterDescriptionByName(name, out parameterDescription);
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError("Setting Global params failed");
            return;
        }

        result = FMODUnity.RuntimeManager.StudioSystem.setParameterByID(parameterDescription.id, value);
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError("Setting Global params failed");
        }
    }
    public void PlayInteractableAudio(string interactable)
    {
        
    }
    
}
