using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyAI : Enemy
{
    public float attackRadius = 2.0f;
    public float rotationSpeed = 5f;
    public int damage = 10;
    
    private float circlingTime = 0.0f;
    [SerializeField] float[] changeDirectionTime = new float[2];
    private int circlingDirection = 1;
    private float currentChangeTime;
    
    [SerializeField] float[] attackCooldown = new float[2];
    [SerializeField] private float currentAttackCooldown;
    [SerializeField] private bool canAttack = true;
    [SerializeField] private bool isAttacking = false;
    [SerializeField] float AttackTime = 2f;
    [SerializeField] private float attackDistance = 1.5f;
    public override void Start()
    {
        base.Start();
        currentChangeTime = Random.Range(changeDirectionTime[0], changeDirectionTime[1]);
    }

    public override void Idle()
    {
        if (IsPlayerInSight() && IsPlayerInRadius(lookRadius))
        {
            currentState = State.Attack;
            agent.isStopped = false;
        }
        else
        {
            base.Idle();
        }
    }

    public override void Search()
    {
        base.Search();

        searchTime += Time.deltaTime;

        if (IsPlayerInSight() && IsPlayerInRadius(lookRadius))
        {
            currentState = State.Attack;
            searchTime = 0;
        }
        else if (searchTime >= searchDuration || agent.remainingDistance < 0.5f)
        {
            currentState = State.Idle;
            searchTime = 0;
        }
    }

    public override void Attack()
    {
        base.Attack();

        if (IsPlayerInRadius(attackRadius))
        {
            if(!isAttacking)
                CircleAroundPlayer();
            // handle attacking
            if (canAttack)
            {
                StartCoroutine(AttackCooldown());
                StartCoroutine(DoAttack());
            }
        }
        else if (IsPlayerInRadius(lookRadius) && !IsPlayerInSight())
        {
            lastKnownPosition = target.position;
            currentState = State.Search;
            agent.isStopped = false;
        }
        else if (!IsPlayerInRadius(lookRadius))
        {
            currentState = State.Idle;
            agent.isStopped = true;
        }
        else
        {
            agent.SetDestination(target.position);
        }

        FaceTarget();
    }
    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(Random.Range(attackCooldown[0], attackCooldown[1])+ AttackTime);
        canAttack = true;
        Debug.Log("test");
    }

    IEnumerator DoAttack()
    {
        isAttacking = true;
        // move towards the target and then move away and set isAttacking to false
        float timer = 0f;
        while (timer <= AttackTime)
        {
            agent.SetDestination(target.position);
            timer += Time.deltaTime;
            yield return null;
        }

        if (Vector3.Distance(transform.position, target.position) < attackDistance)
        {
            GameEventsManager.Instance.PlayerEvents.TakeDamage(damage, transform.position);
        }

        agent.SetDestination(target.position - transform.forward * 2f);
        isAttacking = false;
    }
    void CircleAroundPlayer()
    {
        if(circlingTime >= currentChangeTime)
        {
            circlingDirection *= -1;
            currentChangeTime = Random.Range(changeDirectionTime[0], changeDirectionTime[1]);
            circlingTime = 0;
        }
        else
        {
            circlingTime += Time.deltaTime;
        }
        Vector3 toTarget = (target.position - transform.position).normalized;
        
        float angle = rotationSpeed * Time.deltaTime * circlingDirection;
        
        Quaternion rotation = Quaternion.Euler(0, angle, 0);
        
        Vector3 newDir = rotation * toTarget;
        
        Vector3 newPos = target.position - newDir * (attackRadius-1);
        
        agent.SetDestination(newPos);
    }
    bool IsPlayerInSight()
    {
        Vector3 rayDirection = target.position - transform.position;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, rayDirection, out hit, lookRadius, ~obstacleMask))
        {
            if (hit.collider.tag == "Player")
            {
                return true;
            }
        }

        return false;
    }

    bool IsPlayerInRadius(float radius)
    {
        return Vector3.Distance(target.position, transform.position) <= radius;
    }

    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

}