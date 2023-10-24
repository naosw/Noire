using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] public Transform attackPoint;
    [SerializeField] private float attackRadius = 3;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] private float attackDamage = 10;

    public Transform GetAttackPoint() => attackPoint;
    public float GetAttackRadius() => attackRadius;
    public LayerMask GetEnemyLayer() => enemyLayer;
    public float GetAttackDamage() => attackDamage;
}