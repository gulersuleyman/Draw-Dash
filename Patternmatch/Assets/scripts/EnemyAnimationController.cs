using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public void ShotAnimation(bool isShot)
    {

        if (isShot == _animator.GetBool("isShot")) return;

        _animator.SetBool("isShot", isShot);

    }
    public void WalkAnimation(bool isWalk)
    {

        if (isWalk == _animator.GetBool("isWalk")) return;

        _animator.SetBool("isWalk", isWalk);

    }
}
