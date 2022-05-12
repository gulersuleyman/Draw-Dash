using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraplingGun : MonoBehaviour
{

    public float lazerSpawnTime=1;
    [SerializeField] private GameObject lazer;
   // public Transform hitpoint;
    private float currentSpawnTime=0;
    private float timeBoundary;

    ObjectPooler objectPooler;
    GunTargetPosition _target;
    PlayerController _playerController;
    // Start is called before the first frame update
    void Start()
    {
        
        objectPooler = ObjectPooler.Instance;
        _playerController = FindObjectOfType<PlayerController>();
        _target = FindObjectOfType<GunTargetPosition>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(_target.isNear)
        {
            if (_playerController._enemyCount != 0 && !GameManager.Instance.levelFinished)
            {
                currentSpawnTime += Time.deltaTime;
                if (currentSpawnTime > timeBoundary)
                {
                    Spawn();
                    ResetTimes();
                }
            }
        }
        
        
    }


    void Spawn()
    {
        // GameObject templazer = Instantiate(lazer, transform.position,Quaternion.identity);

        // templazer.transform.parent = null;

        objectPooler.SpawnFromPool("bullet", transform.position, Quaternion.identity);
        
    }
    void ResetTimes()
    {
        currentSpawnTime = 0f;
        timeBoundary = lazerSpawnTime;

    }
}
