using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemy : Enemy
{
    [Header("Target Settings")]
    [Tooltip("The Transform of the player.")]
    public Transform TargetPlayer;

    [Header("Cover and Patrol")]
    [SerializeField] [Tooltip("Possible cover points for the enemy.")]
    private CoverPoint[] coverPoints;
    [SerializeField] [Tooltip("Patrol points for enemy movement.")]
    private Transform[] PatrolPoints;
    [SerializeField] [Tooltip("Time to wait at each patrol point.")]
    private float patrolWaitTime = 3f;
    private int currentPatrolIndex = 0;
    private bool switchingPatrol = false;

    [Header("Attack Parameters")]
    [Tooltip("Attack range of the enemy.")]
    public float AttackRange = 10f;
    [Tooltip("Damage inflicted by the enemy.")]
    public float Damage = 10.0f;
    [Tooltip("Time between enemy attacks.")]
    public float TimeBetweenAttacks = 1.0f;
    [SerializeField] [Tooltip("Minimum distance from the player.")]
    private float MinPlayerDistance = 5f;
    [SerializeField] [Tooltip("Minimum height of obstacles for cover.")]
    private float MinObstacleHeight = 1.25f;
    [SerializeField] [Tooltip("How far the enemy can see to decide on attacking.")]
    private float lookRadiusHiding = 50f;

    [Header("AI Settings")]
    [SerializeField] [Tooltip("The NavMeshAgent for the enemy.")]
    private NavMeshAgent Agent;
    [SerializeField] [Tooltip("Enemy layers to consider for AI decisions.")]
    private LayerMask EnemyLayers;
    [SerializeField] [Tooltip("Speed when patrolling.")]
    private float patrolWalkSpeed = 3.5f;
    [SerializeField] [Tooltip("Speed when attacking.")]
    private float attackRunSpeed = 4.5f;
    [SerializeField] [Tooltip("How often to update the slow update loop.")]
    private float slowUpdateTime = 0.5f;

    [Header("Attack Cooldown")]
    [SerializeField] [Tooltip("Range of random cooldowns between attacks.")]
    private Range<float, float> AttackCooldownRange = new Range<float, float>(1.0f, 2.0f);
    private float CurrentAttackCooldown = 0f;
    private float AttackCooldownTimer = 0f;
    private bool CanAttack = false;

    [Header("Visual Effects")]
    [SerializeField] [Tooltip("Line renderer for showing the warning line.")]
    private LineRenderer WarningLineRenderer;
    [SerializeField] [Tooltip("Particle system for laser attacks.")]
    private ParticleSystem LaserParticles;
    [SerializeField] [Tooltip("Particle system for impact effect.")]
    private ParticleSystem impactParticleSystem;
    [Tooltip("Time for the warning effect before an attack.")]
    public float WarningTime = 0.5f;

    [Header("Laser Direction and Speed")]
    [Tooltip("Rotation speed of the laser.")]
    public float rotationSpeed = 2f;
    [SerializeField] [Tooltip("Initial direction of the laser.")]
    private Vector3 initialLaserDirection;
    [SerializeField] [Tooltip("Current direction of the laser.")]
    private Vector3 currentLaserDirection;
    [SerializeField] [Tooltip("Flag to check if initial laser direction is set.")]
    private bool initialDirectionSet = false;
    [Tooltip("Speed of lerp for laser aiming.")]
    public float lerpSpeed = 0.1f;

    [Header("Animation")]
    [Tooltip("Animator for the player.")]
    public Animator PlayerAnimator;

    [Header("State Management")]
    [Tooltip("Current state of the enemy.")]
    public EnemyState currentState;

    [Header("Additional Components")]
    [SerializeField] [Tooltip("Line renderer for the laser attack.")]
    private LineRenderer LaserLineRenderer;
    [SerializeField] [Tooltip("Animator for the enemy.")]
    private Animator anim;
    [SerializeField] [Tooltip("Layers considered as player.")]
    private LayerMask PlayerLayers;
    [SerializeField] [Tooltip("Point from which the laser fires.")]
    private Transform LaserFirePoint;

    private bool isAttacking;
    private bool AttackStarted = false;
    private float lastAttackTime = -1;
    private float lastSpottedTime = 0f;
    private float LoseInterestTimer = 8f;
    private bool moveToNextPatrolPoint = true;

    // Nested class for Range
    public struct Range<T, U>
    {
        public T Lower;
        public U Upper;
        public Range(T first, U second)
        {
            Lower = first;
            Upper = second;
        }
    }

    // Enumeration for Enemy State
    public enum EnemyState { Idle, Attack }
    public override void Start()
    {
        base.Start();
        currentState = EnemyState.Idle;
        Invoke("SlowUpdate", slowUpdateTime);
        CurrentAttackCooldown = Random.Range(AttackCooldownRange.Lower, AttackCooldownRange.Upper);
        LaserParticles.Stop();
        LaserLineRenderer.enabled = false;
    }

    private void SlowUpdate()
    {
        if (gameObject.activeSelf == false) return;
        switch (currentState)
        {
            case EnemyState.Idle:
                IdleBehavior();
                break;
            case EnemyState.Attack:
                AttackBehavior();
                break;
        }
        Invoke("SlowUpdate", slowUpdateTime);
    }

    private void IdleBehavior()
    {
        TransitionToAttack();

        Agent.speed = patrolWalkSpeed;

        if (PatrolPoints.Length == 0) return;  // No patrol points defined

        if (moveToNextPatrolPoint)
        {
            Agent.SetDestination(PatrolPoints[currentPatrolIndex].position);
            moveToNextPatrolPoint = false;
            return; 
        }

        Debug.Log("Point Reached");
        // If the enemy is close to the current patrol point
        if (!switchingPatrol && Agent.velocity == Vector3.zero)
        {
            switchingPatrol = true;
            Invoke("SwitchPatrolPoint", patrolWaitTime);
        }
    }

    void SwitchPatrolPoint()
    {
        currentPatrolIndex = (currentPatrolIndex + 1) % PatrolPoints.Length;
        moveToNextPatrolPoint = true; // Set the flag to true here
        switchingPatrol = false;
    }

    public void TransitionToAttack()
    {
        if (IsPlayerInSight() && IsPlayerInRadius(lookRadiusHiding))
        {
            Invoke("AttackCooldown", CurrentAttackCooldown);
            currentState = EnemyState.Attack;
            Agent.isStopped = false;
        }
    }
    public bool IsPlayerInRadius(float radius)
    {
        return Vector3.Distance(transform.position, TargetPlayer.position) < radius;
    }
    bool IsPlayerInSight()
    {
        // check if there is LOS to the player
        RaycastHit hit;
        if (Physics.Raycast(transform.position, (TargetPlayer.position - transform.position).normalized, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject.layer == 7)
            {
                return true;
            }
        }

        return false;
    }
    private void AttackBehavior()
    {
        if (IsPlayerInSight())
        {
            lastSpottedTime = 0f;
        }
        else
        {
            lastSpottedTime += slowUpdateTime;
        }
        if(lastSpottedTime > LoseInterestTimer)
        {
            currentState = EnemyState.Idle;
        }
        // go back to idle if the player manages to get too far away
        if (Vector3.Distance(transform.position, TargetPlayer.position) > lookRadiusHiding)
        {
            currentState = EnemyState.Idle;
            Agent.isStopped = true;
            return;
        }
        Agent.speed = attackRunSpeed;
        if (Vector3.Distance(transform.position, TargetPlayer.position) > AttackRange)
        {
            CanAttack = false;
        }
        else
        {
            CanAttack = true;
        }
        if (!isAttacking)
        {
            Agent.isStopped = false;
            FindCover(TargetPlayer.position + 0.5f * Vector3.up);
        }
        else if(isAttacking && !AttackStarted && CanAttack)
        {
            StartCoroutine(PeekAndAttack());
        }
    }


    void FaceTarget()
    {
        Vector3 direction = (TargetPlayer.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }
        private IEnumerator PeekAndAttack()
        {
            AttackStarted = true;

            RaycastHit hit;
            Physics.Linecast(transform.position, TargetPlayer.position, out hit);

            while (hit.collider.gameObject.layer != 7)
            {
                FaceTarget();
                Physics.Linecast(transform.position, TargetPlayer.position, out hit);
                Peek();
                yield return null;
            }
            Agent.isStopped = true;
            anim.SetTrigger("Attack");
            yield return new WaitForSeconds(0.75f);
            Vector3 updatedDirection = (TargetPlayer.position - transform.position).normalized;
            yield return new WaitForSeconds(0.25f);
            impactParticleSystem.Play();
            // Enable the main attack laser and perform the attack
            LaserLineRenderer.enabled = true;
            CameraManager.Instance.CameraShake(WarningTime, 5f);
            float timer = 0;
            while(timer < WarningTime)
            {
                FaceTarget();
                timer += Time.deltaTime;
                LaserAttack(updatedDirection); // Pass the updated direction to LaserAttack
                yield return null;
            }

            // reset attack
            LaserLineRenderer.enabled = false;
            AttackStarted = false;
            isAttacking = false;
            initialDirectionSet = false;
            CurrentAttackCooldown = Random.Range(AttackCooldownRange.Lower, AttackCooldownRange.Upper);
            impactParticleSystem.Stop();
            Invoke("AttackCooldown", CurrentAttackCooldown);
        }

        private void LaserAttack(Vector3 initalDirection)
        {
            Vector3 targetDirection = (TargetPlayer.position - transform.position).normalized;

            // Set initial direction the first time the attack is initiated
            if(!initialDirectionSet)
            {
                initialLaserDirection = initalDirection;
                currentLaserDirection = initialLaserDirection;
                initialDirectionSet = true;
            }
            else
            {
                // Lerp the direction based on the initial direction
                currentLaserDirection = Vector3.Lerp(currentLaserDirection, targetDirection, lerpSpeed);
                currentLaserDirection = new Vector3(currentLaserDirection.x, 0, currentLaserDirection.z);
            }

            RaycastHit hit;
            Debug.Log(PlayerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Dash"));
            if (Player.Instance.invulnerableTimer < 0.5f)
            {
                if (Physics.Raycast(transform.position, currentLaserDirection, out hit, Mathf.Infinity) )
                {
                    impactParticleSystem.transform.position = hit.point;
                    impactParticleSystem.transform.forward = -currentLaserDirection.normalized;
                    if (hit.collider.gameObject == TargetPlayer.gameObject)
                    {
                        GameEventsManager.Instance.PlayerEvents.TakeDamage((int)damage, transform.position);
                    }
                
                    LaserLineRenderer.SetPosition(0, LaserFirePoint.position);
                    LaserLineRenderer.SetPosition(1, hit.point);
                }
            }
            else
            {
                if (Physics.Raycast(transform.position, currentLaserDirection, out hit, Mathf.Infinity, ~PlayerLayers) )
                {
                    impactParticleSystem.transform.position = hit.point;
                    impactParticleSystem.transform.forward = -currentLaserDirection.normalized;
                    LaserLineRenderer.SetPosition(0, LaserFirePoint.position);
                    LaserLineRenderer.SetPosition(1, hit.point);
                }
            }
            
            
        }
    public void AttackCooldown()
    {
        isAttacking = true;
    }
    
    private void Peek()
    {
        Agent.SetDestination(TargetPlayer.position);
    }
    
    public void FindCover(Vector3 threatPosition)
    {
        CoverPoint bestCover = null;
        float closestDistance = float.MaxValue;

        foreach (var cover in coverPoints)
        {
            if (cover.IsCoverSafe(threatPosition, EnemyLayers, TargetPlayer.gameObject))
            {
                float distance = Vector3.Distance(transform.position, cover.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    bestCover = cover;
                }
            }
        }
        
        if (bestCover != null)
        {
            Agent.SetDestination(bestCover.transform.position);
        }
    }
    // draw gizmos for radii
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, lookRadiusHiding);
    }
}