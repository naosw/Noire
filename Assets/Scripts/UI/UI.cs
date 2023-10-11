using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UI : MonoBehaviour
{
    [SerializeField] protected GameObject containerGameObject;
    protected CanvasGroup canvasGroup;
    protected bool alternativeGameObject = false;
    private float animationTime = .5f;

    protected virtual void Activate() { }
    protected virtual void Deactivate() { }

    public virtual void Show()
    {
        Activate();
        StopAllCoroutines();
        Display(true);
        StartCoroutine(Fade(0, 1));
    }

    public virtual void Hide()
    {
        Deactivate();
        StopAllCoroutines();
        StartCoroutine(Fade(1, 0));
    }

    protected IEnumerator Fade(float start, float end)
    {
        // canvasGroup.alpha = start;
        
        float time = 0;
        while (time < animationTime)
        {
            time += Time.deltaTime;

            canvasGroup.alpha = Mathf.Lerp(
                start, 
                end, 
                StaticInfoObjects.Instance.FADE_ANIM_CURVE.Evaluate(time * 2)
            );
            yield return null;
        }
        
        // canvasGroup.alpha = end;
        Display(end != 0);
    }

    private void Display(bool active)
    {
        if (alternativeGameObject)
            containerGameObject.SetActive(active);
        else
            gameObject.SetActive(active);
    }
}