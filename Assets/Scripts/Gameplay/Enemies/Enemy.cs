using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 20;
    [SerializeField] ParticleSystem onHitParticleEffects;
    [SerializeField] Material OnHitMaterial;
    private Renderer renderer;
    private Material[][] originalMaterial;
    private Coroutine onHit;
    public Weapon weapon;
    public float damage = 2f;
    public float health;
    public MeshRenderer[] onHits;
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
        gameObject.SetActive(false);
    }
}