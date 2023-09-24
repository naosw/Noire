using System;
using System.Collections;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private const string WALK = "PlayerWalk";
    private const string IDLE = "PlayerIdle";
    private const string DASH = "PlayerDash";
    public event EventHandler OnAttackAnimationStarted;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        animator.SetBool(WALK, Player.Instance.IsWalking());
        animator.SetBool(IDLE, Player.Instance.IsIdle());
        animator.SetBool(DASH, Player.Instance.IsDashing());
    }

    public bool AnimatorIsPlaying(int layer)
    {
        return animator.GetCurrentAnimatorStateInfo(layer).length > animator.GetCurrentAnimatorStateInfo(layer).normalizedTime;
    }

    public bool AnimatorIsPlaying(int layer, string stateName)
    {
        return AnimatorIsPlaying(layer) && animator.GetCurrentAnimatorStateInfo(layer).IsName(stateName);
    }

    private void AnimationStartedTrigger()
    {
        OnAttackAnimationStarted?.Invoke(this, EventArgs.Empty);
    }
}
