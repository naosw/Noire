using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathMenuUI : MonoBehaviour
{
    [SerializeField] private Button respawnButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        respawnButton.onClick.AddListener(() =>
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
