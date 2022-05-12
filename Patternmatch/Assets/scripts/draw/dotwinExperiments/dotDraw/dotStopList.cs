using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class dotStopList : MonoBehaviour
{
    [SerializeField] private bool working =false;
    [SerializeField] private bool pathCompleted=false;
    [SerializeField] private float speed=10;
    [SerializeField] private float rotationSpeed=20;
    
    [SerializeField] private float returnSpeed=5;
    [SerializeField] private float returnCenterZ=68;

    [SerializeField] private float time=1;
    
    
    [SerializeField] private bool xAxisExecute;// xAxisExecute yAxisExecute zAxisExecute
    [SerializeField] private bool yAxisExecute;
    [SerializeField] private bool zAxisExecute;
    
    
    [SerializeField] private bool useMoveTowards = false;    
    [SerializeField] private bool useLerp = false;
    [SerializeField] private bool useTransformTranslate = false;
    [SerializeField] private bool useForce = false;

    [SerializeField] private GameObject thisObject;
    private GameObject objectA;
    
     
    private Vector3 aPos;
    [SerializeField] private Vector3 bPos;
    
    private Rigidbody rb;
    
    [SerializeField] private int targetCount = 0;
    [SerializeField] private List<Vector3> stops=new List<Vector3>();
    
    
    private float totalDifference = 0;
    private Vector3 offset = new Vector3(0,0,0);

    private bool itsFinal = false;
    void Start()
    {
        setEssentials();
    }

    // Update is called once per frame
    void Update()
    {
        
        
        
        if (pathCompleted && GameManager.Instance.getPathActive())
        {
            GameManager.Instance.setPathActive( false);
            stops.Clear();
            stops=new List<Vector3>();
            working = false;
        }

        if (pathCompleted)
        {
            Vector3 target = new Vector3(transform.position.x,transform.position.y,returnCenterZ);
            transform.position = Vector3.MoveTowards(transform.position, target, returnSpeed * Time.deltaTime);
        }
        
        if (!working)
        {
            if (itsFinal)
            {
                Quaternion currentRotation = transform.rotation; 
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(currentRotation.x, 0,currentRotation.z), rotationSpeed * Time.deltaTime);
            }
        }
        if(!working) return;

        
        
        if (pathCompleted)
        {
            Vector3 target = new Vector3(transform.position.x,transform.position.y,returnCenterZ);
            transform.position = Vector3.MoveTowards(transform.position, target, returnSpeed * Time.deltaTime);
        }
        else
        {
            Movement();
        }


        //move();

    }

    private void Movement()
    {
        
        
        float perTime = time / (stops.Count + 1);
        if (targetCount >= stops.Count)
        {
            pathCompleted = true;
            GameManager.Instance.setPathActive( false);
            stops.Clear();
            stops=new List<Vector3>();
            working = false;
            GhostTrail.work = false;
        }
        else
        {
            GhostTrail.work = true;
            itsFinal = false;
            transform.LookAt(stops[targetCount]);
            this.transform.DOMove(stops[targetCount], perTime, false).OnComplete(() =>
            {
                targetCount++;
                itsFinal = true;
                
            });
        }
        
    }

    private void setEssentials()
    {

        if (thisObject==null)
        {
            
            thisObject = this.gameObject;
        }
        objectA = thisObject;
         
        
        if (objectA.GetComponent<Rigidbody>()==null&&useForce)
        {
            objectA.AddComponent<Rigidbody>();
        }
        rb = objectA.GetComponent<Rigidbody>();

    }

    private void checkReach()
    {
        
        int nextTarget = targetCount + 1;

        aPos = objectA.transform.position;
        
        bool hasReached = closeEnough(aPos,bPos,0.0001f);

        if (hasReached) Debug.Log("STOP REACHED  : "+ targetCount);
        
            
         

          
        if (hasReached&&!outOfThePath(nextTarget))
        {
            setTarget(nextTarget);
        }
         

    }

    private bool closeEnough(Vector3 A,Vector3 B,float minimumDistance)
    {

        return ( ( Math.Abs(A.x-B.x)+  Math.Abs( A.y-B.y )+  Math.Abs(A.z-B.z  )   )<minimumDistance  );


    }

    private bool outOfThePath(int input)
    {
         
        
        if (input>=stops.Count)
        {
            Debug.Log("NEXT TARGET OUT OF PATH target no "+input);
            pathCompleted = true;
            GameManager.Instance.setPathActive( false);
            stops.Clear();
            stops=new List<Vector3>();
            working = false;
            return true;
        }
        else
        {
            return false;
        }

         

    }

    private void setTarget(int targetNumber )
    {

        bPos = stops[targetNumber];
        Debug.Log(" NEW TARGET NUMBER IS : "+targetNumber+"   "+stops[targetNumber]);
        targetCount = targetNumber;


    }


    private void move()
    {

        checkReach();
        
        if (useForce)
        {
            velocityMove();
        }
        
    }

    public void setNewStops(List<Vector3> newList)
    {

        stops = newList;
        targetCount = 0;

        bPos = stops[targetCount];

        pathCompleted = false;
        working = true;
        Debug.Log(" new stops has been set ");
        Debug.Log(" stop count is : "+newList.Count);

    }
    
    private void velocityMove()
    {   Vector3 direction=new Vector3(0,0,0);
        
        rb.velocity = direction;//  direction * speed;
        
        
        
        var aPosition = objectA.transform.position;
        var bPosition = bPos;
         
        float xDifference = xAxisExecute ? ((bPosition.x+offset.x)-aPosition.x):0 ;
        float yDifference = yAxisExecute ? ((bPosition.y+offset.y)-aPosition.y ):0 ;
        float zDifference = zAxisExecute ? ((bPosition.z+offset.z)-aPosition.z):0  ;
//        Debug.Log("zAxis target  : "+(bPosition.z+offset.z+"  zoffset  : "+offset.z));

        totalDifference = Math.Abs(xDifference) + Math.Abs(yDifference) + Math.Abs(zDifference);
        float xPowerFactor = ((Math.Abs(totalDifference) < 0.000001f)) ? 0 : Math.Abs(xDifference /  totalDifference );
        float yPowerFactor = ((Math.Abs(totalDifference) < 0.000001f)) ? 0 : Math.Abs(yDifference /  totalDifference );
        float zPowerFactor = ((Math.Abs(totalDifference) < 0.000001f)) ? 0 : Math.Abs(zDifference /  totalDifference);
        
       // Debug.Log("  xDifference : "+xDifference+"  yDifference : "+yDifference+"  zDifference : "+zDifference);
       // Debug.Log("power total must be : "+speed+"  xAxisPower : "+(speed *xPowerFactor)+"  yAxisPower : "+(speed *yPowerFactor)+"  zAxisPower : "+(speed *zPowerFactor));
       // Debug.Log("power total is : "+(speed *(xPowerFactor+yPowerFactor+zPowerFactor))+"  xpowerFactor : "+(xPowerFactor)+"  ypowerFactor : "+(yPowerFactor)+"  zpowerFactor : "+(zPowerFactor));

        if (xAxisExecute && (Math.Abs(xDifference) > 0.1f))
        {
             
            direction=new Vector3(((xDifference > 0) ? 1 : -1),direction.y,direction.z);
        }
        if (yAxisExecute && (Math.Abs(yDifference) > 0.1f))
        {
             
            direction=new Vector3(direction.x,((yDifference > 0) ? 1 : -1),direction.z);
        }
        if (zAxisExecute && (Math.Abs(zDifference) > 0.1f))
        {
            direction=new Vector3(direction.x,direction.y,((zDifference > 0) ? 1 : -1));
        }

       
        
        
        rb.velocity = new Vector3(direction.x*(speed*xPowerFactor),direction.y*(speed*yPowerFactor),direction.z*(speed*zPowerFactor));//  direction * speed;
      //  Debug.Log("direction :  "+direction +"  speed : "+speed);
 



    }


}

