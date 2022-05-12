using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public void RunningAnimation(bool isRunning)
    {
        
            if (isRunning == _animator.GetBool("isRunning")) return;

            _animator.SetBool("isRunning", isRunning);
        
    }
    public void AttackAnimation(bool Attack)
    {

        if (Attack == _animator.GetBool("Attack")) return;

        _animator.SetBool("Attack", Attack);

    }
    public void FinalRunAnimation(bool FinalRun)
    {

        if (FinalRun == _animator.GetBool("FinalRun")) return;

        _animator.SetBool("FinalRun", FinalRun);

    }
    public void JumpRunAnimation(bool isJump)
    {

        if (isJump == _animator.GetBool("isJump")) return;

        _animator.SetBool("isJump", isJump);

    }
    public void DeathAnimation(bool isDeath)
    {

        if (isDeath == _animator.GetBool("isDeath")) return;

        _animator.SetBool("isDeath", isDeath);

    }
    public void FlyAnimation(bool isFlying)
    {

        if (isFlying == _animator.GetBool("isFlying")) return;

        _animator.SetBool("isFlying", isFlying);

    }
    public void GrowAnimation(bool isGrowing)
    {

        if (isGrowing == _animator.GetBool("isGrowing")) return;

        _animator.SetBool("isGrowing", isGrowing);

    }
}
