using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Enemy : MonoBehaviour
{   
    [Header("Enemy Properties")]
    [Tooltip("The maximum health of the enemy")]
    [SerializeField] float maxHealth = 20;
    [Tooltip("The enemy weapon damage")]
    [SerializeField] protected float damage = 2f;
    [Header("Enemy Effects")]
    [Tooltip("The particle effects that play when the enemy is hit")]
    [SerializeField] ParticleSystem onHitParticleEffects;
    [Tooltip("The material that is applied to the enemy on hit")]
    [SerializeField] Material OnHitMaterial;
    [Tooltip("The players current weapon")]
    [SerializeField] private Weapon weapon;
    [Tooltip("Mesh Renderers to apply the on hit material to")]
    [SerializeField] private MeshRenderer[] onHits;
    
    // private unserialized fields
    private Renderer renderer;
    private float health;
    private Material[][] originalMaterial;
    private Coroutine onHit;
    private void Awake()
    {
        health = maxHealth;
        //weapon = Player.Instance.GetWeapon();
        renderer = GetComponent<Renderer>();
    }

    public virtual void Start()
    {
        originalMaterial = new Material[onHits.Length][];
        for (int i = 0; i < onHits.Length; i++)
        {
            originalMaterial[i] = onHits[i].materials;
        }
        
        if (onHitParticleEffects != null)
            onHitParticleEffects.Stop();
    }

    
    public void OnHit()
    {
        PlayOnHitEffects();
        RecieveDamage();
    }

    private void RecieveDamage()
    {
        Debug.Log(health);
        health -= weapon.GetAttackDamage();
        if(health <= 0)
            Die();
    }
    public void PlayOnHitEffects()  
    {
        if (onHit != null)
            StopCoroutine(onHit);
        onHit = StartCoroutine(PlayOnHitEffectsWithDelay());
    }

    private IEnumerator PlayOnHitEffectsWithDelay()
    {
        if (onHitParticleEffects != null)
        {
            onHitParticleEffects.transform.LookAt(Player.Instance.transform.position + new Vector3(0, Player.Instance.GetPlayerHitBoxHeight(), 0));
            onHitParticleEffects.Play();
        }
        for (int i = 0; i < onHits.Length; i++)
        {
            for (int j = 0; j < onHits[i].materials.Length; j++)
            {
                onHits[i].materials[j] = OnHitMaterial;
            }
             
        }
        yield return new WaitForSeconds(.2f);
        for (int i = 0; i < onHits.Length; i++)
        {
            onHits[i].materials = originalMaterial[i];
        }
        onHit = null;
    }

    public void Die()
    {
        //PlayDyingSound
        FMODUnity.RuntimeManager.PlayOneShot("event:/Character/Enemy/EyeballDie", transform.position);
        gameObject.SetActive(false);
    }
}