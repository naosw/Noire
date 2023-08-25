using Cinemachine;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] float rotateSpeed = 10f;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Weapon weapon;

    public static Player Instance { get; private set; }
    private enum State
    {
        Idle,
        Walk,
        Attack1
    }

    private const string ATTACK1 = "Attack1";

    private State state;
    private Animator animator;
    private float attack1Cooldown;
    private float attack1CooldownCounter;
    private float playerHitBoxHeight = 1f;

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
        Shader.SetGlobalVector("_PositionMoving", transform.position);
        if(IsIdle() || IsWalking())
            HandleMovement();
    }

    void LateUpdate()
    {
        Vector3 pos = transform.position;
        pos.y = Terrain.activeTerrain.SampleHeight(transform.position);
        transform.position = pos;
    }

    private void HandleMovement()
    {
        Vector3 inputVector = GameInput.Instance.GetMovementVectorNormalized();
        if (inputVector == Vector3.zero) { 
            state = State.Idle;
            return;
        }

        // calculates the rotation matrix for isometric camera
        Matrix4x4 isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, virtualCamera.transform.eulerAngles.y, 0));
        Quaternion lookDir = Quaternion.LookRotation(isoMatrix.MultiplyPoint3x4(inputVector), Vector3.up);

        float moveDistance = moveSpeed * Time.deltaTime;

        transform.rotation = Quaternion.Slerp(transform.rotation, lookDir, Time.deltaTime * rotateSpeed);
        transform.position += moveDistance * transform.forward;

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

    public Weapon GetWeapon() => weapon;
}
