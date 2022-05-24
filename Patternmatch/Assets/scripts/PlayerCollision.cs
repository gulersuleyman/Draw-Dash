using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cinemachine;
public class PlayerCollision : MonoBehaviour
{

    [SerializeField] GameObject pointer;
    [SerializeField] GameObject hitParticle;
    [SerializeField] GameObject wallHitParticle;
    [SerializeField] GameObject growingParticle;
    [SerializeField] GameObject ghostParticle;
    [SerializeField] GameObject freezeParticle;
    [SerializeField] GameObject firstFreezeParticle;
    [SerializeField] GameObject flameParticle;
    [SerializeField] GameObject powerGhost;
    [SerializeField] GameObject deathImage;

    [SerializeField] Material freezeMaterial;


    public Image Bar;
    public GameObject healthBar;
    public float fill;
    [SerializeField] float clearAmount=0.05f;

    public Transform checkField;
    public bool wallHit;
    public bool grown = false;
    public bool freezed = false;
    public bool flamed = false;
    public bool isDeath=false;
    public LayerMask layer;
    public Collider[] detectedObject; 

    PlayerController _playerController;
    AnimationController _animationController;
    EnemyAnimationController[] _enemyAnimations;
    Rigidbody _rigidbody;
    GunTargetPosition _gunTarget;
    GraplingGun _bullet;
    CapsuleCollider _collider;
    stopListMover _mover;
    // RagdollDismembermentVisual _dismemberment;
    // CharacterJoint _joint;
    // Start is called before the first frame update
    CinemachineVirtualCamera vcam;
    CinemachineBasicMultiChannelPerlin noise;

    

    
    void Start()
    {
        vcam = GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>();
        noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        _collider = GetComponent<CapsuleCollider>();
        _bullet = FindObjectOfType<GraplingGun>();
        _gunTarget = FindObjectOfType<GunTargetPosition>();
        _rigidbody = GetComponent<Rigidbody>();
        _animationController = GetComponent<AnimationController>();
        wallHit = false;
        _playerController = GetComponent<PlayerController>();
        _enemyAnimations = FindObjectsOfType<EnemyAnimationController>();
        _mover = GetComponent<stopListMover>();



       // LevelSystem.Instance.DidYouReturnPanel = false;
       // LevelSystem.Instance.DidYouNextLevelPanel = false;

    }

    // Update is called once per frame
    void Update()
    {
        if(Physics.CheckSphere(checkField.position,checkField.lossyScale.x/2,layer))
        {
            detectedObject = Physics.OverlapSphere(checkField.position, checkField.lossyScale.x / 2);
            foreach (var hit in detectedObject)
            {
                if (layer == (layer | (1 << hit.gameObject.layer)))
                {
                    if(_mover.canDie && !freezed)
                    {
                        
                        _animationController.DeathAnimation(true);
                        GameManager.Instance.levelFinished = true;
                        Invoke("EndGame", 0.5f);
                        StartCoroutine(OnDeath());
                    }
                    else
                    {
                        KillEnemy(hit.gameObject);
                        //tek tek çalışabilir
                    }

                    
                }
                
                 
                
            }
        }
    }

    void EndGame()
    {
       // LevelSystem.Instance.DidYouReturnPanel = true;
    }

