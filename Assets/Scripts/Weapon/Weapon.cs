using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] public float pos1y;
    [SerializeField] public float pos2x;
    [SerializeField] public float pos2y;
    [SerializeField] public float pos3y;
    [SerializeField] public float pos4x;
    [SerializeField] public float pos4y;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 2;
    [SerializeField] private float attackCooldown = .4f;
    [SerializeField] LayerMask enemyLayer;


    void OnDrawGizmosSelected()
    {
        Vector3 pos1 = transform.position + transform.TransformDirection(Vector3.up) * pos1y;
        Vector3 pos2 = transform.position + transform.TransformDirection(Vector3.right) * pos2x + transform.TransformDirection(Vector3.up) * pos2y;

        Vector3 pos3 = transform.position + transform.TransformDirection(Vector3.up) * pos3y;
        Vector3 pos4 = transform.position + transform.TransformDirection(Vector3.right) * pos4x + transform.TransformDirection(Vector3.up) * pos4y;

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(pos1, .1f);
        Gizmos.DrawSphere(pos2, .1f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(pos3, .1f);
        Gizmos.DrawSphere(pos4, .1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    public Transform GetAttackPoint() => attackPoint;
    public float GetAttackRadius() => attackRadius;
    public LayerMask GetEnemyLayer() => enemyLayer;

    public float GetAttackCooldown() => attackCooldown;
}
