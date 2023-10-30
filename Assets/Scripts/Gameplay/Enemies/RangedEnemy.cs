using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemy : Enemy
{
    public Transform TargetPlayer;
    [SerializeField] private CoverPoint[] coverPoints;
    public float AttackRange = 10;
    public float Damage = 10.0f;
    public float TimeBetweenAttacks = 1.0f;
    public LayerMask EnemyLayers;
    public NavMeshAgent Agent;
    public float HideSensitivity = 0;
    public float MinPlayerDistance = 5f;
    public float MinObstacleHeight = 1.25f;
    public float lookRadiusHiding = 50f;
    public float lastAttackTime = -1;
    public bool isAttacking;
    public bool AttackStarted = false;
    public float slowUpdateTime = 0.5f;
    public Range<float, float> AttackCooldownRange = new Range<float, float>(1.0f, 2.0f);
    public float CurrentAttackCooldown = 0f;
    public float AttackCooldownTimer = 0f;
    public bool CanAttack = false;
    public Transform[] PatrolPoints;
    public int currentPatrolIndex = 0;
    public float patrolWaitTime = 0.5f;
    public bool switchingPatrol = false;
    public LineRenderer WarningLineRenderer;
    public ParticleSystem LaserParticles;
    [SerializeField] private ParticleSystem impactParticleSystem;
    public float WarningTime = 0.5f;
    public Animator PlayerAnimator;
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
    public enum EnemyState { Idle, Attack }
    public EnemyState currentState;
    public LineRenderer LaserLineRenderer;
    public float patrolWalkSpeed = 3.5f;
    public float attackRunSpeed = 4.5f;
    public Animator anim;
    public LayerMask PlayerLayers;
    public Transform LaserFirePoint;
    public float lastSpottedTime = 0f;
    public float LoseInterestTimer = 8f;
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

    public bool moveToNextPatrolPoint = true;

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

    public float rotationSpeed = 2f;
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

        private Vector3 initialLaserDirection;
        private Vector3 currentLaserDirection;
        private bool initialDirectionSet = false;
        public float lerpSpeed = 0.1f; // Adjust this value to control the speed of lerp

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