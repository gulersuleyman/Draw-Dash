using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    Animator _animator;
    int animationIndex;
    PlayerCollision _playerCollision;
    // Start is called before the first frame update
    void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerCollision = FindObjectOfType<PlayerCollision>();
        animationIndex = Random.Range(1, 3);

        if(animationIndex==1)
        {
            HurricaneAnimation(true);
        }
        if(animationIndex==2)
        {
            BicycleAnimation(true);
            transform.LookAt(transform.position);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180f+transform.eulerAngles.y, transform.eulerAngles.z);
        }

        Destroy(this.gameObject, 2.1f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            if(!_playerCollision.freezed)
            {
                _playerCollision.KillEnemy(collision.gameObject);
                _playerCollision.CreateParticle(collision);
            }
            else
            {
                collision.gameObject.GetComponent<EnemyController>().KillFreezedEnemy(collision.gameObject);
            }
            
        }
        if (collision.gameObject.CompareTag("Minyon") || collision.gameObject.CompareTag("Boss"))
        {
            if (!_playerCollision.freezed)
            {
                _playerCollision.KillEnemy(collision.gameObject);
                _playerCollision.CreateParticle(collision);
            }
            else if(collision.gameObject.GetComponent<MinyonController>() !=null)
            {
                collision.gameObject.GetComponent<MinyonController>().KillFreezedMinyonEnemy(collision.gameObject);
            }
        }
    }

    public void HurricaneAnimation(bool isHurricane)
    {

        if (isHurricane == _animator.GetBool("isHurricane")) return;

        _animator.SetBool("isHurricane", isHurricane);

    }
    public void BicycleAnimation(bool isBicycle)
    {

        if (isBicycle == _animator.GetBool("isBicycle")) return;

        _animator.SetBool("isBicycle", isBicycle);

    }
}
