using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public GameObject bullet;
    [SerializeField] private bool bulletAutomaticDestroy;
    public float destroyDistance;
    
    public float bulletSpeed;
    public float bulletGravity=0;

    public static bool working = false;
    
    [SerializeField] private float currentDistance  ;
    
    [SerializeField] private float fireDistance= 40 ;
    
  //  public Transform playert;

    void Update()
    {
      //  currentDistance = Vector3.Distance(playert.position, transform.position);
        if (false)   
        { 
            
            shoot();
          
          

        }
        

    }

    public void shoot()
    {

     //   if ( currentDistance>fireDistance)
     //       return;
        
        GameObject instBullet = Instantiate(bullet,transform.position,Quaternion.identity) as GameObject;
           
        bulletMove info = instBullet.GetComponent<bulletMove>();
          
        if (bulletAutomaticDestroy)
        {
            info.automaticDestroy = true;
            info.destroyDistance = destroyDistance;
        }

        info.gunPointingDirection = this.gameObject.transform.forward;
        info.bulletSpeed = bulletSpeed;
        //
        info.gravity = bulletGravity;
       // info.firedGun = transform;
        info.work = true;


    }


}
