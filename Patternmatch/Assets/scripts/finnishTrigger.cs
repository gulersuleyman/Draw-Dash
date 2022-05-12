using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class finnishTrigger : MonoBehaviour
{
    [SerializeField] private bool working=true;
    [SerializeField] private bool rotating;
    //[SerializeField] private bool negativeTurn;// not working
    [SerializeField] private Vector3 desiredReachTimes=new Vector3(1,1,1);
    [SerializeField] private Vector3 desiredAngles=new Vector3(0,0,0);
    [SerializeField] private bool turnX;
    [SerializeField] private bool turnY;
    [SerializeField] private bool turnZ;
     
    [SerializeField] private float xSpeed;
    [SerializeField] private float ySpeed;
    [SerializeField] private float zSpeed;
    
    [SerializeField] GameObject [] oldKatanas=new GameObject[2];
    [SerializeField] GameObject [] newKatanas=new GameObject[2];

    public Animator _animator;
    [SerializeField] Transform playerT;

    public Quaternion currentRotation;

    [SerializeField] private Transform hangingPoint;
    [SerializeField] private Vector3 _hangingPoint;
    
    [SerializeField] private List<GameObject> carnageCluster=new List<GameObject>();
    private int meatCount = 0;

    private float nextSceneSkipTime;
    private bool onceHappened = false;
    
    // Start is called before the first frame update
    void Start()
    {
        findSpeed();
        _hangingPoint = hangingPoint.position;
        nextSceneSkipTime = GameManager.Instance.nextSceneSkipTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (working)
        {
             
            if (rotating&&(turnX||turnY||turnZ))
            {
                rotate(playerT);
            }
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.levelFinished = true;
            closeOldKatana();
            Vector3 finishPoint = new Vector3(0, playerT.position.y, playerT.position.z);
            transform.LookAt(finishPoint);
            playerT.DOMove(finishPoint, 0.5f, false).OnComplete(() =>
            {
                rotating = true;
            });
            
            activateAnimation();

        }
    }

    private void closeOldKatana()
    {

        foreach (var e in oldKatanas)
        {
            e.SetActive(false);
        }



    }

    private void openNewKatana()
    {

        foreach (var e in newKatanas)
        {
            e.SetActive(true);
        }


    }

    public void collectBodyParts(GameObject meat,float time)
    {
     carnageCluster.Add(meat);
     Invoke(nameof(hangTheMeat),time);

    }

    private void hangTheMeat()
    {
        carnageCluster[meatCount].GetComponent<MeshRenderer>().enabled = false;
        carnageCluster[meatCount].GetComponent<BoxCollider>().isTrigger = true;
        carnageCluster[meatCount].GetComponent<Rigidbody>().isKinematic = true;
        carnageCluster[meatCount].transform.position = _hangingPoint;
        meatCount++;
    }

    private void releaseTheMeat()
    {

        foreach (var e in carnageCluster)
        {
            e.GetComponent<MeshRenderer>().enabled = true;
            e.GetComponent<BoxCollider>().isTrigger = false;
            e.GetComponent<Rigidbody>().isKinematic = false;
             
        }


    }


    private void rotate(Transform subject)
    {
         currentRotation = subject.rotation; 
        
        if (turnX)
            subject.rotation = Quaternion.RotateTowards(subject.rotation, Quaternion.Euler(desiredAngles.x, currentRotation.y, currentRotation.z), xSpeed * Time.deltaTime);
        
        if (turnY)
            subject.rotation = Quaternion.RotateTowards(subject.rotation, Quaternion.Euler(currentRotation.x, desiredAngles.y, currentRotation.z), ySpeed * Time.deltaTime);
        
        if (turnZ)
            subject.rotation = Quaternion.RotateTowards(subject.rotation, Quaternion.Euler(currentRotation.x, currentRotation.y, desiredAngles.z), zSpeed * Time.deltaTime);

        if (Math.Abs(currentRotation.x - desiredAngles.x) < 0.01f)
            turnX = false;
        if (Math.Abs(currentRotation.y - desiredAngles.y) < 0.01f)
            turnY = false;
        if (Math.Abs(currentRotation.z - desiredAngles.z) < 0.01f)
            turnZ = false;


       // Debug.Log("currentRotation.y  :  "+currentRotation.y);
        if ((Math.Abs(currentRotation.y + 1) < 0.2)||(Math.Abs(currentRotation.y - 1) < 0.2))
        {
          //  Debug.Log("TURN COMPLETE");
            //openNewKatana();
           // releaseTheMeat();

        }
        
        
    }

    private void activateAnimation()
    {
        _animator.SetBool("end", true);
        if (!onceHappened)
        {
            Invoke(nameof(triggerNextSceneLoad),nextSceneSkipTime);
            onceHappened = true;
        }
        

    }

    private void triggerNextSceneLoad()
    {

        GameManager.Instance.LoadNextLevel();


    }



    private void findSpeed()
    {

        desiredReachTimes.x = desiredReachTimes.x < 0.1f ? 0.1f : desiredReachTimes.x;
        desiredReachTimes.y = desiredReachTimes.y < 0.1f ? 0.1f : desiredReachTimes.y;
        desiredReachTimes.z = desiredReachTimes.z < 0.1f ? 0.1f : desiredReachTimes.z;

        Quaternion currentRotation = transform.rotation;

        xSpeed = (desiredAngles.x - currentRotation.x) / desiredReachTimes.x;
        ySpeed = (desiredAngles.y - currentRotation.y) / desiredReachTimes.y;
        zSpeed = (desiredAngles.z - currentRotation.z) / desiredReachTimes.z;



    }
 
    public void rotateOn()
    {
        rotating = true;
    }
    
    public void rotateOff()
    {
        rotating = false;
    }
}
