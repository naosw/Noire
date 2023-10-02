using System;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInteract))]
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour, IDataPersistence
{
    [Header("Fields")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    public static Player Instance { get; private set; }
    private Animator animator;
    private PlayerInteract playerInteract; 
        
    private readonly Quaternion rightRotation = Quaternion.Euler(new Vector3(0, 90, 0));
    
    [Header("Player Controller")]
    [SerializeField] private float moveSpeed = 12f;
    private PlayerState state;

    private CharacterController controller;
    private Vector3 moveDir;
    
    [Header("Player combat")]
    [SerializeField] private Weapon weapon;
    [SerializeField] private AbilitySO[] playerAbilitiesList; // up to three abilities is currently supported
    private Dictionary<int, AbilitySO> playerAbilities;
    private float playerHitBoxHeight = 1f;

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
    
    // ***************************** EVENT FUNCTIONS ***************************** //
    
    private void Awake()
    {
        state = PlayerState.Idle;
        dreamState = DreamState.Neutral;
        
        Instance = this;
        
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        playerInteract = GetComponent<PlayerInteract>();
        
        playerHealthSO.ResetHealth(); // resets to 50% HP as default
        
        dreamShardsSO.SetCurrencyCount(0);
        dreamThreadsSO.SetCurrencyCount(0);
        
        // convert ability list to lookup dictionary
        UpdateAbilities();
    }

    private void Start()
    {
        GameInput.Instance.OnInteract += GameInput_OnInteract;
        GameInput.Instance.OnAbilityCast += GameInput_OnAbilityCast;
        GameEventsManager.Instance.playerEvents.OnTakeDamage += OnTakingDamage;
        GameEventsManager.Instance.playerEvents.OnDreamShardsChange += dreamShardsSO.Change;
        GameEventsManager.Instance.playerEvents.OnDreamThreadsChange += dreamThreadsSO.Change;
    }

    private void OnDestroy()
    {
        GameInput.Instance.OnInteract -= GameInput_OnInteract;
        GameInput.Instance.OnAbilityCast -= GameInput_OnAbilityCast;
        GameEventsManager.Instance.playerEvents.OnTakeDamage -= OnTakingDamage;
        GameEventsManager.Instance.playerEvents.OnDreamShardsChange -= dreamShardsSO.Change;
        GameEventsManager.Instance.playerEvents.OnDreamThreadsChange -= dreamThreadsSO.Change;
    }
    
    private void Update()
    {
        if (IsDead())
            return;
        
        HandleDrowsiness();
        HandleAbilityCast();
        
        if(CanCastAbility())
            HandleMovement();
    }

    // ***************************** TRIGGER FUNCTIONS ***************************** //
    
    private void GameInput_OnInteract()
    {
        playerInteract.Interact();
    }
    
    private void GameInput_OnAbilityCast(object sender, GameInput.OnAbilityCastArgs e)
    {
        if (CanCastAbility())
        {
            
            if (playerAbilities.TryGetValue(e.abilityID, out AbilitySO ability)){
                bool status = ability.Activate();
                if (status)
                    state = PlayerState.Casting;
            }
            else
            {
                Debug.Log("Ability not available");
            }
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
    private void UpdateAbilities()
    {
        playerAbilities = new Dictionary<int, AbilitySO>();
        foreach (AbilitySO ability in playerAbilitiesList)
        {
            if (ability.applicableDreamStates.Contains(dreamState))
            {
                playerAbilities.Add(ability.abilityID, ability);
            }
        }
    }
    
    // called after ending attacks/dashing for state transition
    public void ResetStateAfterAction()
    {
        state = GameInput.Instance.GetMovementVectorNormalized() != Vector3.zero 
            ? PlayerState.Walk 
            : PlayerState.Idle;
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

    public void SetAnimatorTrigger(string triggerName)
    {
        animator.SetTrigger(triggerName);
    }
    
    // move towards `moveDir` with speed
    public void Move(float speed)
    {
        Vector3 moveDist = speed * Time.deltaTime * moveDir;
        
        // snap to terrain
        float terrainHeight = Terrain.activeTerrain.SampleHeight(transform.position + moveDist) + .1f;
        moveDist.y += terrainHeight - transform.position.y;
        
        controller.Move(moveDist);
        
        // rotation
        transform.forward = new Vector3(moveDir.x, 0, moveDir.z);
    }
    
    // called when player is either moving or idle
    private void HandleMovement()
    {
        Vector3 inputVector = GameInput.Instance.GetMovementVectorNormalized();
        if (inputVector == Vector3.zero)
        {
            state = PlayerState.Idle;
            return;
        }
        
        // calculates orthographic camera angle
        Vector3 forward = virtualCamera.transform.forward;
        forward.y = 0;
        Vector3 right = rightRotation * forward;
        forward *= inputVector.z;
        right *= inputVector.x;
        moveDir = (forward + right).normalized;
        
        // move
        state = PlayerState.Walk;
        Move(moveSpeed);
    }
    
    // called every frame to decrease cooldown and handle ability states
    private void HandleAbilityCast()
    {
        foreach (AbilitySO ability in playerAbilities.Values)
            ability.Continue();
    }
    
    
    // called after attacks
    public void HandleAttackOnHitEffects()
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
        DreamState prevDreamState = dreamState;
        
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
        UpdateAbilities();
    }
    
    // called when drowsiness == 0
    // TODO: currency drops
    // TODO: reset to save points
    private void HandleDeath()
    {
        state = PlayerState.Dead;
        Loader.Load(Loader.Scene.DeathScene);
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
        data.dreamShards = dreamShardsSO.GetCurrencyCount();
        data.dreamThreads = dreamThreadsSO.GetCurrencyCount();
        data.position = transform.position;
    }
    
    // ***************************** GETTERS/SETTERS ***************************** //
    
    public bool IsWalking() => state == PlayerState.Walk;
    public bool IsIdle() => state == PlayerState.Idle;
    public bool IsCasting() => state == PlayerState.Casting;
    public bool IsDead() => state == PlayerState.Dead;
    public bool CanCastAbility() => IsIdle() || IsWalking();
    public float GetPlayerHitBoxHeight() => playerHitBoxHeight;
    public Weapon GetWeapon() => weapon;
}
