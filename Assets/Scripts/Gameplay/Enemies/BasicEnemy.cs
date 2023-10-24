using System.Collections;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    [SerializeField] ParticleSystemBase onHitParticleEffects;
    [SerializeField] private ParticleSystemBase onHitCutEffects;
    [SerializeField] Material OnHitMaterial;

    private Renderer renderer;
    private Material originalMaterial;

    private Coroutine onHit;

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
    }

    private void Start()
    {
        originalMaterial = renderer.material;
        if (onHitParticleEffects != null)
            onHitParticleEffects.Stop();
    }

    public void OnHit() {
        if (onHit != null) 
            StopCoroutine(onHit);

        onHit = StartCoroutine(PlayOnHitEffectsWithDelay());
    }

    private IEnumerator PlayOnHitEffectsWithDelay()
    {
        yield return new WaitForSeconds(.2f);

        // handles particle effects
        if (onHitParticleEffects != null)
        {
            var lookAt = Player.Instance.transform.position +
                         new Vector3(0, Player.Instance.GetPlayerHitBoxHeight(), 0);
            
            onHitParticleEffects.transform.LookAt(lookAt);
            onHitCutEffects.Restart();
            onHitParticleEffects.Restart();
        }

        // handles material change
        renderer.material = OnHitMaterial;

        // change material back when attack ended
        yield return new WaitForSeconds(.2f);
        renderer.material = originalMaterial;

        // set current coroutine to null
        onHit = null;
    }
}