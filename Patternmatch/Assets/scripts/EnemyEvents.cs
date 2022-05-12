using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEvents : MonoBehaviour
{
    public int fireCount;
    public int fireBoundary;
    public bool isMoving;

    EnemyController _enemyController;
    PlayerController _playerController;
    
    private void Awake()
    {
        
        _enemyController = GetComponentInParent<EnemyController>();
        _playerController = FindObjectOfType<PlayerController>();
    }




    void Shot()
    {
        _enemyController.transform.LookAt(new Vector3(_playerController.transform.position.x,_playerController.transform.position.y+3f,_playerController.transform.position.z));
        ObjectPooler.Instance.SpawnFromPool("EnemyBullet", transform.position, Quaternion.identity);
        fireCount++;
    }
    void ShotEnd()
    {
        if(fireCount >=fireBoundary)
        {
            isMoving = true; //varınca false ve firecount resetlenir ve tekrar ates etme animi ve lookat player
            _enemyController.MoveEnemy();
        }
    }
}
