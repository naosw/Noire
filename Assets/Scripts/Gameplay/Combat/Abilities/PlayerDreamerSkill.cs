using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDreamerSkill", menuName = "Abilities/PlayerDreamerSkill")]
public class PlayerDreamerSkill : AbilitySO
{
    [SerializeField] private GameObject SphereDepthSample;
    [SerializeField] private float finalRadius = 100f;
    [SerializeField] private float skillDuration = 5f;
    private float animationTime = 1f;
    private GameObject sphereInstance;
    
    protected override void Initialize()
    {
        Player.Instance.StartCoroutine(ExpandRealm());
    }
    
    protected override void Cast()
    {
        Debug.Log("CASTED dreamer");
        Player.Instance.StartCoroutine(WaitEndOfAction(skillDuration));
    }
    
    protected override void Finish()
    {
        Player.Instance.StartCoroutine(CloseRealm());
        state = AbilityState.OnCooldown;
    }

    private IEnumerator ExpandRealm()
    {
        if(!sphereInstance)
            sphereInstance = Instantiate(SphereDepthSample, Player.Instance.transform);
        sphereInstance.SetActive(true);

        float time = 0;
        while (time < animationTime)
        {
            var s = Mathf.Lerp(0, finalRadius, StaticInfoObjects.Instance.OPEN_REALM_CURVE.Evaluate(time));
            sphereInstance.transform.localScale = new Vector3(s,s,s);
            time += Time.deltaTime;
            yield return null;
        }
        sphereInstance.transform.localScale = new Vector3(finalRadius,finalRadius,finalRadius);
    }
    
    private IEnumerator CloseRealm()
    {
        float time = 0;
        while (time < animationTime)
        {
            var s = Mathf.Lerp(finalRadius, 0, StaticInfoObjects.Instance.CLOSE_REALM_CURVE.Evaluate(time));
            sphereInstance.transform.localScale = new Vector3(s,s,s);
            time += Time.deltaTime;
            yield return null;
        }
        sphereInstance.SetActive(false);
    }
}
