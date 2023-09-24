using Cinemachine;
using System.Collections;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
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

    private float drowsiness = 100;


    private void Awake()
    {
        state = State.Idle;
        Instance = this;
        animator = GetComponent<Animator>();
        attack1Cooldown = weapon.GetAttackCooldown();
        attack1CooldownCounter = 0;
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
        attack1CooldownCounter -= Time.deltaTime;
        if(IsIdle() || IsWalking())
            HandleMovement();
        HandleDreamState()
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

    public bool IsWalking() => state == State.Walk;
    public bool IsIdle() => state == State.Idle;
    public bool IsAttacking1() => state == State.Attack1;
    public float GetPlayerHitBoxHeight() => playerHitBoxHeight;

    Weapon GetWeapon() => weapon;

    [SerializeField] private float LucidThreshold; // needs value 
    [SerializeField] private float DeepThreshold; // must be less than LucidThreshold

    private enum DreamState
    {
        Neutral,
        Lucid,
        Deep
    }

    private DreamState dreamState;

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
}
