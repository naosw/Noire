using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using ANIM = SceneTransitionAnim;

[DisallowMultipleComponent]
[RequireComponent(typeof(Canvas))]
public class SceneTransitioner : MonoBehaviour
{
    public static SceneTransitioner Instance { get; private set; }
    
    [SerializeField] private List<TransitionSO> Transitions = new();
    
    private AsyncOperation LoadLevelOperation;
    private TransitionSO ActiveEnterTransition;
    private TransitionSO ActiveExitTransition;
    private Canvas TransitionCanvas;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning($"Invalid configuration. Duplicate Instances found! First one: {Instance.name} Second one: {name}. Destroying second one.");
            Destroy(gameObject);
            return;
        }

        SceneManager.activeSceneChanged += HandleSceneChange;
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        TransitionCanvas = GetComponent<Canvas>();
        TransitionCanvas.enabled = false;
    }

    public void LoadScene(
        string scene, 
        ANIM animEnter = ANIM.None,
        ANIM animExit = ANIM.None, 
        LoadSceneMode mode = LoadSceneMode.Single)
    {
        LoadLevelOperation = SceneManager.LoadSceneAsync(scene, mode);

        var enterTransition = Transitions.Find(
            (transition) => transition.animName == animEnter
        );
        var exitTransition = Transitions.Find(
            (transition) => transition.animName == animExit
        );
        
        if (enterTransition != null)
        {
            LoadLevelOperation.allowSceneActivation = false;
            TransitionCanvas.enabled = true;
            ActiveEnterTransition = enterTransition;
            ActiveExitTransition = exitTransition;
            StartCoroutine(Exit());
        }
        else
        {
            Debug.LogWarning($"No transition found for" +
                $" TransitionMode {animEnter}!" +
                $" Maybe you are misssing a configuration?");
        }
    }

    private IEnumerator Exit()
    {
        yield return StartCoroutine(ActiveExitTransition.Exit(TransitionCanvas));
        LoadLevelOperation.allowSceneActivation = true;
    }

    private IEnumerator Enter()
    {
        yield return StartCoroutine(ActiveEnterTransition.Enter(TransitionCanvas));
        TransitionCanvas.enabled = false;
        LoadLevelOperation = null;
        ActiveExitTransition = null;
    }

    private void HandleSceneChange(Scene oldScene, Scene newScene)
    {
        if (ActiveExitTransition != null)
        {
            StartCoroutine(Enter());
        }
    }
}
