using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class visualIndicator : MonoBehaviour
{
    public float startDistance = 2f;

    private TrailRenderer trail;

    [SerializeField] private bool drawingActive = false;

    private GameObject subject;

    public ParticleSystem particle;


    [SerializeField] private float visualTopLimitScreenPercentage=50;
    [SerializeField] private float visualBottomLimitScreenPercentage=10;
    [SerializeField] private float rVisualSideLimitScreenPercentage=90;
    [SerializeField] private float lVisualSideLimitScreenPercentage=10;
    [SerializeField] private bool useFixedRatio=false;
    [SerializeField] private int [] x_y_Ratio=new int[2];
    
    private bool masterAssignmentHappened = false;
    public float _hMostLimit;
    public float _hLeastLimit;
    public float _vMostLimit;
    public float _vLeastLimit;
    
    private bool useDrawActivator=false;
    
    void Start()
    { 
        trail = GetComponent<TrailRenderer>();
        subject= GameObject.FindGameObjectWithTag("drawActivetor");
      //  setBorders();
        if (subject!=null)
        {
            useDrawActivator = true;
        }

    }
   

     
    void Update()
    {
        if (useDrawActivator)
            drawingActive = subject.activeSelf;
        
        
        move();
       
        if (Input.GetMouseButtonDown(0)&&!GameManager.Instance.isMoving&&drawingActive)
        {
            draw();
        }
 
        if (Input.GetMouseButtonUp(0)||GameManager.Instance.isMoving)
        {
            clear();
        }

    }

    void draw()
    {
        if (validateInput('h')&&validateInput('v'))
        {
            trail.enabled = true;
            particle.Play();
        }
        
    }

    void clear()
    {
        if (trail.enabled)
        {
            trail.Clear();
            particle.Stop();
            trail.enabled = false;
        }
        
    
    }
    

    void move()
    {
        Vector3 temp = Input.mousePosition;
        temp.z = startDistance;
        
        Vector3 position=Camera.main.ScreenToWorldPoint(temp); 

        if (!validateInput('h'))
        {
            position.x = transform.position.x;
           // position.z =transform.position.z;
        }

        if (!validateInput('v'))
        {
            position.y = transform.position.y;
            position.z =transform.position.z;
        }
    
        transform.position = position;
        
    }
    
    public bool validateInput(char A)
    {
        //-----delete later
        if ((A == 'x')||(A == 'X')) A = 'h';
        if ((A == 'y')||(A == 'Y')) A = 'v';
        //---- delete later
        switch (A)
        {
            case 'h':case 'H':
                if (Input.mousePosition.x > _hMostLimit)
                    return false;
                else if (Input.mousePosition.x < _hLeastLimit)
                    return false;
                break;
            case 'v':case 'V':
                if (Input.mousePosition.y > _vMostLimit)
                    return false;
                else if (Input.mousePosition.y < _vLeastLimit)
                    return false;
                break;
            default:
                //  Debug.Log("PROBLEM HAPPENED  1");
                return false;
        }

        return true;
    }

    public void setLimits(float hMost,float hLeast,float vMost,float vLeast)
    {

        _hMostLimit = hMost;
        _hLeastLimit = hLeast;
        
        _vMostLimit = vMost;
        _vLeastLimit = vLeast;

        masterAssignmentHappened = true;


    }

    private void setBorders()
    {

        if(masterAssignmentHappened) return;
        
        _vMostLimit = (Screen.height / 100 )* visualTopLimitScreenPercentage;  //  Mathf.Round(Screen.height / 100) * yMostLimit;
        _vLeastLimit =( Screen.height / 100) * visualBottomLimitScreenPercentage;
        _hMostLimit = ((Screen.width / 100) * rVisualSideLimitScreenPercentage)+(( Screen.width- ((Screen.width / 100)*100))/2);
        _hLeastLimit= ((Screen.width / 100) * lVisualSideLimitScreenPercentage)+(( Screen.width- ((Screen.width / 100)*100))/2);

        if (useFixedRatio)
        {
            float width = _hMostLimit - _hLeastLimit;
            float oneUnit = (width / x_y_Ratio[0]);
            _vMostLimit = _vLeastLimit + (oneUnit * x_y_Ratio[1]);
        }


    }

     public void on_Or_Off(bool do_Or_DoNot)
    {

        if (do_Or_DoNot)
        {
            drawingActive = true;
        }
        else
        {
            drawingActive = false;
        }



    }



}
