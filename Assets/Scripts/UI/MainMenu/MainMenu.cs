using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
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

    private void ToggleButtons(bool val)
    {
        newGameButton.interactable = val;
        continueGameButton.interactable = val;
        loadGameButton.interactable = val;
        quitGameButton.interactable = val;
    }

    private void OnNewGameClicked()
    {
        Hide();
        saveSlotsMenu.Show(false);
    }

    private void OnContinueGameClicked()
    {
        ToggleButtons(false);
        if(!Loader.Load(DataPersistenceManager.Instance.CurrentScene))
            ToggleButtons(true);
    }
    
    private void OnLoadGameClicked() 
    {
        Hide();
        saveSlotsMenu.Show(true);
    }

    private void OnQuitGameClicked()
    {
        ToggleButtons(false);
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