    IEnumerator OnDeath()
    {
        if(!isDeath)
        {
            deathImage.transform.parent.gameObject.SetActive(true);
            deathImage.gameObject.SetActive(true);
            _rigidbody.useGravity = true;
            _rigidbody.isKinematic = false;
            _rigidbody.AddForce(Vector3.up * 20f * Time.deltaTime,ForceMode.Impulse);

            yield return new WaitForSeconds(1f);
            
            _rigidbody.isKinematic = true;
            deathImage.transform.parent.gameObject.SetActive(false);
            deathImage.gameObject.SetActive(false);
            isDeath = true;
        }
        else
        {
            yield return null;
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            if(!freezed)
            {
                KillEnemy(collision.gameObject);

                CreateParticle(collision);
            }
            else
            {
                collision.gameObject.GetComponent<EnemyController>().KillFreezedEnemy(collision.gameObject);
            }
            
            

           
            // collision.gameObject.GetComponent<EnemyController>().isDeath = true;
            // collision.gameObject.GetComponent<EnemyController>().healthBar.gameObject.SetActive(false);
            // collision.gameObject.GetComponent<EnemyController>().deathWithSword = true;
        }
       /* if(collision.gameObject.CompareTag("Minyon"))
      {

            if(!freezed)
            {
                /* MinyonController minyon = collision.gameObject.GetComponent<MinyonController>();
                  minyon.isDeath = true;
                  collision.gameObject.GetComponent<CapsuleCollider>().enabled = false;
                  collision.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
                  collision.gameObject.GetComponentInChildren<Animator>().enabled = false;*/
            /*    KillEnemy(collision.gameObject);
                CreateParticle(collision);
            }
            else
           {
                collision.gameObject.GetComponent<MinyonController>().KillFreezedMinyonEnemy(collision.gameObject);
                
                collision.gameObject.GetComponentInChildren<Animator>().enabled = false;
            }
            
        } */
        


    }

    public void KillEnemy(GameObject collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            

            EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
            enemy.isDeath = true;
            enemy.healthBar.gameObject.SetActive(false);
            enemy.deathWithSword = true;
            
            _playerController._enemyCount--;

            if (flamed)
            {
                GameObject particle = Instantiate(flameParticle, collision.gameObject.transform.position, Quaternion.identity);
                particle.transform.parent = collision.gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).transform;
                particle.transform.eulerAngles = new Vector3(-90, 180, 0);

                if (grown)
                {
                    particle.transform.localScale = particle.transform.localScale * 2f;
                }
            }


