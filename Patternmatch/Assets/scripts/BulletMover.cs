using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BulletMover : MonoBehaviour
{
    
    [SerializeField] GameObject explosionParticle;

    Transform _target;


    PlayerController _playerController;
    PlayerCollision _playerCollision;
    private void OnEnable()
    {
        _playerCollision = FindObjectOfType<PlayerCollision>();
        _playerController = FindObjectOfType<PlayerController>();
      //  _target = FindObjectOfType<GunTargetPosition>().transform;
        explosionParticle.SetActive(false);
        Fire();
    }

    public void Fire()
    {


      /*  transform.DOMove(_target.position,0.6f).OnComplete(() =>
        {
           
            Invoke("ActiveFalse", 0.05f);
        });*/



    }

    public void ActiveFalse()
    {
        this.gameObject.SetActive(false);
    }
    
   /* private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            explosionParticle.gameObject.transform.parent = null;
            explosionParticle.gameObject.SetActive(true);
           
            
        }
        if(other.gameObject.CompareTag("Boss"))
        {

             other.gameObject.transform.DOScale(other.gameObject.transform.localScale - new Vector3(0.7f, 0.7f, 0.7f), 0.2f);

           // other.gameObject.transform.localScale = other.gameObject.transform.localScale - new Vector3(0.7f, 0.7f, 0.7f);

            if (other.gameObject.transform.localScale.x <= 1.3f || _playerCollision.freezed)
            {
                _playerController._enemyCount--;
                other.gameObject.GetComponent<MinyonController>().EnableParticle();

                 MinyonController enemy = other.gameObject.GetComponent<MinyonController>();
                 enemy.isDeath = true;

                 enemy.deathWithSword = true;

                 

                 other.gameObject.GetComponent<CapsuleCollider>().enabled = false;
                 other.gameObject.GetComponentInChildren<Animator>().enabled = false;
                 other.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
               // Destroy(other.gameObject);
            }
        }
        if(other.gameObject.CompareTag("Minyon"))
        {
            
                    if (!_playerCollision.freezed)
                    {
                        _playerController._enemyCount--;
                other.gameObject.GetComponent<CapsuleCollider>().enabled = false;
                other.gameObject.GetComponentInChildren<Animator>().enabled = false;
                other.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
                other.gameObject.GetComponent<MinyonController>().EnableParticle();
                other.gameObject.GetComponent<MinyonController>().isDeath = true;

                    }
                    else
                    {
                other.gameObject.GetComponent<MinyonController>().KillFreezedMinyonEnemy(other.gameObject);
                        
                    }
               
        }
    }*/



}
