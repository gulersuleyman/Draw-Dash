
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ragdoll1 : MonoBehaviour
{
    public float hitPower;
    public bool RagdollActivator;
    
    public Animator ThiefCharacterAnim;
   
    public bool addforce;
    public List<Rigidbody> RB = new List<Rigidbody>();

    public Rigidbody rb;

    EnemyController _enemyController;
    PlayerController _playerController;
    MinyonController _minyonController;

    bool test;
    void Start()
    {
     
        ragdolClose();
        //  parentCol(true);

        _playerController = FindObjectOfType<PlayerController>();
        _enemyController = GetComponentInParent<EnemyController>();
        _minyonController = GetComponentInParent<MinyonController>();
        
    }

    private void Update()
    {
       /* if (RagdollActivator)
        {
           // ragdollOpen();
            
        }
        else
        {
           // ragdolClose();
        }*/


    }

    public void falss()
    {

        addforce = false;
    }

    void ragdolClose()
    {
        physiccState(true);
        isKinematic(true);
        capsuleTrigger(true);
        boxColliderTrigger(true);
        capsuleColl(false);
        boxCollider(false);
        SphereTrigger(true);
        SphereCollider(false);
    }
    
    void physiccState(bool state)
    {
        Rigidbody[] rg = GetComponentsInChildren<Rigidbody>();

        foreach (var VARIABLE in rg)
        {
            VARIABLE.useGravity = state;
        }
   
    }   
    void isKinematic(bool state)
    {
        Rigidbody[] rg = GetComponentsInChildren<Rigidbody>();

        foreach (var VARIABLE in rg)
        {
            VARIABLE.isKinematic = state;
        }
   
    }
    public void AdForceee(bool state)
    {
        Rigidbody[] rg = GetComponentsInChildren<Rigidbody>();

        
        if (_enemyController.deathWithSword)
        {

            
                Vector3 deathDirection = new Vector3(-_playerController.transform.position.x + _enemyController.transform.position.x, 2f, -_playerController.transform.position.z + _enemyController.transform.position.z).normalized;
                rb.AddForce(deathDirection * hitPower, ForceMode.Impulse);
            
            
             
            foreach (var VARIABLE in rg)
            {
                VARIABLE.AddForce(new Vector3(0f, 2f, 0f));
            }
            
        }
        else
        {

            foreach (var VARIABLE in rg)
            {
                VARIABLE.AddForce(new Vector3(0f, 2f, 0f));
            }
            
        }

        

    }  
    
    void capsuleColl(bool state)
    {
        CapsuleCollider[] col = GetComponentsInChildren<CapsuleCollider>();

        foreach (var VARIABLE in col)
        {
            VARIABLE.enabled = state;
            
        }

    }
    void capsuleTrigger(bool state)
    {
        CapsuleCollider[] col = GetComponentsInChildren<CapsuleCollider>();

        foreach (var VARIABLE in col)
        {
            VARIABLE.enabled = state;
            
        }

    } 
    void SphereCollider(bool state)
    {
        SphereCollider[] col = GetComponentsInChildren<SphereCollider>();

        foreach (var VARIABLE in col)
        {
            VARIABLE.enabled = state;
            
        }

    } 
    void SphereTrigger(bool state)
    {
        SphereCollider[] col = GetComponentsInChildren<SphereCollider>();

        foreach (var VARIABLE in col)
        {
            VARIABLE.isTrigger = state;
            
        }

    } 
   
    void parentCol(bool state)
    {
        CapsuleCollider[] col = GetComponentsInParent<CapsuleCollider>();

        foreach (var VARIABLE in col)
        {
            VARIABLE.isTrigger = state;
        }

    } 

    void RigidbodyParentClosed(bool state)
    {
        Rigidbody rb = GetComponentInParent<Rigidbody>();

        Destroy(rb);


    } 
    void boxCollider(bool state)
    {
        BoxCollider[] col = GetComponentsInChildren<BoxCollider>();

        foreach (var VARIABLE in col)
        {
            VARIABLE.enabled = state;
        }

    }
    void boxColliderTrigger(bool state)
    {
        BoxCollider[] col = GetComponentsInChildren<BoxCollider>();

        foreach (var VARIABLE in col)
        {
            VARIABLE.isTrigger = state;
        }

    }

    public void ragdollOpen()
    {
        ThiefCharacterAnim.enabled = false;
        parentCol(false);
        physiccState(true);
        
        
        isKinematic(false);
        SphereTrigger(false);
        
        boxColliderTrigger(false);
        capsuleTrigger(false);
        SphereCollider(true);
        capsuleColl(true);
        boxCollider(true);
        
            
            // NavmeshControllerClosed(true);
            if (addforce == true)
            {
                AdForceee(true);
            }
            RigidbodyParentClosed(true);

        //  NavmeshClosed(true);


        
            
        
        
    }
    
}
