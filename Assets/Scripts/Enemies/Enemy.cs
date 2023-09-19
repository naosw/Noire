using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public enum State
    {
        Idle,
        Search,
        Attack
    }

    public State currentState = State.Idle;
    public float lookRadius = 10.0f;
    public LayerMask obstacleMask;
    public NavMeshAgent agent;
    public Transform target;
    public float searchTime = 0.0f;
    public float searchDuration = 5.0f;
    public Vector3 lastKnownPosition;
    public float maxHealth = 100;
    [SerializeField] ParticleSystem onHitParticleEffects;
    [SerializeField] Material OnHitMaterial;
    private Renderer renderer;
    private Material originalMaterial;
    private Coroutine onHit;
    public Weapon weapon;

    public float health { get; private set; }
    private void Awake()
    {
        health = maxHealth;
        //weapon = Player.Instance.GetWeapon();
        renderer = GetComponent<Renderer>();
        agent = GetComponent<NavMeshAgent>();
    }

    public virtual void Start()
    {
        originalMaterial = renderer.material;
        if (onHitParticleEffects != null)
            onHitParticleEffects.Stop();
    }

    public virtual void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                Idle();
                break;
            case State.Search:
                Search();
                break;
            case State.Attack:
                Attack();
                break;
        }
    }

    public virtual void Idle()
    {
        agent.isStopped = true;
    }

    public virtual void Search()
    {
        agent.isStopped = false;
        agent.SetDestination(lastKnownPosition);
    }

    public virtual void Attack()
    {
        agent.isStopped = false;
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
        yield return new WaitForSeconds(.2f);
        
        if (onHitParticleEffects != null)
        {
            onHitParticleEffects.transform.LookAt(Player.Instance.transform.position + new Vector3(0, Player.Instance.GetPlayerHitBoxHeight(), 0));
            onHitParticleEffects.Play();
            Debug.Log("PLAY PARTI");
        }
        renderer.material = OnHitMaterial;
        yield return new WaitForSeconds(.2f);
        renderer.material = originalMaterial;
        onHit = null;
    }

    public void Die()
    {
        
    }
}