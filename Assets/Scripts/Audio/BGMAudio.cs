using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMAudio : MonoBehaviour
{
    // public static BGMAudio Instance;
    public FMODUnity.EventReference bgmAudioEvent;
    private FMOD.Studio.EventInstance bgmState;

    // private void Awake()
    // {
    //     if (Instance != null) 
    //     {
    //         Destroy(gameObject);
    //         return;
    //     }
    //     Instance = this;
    //     DontDestroyOnLoad(gameObject);
    // }

    // Update is called once per frame
    private void Start()
    {
        bgmState = FMODUnity.RuntimeManager.CreateInstance(bgmAudioEvent);
        SceneManager.activeSceneChanged += StopBgmAudio;
    }
    
    bool IsPlaying(FMOD.Studio.EventInstance instance) {
        instance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE state);
        return state != FMOD.Studio.PLAYBACK_STATE.STOPPED;
    }

    public void PlayBgmAudio(){
        if (!IsPlaying(bgmState))
        {
            bgmState = FMODUnity.RuntimeManager.CreateInstance(bgmAudioEvent);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(bgmState, transform, false);
            bgmState.start();
        }
    }
    
    public void StopBgmAudio(Scene oldScene, Scene newScene){
        Debug.Log("STOPEED");
        bgmState.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        bgmState.release();
    }
}
