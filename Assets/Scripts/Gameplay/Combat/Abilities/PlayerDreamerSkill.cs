using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "PlayerDreamerSkill", menuName = "Abilities/PlayerDreamerSkill")]
public class PlayerDreamerSkill : AbilitySO
{
    [SerializeField] private GameObject SphereDepthSample;
    [SerializeField] ScriptableRendererFeature playerSilhouetteFeature;
    [SerializeField] private float finalRadius = 100f;
    [SerializeField] private float skillDuration = 5f;
    
    private float animationTime = 1.5f;
    private GameObject sphereInstance;
    private float originalBloomIntensity;

    public override void Ready()
    {
        state = AbilityState.Ready;
        originalBloomIntensity = PostProcessingManager.Instance.GetBloomIntensity();
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
        PostProcessingManager.Instance.SetBloomIntensity(0);

        float time = 0;
        while (time < animationTime)
        {
            float eval = time / animationTime;
            
            // realm size
            float s = Mathf.Lerp(0, finalRadius, StaticInfoObjects.Instance.OPEN_REALM_CURVE.Evaluate(eval));
            sphereInstance.transform.localScale = new Vector3(s,s,s);
            
            // post effects
            PostProcessingManager.Instance.SetLensDistortionIntensity(
                Mathf.Lerp(0, -1, StaticInfoObjects.Instance.LD_OPEN_REALM_CURVE.Evaluate(eval)));
            PostProcessingManager.Instance.SetChromaticAberrationIntensity(
                Mathf.Lerp(0, 1, StaticInfoObjects.Instance.CA_OPEN_REALM_CURVE.Evaluate(eval)));
            
            time += Time.deltaTime;
            yield return null;
        }

        PostProcessingManager.Instance.SetLensDistortionIntensity(0);
        PostProcessingManager.Instance.SetChromaticAberrationIntensity(0);
        sphereInstance.transform.localScale = new Vector3(finalRadius,finalRadius,finalRadius);
    }
    
    private IEnumerator CloseRealm()
    {
        float time = 0;
        while (time < animationTime)
        {
            float eval = time / animationTime;
            
            float s = Mathf.Lerp(finalRadius, 0, StaticInfoObjects.Instance.CLOSE_REALM_CURVE.Evaluate(eval));
            sphereInstance.transform.localScale = new Vector3(s,s,s);
            time += Time.deltaTime;
            yield return null;
        }
        
        sphereInstance.SetActive(false);
        playerSilhouetteFeature.SetActive(true);
        PostProcessingManager.Instance.SetBloomIntensity(originalBloomIntensity);
    }
}
