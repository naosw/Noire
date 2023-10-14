using System;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

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
    [SerializeField] private AbilitySO[] playerAbilitiesList;  // up to three abilities is currently supported
    [SerializeField] private float invulnerableTimerMax = 1f;
    private Dictionary<int, AbilitySO> playerAbilities;
    private float playerHitBoxHeight = 1f;
    private float invulnerableTimer = 0f;

    [Header("Player Health/Stamina")]
    [SerializeField] private PlayerHealthSO playerHealthSO;
    [SerializeField] private PlayerStaminaSO playerStaminaSO;
    
    [Header("Player Stats")] 
    [SerializeField] private PlayerStatisticsSO dreamShardsSO;
    [SerializeField] private PlayerStatisticsSO dreamThreadsSO;
    
    [Header("Player Dream State")]
    [Range(0,.5f)] public readonly float LucidThreshold = 0.2f;
    [Range(.5f,1)] public readonly float DeepThreshold = 0.8f;
    private DreamState dreamState;

    [Header("Player Items")] 
    [SerializeField] private InventorySO playerInventory;
    
    #region EVENT FUNCTIONS
    
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
        
        playerInventory.Init();
        
        // convert ability list to lookup dictionary
        UpdateAbilities();
    }

    private void Start()
    {
        GameInput.Instance.OnInteract += GameInput_OnInteract;
        GameInput.Instance.OnAbilityCast += GameInput_OnAbilityCast;
        
        GameEventsManager.Instance.PlayerEvents.OnTakeDamage += OnTakingDamage;
        GameEventsManager.Instance.PlayerEvents.OnHealthRegen += RegenDrowsiness;
        GameEventsManager.Instance.PlayerEvents.OnDreamShardsChange += OnDreamShardsChange;
        GameEventsManager.Instance.PlayerEvents.OnDreamThreadsChange += OnDreamThreadsChange;
    }

    private void OnDestroy()
    {
        GameInput.Instance.OnInteract -= GameInput_OnInteract;
        GameInput.Instance.OnAbilityCast -= GameInput_OnAbilityCast;
        GameEventsManager.Instance.PlayerEvents.OnTakeDamage -= OnTakingDamage;
        GameEventsManager.Instance.PlayerEvents.OnHealthRegen -= RegenDrowsiness;
        GameEventsManager.Instance.PlayerEvents.OnDreamShardsChange -= OnDreamShardsChange;
        GameEventsManager.Instance.PlayerEvents.OnDreamThreadsChange -= OnDreamThreadsChange;
    }
    
    private void Update()
    {
        if (IsDead())
            return;
        
        HandleHealthAndStamina();
        HandleAbilityCast();
        
        if(CanCastAbility())
            HandleMovement();
    }
    #endregion

    #region TRIGGER FUNCTIONS
    private void GameInput_OnInteract()
    {
        playerInteract.Interact();
    }
    
    private void GameInput_OnAbilityCast(int abilityID)
    {
        if (CanCastAbility())
        {
            if (playerAbilities.TryGetValue(abilityID, out AbilitySO ability)){
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
    private void OnTakingDamage(float dmg)
    {
        if (invulnerableTimer > 0)
            return;
        
        invulnerableTimer = invulnerableTimerMax;
        playerHealthSO.InflictDamage(dmg);
        GameEventsManager.Instance.PlayerEvents.UpdateHealthBar();
        
        HandleDreamState();
        if (playerHealthSO.IsDead())
            HandleDeath();
    }
    
    // called when restoring drowsiness (hp)
    private void RegenDrowsiness(float value)
    {
        if(playerHealthSO.RegenHealth(value) == 1){
            HandleDreamState();
        }
        else
        {
            // should not decrease potion
        }
    }
    
    // handle when currency change occurs
    private void OnDreamShardsChange(float val)
    {
        dreamShardsSO.Change(val);
        GameEventsManager.Instance.PlayerEvents.DreamShardsChangeFinished();
    }
    
    private void OnDreamThreadsChange(float val)
    {
        dreamThreadsSO.Change(val);
        GameEventsManager.Instance.PlayerEvents.DreamThreadsChangeFinished();
    }
    #endregion
    
    #region HANDLE FUNCTIONS
    public bool AddItem(CollectableItemSO item) => playerInventory.Add(item);
    public bool RemoveItem(CollectableItemSO item) => playerInventory.Remove(item);
    
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
    private void HandleHealthAndStamina()
    {
        if(invulnerableTimer > 0)
            invulnerableTimer -= Time.deltaTime;

        if (playerStaminaSO.RegenStamina())
            GameEventsManager.Instance.PlayerEvents.UpdateHealthBar();
    }
    
    // called upon changing HP
    private void HandleDreamState()
    {
        DreamState prevDreamState = dreamState;
        
        float currentDrowsinessPercentage = playerHealthSO.GetCurrentDrowsinessPercentage;
        if (currentDrowsinessPercentage <= LucidThreshold)
            dreamState = DreamState.Lucid;
        else if (currentDrowsinessPercentage >= DeepThreshold)
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
        Loader.Load(GameScene.DeathScene);
    }
    #endregion
    
    #region IDataPersistence
    
    /** IMPORTANT DEBUGGING INFORMATION:
     * if you get an error saying loading error or something in main scene,
     * please DISABLE DataPersistenceManager in scene/Globals or toggle on "Disable Data Persistence"
    */
    
    public void LoadData(GameData data)
    {
        // data.currentScene should have already been loaded.   
        playerHealthSO.SetCurrentDrowsiness(data.Drowsiness);
        dreamShardsSO.SetCurrencyCount(data.DreamShards);
        dreamThreadsSO.SetCurrencyCount(data.DreamThreads);
        transform.position = data.Position;
    }


    public void SaveData(GameData data)
    {
        // IMPORTANT: here we need to save the current scene, 
        // which was the last `targetScene` the loader had loaded
        data.CurrentScene = SceneManager.GetActiveScene().name;
        
        data.Drowsiness = playerHealthSO.GetCurrentDrowsiness;
        data.DreamShards = dreamShardsSO.GetCurrencyCount();
        data.DreamThreads = dreamThreadsSO.GetCurrencyCount();
        data.Position = transform.position;
    }
    #endregion
    
    #region GETTERS AND SETTERS
    public bool IsWalking() => state == PlayerState.Walk;
    public bool IsIdle() => state == PlayerState.Idle;
    public bool IsCasting() => state == PlayerState.Casting;
    public bool IsDead() => state == PlayerState.Dead;
    public bool CanCastAbility() => IsIdle() || IsWalking();
    public float GetPlayerHitBoxHeight() => playerHitBoxHeight;
    public Weapon GetWeapon() => weapon;
    #endregion
}
