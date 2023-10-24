using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private CanvasGroup fadein;
    [SerializeField] private float fadeinTime = 5f;
    [SerializeField] private AnimationCurve fadeinCurve;
    [SerializeField] private ScriptableRendererFeature fogRendererFeature; 

    private void Start()
    {
        fogRendererFeature.SetActive(false);
        StartCoroutine(FadeInCoroutine());
    }

    private IEnumerator FadeInCoroutine()
    {
        float time = 0;

        while (time < fadeinTime)
        {
            fadein.alpha = Mathf.Lerp(1, 0, fadeinCurve.Evaluate(time / fadeinTime));
            time += Time.deltaTime;
            yield return null;
        }

        fadein.gameObject.SetActive(false);
    }
}