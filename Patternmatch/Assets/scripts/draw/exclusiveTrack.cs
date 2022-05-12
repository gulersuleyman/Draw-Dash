using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class exclusiveTrack : MonoBehaviour
{
    [SerializeField] private GameObject objectA;
    private GameObject objectB;
    [SerializeField] private GameObject objectC;
    private Vector3 positionA;
    private Vector3 positionC;
    public bool trackIsOn = false;
    
    private defaultTracker4 trackerB;
    private defaultTracker4 trackerA;
   // --private mouseTrack trackerC;
    public float frontDistance;

    [SerializeField] private bool playerGiveUp;
    [SerializeField] private float tryTime=0.4f;
    
    
    
    void Start()
    {
        objectB = this.gameObject;
        trackerB = this.gameObject.GetComponent<defaultTracker4>();
        trackerA = objectA.GetComponent<defaultTracker4>();
      //--  trackerC = objectC.GetComponent<mouseTrack>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        positionA = objectA.transform.position;
        positionC = objectC.transform.position;

        manageActions();//draw check should check this

    }

    private void manageActions()
    {
        
        if( Input.GetMouseButton(0)/* ||  Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved  */)
        {
//            Debug.Log("!trackIsON is  : "+!trackIsOn);
            if (!trackIsOn)
            {
                Debug.Log(" IM IN ");
            //   trackerB.setOn(false);
              trackerB.enabled = false;
              //  trackerA.setOn(false);
                returnToPlayer();
           
               //---- trackerC.trackMouse();                
                float xDistance = getDif(objectB, objectC, 'x');
                float yDistance = getDif(objectB, objectC, 'y');
                float zDistance = getDif(objectB, objectC, 'z');
                
                trackerB.setOffset(-xDistance,-yDistance,-zDistance);
                
              
              
                trackerB.enabled = true;
            //  trackerB.setOn(true);
               // trackerA.setOn(true);
                trackIsOn = true;
            } 
          
             
            
        }
        else if  (trackIsOn /*|| (trackIsOn&& Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)*/)  // (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended|| Input.GetMouseButtonUp(0))
        {
            Debug.Log(" IM OUT ");
            trackerB.enabled = false;
          //  trackerB.setOn(false);
            trackIsOn = false;
            if (playerGiveUp)
            {
                Invoke(nameof(returnToPlayer),tryTime);
                
            }
            
        }

        
        
    }

    private void returnToPlayer()
    {
        if (!trackIsOn)
        {
            objectB.transform.position = ( objectA.transform.position+(objectA.transform.forward*frontDistance)); 
            //trackerA.setOn(false);
        }
            
    }
    private float getDif(GameObject A, GameObject B, char S)
    {
        float result = 0;

        switch (S)
        {
            case 'X':case 'x':
                result = B.transform.position.x-A.transform.position.x;
                break;
            case 'Y':case 'y':
                result=B.transform.position.y-A.transform.position.y;
                break;
            case 'Z':case 'z':
                result=B.transform.position.z-A.transform.position.z;
                break;
        }

        return result;
    }
}
