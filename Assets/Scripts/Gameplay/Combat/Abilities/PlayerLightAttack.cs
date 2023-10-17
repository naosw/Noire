using UnityEngine;

[CreateAssetMenu(fileName = "PlayerLightAttack", menuName = "Abilities/PlayerLightAttack")]
public class PlayerLightAttack : AbilitySO
{
    [SerializeField] private float attackDuration = 0.4f;
    
    protected override void Initialize()
    {
        Player.Instance.SetAnimatorTrigger(abilityAnimationTrigger);
    }
    
    protected override void Cast()
    {
        Debug.Log("CASTED attack");
        Player.Instance.HandleAttackOnHitEffects();
        Player.Instance.StartCoroutine(WaitEndOfAction(attackDuration));
    }
    
    protected override void Finish()
    {
        state = AbilityState.OnCooldown;
        Player.Instance.ResetStateAfterAction();
    }
}
