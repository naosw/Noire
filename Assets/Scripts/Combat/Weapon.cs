using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] public Transform p1_bot;
    [SerializeField] public Transform p2_top;
    [SerializeField] public Transform attackPoint;
    [SerializeField] private float attackRadius = 2;
    [SerializeField] private float attackCooldown = .4f;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] private float attackDamage = 10;

    public Transform GetAttackPoint() => attackPoint;
    public float GetAttackRadius() => attackRadius;
    public LayerMask GetEnemyLayer() => enemyLayer;
    public float GetAttackCooldown() => attackCooldown;
    public float GetAttackDamage() => attackDamage;
}