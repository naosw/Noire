using System.Collections;
using TMPro;
using UnityEngine;

public class DeathMenuUI : UI
{
    [SerializeField] private ButtonUI respawnButton;
    [SerializeField] private ButtonUI mainMenuButton;
    [SerializeField] private TextMeshProUGUI deathMenuDisplay;
    [SerializeField] private float typingSpeed = 0.1f;
    [SerializeField] private CanvasGroup canvasGroup;

    private string deathMessage = "You woke up.\n Nothing has changed.";
    
    private void Start()
    {
        mainMenuButton.AddListener(() => Loader.Load(GameScene.MainMenuScene));
        respawnButton.AddListener(Loader.Respawn);
        
        canvasGroup.alpha = 0;
        ToggleButtons(false);
        StartCoroutine(DisplayLine(deathMessage));
    }
    
    // displays the text one character at a time
    private IEnumerator DisplayLine(string line) 
    {
        // set the text to the full line, but set the visible characters to 0
        deathMenuDisplay.text = line;
        deathMenuDisplay.maxVisibleCharacters = 0;

        // display each letter one at a time
        foreach (var _ in line)
        {
            PlayTypingSound();
            deathMenuDisplay.maxVisibleCharacters++;
            yield return new WaitForSeconds(typingSpeed);
        }

        ToggleButtons(true);
        StartCoroutine(FadeInButtons());
    }

    private void ToggleButtons(bool activate)
    {
        respawnButton.gameObject.SetActive(activate);
        mainMenuButton.gameObject.SetActive(activate);
    }

    private IEnumerator FadeInButtons()
    {
        float time = 0;
        while (time < 2)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, time * 0.5f);
            time += Time.deltaTime;
            yield return null;
        }
    }

    private void PlayTypingSound()
    {
        // TODO FELIX: implement some typing sound when u see this
    }
}
