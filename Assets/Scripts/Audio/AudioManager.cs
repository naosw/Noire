using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ChangeGlobalParaByName(string name, float value)
    {
        FMOD.Studio.PARAMETER_DESCRIPTION parameterDescription;
        var result =
            FMODUnity.RuntimeManager.StudioSystem.getParameterDescriptionByName(name, out parameterDescription);
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError("Setting weather params failed");
            return;
        }

        result = FMODUnity.RuntimeManager.StudioSystem.setParameterByID(parameterDescription.id, value);
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError("Setting weather params failed");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
