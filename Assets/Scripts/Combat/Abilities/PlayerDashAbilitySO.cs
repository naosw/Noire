using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDashAbility", menuName = "Abilities/PlayerDash")]
public class PlayerDashAbilitySO : AbilitySO
{
    [Header("Ability-specific Fields")]
    [SerializeField] private float dashSpeed = 30;
    [SerializeField] private float dashFalloff = 50;
    [SerializeField] private float movespeedThreshold = 12;
    
    private float currentDashSpeed;

    protected override void Initialize()
    {
        currentDashSpeed = dashSpeed;
        Player.Instance.SetAnimatorTrigger(abilityAnimationTrigger);
    }

    protected override void Cast()
    {
        if (currentDashSpeed < movespeedThreshold)
        {
            Finish();
            return;
        }
        
        currentDashSpeed -= dashFalloff * Time.deltaTime;
        Player.Instance.Move(currentDashSpeed);
    }
    
    protected override void Finish()
    {
        currentDashSpeed = 0;
        state = AbilityState.OnCooldown;
        Player.Instance.ResetStateAfterAction();
    }
}
