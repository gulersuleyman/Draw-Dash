using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class EnemyBulletMover : MonoBehaviour
{
    Transform _target;


    PlayerController _playerController;

    private void Start()
    {
        _playerController = FindObjectOfType<PlayerController>();
        _target = FindObjectOfType<GunTargetPosition>().transform;

        Fire();
    }

    public void Fire()
    {


        transform.DOMove(new Vector3(_playerController.transform.position.x,_playerController.transform.position.y+8f,_playerController.transform.position.z), 0.8f).SetEase(Ease.Linear).OnComplete(() =>
        {

            this.gameObject.SetActive(false);
        });



    }
}
