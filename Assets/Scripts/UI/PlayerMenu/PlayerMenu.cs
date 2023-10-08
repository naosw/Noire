using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class PlayerMenu : UI
{
    public static PlayerMenu Instance { get; private set; }
    
    [Header("PlayerMenu Navigation Buttons")]
    [SerializeField] private Button swipeLeft;
    [SerializeField] private Button swipeRight;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameInput.Instance.OnPlayerMenuToggle += GameInput_OnPlayerMenuToggle;
    }
    
    private void OnDestroy()
    {
        GameInput.Instance.OnPlayerMenuToggle -= GameInput_OnPlayerMenuToggle;
    }

    private void GameInput_OnPlayerMenuToggle()
    {
        Show();
    }
}