using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class UI : MonoBehaviour
{
    [SerializeField] protected GameObject containerGameObject;
    protected CanvasGroup canvasGroup;
    protected RectTransform rectTransform;
    protected bool alternativeGameObject = false;
    private float animationTime = .5f;

    protected virtual void Activate() { }
    protected virtual void Deactivate() { }

    public virtual void Show()
    {
        Activate();
        Display(true);
        StartCoroutine(Fade(0, 1));
    }

    public virtual void Hide()
    {
        Deactivate();
        StartCoroutine(Fade(1, 0));
    }

    protected IEnumerator Fade(float start, float end)
    {
        GameEventsManager.Instance.GameStateEvents.MenuToggle(true);
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
        
        canvasGroup.alpha = end;
        if(end == 0)
            Display(false);
        GameEventsManager.Instance.GameStateEvents.MenuToggle(false);
    }

    private void Display(bool active)
    {
        if (alternativeGameObject)
            containerGameObject.SetActive(active);
        else
            gameObject.SetActive(active);
    }

    protected void Init()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
    }
}