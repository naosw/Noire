using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "PlayerDreamerSkill", menuName = "Abilities/PlayerDreamerSkill")]
public class PlayerDreamerSkill : AbilitySO
{
    [SerializeField] private GameObject SphereDepthSample;
    [SerializeField] ScriptableRendererFeature playerSilhouetteFeature;
    [SerializeField] private float finalRadius = 100f;
    [SerializeField] private float skillDuration = 5f;
    [SerializeField] private Volume postProcessVolume;
    
    private float animationTime = 1f;
    private GameObject sphereInstance;
    private LensDistortion lensDistortion;
    private ChromaticAberration chromaticAberration;

    public override void Ready()
    {
        state = AbilityState.Ready;
        if(!postProcessVolume.profile.TryGet(out lensDistortion))
            Debug.LogError("Did not find a lens distortion in PostEffects");
        if(!postProcessVolume.profile.TryGet(out chromaticAberration))
            Debug.LogError("Did not find a chromatic aberration in PostEffects");
    }
    
    protected override void Initialize()
    {
        Player.Instance.StartCoroutine(ExpandRealm());
    }
    
    protected override void Cast()
    {
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
            sphereInstance = Instantiate(SphereDepthSample);
        
        sphereInstance.transform.position = Player.Instance.transform.position;
        playerSilhouetteFeature.SetActive(false);
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
        playerSilhouetteFeature.SetActive(true);
    }
}
