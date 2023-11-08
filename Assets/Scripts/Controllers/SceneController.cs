using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

/// <summary>
/// A scene controller supports virtual implementations of the following functions:
/// <code>Init</code> Called in Awake()
/// <code>LateInit</code> Called in Start(), This is called after DataPersistence!
/// </summary>

public class SceneController : MonoBehaviour
{
    [Header("Interactable Objects")]
    [SerializeField] protected InteractableObject[] unaffectedInteractableObjects;
    
    [Header("Audio")]
    [SerializeField] protected BGMAudio bgmAudio;
    
    [Header("Title text")]
    [SerializeField] private CanvasGroup SceneTitle;
    [SerializeField] private CanvasGroup UI;
    [SerializeField] private AnimationCurve titleIntensityCurve;
    [SerializeField] private AnimationCurve UIIntensityCurve;
    private float titleAnimationTime = 3;
    
    protected List<InteractableObject> interactablesList;

    private void Awake()
    {
        SceneManager.sceneLoaded += FindAllInteractables;
        Init();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= FindAllInteractables;
    }

    private void Start()
    {
        StartCoroutine(DisplaySceneName());
        LateInit();
    }

    protected virtual void Init() { }

    protected virtual void LateInit()
    {
        StartCoroutine(DisplaySceneName());
    }

    private IEnumerator DisplaySceneName()
    {
        GameEventsManager.Instance.GameStateEvents.MenuToggle(true);
        
        SceneTitle.gameObject.SetActive(false);
        UI.alpha = 0;
        yield return new WaitForSeconds(1);
        
        SceneTitle.gameObject.SetActive(true);
        float time = 0;
        while (time < 1)
        {
            SceneTitle.alpha = Mathf.Lerp(1, 0, titleIntensityCurve.Evaluate(time));
            time += Time.deltaTime / titleAnimationTime;
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
        
        GameEventsManager.Instance.GameStateEvents.MenuToggle(false);
    }
    
    protected void ToggleAllInteractables(bool active)
    {
        if (active)
            foreach (var interactable in interactablesList)
                interactable.Enable();
        else
            foreach (var interactable in interactablesList)
                interactable.Disable();
    }
    
    protected void FindAllInteractables(Scene scene, LoadSceneMode mode)
    {
        interactablesList = FindObjectsOfType<MonoBehaviour>(true)
            .OfType<InteractableObject>()
            .Except(unaffectedInteractableObjects)
            .ToList();
    }
}