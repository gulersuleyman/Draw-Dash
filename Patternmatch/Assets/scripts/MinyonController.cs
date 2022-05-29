using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class MinyonController : MonoBehaviour
{
    public GameObject[] armors;
    [SerializeField] GameObject freezeParticle;
    [SerializeField] GameObject deathParticle;
    [SerializeField] GameObject deathParticle2;

    public bool isDeath = false;
    public bool deathWithSword = false;
    public bool isNear = false;


    PlayerCollision _playerCollision;
    PlayerController _playerController;
    CapsuleCollider _capsuleCollider;
    Animator _animator;
    Rigidbody _rigidbody;
    EnemyAnimationController _enemyAnimationController;
    SkinnedMeshRenderer _mesh;
    

    void Awake()
    {
        _mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        _playerCollision = FindObjectOfType<PlayerCollision>();
        
        _enemyAnimationController = GetComponent<EnemyAnimationController>();
        _playerController = FindObjectOfType<PlayerController>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
        _animator = GetComponentInChildren<Animator>();
        _rigidbody = GetComponent<Rigidbody>();

        transform.LookAt(_playerController.transform.position);
        transform.localEulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        

    }
   

    private void OnTriggerEnter(Collider other)
    {

        
        if (other.gameObject.CompareTag("EnemyMover"))
        {
            if (!_playerCollision.freezed)
                MoveToPlayer();
        }
        if(other.gameObject.CompareTag("Stone"))
        {
            Vector3 direction = other.gameObject.transform.position - this.transform.position;
            

            if (!_playerController.levelFinished)
            {
                transform.DOMove((new Vector3(-direction.x / 4, 0, -direction.z / 4) + transform.position), 0.3f);
            }
        }

        if(other.gameObject.CompareTag("WaitDamage"))
        {
            _playerController._enemyCount--;
            _capsuleCollider.enabled = false;
            _animator.enabled = false;
            _mesh.enabled = false;
            Instantiate(deathParticle, other.gameObject.transform.position, Quaternion.identity);
            Instantiate(deathParticle2, other.gameObject.transform.position, Quaternion.identity);
            isDeath = true;
        }


    }

    


    public void KillFreezedMinyonEnemy(GameObject other)
    {
        isDeath = true;
        _playerController._enemyCount--;
        _mesh.enabled = false;
        Instantiate(deathParticle, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        Instantiate(deathParticle2, other.gameObject.transform.position, Quaternion.identity);

        Instantiate(freezeParticle, other.gameObject.transform.position, Quaternion.identity);
    }
    public void EnableParticle()
    {
        Instantiate(deathParticle, new Vector3(transform.position.x, transform.position.y+2f , transform.position.z), Quaternion.identity);
        Instantiate(deathParticle2, gameObject.transform.position, Quaternion.identity);
    }
    
    public void MoveToPlayer()
    {
        isNear = true;
        _animator.SetBool("isRunning", true);
        _rigidbody.isKinematic = false;
    }
}
