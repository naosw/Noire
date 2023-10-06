using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
[RequireComponent(typeof(Canvas))]
public class SceneTransitioner : MonoBehaviour
{
    public static SceneTransitioner Instance { get; private set; }

    [SerializeField] private TransitionSO fadeTransition;
    private Canvas transitionCanvas;
    private AsyncOperation loadLevelOperation;
    private bool isLoading = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        transitionCanvas = GetComponent<Canvas>();
        transitionCanvas.enabled = false;
    }

    private void Start()
    {
        SceneManager.activeSceneChanged += HandleSceneChange;
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= HandleSceneChange;
    }

    public bool LoadScene(string scene, LoadSceneMode mode = LoadSceneMode.Single)
    {
        if (isLoading)
            return false;
        
        loadLevelOperation = SceneManager.LoadSceneAsync(scene, mode);

        loadLevelOperation.allowSceneActivation = false;
        transitionCanvas.enabled = true;

        StartCoroutine(Exit());

        return true;
    }

    private IEnumerator Exit()
    {
        // start fade out
        isLoading = true;
        yield return StartCoroutine(fadeTransition.Exit(transitionCanvas));
        loadLevelOperation.allowSceneActivation = true;
    }

    private IEnumerator Enter()
    {
        // start to fade in with next scene
        yield return StartCoroutine(fadeTransition.Enter(transitionCanvas));
        
        // finished loading
        transitionCanvas.enabled = false;
        loadLevelOperation = null;
        isLoading = false;
    }

    private void HandleSceneChange(Scene oldScene, Scene newScene)
    {
        StartCoroutine(Enter());
    }
}