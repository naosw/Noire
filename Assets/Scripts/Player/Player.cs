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
public class Player : MonoBehaviour, IPlayer, IDataPersistence
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
    [SerializeField] private float invulnerableTimerMax = .5f;
    [SerializeField] private ParticleSystemBase onHitParticleEffects;
    private Dictionary<int, AbilitySO> playerAbilities;
    private readonly float playerHitBoxHeight = 1f;
    private float invulnerableTimer;
    private Material onhitMaterial;
    private Coroutine onHitCoroutine;

    [Header("Player Health/Stamina")]
    [SerializeField] private PlayerHealthSO playerHealthSO;
    [SerializeField] private PlayerStaminaSO playerStaminaSO;
    
    [Header("Player Stats")] 
    [SerializeField] private PlayerStatisticsSO dreamShardsSO;
    [SerializeField] private PlayerStatisticsSO dreamThreadsSO;
    
    [Header("Player Dream State")]
    [Range(0,.5f)] public readonly float LucidThreshold = 0.2f;
    [Range(.5f,1)] public readonly float DeepThreshold = 0.8f;
    public DreamState DreamState { get; private set; }

    [Header("Player Items")] 
    [SerializeField] private InventorySO playerInventory;
    
    #region IPlayer
    public bool IsWalking() => state == PlayerState.Walk;
    public bool IsIdle() => state == PlayerState.Idle;
    public bool IsCasting() => state == PlayerState.Casting;
    public bool IsDead() => state == PlayerState.Dead;

    public AbilitySO CanCastAbility(int abilityId)
    {
        bool hasAbility = playerAbilities.TryGetValue(abilityId, out AbilitySO ability);
        
        if ((IsIdle() || IsWalking())
            && hasAbility
            && playerStaminaSO.CurrentStamina >= ability.staminaCost)
        {
            return ability;
        }

        return null;
    }
    public float GetPlayerHitBoxHeight() => playerHitBoxHeight;
    public Weapon GetWeapon() => weapon;
    public bool AddItem(CollectableItemSO item) => playerInventory.Add(item);
    public bool RemoveItem(CollectableItemSO item) => playerInventory.Remove(item);
    #endregion
    
    #region EVENT FUNCTIONS
    
    private void Awake()
    {
        Instance = this;
        
        state = PlayerState.Idle;
        DreamState = DreamState.Neutral;
        
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        playerInteract = GetComponent<PlayerInteract>();
        
        playerHealthSO.ResetHealth();
        playerStaminaSO.ResetStamina();
        
        dreamShardsSO.SetCurrencyCount(0);
        dreamThreadsSO.SetCurrencyCount(0);
        
        playerInventory.Init();
        
        // convert ability list to lookup dictionary
        UpdateAbilities();
    }

    private void Start()
    {
        Shader.SetGlobalColor("_FullScreenVoronoiColor", StaticInfoObjects.Instance.VORONOI_INDICATOR[DreamState]);
        
        GameInput.Instance.OnInteract += OnInteract;
        GameInput.Instance.OnAbilityCast += OnAbilityCast;
        
        GameEventsManager.Instance.PlayerEvents.OnTakeDamage += OnTakingDamage;
        GameEventsManager.Instance.PlayerEvents.OnHealthRegen += OnRegenDrowsiness;
        GameEventsManager.Instance.PlayerEvents.OnDreamShardsChange += OnDreamShardsChange;
        GameEventsManager.Instance.PlayerEvents.OnDreamThreadsChange += OnDreamThreadsChange;
    }

    private void OnDestroy()
    {
        GameInput.Instance.OnInteract -= OnInteract;
        GameInput.Instance.OnAbilityCast -= OnAbilityCast;
        
        GameEventsManager.Instance.PlayerEvents.OnTakeDamage -= OnTakingDamage;
        GameEventsManager.Instance.PlayerEvents.OnHealthRegen -= OnRegenDrowsiness;
        GameEventsManager.Instance.PlayerEvents.OnDreamShardsChange -= OnDreamShardsChange;
        GameEventsManager.Instance.PlayerEvents.OnDreamThreadsChange -= OnDreamThreadsChange;
    }
    
    private void Update()
    {
        if (IsDead())
            return;
        
        HandleStamina();
        HandleAbilityCast();
        
        if(IsWalking() || IsIdle())
            HandleMovement();
    }
    #endregion

    #region TRIGGER FUNCTIONS
    private void OnInteract()
    {
        playerInteract.Interact();
    }
    
    private void OnAbilityCast(int abilityId)
    {
        var ability = CanCastAbility(abilityId);
        if (ability != null)
        {
            if (ability.Activate())
            {
                state = PlayerState.Casting;
                playerStaminaSO.UseStamina(ability.staminaCost);
                GameEventsManager.Instance.PlayerEvents.UpdateStaminaBar();
            }
            else
            {
                Debug.Log($"Ability {abilityId} not available, either on cooldown or locked");
            }
        }
        else
        {
            Debug.Log($"Ability cast {abilityId} failed, either already casting or not enough stamina");
        }
    }
    
    // called when taking any damage
    private void OnTakingDamage(float dmg, Vector3 source)
    {
        if (invulnerableTimer > 0)
        {
            Debug.Log("Invulnerable -- did not take hit: " + dmg);
            return;
        }

        // take damage
        invulnerableTimer = invulnerableTimerMax;
        playerHealthSO.InflictDamage(dmg);
        GameEventsManager.Instance.PlayerEvents.UpdateHealthBar();
        
        // play onhit effects (material change + animation)
        if (onHitCoroutine != null)
            StopCoroutine(onHitCoroutine);
        onHitCoroutine = StartCoroutine(PlayOnHitEffects(source));
        
        // handle effects
        HandleDreamState();
        if (playerHealthSO.IsDead())
            HandleDeath();
    }

    private IEnumerator PlayOnHitEffects(Vector3 source)
    {
        if (!onHitParticleEffects)
        {
            Debug.LogError("Did not find onHitParticleEffects. This may be intentional");
            yield return null;
        }
        else
        {
            onHitParticleEffects.transform.LookAt(source);
            onHitParticleEffects.Play();
            CameraManager.Instance.CameraShake(invulnerableTimerMax, 5f);
            yield return new WaitForSeconds(invulnerableTimerMax);
            onHitParticleEffects.Stop();
        }
        onHitCoroutine = null;
    }
    
    // called when restoring drowsiness (hp)
    private void OnRegenDrowsiness(float value)
    {
        if(playerHealthSO.RegenHealth(value))
        {
            HandleDreamState();
            GameEventsManager.Instance.PlayerEvents.UpdateHealthBar();
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
    
    #region HELPER SUBROUTINES
    
    private void UpdateAbilities()
    {
        playerAbilities = new Dictionary<int, AbilitySO>();
        foreach (AbilitySO ability in playerAbilitiesList)
        {
            if (ability.applicableDreamStates.Contains(DreamState))
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

    #endregion
    
    #region HANDLE FUNCTIONS
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
    private void HandleStamina()
    {
        if(invulnerableTimer > 0)
            invulnerableTimer -= Time.deltaTime;

        if (playerStaminaSO.RegenStamina())
            GameEventsManager.Instance.PlayerEvents.UpdateStaminaBar();
    }
    
    // called upon changing HP
    private void HandleDreamState()
    {
        DreamState prevDreamState = DreamState;
        
        float currentDrowsinessPercentage = playerHealthSO.CurrentDrowsinessPercentage;
        if (currentDrowsinessPercentage < LucidThreshold)
            DreamState = DreamState.Lucid;
        else if (currentDrowsinessPercentage > DeepThreshold)
            DreamState = DreamState.Deep;
        else
            DreamState = DreamState.Neutral;

        if (prevDreamState != DreamState)
            HandleDreamStateTransition(prevDreamState);
    }
    
    // called when transitioning between dream states
    private void HandleDreamStateTransition(DreamState prevDreamState)
    {
        UpdateAbilities();
        Shader.SetGlobalColor("_FullScreenVoronoiColor", StaticInfoObjects.Instance.VORONOI_INDICATOR[DreamState]);
    }
    
    // called when drowsiness == 0
    private void HandleDeath()
    {
        dreamShardsSO.OnDeath();
        dreamThreadsSO.OnDeath();
        GameEventsManager.Instance.PlayerEvents.DreamShardsChangeFinished();
        GameEventsManager.Instance.PlayerEvents.DreamThreadsChangeFinished();
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
        
        dreamShardsSO.SetCurrencyCount(data.DreamShards);
        dreamThreadsSO.SetCurrencyCount(data.DreamThreads);
        transform.position = data.Position;
        playerInventory.FromSerializedInventory(data.Inventory);
    }

    public void SaveData(GameData data)
    {
        // IMPORTANT: here we need to save the current scene, 
        // which was the last `targetScene` the loader had loaded
        data.CurrentScene = SceneManager.GetActiveScene().name;
        
        data.DreamShards = dreamShardsSO.GetCurrencyCount();
        data.DreamThreads = dreamThreadsSO.GetCurrencyCount();
        data.Position = transform.position;
        data.Inventory = playerInventory.ToSerializableInventory();
    }
    
    #endregion
}
