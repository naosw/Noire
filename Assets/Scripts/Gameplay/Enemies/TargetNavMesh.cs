using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.AI;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class TargetNavMesh : MonoBehaviour
{
    [SerializeField] Transform NavMeshTarget;
    NavMeshAgent agent;

    public float attackDistance = 30f;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        FaceTarget();
        if(Vector3.Distance(NavMeshTarget.position, transform.position) >= attackDistance)
            agent.SetDestination(NavMeshTarget.position);
        else
        {
            agent.SetDestination(transform.position);
        }
    }
    void FaceTarget()
    {
        Vector3 direction = (NavMeshTarget.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 15);
    }
}
