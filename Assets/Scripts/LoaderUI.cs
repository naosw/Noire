using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoaderUI : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI progressText;
    
    private void Awake()
    {
        slider.value = 0;
        progressText.text = "0 %";
        
        StartCoroutine(LoadLevelAsync());
    }

    private IEnumerator LoadLevelAsync()
    {
        yield return new WaitForSeconds(1);
        var operation = SceneManager.LoadSceneAsync(Loader.targetScene.ToString(), LoadSceneMode.Single);
        
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            slider.value = progress;
            progressText.text = progress * 100f + " %";
            yield return null;
        }
        
        yield return new WaitForSeconds(1);
        
        Debug.Log("Scene Loaded");
    }
}