            collision.gameObject.GetComponent<CapsuleCollider>().enabled = false;
            collision.gameObject.GetComponentInChildren<Animator>().enabled = false;
            collision.gameObject.GetComponentInChildren<ragdoll1>().RagdollActivator = true;
            collision.gameObject.GetComponentInChildren<ragdoll1>().ragdollOpen();
        }
        if(collision.gameObject.CompareTag("Minyon"))
        {
            collision.gameObject.GetComponent<MinyonController>().EnableParticle();

            MinyonController enemy = collision.gameObject.GetComponent<MinyonController>();
            enemy.isDeath = true;
            
            enemy.deathWithSword = true;

            _playerController._enemyCount--;

            if (flamed)
            {
                GameObject particle = Instantiate(flameParticle, collision.gameObject.transform.position, Quaternion.identity);
                particle.transform.parent = collision.gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).transform;
                particle.transform.eulerAngles = new Vector3(-90, 180, 0);

                if (grown)
                {
                    particle.transform.localScale = particle.transform.localScale * 2f;
                }
            }
            collision.gameObject.GetComponent<CapsuleCollider>().enabled = false;
            collision.gameObject.GetComponentInChildren<Animator>().enabled = false;
            collision.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        }
        if (collision.gameObject.CompareTag("Boss"))
        {
             collision.gameObject.transform.DOScale(collision.gameObject.transform.localScale - new Vector3(0.7f, 0.7f, 0.7f), 0.1f);
            //collision.gameObject.transform.localScale = collision.gameObject.transform.localScale - new Vector3(0.7f, 0.7f, 0.7f);
            StartCoroutine(StopBoss(collision.gameObject));
            if (collision.gameObject.transform.localScale.x <= 1.2f)
            {
                collision.gameObject.GetComponent<MinyonController>().EnableParticle();

                MinyonController enemy = collision.gameObject.GetComponent<MinyonController>();
                enemy.isDeath = true;

                enemy.deathWithSword = true;

                _playerController._enemyCount--;

                collision.gameObject.GetComponent<CapsuleCollider>().enabled = false;
                collision.gameObject.GetComponentInChildren<Animator>().enabled = false;
                collision.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
                
            }
        }


    }
    IEnumerator StopBoss(GameObject collision)
    {
        collision.gameObject.GetComponent<Rigidbody>().isKinematic = true;

        yield return new WaitForSeconds(0.1f);

        collision.gameObject.GetComponent<Rigidbody>().isKinematic = true;  // şimdilik
    }

    public void CreateParticle(Collision collision)
    {
        Vector3 particleTransform = new Vector3(collision.gameObject.transform.position.x, collision.gameObject.transform.position.y + 3f, collision.gameObject.transform.position.z);
        Instantiate(hitParticle, particleTransform, Quaternion.identity);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Stone"))
        {
            if (!GameManager.Instance.levelFinished)
                Stunned(other);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Water"))
        {
            other.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        }

        

        if (other.gameObject.CompareTag("EnemyBullet"))
        {
            fill = Mathf.Clamp(fill, 0f, 1f);
            fill -= clearAmount;
            Bar.fillAmount = fill;
           
            if(fill<=0.05f)
            {
                _animationController.DeathAnimation(true);

                foreach (var anims in _enemyAnimations)
                {
                    anims.ShotAnimation(false);
                }
                GameManager.Instance.levelFinished = true;
                healthBar.gameObject.SetActive(false);
            }
        }
        if (other.gameObject.CompareTag("Wall"))
        {
            if(!grown)
            {
                Stunned(other);
            }
            
            else
            {
                
                other.gameObject.GetComponent<MeshRenderer>().enabled = false;
                
                    other.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                    Rigidbody[] rb = other.gameObject.transform.GetChild(0).gameObject.GetComponentsInChildren<Rigidbody>();
                    foreach (var r in rb)
                    {
                        r.AddExplosionForce(1600f, transform.position, 80f, 10f);
                    }
                
                
                Vector3 wallHitTransform = new Vector3(transform.position.x, transform.position.y + 4f, transform.position.z);
                Instantiate(wallHitParticle, wallHitTransform, Quaternion.identity);
                Destroy(other.gameObject, 1.3f);
            }
            
            
        }
        if(other.gameObject.CompareTag("Stone"))
        {
            if(!GameManager.Instance.levelFinished)
            Stunned(other);
        }
        


        
        if (other.gameObject.CompareTag("Box"))
        {
            if(_playerController.levelFinished)
            _animationController.JumpRunAnimation(true);
            
        }
        if (other.gameObject.CompareTag("GrowPower"))
        {
            
            GameObject particle=  Instantiate(growingParticle, transform.position, Quaternion.identity);
            particle.transform.eulerAngles = new Vector3(-90, 0, 0);
           // _animationController.GrowAnimation(true);
            
           // _playerController.DisableBone();
            grown = true;
            other.gameObject.transform.DOScale(Vector3.zero, 1f);
            other.gameObject.transform.DOMove(new Vector3(transform.position.x, transform.position.y + 4f, transform.position.z), 0.5f);

            transform.DOScale(transform.localScale * 1.5f, 0.3f);
            transform.DOMoveY(transform.position.y + 2f, 0.3f);
            Destroy(other.gameObject);
        }
        if(other.gameObject.CompareTag("GhostPower"))
        {
            GameObject particle = Instantiate(ghostParticle, transform.position, Quaternion.identity);
            particle.transform.eulerAngles = new Vector3(-90, 0, 0);

            float distance = 50;
            float direction = 1f;
            float turnValue = 1;
            GameObject[] ghosts = new GameObject[6];
            for (int i = 0; i < 6; i++)
            {
              direction = 2f;
              GameObject ghost=  Instantiate(powerGhost.gameObject, transform.position, Quaternion.identity);

                if (grown)
                    ghost.transform.localScale *= 2;


              ghosts[i] = ghost;
                if(i<2)
                {
                    ghosts[i].transform.DOMove(transform.position + (new Vector3(0.2f * direction * turnValue, 0, -0.2f * direction) * distance), 1.5f);
                    turnValue = -1;
                }
                else if(i>=2 && i<4)
                {
                    ghosts[i].transform.DOMove(transform.position + (new Vector3(0.2f * direction * turnValue, 0, 0.2f * direction) * distance), 1.5f);
                    turnValue = 1;
                }
                else //if(i==4)
                {
                    direction = 3f;
                    ghosts[i].transform.DOMove(transform.position + (new Vector3(0, 0, turnValue * 0.2f * direction) * distance), 1.5f);
                    turnValue = -1;
                }


                
              
            }
            Destroy(other.gameObject);
        }
        if(other.gameObject.CompareTag("FreezePower"))
        {
            GameObject particle = Instantiate(freezeParticle, transform.position, Quaternion.identity);
            particle.transform.eulerAngles = new Vector3(-90, 0, 0);

            freezed = true;

            foreach (var enemy in _gunTarget._enemies)
            {
                
                enemy.GetComponentInChildren<Animator>().enabled = false;
                if(enemy.GetComponent<EnemyController>() != null)
                {
                    enemy.GetComponent<EnemyController>().healthBar.gameObject.SetActive(false);
                    enemy.GetComponent<EnemyController>().clearAmount = 1f;
                }
                
                enemy.GetComponentInChildren<SkinnedMeshRenderer>().material = freezeMaterial;
                Instantiate(firstFreezeParticle, new Vector3(enemy.transform.position.x,enemy.transform.position.y+3f,enemy.transform.position.z), Quaternion.identity);
            }

            MinyonController[] minyonlar = FindObjectsOfType<MinyonController>();

            foreach (var minyon in minyonlar)
            {
                if(!minyon.isDeath)
                {
                    minyon.gameObject.GetComponentInChildren<Animator>().enabled = false;
                    minyon.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = freezeMaterial;
                }
                
            }

            _bullet.lazerSpawnTime = 0.27f;

            Destroy(other.gameObject);
        }
        if(other.gameObject.CompareTag("FlamePower"))
        {

            flamed = true;
            GameObject particle = Instantiate(flameParticle,new Vector3( transform.position.x,transform.position.y+3f,transform.position.z), Quaternion.identity);
            particle.transform.parent = transform;
            particle.transform.eulerAngles = new Vector3(0, 180,0);
            _collider.radius *= 2f;


            if (grown)
                particle.transform.localScale = particle.transform.localScale * 2f;


            Destroy(other.gameObject, 0.2f);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Box"))
        {
            _animationController.JumpRunAnimation(false);

        }
        
    }

    public void Noise(float amplitudeGain, float frequencyGain)
    {
        noise.m_AmplitudeGain = amplitudeGain;
        noise.m_FrequencyGain = frequencyGain;
    }

    private IEnumerator ShakeCamera(float shakeTiming)
    {

        Noise(0.2f, 0.2f);
        yield return new WaitForSeconds(shakeTiming);
        Noise(0, 0);
    }
    void Stunned(Collider other)
    {
        
            
            _mover.WallHit();
            StartCoroutine(ShakeCamera(0.1f));

            Vector3 direction = other.gameObject.transform.position - this.transform.position;
            wallHit = true;
            _animationController.AttackAnimation(false);

            // Vector3 wallHitTransform = new Vector3(transform.position.x, transform.position.y + 4f, transform.position.z);
            // Instantiate(wallHitParticle, wallHitTransform, Quaternion.identity);
            // _rigidbody.AddForce(new Vector3(direction.x, 0, direction.z) * Time.deltaTime *power);

            if (!_playerController.levelFinished)
            {
                StartCoroutine(WaitDamage());
                transform.DOMove((new Vector3(-direction.x / 3, 0, -direction.z / 3) + transform.position), 0.2f).OnComplete(() =>
                {
                    pointer.gameObject.SetActive(false);
                    pointer.transform.position = transform.position;
                    pointer.gameObject.SetActive(true);
                    wallHit = false;
                });

            }
            

    }
    public IEnumerator WaitDamage()
    {
        this.transform.GetChild(7).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        this.transform.GetChild(7).gameObject.SetActive(false);
    }
}
