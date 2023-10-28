using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class UIBlurBackground : UI
{
    [SerializeField] ScriptableRendererFeature kawaseBlur;

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        Hide();
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
        if(paused)
            Show();
        else
            Hide();
    }

    protected override void LateActivate()
    {
        kawaseBlur.SetActive(true);
    }

    protected override void LateDeactivate()
    {
        kawaseBlur.SetActive(false);
    }
}
