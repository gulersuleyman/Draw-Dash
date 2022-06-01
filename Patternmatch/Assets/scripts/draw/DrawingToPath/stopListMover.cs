using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
public class stopListMover : MonoBehaviour
{
    [SerializeField] GameObject waitDamage;

    [SerializeField] Transform center;
    

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
    
    [SerializeField] private bool useForce = false;

    [SerializeField] private GameObject thisObject;
    private GameObject objectA;
    
     
    private Vector3 aPos;
    [SerializeField] private Vector3 bPos;
    
    private Rigidbody rb;
    
    [SerializeField] private int targetCount = 30;
    [SerializeField] private List<Vector3> stops=new List<Vector3>();
    
    
    private float totalDifference = 0;
    private Vector3 offset = new Vector3(0,0,0);
    private Vector3 cameraFirstOffset;
    private bool itsFinal = false;
    private bool moveCamera = false;

    bool targetMove;
    public bool canDie;

    AnimationController _animationController;
    PlayerCollision _playerCollision;
    GunTargetPosition _gunTarget;
    Rigidbody _rigidbody;
    CinemachineVirtualCamera _vcam;
    void Start()
    {
        
        
        _rigidbody = GetComponent<Rigidbody>();
        _gunTarget = FindObjectOfType<GunTargetPosition>();
        _animationController = GetComponent<AnimationController>();
        _playerCollision = GetComponent<PlayerCollision>();
        _vcam = FindObjectOfType<CinemachineVirtualCamera>();

        setEssentials();

        canDie = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
       
        

        if (GameManager.Instance.levelFinished)
        {
            GhostTrail.work = false;
        }
        
        if (pathCompleted && GameManager.Instance.getPathActive())
        {
            GameManager.Instance.setPathActive( false);
            stops.Clear();
            stops=new List<Vector3>();
            working = false;
            CameraMove(84.7f);
            canDie = true;
            
        }
        
        
        if (!working&&(!GameManager.Instance.levelFinished))
        {
            if (itsFinal)
            {
                CinemachineTransposer transposer = _vcam.GetCinemachineComponent<CinemachineTransposer>();

                canDie = true;
                Quaternion currentRotation = transform.rotation;

               // if (!GameManager.Instance.levelFinished)
                   // transform.LookAt(center);
               
              transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(currentRotation.x, -transform.position.x *1.5f,currentRotation.z), rotationSpeed * Time.deltaTime);
                transposer.m_FollowOffset = new Vector3(transform.position.x / 3f, transposer.m_FollowOffset.y, transposer.m_FollowOffset.z);
            }
        }
        if(!working) return;

        
        
        if (pathCompleted)
        {
            transform.LookAt(_gunTarget.transform.position);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

            canDie = true;
            _animationController.AttackAnimation(false);
            
            Vector3 target = new Vector3(transform.position.x,transform.position.y,returnCenterZ);
            transform.position = Vector3.MoveTowards(transform.position, target, returnSpeed * Time.deltaTime);
            StartCoroutine(waitDamageActive());
        }
        else 
        {
            if (GameManager.Instance.levelFinished) return;

            canDie = false;

            _animationController.AttackAnimation(true);
            
            Movement();
            CameraMove(70);
        }
        
        

    }
    private void CameraMove(float yPos)
    {
        CinemachineTransposer transposer = _vcam.GetCinemachineComponent<CinemachineTransposer>();
        Vector3 offset;
        offset = transposer.m_FollowOffset;
        offset = Vector3.MoveTowards(offset, new Vector3(offset.x, yPos, offset.z), 0.25f);
        
        transposer.m_FollowOffset = offset;
        
    }

    private void Movement()
    {
        if(GameManager.Instance.levelFinished) return;
        
        
        float perTime = time / (stops.Count + 1);
        if (stops.Count < 0)
        {
            perTime = (time/5) / (stops.Count + 1);
        }
        if (targetCount >= stops.Count)
        {
            pathCompleted = true;
            GameManager.Instance.setPathActive( false);
            stops.Clear();
            stops=new List<Vector3>();
            working = false;
            GhostTrail.work = false;
            if (!GameManager.Instance.levelFinished)
                transform.LookAt(center);
           // transform.LookAt(_gunTarget.transform.position);
            _animationController.AttackAnimation(false);
            StartCoroutine(_playerCollision.WaitDamage());

        }
        else
        {
           
             if(!_playerCollision.wallHit)
            {
                
                GhostTrail.work = true;
                itsFinal = false;
                

                MoveLine(targetCount);
                targetMove = true;
                if(targetMove)
                Invoke("timer", 0.2f);

            }
            else
            {
                pathCompleted = true;
                GameManager.Instance.setPathActive(false);
                stops.Clear();
                stops = new List<Vector3>();
                working = false;
                GhostTrail.work = false;

                if(!GameManager.Instance.levelFinished)
                  transform.LookAt(center);
                // transform.LookAt(_gunTarget.transform.position);
                
                _animationController.AttackAnimation(false);
                _playerCollision.wallHit = false;
            }
            
        }
    }

    void MoveLine(int value)
    {
        _rigidbody.MovePosition(stops[value]);
    }
    void timer()
    {

        if(targetMove)
        targetCount++;

        targetMove = false;
        
    } 
    public void WallHit()
    {
        pathCompleted = true;
        GhostTrail.work = false;
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
        

    }
    
    private void velocityMove()
    {   Vector3 direction=new Vector3(0,0,0);
        
        rb.velocity = direction;//  direction * speed;
        
        var aPosition = objectA.transform.position;
        var bPosition = bPos;
         
        float xDifference = xAxisExecute ? ((bPosition.x+offset.x)-aPosition.x):0 ;
        float yDifference = yAxisExecute ? ((bPosition.y+offset.y)-aPosition.y ):0 ;
        float zDifference = zAxisExecute ? ((bPosition.z+offset.z)-aPosition.z):0  ;

        totalDifference = Math.Abs(xDifference) + Math.Abs(yDifference) + Math.Abs(zDifference);
        float xPowerFactor = ((Math.Abs(totalDifference) < 0.000001f)) ? 0 : Math.Abs(xDifference /  totalDifference );
        float yPowerFactor = ((Math.Abs(totalDifference) < 0.000001f)) ? 0 : Math.Abs(yDifference /  totalDifference );
        float zPowerFactor = ((Math.Abs(totalDifference) < 0.000001f)) ? 0 : Math.Abs(zDifference /  totalDifference);
        
       
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

    }

    IEnumerator waitDamageActive()
	{
        waitDamage.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.4f);
        waitDamage.gameObject.SetActive(false);
	}
}
