using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    
    [Header("Fields")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Weapon weapon;
    
    [Header("Basic Movements")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] float rotateSpeed = 10f;
    [SerializeField] private float dashSpeed = 4;
    [SerializeField] private float dashFalloff = 2;
    [SerializeField] private float dashCooldown;
    [SerializeField] float currentDashSpeed;
    private bool isDashing = false;
    private Vector3 dashDirection;
    private CharacterController controller;
    
    private State state;
    private enum State
    {
        Idle,
        Walk,
        Attack1,
        Dash,
    }

    private const string ATTACK1 = "Attack1";
    private Animator animator;
    private float attack1Cooldown;
    private float attack1CooldownCounter;
    private float playerHitBoxHeight = 1f;
    private Quaternion rightRotation = Quaternion.Euler(new Vector3(0, 90, 0));

    [Header("Player Health")]
    [SerializeField] private PlayerHealthSO playerHealthSO;
    [SerializeField] private float bufferDecreaseRate = 1f;
    [SerializeField] private float maxRegenHitCooldown = 4f;
    [SerializeField] private float playerHitIFrames = 1f;
    private float currentBufferCooldown = 0f;
    private bool bufferOnCooldown = false;
    private bool isDead = false;
    private float currentIFrameTimer = 0f;
    private bool startDash = false;

    [Header("Player Health")] 
    [SerializeField] private PlayerStatistics dreamShards;
    [SerializeField] private PlayerStatistics dreamThreads;

    [Header("Player Audio")] 
    [SerializeField] private PlayerAudio playerAudio;
    // TODO: modify PlayerAudio.cs
    
    [Header("Player Dream State")]
    [SerializeField] private float LucidThreshold;
    [SerializeField] private float DeepThreshold; // must be less than LucidThreshold
    private DreamState dreamState;
    private enum DreamState
    {
        Neutral,
        Lucid,
        Deep
    }

    public event UnityAction updateHealthBar;
    
    private void Awake()
    {
        state = State.Idle;
        
        Instance = this;
        
        animator = GetComponent<Animator>();
        
        attack1Cooldown = weapon.GetAttackCooldown();
        attack1CooldownCounter = 0;
        
        playerHealthSO.ResetHealth();
        
        controller = GetComponent<CharacterController>();
        
        dreamShards.setCurrencyCount(0);
        dreamThreads.setCurrencyCount(0);
    }

    private void Start()
    {
        GameInput.Instance.OnAttack1 += GameInput_OnAttack1;
        GameInput.Instance.OnDash += GameInput_OnDash;
    }

    private void GameInput_OnAttack1(object sender, System.EventArgs e)
    {
        HandleAttack1();
    }

    private void GameInput_OnDash(object sender, System.EventArgs e)
    {
        startDash = true;
    }

    private void Update()
    {
        HandleDrowsiness();
        attack1CooldownCounter -= Time.deltaTime;
        if(IsIdle() || IsWalking() || IsDashing())
            HandleMovement();
        HandleDreamState();
    }
    
    public void PlaySteps(string path)
    {
        FMODUnity.RuntimeManager.PlayOneShot(path, GetComponent<Transform>().position);
    }

    public void PlaySwoosh(string path)
    {
        FMODUnity.RuntimeManager.PlayOneShot(path, GetComponent<Transform>().position);
    }
    
    private void HandleMovement()
    {
        if (currentDashSpeed > 0)
        {
            if (startDash) startDash = false;
            currentDashSpeed -= dashFalloff * Time.deltaTime;
            if (currentDashSpeed < moveSpeed)
            {
                currentDashSpeed = 0;
                isDashing = false; 
            }
        }

        if (!isDashing) 
        {
            Vector3 inputVector = GameInput.Instance.GetMovementVectorNormalized();
            if (inputVector == Vector3.zero && currentDashSpeed <= 0)
            {
                state = State.Idle;
                return;
            }
        
            // calculates orthographic camera angle
            Vector3 forward = virtualCamera.transform.forward;
            forward.y = 0;
            Vector3 right = rightRotation * forward;

            forward *= inputVector.z;
            right *= inputVector.x;
            
            Vector3 moveDir = forward + right;
            
            float moveDistance = moveSpeed * Time.deltaTime;
            if (startDash && currentDashSpeed <= 0)
            {
                Dash(moveDir);
                isDashing = true; 
            }
            
            if (!isDashing)
            {
                moveDir = moveDir.normalized * moveDistance;
                
                float terrainHeight = Terrain.activeTerrain.SampleHeight(transform.position + moveDir) + .1f;
                moveDir.y += terrainHeight - transform.position.y;
                
                controller.Move(moveDir);
                if (moveDir != Vector3.zero)
                {
                    transform.forward = Vector3.Slerp(transform.forward, new Vector3(moveDir.x, 0, moveDir.z), Time.deltaTime * rotateSpeed);
                    state = State.Walk;
                }
            }
        }
        
        if (isDashing)
        {
            state = State.Dash;
            Vector3 dashMove = dashDirection * currentDashSpeed * Time.deltaTime;
            transform.forward = Vector3.Slerp(transform.forward, new Vector3(dashMove.x, 0, dashMove.z), Time.deltaTime * rotateSpeed);
            
            float terrainHeight = Terrain.activeTerrain.SampleHeight(transform.position + dashMove) + .1f;
            dashMove.y += terrainHeight - transform.position.y;

            controller.Move(dashMove);
        }
    }

    private void Dash(Vector3 dir)
    {
        currentDashSpeed = dashSpeed;
        dashDirection = dir.normalized;
    }
    
    private void HandleAttack1()
    {
        if (!IsAttacking1() && attack1CooldownCounter <= 0)
        {
            animator.SetTrigger(ATTACK1);
            state = State.Attack1;
            HandleAttackOnHitEffects();
            StartCoroutine(HandleAttack1Duration());
        }
    }

    private IEnumerator HandleAttack1Duration()
    {
        float attackDuration = .25f;
        yield return new WaitForSeconds(attackDuration);
        state = State.Idle;
        attack1CooldownCounter = attack1Cooldown;
    }

    private void HandleAttackOnHitEffects()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(weapon.GetAttackPoint().position, weapon.GetAttackRadius(), weapon.GetEnemyLayer());
        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().OnHit();
        }
    }

    private void HandleDrowsiness()
    {
        if(currentIFrameTimer <= playerHitIFrames)
            currentIFrameTimer += Time.deltaTime;

        if (currentBufferCooldown <= 0)
            bufferOnCooldown = false;
        else
        {
            bufferOnCooldown = true;
            currentBufferCooldown -= Time.deltaTime;
        }
        
        if (!bufferOnCooldown)
        {
            updateHealthBar?.Invoke();
            playerHealthSO.RegenBuffer(bufferDecreaseRate * Time.deltaTime);
        }

        if (playerHealthSO.IsDead())
            HandleDeath();
    }
    
    private void HandleDreamState()
    {
        if (drowsiness < LucidThreshold)
        {
            dreamState = DreamState.Lucid;
        }
        else if (drowsiness < DeepThreshold)
        {
            dreamState = DreamState.Deep;
        } else 
        {
            dreamState = DreamState.Neutral;
        }
    } 
    
    private void HandleDeath()
    {
        Debug.Log("u ded lol.\n");
    }

    public void HandleHit(float bufferDamage)
    {
        if (isDead)
            return;
        if (currentIFrameTimer <= playerHitIFrames)
            return;
        
        currentBufferCooldown = maxRegenHitCooldown;
        
        playerHealthSO.InflictDamage(bufferDamage);
        updateHealthBar?.Invoke();
        
        if (playerHealthSO.GetCurrentDrowsiness <= 0)
            isDead = true;
    }
    
    public bool IsDashing() => state == State.Dash;
    public bool IsWalking() => state == State.Walk;
    public bool IsIdle() => state == State.Idle;
    public bool IsAttacking1() => state == State.Attack1;
    public float GetPlayerHitBoxHeight() => playerHitBoxHeight;
    public Weapon GetWeapon() => weapon;
}
