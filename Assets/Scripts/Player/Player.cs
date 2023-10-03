using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static UnityEditor.PlayerSettings;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour, IDataPersistence
{
    [Header("Fields")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    public static Player Instance { get; private set; }
    private Animator animator;
    private readonly Quaternion rightRotation = Quaternion.Euler(new Vector3(0, 90, 0));
    
    private const string DASH = "Dash";
    private const string ATTACK1 = "Attack1";
    
    [Header("Player Controller")]
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private float rotateSpeed = 30f;
    [SerializeField] private float dashSpeed = 200;
    [SerializeField] private float dashFalloff = 400;
    [SerializeField] private float dashCooldown = 0.5f;
    private float currentDashSpeed;
    
    private State state;
    private enum State
    {
        Idle,
        Walk,
        Attack1,
        Dash,
        Dead
    }

    private CharacterController controller;
    private float dashCooldownCounter;
    private Vector3 moveDir;
    private Vector3 lastInteractDir;
    
    [Header("Player combat")]
    [SerializeField] private Weapon weapon;
    private float playerHitBoxHeight = 1f;
    private float attack1Cooldown; // weapon determines this field
    private float attack1CooldownCounter;
    private float attackDuration = .25f;
    private float attackDamage = 5;


    [Header("Player Health")]
    [SerializeField] private PlayerHealthSO playerHealthSO;
    [SerializeField] private float bufferDecreaseRate = 8f;
    [SerializeField] private float maxRegenHitCooldown = 2.5f;
    [SerializeField] private float playerHitIFrames = 1f;
    private float currentBufferCooldown = 0f;
    private bool bufferOnCooldown = false;
    private float currentIFrameTimer = 0f;
    public event UnityAction UpdateHealthBar;
    
    [Header("Player Stats")] 
    [SerializeField] private PlayerStatisticsSO dreamShardsSO;
    [SerializeField] private PlayerStatisticsSO dreamThreadsSO;

    [Header("Player Audio")] 
    [SerializeField] private PlayerAudio playerAudio;
    // TODO: modify PlayerAudio.cs
    
    [Header("Player Dream State")]
    [SerializeField] [Range(0,.5f)] private float lucidThreshold;
    [SerializeField] [Range(.5f,1)] private float deepThreshold;
    private DreamState dreamState;
    private enum DreamState
    {
        Neutral,
        Lucid,
        Deep
    }
    
    // ***************************** EVENT FUNCTIONS ***************************** //
    
    private void Awake()
    {
        state = State.Idle;
        dreamState = DreamState.Neutral;
        
        Instance = this;
        
        animator = GetComponent<Animator>();
        
        attack1Cooldown = weapon.GetAttackCooldown();
        attack1CooldownCounter = attack1Cooldown;

        dashCooldownCounter = dashCooldown;
        
        playerHealthSO.ResetHealth(); // resets to 50% HP as default
        
        controller = GetComponent<CharacterController>();
        
        dreamShardsSO.SetCurrencyCount(0);
        dreamThreadsSO.SetCurrencyCount(0);
    }

    private void Start()
    {
        GameInput.Instance.OnAttack1 += GameInput_OnAttack1;
        GameInput.Instance.OnDash += GameInput_OnDash;
        GameEventsManager.Instance.playerEvents.OnTakeDamage += OnTakingDamage;
        GameEventsManager.Instance.playerEvents.OnDreamShardsChange += dreamShardsSO.Change;
        GameEventsManager.Instance.playerEvents.OnDreamThreadsChange += dreamThreadsSO.Change;
    }

    private void OnDestroy()
    {
        GameInput.Instance.OnAttack1 -= GameInput_OnAttack1;
        GameInput.Instance.OnDash -= GameInput_OnDash;
        GameEventsManager.Instance.playerEvents.OnTakeDamage -= OnTakingDamage;
        GameEventsManager.Instance.playerEvents.OnDreamShardsChange -= dreamShardsSO.Change;
        GameEventsManager.Instance.playerEvents.OnDreamThreadsChange -= dreamThreadsSO.Change;
    }
    
    private void Update()
    {
        if (IsDead())
            return;
        
        attack1CooldownCounter -= Time.deltaTime;
        dashCooldownCounter -= Time.deltaTime;
        
        HandleDrowsiness();
        if (IsIdle() || IsWalking())
            HandleMovement();

        if (IsDashing())
            HandleDash();
    }

    // ***************************** TRIGGER FUNCTIONS ***************************** //
    
    private void GameInput_OnAttack1(object sender, System.EventArgs e)
    {
        // handles attack with coroutines
        if (!IsAttacking1() && attack1CooldownCounter <= 0)
        {
            animator.SetTrigger(ATTACK1);
            attack1CooldownCounter = attack1Cooldown;
            state = State.Attack1;
            HandleAttackOnHitEffects();
            StartCoroutine(HandleAttack1Duration());
        }
    }

    private void GameInput_OnDash(object sender, System.EventArgs e)
    {
        // handles by setting state -> later handled in Update()
        if (!IsDashing() && dashCooldownCounter <= 0)
        {
            animator.SetTrigger(DASH);
            currentDashSpeed = dashSpeed;
            dashCooldownCounter = dashCooldown;
            state = State.Dash;
        }
    }
    
    // called when taking any damage
    private void OnTakingDamage(float bufferDamage)
    {
        if (IsDead())
            Debug.LogError("Cannot take dmg if dead. This should not happen -- should've handled death earlier");
        if (currentIFrameTimer <= playerHitIFrames)
            return;
        
        currentBufferCooldown = maxRegenHitCooldown;
        
        playerHealthSO.InflictDamage(bufferDamage);
        UpdateHealthBar?.Invoke();
        HandleDreamState();

        if (playerHealthSO.IsDead())
            HandleDeath();
    }
    
    // ***************************** HANDLE FUNCTIONS ***************************** //
    
    // called after ending attacks/dashing for state transition
    private void ResetStateAfterAction()
    {
        if (GameInput.Instance.GetMovementVectorNormalized() != Vector3.zero)
            state = State.Walk;
        else
            state = State.Idle;
    }
    
    // TODO: move this to playeraudio.cs
    public void PlaySteps(string path)
    {
        FMODUnity.RuntimeManager.PlayOneShot(path, GetComponent<Transform>().position);
    }
    
    // TODO: move this to playeraudio.cs
    public void PlaySwoosh(string path)
    {
        FMODUnity.RuntimeManager.PlayOneShot(path, GetComponent<Transform>().position);
    }
    
    // called when player is either moving or idle
    private void HandleMovement()
    {
        Vector3 inputVector = GameInput.Instance.GetMovementVectorNormalized();
        if (inputVector == Vector3.zero)
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
        
        moveDir = moveSpeed * Time.deltaTime * (forward + right).normalized;
        
        // snap to terrain
        float terrainHeight = Terrain.activeTerrain.SampleHeight(transform.position + moveDir) + .1f;
        moveDir.y += terrainHeight - transform.position.y;
        
        if (moveDir != Vector3.zero)
        {
            transform.forward = Vector3.Slerp(transform.forward, new Vector3(moveDir.x, 0, moveDir.z), Time.deltaTime * rotateSpeed);
            state = State.Walk;
        }
        
        controller.Move(moveDir);
    }
    
    // subroutine used by attack to simulate cooldown
    private IEnumerator HandleAttack1Duration()
    {
        yield return new WaitForSeconds(attackDuration);
        ResetStateAfterAction();
    }
    
    // called after dashing
    private void HandleDash()
    {
        if (currentDashSpeed > 0)
        {
            currentDashSpeed -= dashFalloff * Time.deltaTime;
            if (currentDashSpeed < moveSpeed)
            {
                currentDashSpeed = 0;
                ResetStateAfterAction();
                return;
            }
            
            Vector3 dashMove = currentDashSpeed * Time.deltaTime * moveDir;
            
            float terrainHeight = Terrain.activeTerrain.SampleHeight(transform.position + dashMove) + .1f;
            dashMove.y += terrainHeight - transform.position.y;
            
            controller.Move(dashMove);
        }
    }
    
    // called after attacks
    private void HandleAttackOnHitEffects()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(weapon.GetAttackPoint().position, weapon.GetAttackRadius(), weapon.GetEnemyLayer());
        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().OnHit();
        }
    }

    // called on every frame for buffer regen
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
            UpdateHealthBar?.Invoke();
            playerHealthSO.RegenBuffer(bufferDecreaseRate * Time.deltaTime);
        }
        
        // TODO: remove this later since it is never triggered, should only trigger death in HandleHit()
        if (playerHealthSO.IsDead())
            HandleDeath();
    }
    
    // called upon changing HP
    private void HandleDreamState()
    {
        var prevDreamState = dreamState;
        
        float currentDrowsinessPercentage = playerHealthSO.GetCurrentDrowsinessPercentage;
        if (currentDrowsinessPercentage <= lucidThreshold)
            dreamState = DreamState.Lucid;
        else if (currentDrowsinessPercentage >= deepThreshold)
            dreamState = DreamState.Deep;
        else
            dreamState = DreamState.Neutral;

        if (prevDreamState != dreamState)
            DreamStateTransition(prevDreamState);
    }
    
    // called when transitioning between dream states
    private void DreamStateTransition(DreamState prevDreamState)
    {
        // TODO: add visual effects, change enemy style, change world settings, etc
        return;
    }
    
    // called when drowsiness == 0
    // TODO: currency drops
    // TODO: reset to save points
    private void HandleDeath()
    {
        state = State.Dead;
        LoaderStatic.Load(GameScene.DeathScene);
    }
    
    // called when restoring drowsiness (hp)
    public int RegenDrowsiness(float value)
    {
        if(playerHealthSO.RegenHealth(value) == 1){
            HandleDreamState();
            return 1;
        }
        return 0;
    }
    
    // ***************************** IDataPersistence methods ***************************** //
    
    /** IMPORTANT DEBUGGING INFORMATION:
     * if you get an error saying loading error or something in main scene,
     * please DISABLE DataPersistenceManager in scene/Globals or toggle on "Disable Data Persistence"
    */
    
    public void LoadData(GameData data)
    {
        // data.currentScene should have already been loaded.   
        playerHealthSO.SetCurrentDrowsiness(data.drowsiness);
        attackDamage = data.attackDamage;
        dreamShardsSO.SetCurrencyCount(data.dreamShards);
        dreamThreadsSO.SetCurrencyCount(data.dreamThreads);
        transform.position = data.position;
    }


    public void SaveData(GameData data)
    {
        // IMPORTANT: here we need to save the current scene, 
        // which was the last `targetScene` the loader had loaded
        data.currentScene = SceneManager.GetActiveScene().name;
        
        data.drowsiness = playerHealthSO.GetCurrentDrowsiness;
        data.attackDamage = attackDamage;
        data.dreamShards = dreamShardsSO.GetCurrencyCount();
        data.dreamThreads = dreamThreadsSO.GetCurrencyCount();
        data.position = transform.position;
    }
    
    // ***************************** GETTERS/SETTERS ***************************** //
    
    public bool IsDashing() => state == State.Dash;
    public bool IsWalking() => state == State.Walk;
    public bool IsIdle() => state == State.Idle;
    public bool IsAttacking1() => state == State.Attack1;
    public bool IsDead() => state == State.Dead;
    public float GetPlayerHitBoxHeight() => playerHitBoxHeight;
    public Weapon GetWeapon() => weapon;
}
