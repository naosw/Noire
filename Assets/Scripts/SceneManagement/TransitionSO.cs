using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Fade", menuName = "Scene Transitions/Fade")]
public class TransitionSO : ScriptableObject
{
    [SerializeField] protected AnimationCurve intensityCurve;
    [SerializeField] protected float animationTime = 0.25f;
    protected Image AnimatedObject;
    
    public IEnumerator Enter(Canvas Parent)
    {
        float time = 0;
        Color startColor = Color.black;
        Color endColor = new Color(0, 0, 0, 0);
        while (time < 1)
        {
            AnimatedObject.color = Color.Lerp(
                startColor, 
                endColor, 
                intensityCurve.Evaluate(time)
            );
            yield return null;
            time += Time.deltaTime / animationTime;
        }

        Destroy(AnimatedObject.gameObject);
    }

    public IEnumerator Exit(Canvas Parent)
    {
        AnimatedObject = CreateImage(Parent);
        AnimatedObject.rectTransform.anchorMin = Vector2.zero;
        AnimatedObject.rectTransform.anchorMax = Vector2.one;
        AnimatedObject.rectTransform.sizeDelta = Vector2.zero;

        float time = 0;
        Color startColor = new Color(0, 0, 0, 0);
        Color endColor = Color.black;
        while (time < 1)
        {
            AnimatedObject.color = Color.Lerp(
                startColor, 
                endColor, 
                intensityCurve.Evaluate(time)
            );
            yield return null;
            time += Time.deltaTime / animationTime;
        }
    }
    
    protected Image CreateImage(Canvas parent)
    {
        GameObject child = new GameObject("Transition Image");
        child.transform.SetParent(parent.transform, false);

        return child.AddComponent<Image>();
    }
}