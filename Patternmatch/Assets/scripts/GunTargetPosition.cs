using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GunTargetPosition : MonoBehaviour
{
    
     public GameObject[] _enemies;

     public bool isNear=false; 

     public PointList ListOfPointLists = new PointList();


    PlayerController _playerController;
    PlayerCollision _playerCollision;
    Vector3 firstPosition;

    [System.Serializable]
    public class Point
    {
        public GameObject go;
        public int enemyGroupNo;
    }

    [System.Serializable]
    public class PointList
    {
        public List<Point> list;
    }
    void Start()
    {
        firstPosition = transform.position;

        _playerController = FindObjectOfType<PlayerController>();
        _playerCollision = FindObjectOfType<PlayerCollision>();
    }

    // Update is called once per frame
    void Update()
    {

        if(_enemies!=null)
        {
            for (int i = 0; i < _enemies.Length; i++)
            {
                /* if(_enemies[i].gameObject.GetComponent<EnemyController>() !=null)
                 {
                     if (!_enemies[i].gameObject.GetComponent<EnemyController>().isDeath)  
                     {
                         transform.position = _enemies[i].transform.position +new Vector3(0,5,0);

                         return;
                     }
                 }
                 else*/
              //  if (_enemies[i].gameObject?.GetComponent<MinyonController>() != null)
               // {
                    if (!_enemies[i].gameObject.GetComponent<MinyonController>().isDeath && _enemies[i].gameObject.GetComponent<MinyonController>().isNear)
                    {

                    
                        transform.position = _enemies[i].transform.position + new Vector3(0, 5, 0);

                    isNear = true;

                    return;
                    }
                    else
                {
                    isNear = false;
                }
               // }
              //  else return;
                

                
            }
        }
        else
        {
            transform.position = firstPosition;
        }
        


    }
    private void FixedUpdate()
    {
        foreach (var enemy in _enemies)
        {
            if ( !_playerCollision.freezed)
            {
                // enemy.GetComponent<Rigidbody>().MovePosition(Vector3.Lerp(enemy.transform.position, _playerController.transform.position, .005f));

                enemy.GetComponent<Rigidbody>().MovePosition(Vector3.MoveTowards(enemy.transform.position, _playerController.transform.position, .18f));

                enemy.transform.LookAt(_playerController.transform.position);
                enemy.transform.eulerAngles = new Vector3(0, enemy.transform.eulerAngles.y, 0);
            }
        }
    }
    void forList()
    {
        /* foreach (var enemy in ListOfPointLists.list)
                         {
                             if(enemy.enemyGroupNo==1)
                             {
                                 enemy.go.GetComponent<MinyonController>().MoveToPlayer();
                             }

                         }*/
        foreach (var enemy in _enemies)
        {
            // enemy.GetComponent<MinyonController>().MoveToPlayer();
        }
    }
}
