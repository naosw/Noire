using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.PlayerSettings;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float dashSpeed = 4;
    [SerializeField] private float dashFalloff = 2;
    [SerializeField] private float dashCooldown;
    [SerializeField] float rotateSpeed = 10f;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Weapon weapon;
    [SerializeField] private float playerRadius = 1.5f;
    [SerializeField] private float playerHeight = 6f;
    [SerializeField] private LayerMask collidableLayers;
    private bool isDashing = false;
    private Vector3 dashDirection;
    [SerializeField] float currentDashSpeed;
    public static Player Instance { get; private set; }
    private enum State
    {
        Idle,
        Walk,
        Attack1,
        Dash,
    }

    private const string ATTACK1 = "Attack1";

    private State state;
    private Animator animator;
    private float attack1Cooldown;
    private float attack1CooldownCounter;
    private float playerHitBoxHeight = 1f;
    private Quaternion rightRotation = Quaternion.Euler(new Vector3(0, 90, 0));

    [Header("Player Health")]
    [SerializeField] private PlayerHealthSO playerHealthSO;
    [SerializeField] private float bufferDecreaseRate = 1f;
    [SerializeField] private float maxRegenHitCooldown = 4f;
    private float currentBufferCooldown = 0f;
    private bool bufferOnCooldown = false;
    private bool IsDead;
    public float playerHitIFrames = 1f;
    private float currentIFrameTimer = 0f;
    private CharacterController controller;
    private bool startDash = false;

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
    }

    private void Start()
    {
        GameInput.Instance.OnAttack1 += GameInput_OnAttack1;
    }
    private void GameInput_OnAttack1(object sender, System.EventArgs e)
    {
        HandleAttack1();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.V)) startDash = true;
        HandleDrowsiness();
        attack1CooldownCounter -= Time.deltaTime;
        if(IsIdle() || IsWalking() || IsDashing())
            HandleMovement();
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

            Vector3 forward = virtualCamera.transform.forward;
            forward.y = 0;
            Vector3 right = rightRotation * forward;

            forward *= inputVector.z;
            right *= inputVector.x;

            float moveDistance = moveSpeed * Time.deltaTime;
            
            Vector3 moveDir = forward + right;
            
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
        {
            currentIFrameTimer += Time.deltaTime;
        }

        if (currentBufferCooldown <= 0)
        {
            bufferOnCooldown = false;
        }
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
        {
            HandleDeath();
        }

    }
    
    private void HandleDeath()
    {
        Debug.Log("u ded lol.\n");
    }

    public void HandleHit(float bufferDamage)
    {
        if (IsDead)
            return;
        if (currentIFrameTimer <= playerHitIFrames)
            return;
        
        currentBufferCooldown = maxRegenHitCooldown;
        
        playerHealthSO.InflictDamage(bufferDamage);
        updateHealthBar?.Invoke();
        
        if (playerHealthSO.GetCurrentDrowsiness <= 0)
        {
            IsDead = true;
        }
    }
    public bool IsDashing() => state == State.Dash;
    public bool IsWalking() => state == State.Walk;
    public bool IsIdle() => state == State.Idle;
    public bool IsAttacking1() => state == State.Attack1;
    public float GetPlayerHitBoxHeight() => playerHitBoxHeight;
    public Weapon GetWeapon() => weapon;
}
