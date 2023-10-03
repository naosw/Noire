using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public abstract class TransitionSO : ScriptableObject
{
    public SceneTransitionAnim animName;
    [SerializeField] protected AnimationCurve intensityCurve;
    [SerializeField] protected float animationTime = 0.25f;
    protected Image AnimatedObject;

    public abstract IEnumerator Enter(Canvas parent);
    public abstract IEnumerator Exit(Canvas parent);

    // OPTIMIZATION: pool this object
    protected virtual Image CreateImage(Canvas parent)
    {
        GameObject child = new GameObject("Transition Image");
        child.transform.SetParent(parent.transform, false);

        return child.AddComponent<Image>();
    }
}