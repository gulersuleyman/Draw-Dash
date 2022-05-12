using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class defaultTracker4 : MonoBehaviour
{
    [SerializeField] private bool work=true;

    [SerializeField] private bool teleportNow=false;

    [SerializeField] private bool faceTheTarget = false;
    [SerializeField] private float turn_speed = 20;
    [SerializeField] private bool faceX;
    [SerializeField] private bool faceY;
    [SerializeField] private bool faceZ;
    
    
    private bool _faceTheTarget = false;
    
    
    
    public bool willTrack_elseCarry_ =true;
    public Transform objectB;

    public bool  keepCurrentPositions = true;
    public float ifNotFixedX ; 
    public float ifNotFixedY ;
    public float ifNotFixedZ ;

    public bool trackX = false;
    public bool trackY = false;
    public bool trackZ = false;

    public Vector3 offset = new Vector3();
    private Vector3 firstOffset = new Vector3();
    [SerializeField] private bool useRelativeOffset;
    [SerializeField] private Vector3 relativeOffset;
    
    
    public bool useLimitationX =false; 
    public float [] trackXinBetween = new float [2];
    public bool useLimitationY =false ;
    public float [] trackYinBetween = new float [2];
    public bool useLimitationZ =false ;
    public float [] trackZinBetween = new float [2]; 

    public float trackSpeed = 500; 
    public bool useTransformPos = true;
    
    public bool useMoveTowards = false;    
    public bool useLerp = false;
    public bool useTransformTranslate = false;
    public bool useVelocity = false;
    [SerializeField] private bool useTwoStagePower;
    [SerializeField] private float stageChangeDistance=1f;
    [SerializeField] private float divideMainPowerBY=2f;
    
     

    private Transform objectA;
    private Vector3 aPos;
    private Vector3 bPos;
    
    public float freePosX ; 
    public float freePosY ;
    public float freePosZ ;


    private float totalDifference = 0;
    

    private Rigidbody rb;

    private void Start()
    {
        _faceTheTarget = faceTheTarget;

        takeFirstAdditionalDistances();
         

        if (willTrack_elseCarry_ )
        {
            if(objectB==null)
             objectB = findPlayer().transform;
            
            objectA = this.gameObject.transform;
        }
        else
        {
            objectA = objectB;
            objectB=this.gameObject.transform;
        }

        if (!keepCurrentPositions)
        {
            moveToPosition(ifNotFixedX, ifNotFixedY, ifNotFixedZ);
        }

        updateAB_Pos();

        validateLimitations();
        
        rb =  objectA.GetComponent<Rigidbody>();
        
    }

    
    void FixedUpdate()
    {
        rotating(_faceTheTarget);
        
        updateAB_Pos(); // maybe dont need to be outside  ? 
        updateFreePos();
        
        if (work)
        {
            updateAB_Pos();
            updateFreePos();
            move();
        }

        if (teleportNow)
        {
            teleportToTarget();
            teleportNow = false;
        }

        
        


    }

    private void rotating(bool doOrNot)
    {

        if (doOrNot)
        {
            float speed = turn_speed;
            Vector3 from = objectA.transform.position;
            Vector3 to = objectB.transform.position;

            if (!faceX)
                to=new Vector3(from.x,to.y,to.z);
            if (!faceY)
                to=new Vector3(to.x,from.y,to.z);
            if (!faceZ)
                to=new Vector3(to.x,to.y,from.z);

            Quaternion lookRotation = Quaternion.LookRotation((to - from).normalized);


            if (useTwoStagePower&&(totalDifference<stageChangeDistance))
            {
                speed /= divideMainPowerBY;
            }

            

            objectA.transform.rotation = Quaternion.Slerp(objectA.transform.rotation, lookRotation, Time.deltaTime * speed);
        }
        
        
    }

    public void setRelativeOffsets(float x, float y, float z)//no need
    {
         relativeOffset=new Vector3(x,y,z);
    }

  

    public void resetOffset()
    {
        
        offset=new Vector3(firstOffset.x,firstOffset.y,firstOffset.z);
    }
    
    public void calibrateOffset(float offsetX,float offsetY, float offsetZ)
    {
        
        offset=new Vector3(offset.x+offsetX,offset.y+offsetY,offset.z+offsetZ);
    }
    public void setOffset(float offsetX,float offsetY, float offsetZ)
    {
        
        offset=new Vector3(offsetX,offsetY,offsetZ);
    }

    public void teleportToTarget()
    {
        var aPosition = objectA.transform.position;
        var bPosition = objectB.transform.position;

        float theX = trackX ? (bPosition.x + offset.x) : aPosition.x;
        float theY = trackY ? (bPosition.y + offset.y) : aPosition.y;
        float theZ = trackZ ? (bPosition.z + offset.z) : aPosition.z;
        
        Vector3 pointToReach=new Vector3(theX,theY,theZ);
        objectA.transform.position = pointToReach;
    }
    
    private void move()
    {
        float targetX =( (trackX &&  checkLimitation('x'))? ( bPos.x + offset.x ) : freePosX );
        float targetY =( (trackY &&  checkLimitation('x') )? ( bPos.y + offset.y ) : freePosY );
        float targetZ =( (trackZ &&  checkLimitation('x') )? ( bPos.z + offset.z ) : freePosZ );

        Vector3 target = new Vector3(targetX, targetY, targetZ);
        if (useTransformPos)
            objectA.position = target;

        else if (useMoveTowards)
        {
            objectA.position = Vector3.MoveTowards(objectA.position, target, Time.deltaTime * trackSpeed);
        }
        else if (useLerp)
        {
            objectA.position = Vector3.Lerp(objectA.position, target, Time.deltaTime * trackSpeed/100);
        }
        else if (useTransformTranslate) 
        {
            transformTranslateMovement();
        }
        else if (useVelocity)
        {
            velocityMove();
        }
        //  objectA.Translate(target * (Time.deltaTime * trackSpeed), spaceSelf ? Space.Self : Space.World  ); // not sure how will work

        // Vector3.up "+y"   Vector3.forward  "+z"  transform.forward "objects facing direction" 


    }

    private void velocityMove()
    {   Vector3 direction=new Vector3(0,0,0);
        
        rb.velocity = direction;//  direction * speed;
        
        float speed = trackSpeed / 100;
        
        var aPosition = objectA.position;
        var bPosition = objectB.position;
         
        float xDifference = trackX ? ((bPosition.x+offset.x)-aPosition.x):0 ;
        float yDifference = trackY ? ((bPosition.y+offset.y)-aPosition.y ):0 ;
        float zDifference = trackZ ? ((bPosition.z+offset.z)-aPosition.z):0  ;
//        Debug.Log("zAxis target  : "+(bPosition.z+offset.z+"  zoffset  : "+offset.z));

        totalDifference = Math.Abs(xDifference) + Math.Abs(yDifference) + Math.Abs(zDifference);
        float xPowerFactor = ((Math.Abs(totalDifference) < 0.1f)) ? 0 : Math.Abs(xDifference /  totalDifference );
        float yPowerFactor = ((Math.Abs(totalDifference) < 0.1f)) ? 0 : Math.Abs(yDifference /  totalDifference );
        float zPowerFactor = ((Math.Abs(totalDifference) < 0.1f)) ? 0 : Math.Abs(zDifference /  totalDifference);
        
       // Debug.Log("  xDifference : "+xDifference+"  yDifference : "+yDifference+"  zDifference : "+zDifference);
       // Debug.Log("power total must be : "+speed+"  xAxisPower : "+(speed *xPowerFactor)+"  yAxisPower : "+(speed *yPowerFactor)+"  zAxisPower : "+(speed *zPowerFactor));
       // Debug.Log("power total is : "+(speed *(xPowerFactor+yPowerFactor+zPowerFactor))+"  xpowerFactor : "+(xPowerFactor)+"  ypowerFactor : "+(yPowerFactor)+"  zpowerFactor : "+(zPowerFactor));

        if (trackX && (Math.Abs(xDifference) > 0.1f))
        {
             
            direction=new Vector3(((xDifference > 0) ? 1 : -1),direction.y,direction.z);
        }
        if (trackY && (Math.Abs(yDifference) > 0.1f))
        {
             
            direction=new Vector3(direction.x,((yDifference > 0) ? 1 : -1),direction.z);
        }
        if (trackZ && (Math.Abs(zDifference) > 0.1f))
        {
            direction=new Vector3(direction.x,direction.y,((zDifference > 0) ? 1 : -1));
        }

        if (useTwoStagePower&&(totalDifference<stageChangeDistance))
        {
            speed /= divideMainPowerBY;
        }
        
        
        rb.velocity = new Vector3(direction.x*(speed*xPowerFactor),direction.y*(speed*yPowerFactor),direction.z*(speed*zPowerFactor));//  direction * speed;
      //  Debug.Log("direction :  "+direction +"  speed : "+speed);

      if ((Math.Abs(totalDifference) < 0.1f))
      {
          _faceTheTarget = false;
      }
      else _faceTheTarget = faceTheTarget;



    }

    private void transformTranslateMovement()
    {
        
        float reverse = 1;
        float speed = trackSpeed / 100;
        var aPosition = objectA.position;
        var bPosition = objectB.position;
        float xDifference = aPosition.x - bPosition.x-offset.x;
        float yDifference = aPosition.y - bPosition.y-offset.y;
        float zDifference = aPosition.z - bPosition.z-offset.z;
            
            
        if (trackX&&(Math.Abs(xDifference) > 0.2f))  // why not transform position
        {
            reverse = (xDifference < 0) ? 1 : -1;
            transform.Translate(Vector3.forward * (Time.deltaTime*speed* reverse));
        }
        reverse = 1;
        if (trackY&&(Math.Abs(yDifference) > 0.2f))
        {
            reverse = (yDifference < 0) ? 1 : -1;
            transform.Translate(Vector3.up * (Time.deltaTime*speed* reverse));
        }
        reverse = 1;
        if (trackZ&&(Math.Abs(zDifference) > 0.2f))
        {
            reverse = (zDifference < 0) ? 1 : -1;
            transform.Translate(Vector3.right * (Time.deltaTime*speed* reverse));
        }
        
    }

    private GameObject findPlayer()
    {
        GameObject result = findObject("player");
        if (result==null)
            result = findObject("Player");
        else if (result==null)
            result = findObject("PLAYER");
       
        return result;
 
    }
    
    private GameObject findObject( string word ) // finding object first name if not tag if not layer
    { 
        var  result = GameObject.Find(word);
        if(result==null)
            result=GameObject.FindWithTag(word);
        
        return result;
    }

    private void moveToPosition(float fixedX,float fixedY,float fixedZ)
    {
        objectA.position=new Vector3(fixedX,fixedY,fixedZ);
    }

    private void updateFreePos()
    {
        if (keepCurrentPositions)
        {
            freePosX = aPos.x;
            freePosY = aPos.y;
            freePosZ = aPos.z;
        }
        else
        {
            freePosX = ifNotFixedX;
            freePosY = ifNotFixedY;
            freePosZ = ifNotFixedZ;
        }
       
    }

    private void validateLimitations()
    {
        useLimitationX = !(trackXinBetween[0]>=trackXinBetween[1])&&useLimitationX;
         
        useLimitationY = !(trackYinBetween[0]>=trackYinBetween[1])&&useLimitationY;
        
        useLimitationZ = !(trackZinBetween[0]>=trackZinBetween[1])&&useLimitationZ;
    }
    private bool checkLimitation(char vector)
    {
        bool result = true;
        switch (vector)
        {
            case 'x':
                if (useLimitationX && ((aPos.x < trackXinBetween[0]) || (aPos.x > trackXinBetween[1])))
                    result = false;
                break;
                
            case 'y':
                if (useLimitationY && ((aPos.y < trackYinBetween[0]) || (aPos.y > trackYinBetween[1])))
                    result = false;
                break;
            case 'z':
                if (useLimitationZ && ((aPos.z < trackZinBetween[0]) || (aPos.z > trackZinBetween[1])))
                    result = false;
                break;
            default: break;
        }

        return result;
         
    }

    private void updateAB_Pos()
    {
        aPos = objectA.position;
        bPos = objectB.position;

        if (useRelativeOffset)
        {
            updateRelativeOffsets( );
        }
    }
    
    private void updateRelativeOffsets( )
    { 
       offset= ((objectB.right*relativeOffset.x)+(objectB.up*relativeOffset.y)+(objectB.forward*relativeOffset.z) );

    }

    private void takeFirstAdditionalDistances()
    {
        firstOffset=new Vector3(offset.x,offset.y,offset.z);
    }

    public void updateAdditionalDistances(float X,float Y,float Z)
    {
        offset=new Vector3(firstOffset.x+X,firstOffset.y+Y,firstOffset.z+Z);
    }

    public void setOn(bool input)
    {
        work = input;
    }
}
