using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BedRockPlainsController : MonoBehaviour, IDataPersistence
{
    [Header("Opening Lights Animation (Lantern Interact)")]
    [SerializeField] private Light mainLight;
    [SerializeField] private float finalIntensity;
    [SerializeField] private AnimationCurve openLightsIntensityCurve;
    [SerializeField] private float animationTime = 3;
    [SerializeField] private ParticleSystemBase dustParticles;
    [SerializeField] private BGMAudio bgmAudio;
    [SerializeField] private InteractableObject[] unaffectedInteractableObjects;
    [SerializeField] private CanvasGroup SceneTitle;
    [SerializeField] private CanvasGroup UI;
    [SerializeField] private AnimationCurve titleIntensityCurve;
    [SerializeField] private AnimationCurve UIIntensityCurve;
    [SerializeField] private float TitleAnimationTime = 3;
    
    private bool lightsOpened;
    private List<InteractableObject> interactablesList;
    
    private void Awake()
    {
        mainLight.intensity = 0;
        SceneManager.sceneLoaded += FindAllInteractables;
    }

    private void Start()
    {
        StartCoroutine(DisplaySceneName());
        
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

    private void ToggleAllInteractables(bool active)
    {
        if (active)
            foreach (var interactable in interactablesList)
                interactable.Enable();
        else
            foreach (var interactable in interactablesList)
                interactable.Disable();
    }
    
    private void FindAllInteractables(Scene scene, LoadSceneMode mode)
    {
        interactablesList = FindObjectsOfType<MonoBehaviour>(true)
            .OfType<InteractableObject>()
            .Except(unaffectedInteractableObjects)
            .ToList();
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

    private IEnumerator DisplaySceneName()
    {
        GameEventsManager.Instance.GameStateEvents.UIToggle(true);
        
        SceneTitle.gameObject.SetActive(false);
        UI.alpha = 0;
        yield return new WaitForSeconds(1);
        
        SceneTitle.gameObject.SetActive(true);
        float time = 0;
        while (time < 1)
        {
            SceneTitle.alpha = Mathf.Lerp(1, 0, titleIntensityCurve.Evaluate(time));
            time += Time.deltaTime / TitleAnimationTime;
            yield return null;
        }
        SceneTitle.gameObject.SetActive(false);
        
        time = 0;
        while (time < 1)
        {
            UI.alpha = Mathf.Lerp(0, 1, UIIntensityCurve.Evaluate(time));
            time += Time.deltaTime;
            yield return null;
        }
        UI.alpha = 1;
        
        GameEventsManager.Instance.GameStateEvents.UIToggle(false);
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