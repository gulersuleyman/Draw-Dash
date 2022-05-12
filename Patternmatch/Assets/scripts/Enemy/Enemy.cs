using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private bool work ;

    [SerializeField] private bool useGameManagerValues=true;
    
    
    public Transform playert;
    [SerializeField] private float fireDistance= 40 ;
    

    [SerializeField] private float fireRateIn1Min;
    private float waitingTime;

    [SerializeField] private float currentDistance  ;
         

    [SerializeField] private BulletSpawner theGun;
    
    bool shootOff=true;

    private void Start()
    {
        if (useGameManagerValues)
        {
            getValues();
        }
        
        
        waitingTime = 60 / fireRateIn1Min;
        
        
        
       // StartCoroutine(fireOn());  
    }

    private void getValues()
    {
        fireDistance = GameManager.Instance.enemyFireDistance;
        fireRateIn1Min = GameManager.Instance.enemyFireRateIn1Min;
    }

    void Update()
    {
        currentDistance = Vector3.Distance(playert.position, transform.position);
        
        if (work&&(Vector3.Distance(playert.position,transform.position) < fireDistance)&&shootOff)
        {
            InvokeRepeating(nameof(fire),0,waitingTime);

            shootOff = false;
        }
         
      
    }

    public bool CheckDistance()
    {
        
        
        if (Vector3.Distance(playert.position,transform.position) < fireDistance)
        {
            return true;
        }   
    
        return false;
    }

    
    
    IEnumerator fireOn()
    {
         
        if (work)
        {
          //  currentDistance = Vector3.Distance(player.position, transform.position);
          //  Debug.Log(" CURRENT DISTANCE  : "+currentDistance );
         //   if ( currentDistance<fireDistance)
         //   {
        //        Debug.Log(currentDistance+"  IS SMALLER  THAN   :  "+fireDistance);
        if (UnityEngine.Random.Range(0,2) % 2 == 0)
        {
            theGun.shoot();
        }
               
                 
            
        //    } 
         
            yield return new WaitForSeconds(waitingTime);

         
            StartCoroutine(fireOn()); 
        }
        else
        {
            shootOff = true;
        }
         
    }

    private void fire()
    {
        if (work)
        {
            currentDistance = Vector3.Distance(playert.position, transform.position);
//            Debug.Log(" CURRENT DISTANCE  : " + currentDistance);
            if (currentDistance < fireDistance)
            {
//    Debug.Log(currentDistance + "  IS SMALLER  THAN   :  " + fireDistance);
                if (playert.position.z<transform.position.z)
                {
                    theGun.shoot();
                }
                

            }



        }

    }
}
