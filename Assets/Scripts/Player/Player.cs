using Cinemachine;
using System.Collections;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.PlayerSettings;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] float rotateSpeed = 10f;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Weapon weapon;
    [SerializeField] private float playerRadius = 1.5f;
    [SerializeField] private float playerHeight = 6f;
    [SerializeField] private LayerMask collidableLayers;

    public static Player Instance { get; private set; }
    private enum State
    {
        Idle,
        Walk,
        Attack1,
        state2
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
    
    public event UnityAction updateHealthBar;
    private void Awake()
    {
        state = State.Idle;
        Instance = this;
        animator = GetComponent<Animator>();
        attack1Cooldown = weapon.GetAttackCooldown();
        attack1CooldownCounter = 0;
        playerHealthSO.ResetHealth();
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
        HandleDrowsiness();
        attack1CooldownCounter -= Time.deltaTime;
        if(IsIdle() || IsWalking())
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
        // set to terrain height
        Vector3 pos = transform.position;
        pos.y = Terrain.activeTerrain.SampleHeight(transform.position) + .1f;
        transform.position = pos;

        Vector3 inputVector = GameInput.Instance.GetMovementVectorNormalized();
        if (inputVector == Vector3.zero)
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

        if (Physics.CapsuleCast(transform.position,
                                transform.position + Vector3.up * playerHeight,
                                playerRadius,
                                forward,
                                moveDistance,
                                collidableLayers))
            forward = Vector3.zero;
        if (Physics.CapsuleCast(transform.position,
                                transform.position + Vector3.up * playerHeight,
                                playerRadius,
                                right,
                                moveDistance,
                                collidableLayers))
            right = Vector3.zero;

        Vector3 moveDir = forward + right;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
        transform.position += moveDir.normalized * moveDistance;

        if (moveDir != Vector3.zero)
            state = State.Walk;
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
            enemy.GetComponent<Enemy>().PlayOnHitEffects();
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

        if (playerHealthSO.isDead())
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
        
        if (playerHealthSO.CurrentDrowsiness <= 0)
        {
            IsDead = true;
        }
    }

    public bool IsWalking() => state == State.Walk;
    public bool IsIdle() => state == State.Idle;
    public bool IsAttacking1() => state == State.Attack1;
    public float GetPlayerHitBoxHeight() => playerHitBoxHeight;

    public Weapon GetWeapon() => weapon;
}
