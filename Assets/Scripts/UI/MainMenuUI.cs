using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button continueButton;

    private void Awake()
    {
        newGameButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.ValleyofSolura);
        });
        
        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        Time.timeScale = 1f;
    }

}
