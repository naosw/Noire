using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathMenuUI : MonoBehaviour
{
    [SerializeField] private Button respawnButton;
    [SerializeField] private Button mainMenuButton;

    private void Awake()
    {
        // respawnButton.onClick.AddListener(() =>
        // {
        //     Loader.Load(SceneManager.GetActiveScene(), DataPersistenceManager.Instance.CurrentScene);
        // });
        
        mainMenuButton.onClick.AddListener(() =>
        {
            Loader.Load(SceneManager.GetActiveScene(), GameScene.MainMenuScene);
        });

        Time.timeScale = 1f;
    }

}
