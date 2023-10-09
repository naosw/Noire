using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Fade", menuName = "Scene Transitions/Fade")]
public class TransitionSO : ScriptableObject
{
    [SerializeField] protected AnimationCurve intensityCurve;
    [SerializeField] protected float animationTime = 2f;
    private Image img;
    
    public IEnumerator Enter(Canvas Parent)
    {
        float time = 0;
        
        Color startColor = Color.black;
        Color endColor = new Color(0, 0, 0, 0);
        
        while (time < 1)
        {
            img.color = Color.Lerp(
                startColor, 
                endColor, 
                intensityCurve.Evaluate(time)
            );
            yield return null;
            time += Time.deltaTime / animationTime;
        }
        
        Destroy(img.gameObject);
    }

    public IEnumerator Exit(Canvas Parent)
    {
        img = CreateImage(Parent);
        img.rectTransform.anchorMin = Vector2.zero;
        img.rectTransform.anchorMax = Vector2.one;
        img.rectTransform.sizeDelta = Vector2.zero;

        float time = 0;
        Color startColor = new Color(0, 0, 0, 0);
        Color endColor = Color.black;
        while (time < 1)
        {
            img.color = Color.Lerp(
                startColor, 
                endColor, 
                intensityCurve.Evaluate(time)
            );
            yield return null;
            time += Time.deltaTime / animationTime;
        }
    }
    
    private Image CreateImage(Canvas parent)
    {
        GameObject child = new GameObject("Transition");
        child.transform.SetParent(parent.transform, false);

        return child.AddComponent<Image>();
    }
}