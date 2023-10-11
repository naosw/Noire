using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "PlayerDashAbility", menuName = "Abilities/PlayerDash")]
public class PlayerDashAbilitySO : AbilitySO
{
    [Header("Ability-specific Fields")]
    [SerializeField] private float dashSpeed = 30;
    [SerializeField] private float dashTime = 1;
    
    private float dashTimeCounter;

    protected override void Initialize()
    {
        dashTimeCounter = dashTime;
        Player.Instance.SetAnimatorTrigger(abilityAnimationTrigger);
    }

    protected override void Cast()
    {
        if(dashTimeCounter < 0)
            Finish();
        else
        {
            dashTimeCounter -= Time.deltaTime;
            Player.Instance.Move(dashSpeed);
        }
    }
    
    protected override void Finish()
    {
        dashTimeCounter = dashTime;
        state = AbilityState.OnCooldown;
        Player.Instance.ResetStateAfterAction();
    }
}
