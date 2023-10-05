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

    public void LoadScene(string scene, LoadSceneMode mode = LoadSceneMode.Single)
    {
        loadLevelOperation = SceneManager.LoadSceneAsync(scene, mode);
        
        loadLevelOperation.allowSceneActivation = false;
        transitionCanvas.enabled = true;
        
        StartCoroutine(Exit());
    }

    private IEnumerator Exit()
    {
        yield return StartCoroutine(fadeTransition.Exit(transitionCanvas));
        loadLevelOperation.allowSceneActivation = true;
    }

    private IEnumerator Enter()
    {
        yield return StartCoroutine(fadeTransition.Enter(transitionCanvas));
        transitionCanvas.enabled = false;
        loadLevelOperation = null;
    }

    private void HandleSceneChange(Scene oldScene, Scene newScene)
    {
        StartCoroutine(Enter());
    }
}