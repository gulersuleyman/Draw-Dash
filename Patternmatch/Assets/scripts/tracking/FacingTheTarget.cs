using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacingTheTarget : MonoBehaviour  // it makes the object face to given point all the times

{
     
    public GameObject target;

    [SerializeField] private bool faceTheTarget;
     private bool _faceTheTarget;
    
     
    public float turn_speed = 100f;
    [SerializeField] private bool faceX;
    [SerializeField] private bool faceY;
    [SerializeField] private bool faceZ;

    [SerializeField] private bool faceInLimitationX;
    [SerializeField] private bool faceInLimitationY;
    [SerializeField] private bool faceInLimitationZ;

    [SerializeField] private Vector3 minLimits=new Vector3();
    [SerializeField] private Vector3 maxLimits=new Vector3();

    [SerializeField] private bool addSelfPosition;


    private void Start()
    {
        fixLimitations();
    }

    private void fixLimitations()
    {

        if (addSelfPosition)
        {
            Vector3 selfPos = transform.position;
            maxLimits.x += selfPos.x;
            maxLimits.y += selfPos.y;
            maxLimits.z += selfPos.z;
            
            minLimits.x += selfPos.x;
            minLimits.y += selfPos.y;
            minLimits.z += selfPos.z;
        }

        if (maxLimits.x<minLimits.x)
        {
            float temp = maxLimits.x;
            maxLimits.x = minLimits.x;
            minLimits.x = temp;
        }

        if (maxLimits.y<minLimits.y)
        {
            float temp = maxLimits.y;
            maxLimits.y = minLimits.y;
            minLimits.y = temp;
        }
        if (maxLimits.z<minLimits.z)
        {
            float temp = maxLimits.z;
            maxLimits.z = minLimits.z;
            minLimits.z = temp;
        }

    }

    void Update()
    {


        
            Vector3 to = target.transform.position;

            Quaternion lookRotation = Quaternion.LookRotation((to - transform.position).normalized);

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turn_speed);


            _faceTheTarget = checkLimits();
                
        rotating(_faceTheTarget);
          
    }


private void rotating(bool doOrNot)
{

if (doOrNot)
{
    Vector3 from =transform.position;
    Vector3 to = target.transform.position;

    if (!faceX)
        to=new Vector3(from.x,to.y,to.z);
    if (!faceY)
        to=new Vector3(to.x,from.y,to.z);
    if (!faceZ)
        to=new Vector3(to.x,to.y,from.z);

    Quaternion lookRotation = Quaternion.LookRotation((to - from).normalized);




   transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turn_speed);
}
        
        
}

private bool checkLimits()
{
    if (faceTheTarget)
    {
        bool result = true;

        Vector3 targetPos = target.transform.position;

        if (faceInLimitationX)
        {
            if (targetPos.x<minLimits.x||targetPos.x>maxLimits.x)
            {
                result = false;
            }
        }
        if (faceInLimitationY)
        {
            if (targetPos.y<minLimits.y||targetPos.y>maxLimits.y)
            {
                result = false;
            }
        }
        if (faceInLimitationZ)
        {
            if (targetPos.z<minLimits.z||targetPos.z>maxLimits.z)
            {
                result = false;
            }
        }
        

        return result;
    }
    else return false;




}










}
//////-----
//Vector3 rot = lookRotation.eulerAngles; // making  Quaternio vector3

//        if (Vector3.Distance(transform.position, reflectorsFacingPoint.transform.position)==0)
//        {
//             to = mirroredobject.transform.position;
//            lookRotation = Quaternion.LookRotation((to - transform.position).normalized);

            


//        }
//        // rot.y = rot.y - transform.rotation.y;
//        // lookRotation = Quaternion.Euler(rot); // making vector3 Quaternio
//        ////----