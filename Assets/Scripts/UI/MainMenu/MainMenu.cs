using UnityEngine;
using UnityEngine.UI;

public class MainMenu : Menu
{
    [Header("Menu Navigation")]
    [SerializeField] private SaveSlotsMenu saveSlotsMenu;

    [Header("Menu Buttons")]
    [SerializeField] private ButtonUI newGameButton;
    [SerializeField] private ButtonUI continueGameButton;
    [SerializeField] private ButtonUI loadGameButton;
    [SerializeField] private ButtonUI quitGameButton;

    private void Awake()
    {
    }

    private void Start() 
    {
        newGameButton.AddListener(OnNewGameClicked);
        continueGameButton.AddListener(OnContinueGameClicked);
        loadGameButton.AddListener(OnLoadGameClicked);
        quitGameButton.AddListener(OnQuitGameClicked);
        
        Show();
    }

    private void Refresh()
    {
        if (!DataPersistenceManager.Instance.HasGameData()) 
        {
            continueGameButton.Disable();
            loadGameButton.Disable();
        }
    }

    private void ToggleButtons(bool val)
    {
        if (val)
        {
            newGameButton.Enable();
            continueGameButton.Enable();
            loadGameButton.Enable();
            quitGameButton.Enable();
        }
        else
        {
            newGameButton.Disable();
            continueGameButton.Disable();
            loadGameButton.Disable();
            quitGameButton.Disable();
        }
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