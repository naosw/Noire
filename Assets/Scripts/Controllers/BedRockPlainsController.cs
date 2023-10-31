using System.Collections;
using FlatKit;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BedRockPlainsController : SceneController, IDataPersistence
{
    [Header("Opening Lights Animation (Lantern Interact)")]
    [SerializeField] private Light mainLight;
    [SerializeField] private float finalIntensity;
    [SerializeField] private AnimationCurve openLightsIntensityCurve;
    [SerializeField] private float animationTime = 3;
    
    [Header("Fog")]
    [SerializeField] private FlatKitFog fogRendererFeature;
    [SerializeField] private FogSettings fogSettings;
    
    [Header("Particle Effects")]
    [SerializeField] private ParticleSystemBase dustParticles;
    
    private bool lightsOpened;
    
    protected override void Init()
    {
        mainLight.intensity = 0;
        fogRendererFeature.SetActive(true);
        fogRendererFeature.settings = fogSettings;
    }

    protected override void LateInit()
    {
        if (lightsOpened)
        {
            Begin();
        }
        else
        {
            GameEventsManager.Instance.BedrockPlainsEvents.OnLampInteract += OpenLights;
            ToggleAllInteractables(false);
        }
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.BedrockPlainsEvents.OnLampInteract -= OpenLights;
        SceneManager.sceneLoaded -= FindAllInteractables;
    }
    

    private void Begin()
    {
        mainLight.intensity = finalIntensity;
        bgmAudio.PlayBgmAudio();
        dustParticles.Play();
        ToggleAllInteractables(true);
    }
    
    private void OpenLights()
    {
        lightsOpened = true;
        DataPersistenceManager.Instance.SaveGame();
        StartCoroutine(PlayOpeningLightsAnimation());
    }

    private IEnumerator PlayOpeningLightsAnimation()
    {
        yield return new WaitForSeconds(.2f);
        float time = 0;
        while (time < 1)
        {
            mainLight.intensity = Mathf.Lerp(
                0, 
                finalIntensity, 
                openLightsIntensityCurve.Evaluate(time)
            );
            time += Time.deltaTime / animationTime;
            yield return null;
        }

        Begin();
    }

    #region IDataPersistence
    
    // this is called before START. AWAKE -> SCENE LOAD -> START
    // we will initialize the scene in START
    public void LoadData(GameData gameData)
    {
        lightsOpened = gameData.LightsOpen;
    }
    
    public void SaveData(GameData gameData)
    {
        gameData.LightsOpen = lightsOpened;
    }
    #endregion
}