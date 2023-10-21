using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class UIBackgroundManager : MonoBehaviour
{
    public static UIBackgroundManager Instance { get; private set; }
    
    [SerializeField] ScriptableRendererFeature kawaseBlur;
    [SerializeField] private GameObject background;

    private void Awake()
    {
        if (Instance != null) 
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        background.SetActive(false);
        kawaseBlur.SetActive(false);
        GameEventsManager.Instance.GameStateEvents.OnPauseToggle += OnPause;
        GameEventsManager.Instance.GameStateEvents.OnUIToggle += OnPause;
    }

    private void OnDestroy()
    {
        GameEventsManager.Instance.GameStateEvents.OnPauseToggle -= OnPause;
        GameEventsManager.Instance.GameStateEvents.OnUIToggle -= OnPause;
    }

    private void OnPause(bool paused)
    {
        kawaseBlur.SetActive(paused);
        background.SetActive(paused);
    }
}
