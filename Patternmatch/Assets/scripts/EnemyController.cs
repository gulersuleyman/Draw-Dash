
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class EnemyController : MonoBehaviour
{


    [SerializeField] private float lazerSpawnTime;
    [SerializeField] private GameObject lazer;
    [SerializeField] Transform bulletBeginTransform;
    [SerializeField] Transform moveTargetOne;
    [SerializeField] Transform moveTargetTwo;
    [SerializeField] GameObject freezeParticle;

    bool targetOne;
    


    public Image Bar;
    public GameObject healthBar;
    public float fill;
    public float clearAmount;
    [SerializeField] GameObject _gun;


    public bool isDeath = false;
    public bool deathWithSword = false;
    public bool isShoting;


    PlayerCollision _playerCollision;
    PlayerController _playerController;
    CapsuleCollider _capsuleCollider;
    Animator _animator;
    ragdoll1 _ragdoll;
    EnemyEvents _events;
    EnemyAnimationController _enemyAnimationController;
    SkinnedMeshRenderer _mesh;

    // Start is called before the first frame update
    void Awake()
    {
        _mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        _playerCollision = FindObjectOfType<PlayerCollision>();
        _events = GetComponentInChildren<EnemyEvents>();
        _enemyAnimationController = GetComponent<EnemyAnimationController>();
        _playerController = FindObjectOfType<PlayerController>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
        _animator = GetComponentInChildren<Animator>();
        _ragdoll = GetComponentInChildren<ragdoll1>();
        

        transform.LookAt(_playerController.transform.position);
        transform.localEulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    


    private void OnTriggerEnter(Collider other)
    {
     
        if(other.gameObject.CompareTag("bullet"))

        {
            if(!_playerCollision.freezed)
            {
                fill = Mathf.Clamp(fill, 0f, 1f);
                fill -= clearAmount;
                Bar.fillAmount = fill;


                if (fill <= 0.05)
                {
                    _playerController._enemyCount--;
                    _capsuleCollider.enabled = false;
                    _animator.enabled = false;
                    _ragdoll.RagdollActivator = true;
                    _ragdoll.ragdollOpen();
                    isDeath = true;
                    healthBar.gameObject.SetActive(false);
                    _gun.gameObject.transform.parent = null;
                }
            }
            else
            {
                KillFreezedEnemy(other.gameObject);
            }

            
        }

    }
    public void KillFreezedEnemy(GameObject other)
    {
        _playerController._enemyCount--;
        _mesh.enabled = false;
        _capsuleCollider.enabled = false;
        isDeath = true;
        _gun.gameObject.transform.parent = null;
        _gun.GetComponent<Rigidbody>().isKinematic = false;
        _gun.GetComponent<Rigidbody>().AddForce(new Vector3(0.3f, 1, 0.3f) * 500f * Time.deltaTime, ForceMode.Impulse);
        Instantiate(freezeParticle, other.gameObject.transform.position, Quaternion.identity);
    }

    
    public void MoveEnemy()  
    {
        if(_events.isMoving)
        {
            if(!targetOne)
            {
                _events.fireCount = 0;
                _enemyAnimationController.WalkAnimation(true);
                transform.LookAt(moveTargetOne);
                transform.DOMove(moveTargetOne.position, 2f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    transform.LookAt(_playerController.transform.position);
                    transform.localEulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                    _enemyAnimationController.WalkAnimation(false);
                    targetOne = true;
                    _events.isMoving = false;
                });
            }
            else
            {
                _events.fireCount = 0;
                _enemyAnimationController.WalkAnimation(true);
                transform.LookAt(moveTargetTwo);
                transform.DOMove(moveTargetTwo.position, 2f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    transform.LookAt(_playerController.transform.position);
                    transform.localEulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                    _enemyAnimationController.WalkAnimation(false);
                    targetOne = false;
                    _events.isMoving = false;
                });
            }
            
        }
        
    }
    
}
