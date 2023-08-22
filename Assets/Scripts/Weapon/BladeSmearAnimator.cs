using UnityEngine;
using System.Collections;

public class BladeSmearAnimator : MonoBehaviour
{
    [SerializeField] private PlayerAnimator playerAnimator;
    private BladeSmear bladeSmear;

    private void Awake()
    {
        bladeSmear = GetComponent<BladeSmear>();
    }

    private void Start()
    {
        playerAnimator.OnAttackAnimationStarted += PlayerAnimator_OnAttackAnimationStarted;
    }

    private void PlayerAnimator_OnAttackAnimationStarted(object sender, System.EventArgs e)
    {
        bladeSmear.SetActivate(true);
        bladeSmear.ResetSmear();
    }
}
