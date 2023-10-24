using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDashAbility", menuName = "Abilities/PlayerDash")]
public class PlayerDashAbilitySO : AbilitySO
{
    [Header("Ability-specific Fields")]
    [SerializeField] private float dashSpeed = 30;
    [SerializeField] private float dashTime = 1;

    protected override void Initialize()
    {
        Player.Instance.SetAnimatorTrigger(abilityAnimationTrigger);
    }

    protected override void Cast()
    {
        Player.Instance.StartCoroutine(Dash());
    }
    
    protected override void Finish()
    {
        state = AbilityState.OnCooldown;
        Player.Instance.ResetStateAfterAction();
    }

    private IEnumerator Dash()
    {
        float dashTimeCounter = dashTime;
        Player.Instance.invulnerableTimer = 1f;
        while(dashTimeCounter > 0)
        {
            dashTimeCounter -= Time.deltaTime;
            Player.Instance.Move(dashSpeed);
            yield return null;
        }
        Finish();
    }
}
