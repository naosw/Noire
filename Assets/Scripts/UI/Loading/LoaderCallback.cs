using System;
using System.Collections;
using UnityEngine;

public class LoaderCallback : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(LoadLevelAsync());
    }

    private IEnumerator LoadLevelAsync()
    {
        yield return new WaitForSeconds(20);
        SceneTransitioner.Instance.LoadScene(LoaderStatic.targetScene.ToString(), SceneTransitionMode.Fade);
    }
    
}