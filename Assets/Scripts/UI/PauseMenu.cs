using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PauseMenu : UI 
{
    [SerializeField] private ButtonUI resumeButton;
    [SerializeField] private ButtonUI mainMenuButton;
    [SerializeField] private ButtonUI optionsButton;
     
    public static PauseMenu Instance { get; private set; }

    public bool IsGamePaused = false;
    
    private void Awake()
    {
        Instance = this;
        canvasGroup = GetComponent<CanvasGroup>();
    }
    
    private void Start()
    {
        GameInput.Instance.OnPauseToggle += GameInputOnPauseToggle;
        
        resumeButton.AddListener(TogglePauseGame);
        mainMenuButton.AddListener(OnMainMenuClick);
        optionsButton.AddListener(OnOptionsMenuClick);
        
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        GameInput.Instance.OnPauseToggle -= GameInputOnPauseToggle;
    }

    private void GameInputOnPauseToggle() 
    {
        TogglePauseGame();
    }
    
    private void ToggleButtons(bool val)
    {
        if (val)
        {
            resumeButton.Enable();
            mainMenuButton.Enable();
            optionsButton.Enable();
        }
        else
        {
            resumeButton.Disable();
            mainMenuButton.Disable();
            optionsButton.Disable();
        }
    }


    private void OnMainMenuClick()
    {
        ToggleButtons(false);
        TogglePauseGame();
        if (!Loader.Load(GameScene.MainMenuScene))
            ToggleButtons(true);
    }
    
    private void OnOptionsMenuClick()
    {
        Hide();
        OptionsUI.Instance.Show();
    }

    private void TogglePauseGame() 
    {
        IsGamePaused = !IsGamePaused;
        if (IsGamePaused)
        {
            Show();
        }
        else
        {
            if(gameObject.activeSelf)
                Hide();
        }
        GameEventsManager.Instance.GameStateEvents.PauseToggle(IsGamePaused);
    }
}