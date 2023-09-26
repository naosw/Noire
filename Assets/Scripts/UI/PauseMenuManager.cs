using UnityEngine;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour {
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button optionsButton;
     
    private bool isGamePaused = false;


    private void Awake()
    {
        resumeButton.onClick.AddListener(TogglePauseGame);
        mainMenuButton.onClick.AddListener(OnMainMenuClick);
        optionsButton.onClick.AddListener(() => {
            OptionsUI.Instance.Show();
        });
    }
    
    private void Start() 
    {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        Hide();
    }

    private void GameInput_OnPauseAction(object sender, System.EventArgs e) 
    {
        TogglePauseGame();
    }

    private void OnMainMenuClick()
    {
        TogglePauseGame();
        Loader.Load(Loader.Scene.MainMenuScene);
    }

    private void TogglePauseGame() 
    {
        isGamePaused = !isGamePaused;
        if (isGamePaused)
        {
            Time.timeScale = 0f;
            Show();
        }
        else
        {
            Time.timeScale = 1f;
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }

}