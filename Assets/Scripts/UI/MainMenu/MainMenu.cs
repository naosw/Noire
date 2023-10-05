using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : Menu
{
    [Header("Menu Navigation")]
    [SerializeField] private SaveSlotsMenu saveSlotsMenu;

    [Header("Menu Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button quitGameButton;

    private void Awake()
    {
        newGameButton.onClick.AddListener(OnNewGameClicked);
        continueGameButton.onClick.AddListener(OnContinueGameClicked);
        loadGameButton.onClick.AddListener(OnLoadGameClicked);
        quitGameButton.onClick.AddListener(OnQuitGameClicked);
        
        Show();
    }

    private void Start() 
    {
        Refresh();
    }

    private void Refresh()
    {
        if (!DataPersistenceManager.Instance.HasGameData()) 
        {
            continueGameButton.interactable = false;
            loadGameButton.interactable = false;
        }
    }

    private void Disable()
    {
        newGameButton.interactable = false;
        continueGameButton.interactable = false;
        loadGameButton.interactable = false;
        quitGameButton.interactable = false;
    }

    private void OnNewGameClicked()
    {
        Hide();
        saveSlotsMenu.Show(false);
    }

    private void OnContinueGameClicked()
    {
        Disable();
        Loader.Load(DataPersistenceManager.Instance.CurrentScene);
    }
    
    private void OnLoadGameClicked() 
    {
        Hide();
        saveSlotsMenu.Show(true);
    }

    private void OnQuitGameClicked()
    {
        Disable();
        Application.Quit();
    }

    public void Show()
    {
        Refresh();
        gameObject.SetActive(true);
    }

    public void Hide() 
    {
        gameObject.SetActive(false);
    }
}