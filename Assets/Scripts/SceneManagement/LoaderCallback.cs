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
        yield return new WaitForSeconds(2);
        SceneTransitioner.Instance.LoadScene(Loader.TargetScene, Loader.TargetSceneInfoObj.LoadMode);
    }
    
}