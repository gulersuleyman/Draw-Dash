using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw_Move : MonoBehaviour
{

    [SerializeField] Transform leftBoundary;
    [SerializeField] Transform rightBoundary;
    [SerializeField] Transform backBoundary;
    [SerializeField] Transform frontBoundary;

    

    [SerializeField] public bool Working;
    [SerializeField] private char horizontalEquivalent='x';
    [SerializeField] private char verticalEquivalent='z';
    [SerializeField] private float forsakenAxisDefault=0;
    [SerializeField] private bool keepPlayersExtraAxis;
    private bool reverseHorizontal = false;
    private bool reverseVertical=false;
    private bool drawingActive = false;
    [SerializeField] private GameObject player;
    [SerializeField] private stopListMover mover;
    [SerializeField] private visualIndicator visual;
    [SerializeField] private adjustUI [] UI;
    [SerializeField] private bool useAccurateLimiting;
    [SerializeField] private bool horizontalWorldLimitation;
    [SerializeField] private float [] hWorldMinMax=new float[2];
    [SerializeField] private bool verticalWorldLimitation;
    [SerializeField] private float [] vWorldMinMax=new float[2];
    [SerializeField] private float underLimitingMaxPossibleScale = 10;
    //----------------
    [SerializeField] private bool extraSimplifyNodes=false; // rename it
    [SerializeField] private float simplifyingPower=10;
    [SerializeField] private bool useNodeLimit=false;
    [SerializeField] private int nodeLimit=1000;
    [SerializeField] private int nodeDownLimit=10;
    [SerializeField] private float desiredMinimumDifferance=1;
    private float _desiredMinimumDifferance=1;
    [SerializeField]  private float scaleFactor=1;
    private float _scaleFactor;
    [SerializeField] private float maxHorizontalResult = 10;
    [SerializeField] private float maxVerticalResult = 10;
    [SerializeField] private bool preserveDrawingShape=true;
    private bool horizontalPrimaryFactor=true;
    //-------------
    [SerializeField] private float visualTopLimitScreenPercentage=50;
    [SerializeField] private float visualBottomLimitScreenPercentage=10;
    [SerializeField] private float rVisualSideLimitScreenPercentage=90;
    [SerializeField] private float lVisualSideLimitScreenPercentage=10;
    [SerializeField] private bool useFixedRatio=false;
    [SerializeField] private int [] x_y_Ratio=new int[2];//514  --  302
    private float _vMostLimit; //pixel cordinates
    private float _vLeastLimit;
    private float _hMostLimit;
    private float _hLeastLimit;
    //-------------------
    private float[] center=new float[2];
    private float[] firstNodeOffset=new float[2];
    private Vector3 tempNodePoint=new Vector3(); 
    private List<Vector3> tempNodeArray= new List<Vector3>();
    private List<Vector3> finalNodeArray= new List<Vector3>();
    private Vector3[] nodePackage;
    [SerializeField] private bool updateVisual;
    [SerializeField] private GameObject pointerObject;
    TrailRenderer _trail;
    [SerializeField] private Transform parent;
    PlayerController _playerController;
    void Start()
    {
        _trail = pointerObject.gameObject.GetComponent<TrailRenderer>();
        FixLimitations();
        hWorldMinMax[0] = leftBoundary.position.x;
        hWorldMinMax[1] = rightBoundary.position.x;
        vWorldMinMax[0] = backBoundary.position.z;
        vWorldMinMax[1] = frontBoundary.position.z;
        _playerController = FindObjectOfType<PlayerController>();
    }
    void Update()
    {

        
        


        if (updateVisual)
        {
            SetNewPositions();
            updateVisual = false;
        }
        CheckGameManager();
        if (FirstTouch())
        {
           
            StartIntake();
        }
        if (FingerHasMoved())
        {
            
            WhileMoving();
        }
        if (DrawingEnded())
        {
            CompleteAction();
        }
    }
    //-----------------------------
    // conflict solving data altering
    private void CheckGameManager()
    {
        if (!GameManager.Instance.getPathActive()&&!GameManager.Instance.levelFinished)
        {
            GameManager.Instance.setPathActive(true);
            on_Or_Off(true);
        }

        if (GameManager.Instance.levelFinished)
        {
            on_Or_Off(false);
        }


    }
    //-------------------------------
    //start processes ----
    // panel position setting of visual indicators
   // and limit reference assigning as in pixel forms
   //conflict resolving 
    private void FixLimitations()// rename and elaborate
    {
        SolveConflict();
        
        SetBorders();
        
        SetBaseElements();
        
        AlignSubordinates();
        
    }
    private void SetBaseElements()
    {
        _desiredMinimumDifferance=(Screen.width / 100) * desiredMinimumDifferance;

        center=GETPanelCenter();

        if (keepPlayersExtraAxis)
        {
            forsakenAxisDefault = GETAxis('f', player.transform.position);
        }

        _scaleFactor = scaleFactor;

    }
    private void SolveConflict()
    {
        if (scaleFactor<=0)
            scaleFactor = 1;
        if (simplifyingPower<=0)
            simplifyingPower = 1;
        if (simplifyingPower>20)
            simplifyingPower = 20;
    }
    private void SetBorders()
    {

        _vMostLimit = (Screen.height / 50 )* visualTopLimitScreenPercentage;  //  Mathf.Round(Screen.height / 100) * yMostLimit;
        _vLeastLimit =( Screen.height / 50) * visualBottomLimitScreenPercentage;
        _hMostLimit = ((Screen.width / 50) * rVisualSideLimitScreenPercentage)+(( Screen.width- ((Screen.width / 100)*100))/2);
        _hLeastLimit= ((Screen.width / 50) * lVisualSideLimitScreenPercentage)+(( Screen.width- ((Screen.width / 100)*100))/2);

        if (useFixedRatio)
        {
            float width = _hMostLimit - _hLeastLimit;
            float oneUnit = (width / x_y_Ratio[0]);
            _vMostLimit = _vLeastLimit + (oneUnit * x_y_Ratio[1]);
        }
    }
    private void AlignSubordinates()
    {
        visual.setLimits(_hMostLimit,_hLeastLimit,_vMostLimit,_vLeastLimit); // gives the right borders to the visual indicator
        foreach (var e in UI)// sets all uı to the shape
        {
            e.getPosition(_hMostLimit,_hLeastLimit,_vMostLimit,_vLeastLimit,center);
        }
    }
    private void SetNewPositions()
    {
        SetBorders();

        SetBaseElements();
        
        AlignSubordinates();

    }
    private void DrawingIntegrity( bool PrimaryFactorHorizontal) //conflict resolving 
    {
        if(!preserveDrawingShape)return;
        
        float horizontalPotential = (_hMostLimit-_hLeastLimit) ;
        float verticalPotential = (_vMostLimit-_vLeastLimit) ;
        float correctionFactor= (PrimaryFactorHorizontal) ? (verticalPotential/horizontalPotential) : (horizontalPotential/verticalPotential) ;

         
        float result;
        if ( PrimaryFactorHorizontal ) 
        {
            maxVerticalResult = (maxHorizontalResult * (correctionFactor));
        }
        else 
        {
            maxHorizontalResult = (maxVerticalResult * (correctionFactor));
        }

    }
    private float[] GETPanelCenter()
    {
        float [] result=new float[2];
        result[0] = (_hMostLimit + _hLeastLimit)/2;
        result[1] = (_vMostLimit +_vLeastLimit)/2;
        return result;
    }
    //            input taking state evaluating ---
    private bool FirstTouch()
    {

        return Input.GetMouseButtonDown(0);

    }
    private bool FingerHasMoved()
    {
        return (Input.GetAxis("Mouse X") != 0) || (Input.GetAxis("Mouse Y") != 0);
    }
    private bool DrawingEnded()
    {

        return Input.GetMouseButtonUp(0);
    }
    //----------------------------------------------
    // post drawing data altering methods---
    private void FinalizeIt()
    {
        if (tempNodeArray==null)
        {
            tempNodeArray=new List<Vector3>();
            return;
        }
        
        tempNodeArray = MakeFirstNodeTheCenter(tempNodeArray);
        if(extraSimplifyNodes)
            tempNodeArray = NodeSimplification(tempNodeArray);
        
        tempNodeArray = ReDescribeNodesForCreation(tempNodeArray, maxHorizontalResult,maxVerticalResult);
        tempNodeArray = AddCurrentLocation(tempNodeArray);
        tempNodeArray = RealWorldLimitation(tempNodeArray);
        finalNodeArray = tempNodeArray;
        
       
        if (useNodeLimit)// this one missing parts
        {
            if (nodeDownLimit <finalNodeArray.Count)
            {
                mover.setNewStops(finalNodeArray);
                on_Or_Off(false);
            }
        }
        else
        {
           mover.setNewStops(finalNodeArray);
           on_Or_Off(false);
        }
        tempNodeArray=new List<Vector3>();

      

    }
    private List<Vector3> RealWorldLimitation(List<Vector3> input)
    {
        List<Vector3> result=new List<Vector3>();

        foreach (var e in input)
        {
            Vector3 temp = MakeNode(GETAxis('h',e),GETAxis('v',e),GETAxis('f',e));
            if (horizontalWorldLimitation)
            {
                if (GETAxis('h',temp)>hWorldMinMax[1])
                {
                    temp = SetAxis('h', temp, hWorldMinMax[1]);
                }
                else if (GETAxis('h',temp)<hWorldMinMax[0])
                {
                    temp = SetAxis('h', temp, hWorldMinMax[0]);
                }
            }
            if (verticalWorldLimitation)
            {
                if (GETAxis('v',temp)>vWorldMinMax[1])
                {
                    temp = SetAxis('v', temp, vWorldMinMax[1]);
                }
                else if (GETAxis('v',temp)<vWorldMinMax[0])
                {
                    temp = SetAxis('v', temp, vWorldMinMax[0]);
                }
            }
            
            
            
            result.Add( temp); 
           
        }

        return result;



    }
    
    private Vector3  AddCurrentLocationToNode(Vector3 node)
    {
        Vector3 playerPos = player.transform.position;
        playerPos = ResetForsakenAxis(playerPos);
        Vector3 temp = CombineVectorAndNode(playerPos, node);
        return node;
    }
    
    private List<Vector3>  AddCurrentLocation(List<Vector3> input)
    {
        List<Vector3> result=new List<Vector3>();
        Vector3 playerPos = player.transform.position;

 
        
        playerPos = ResetForsakenAxis(playerPos);
        
        
        for(int i=0;i<=input.Count-1; i++)
        {
            Vector3 temp = CombineVectorAndNode(playerPos, input[i]);
            result.Add(temp);
             
        }

        return result;
    }
    private Vector3 ResetForsakenAxis(Vector3 input)
    {
        Vector3 result=new Vector3(0,0,0);
 
        result=AddAxisToVector('h', result,input);
        result=AddAxisToVector('v', result,input);

      //  return   setAxis('f', input, 0);

        return result;

    }
    private Vector3 CombineVectorAndNode(Vector3 primary,Vector3 node)// combines only selected node properties
    {
        Vector3 additive = new Vector3(0,0,0);

        additive=AddAxisToVector('h', additive,node);
        additive=AddAxisToVector('v', additive,node);
        additive=AddAxisToVector('f', additive,node);

        return (primary + additive);



    }
    private Vector3 AddAxisToVector(char A, Vector3 baseVector3 ,Vector3 subject)
    {

        
        
        if ((A == 'H')||(A == 'h')) A = horizontalEquivalent;
        if ((A == 'v')||(A == 'V')) A = verticalEquivalent; 
        if ((A == 'f')||(A == 'F')) A = GETForsakenAxis(); 
        
        switch (A)
        {
            case 'x':case 'X':
            { 
                baseVector3.x += GETAxis(A,subject);
                break;
            }
            case 'y':case 'Y':
            {
                 
                baseVector3.y += GETAxis(A,subject);
                break;
            }
            case 'z':case 'Z':
            {
                 
                baseVector3.z += GETAxis(A,subject);
                break;
            }
        }

        return baseVector3;


    }
    private List<Vector3> NodeSimplification(List<Vector3> input)
    {
        bool lastAdded = false; 
        List<Vector3> result=new List<Vector3>();
        List<List<float>> ratioList=new List<List<float>>()  ;
        List<int> selectedNodes= new List<int>();
        
        // take the first and the last for sure

        for(int i=0;i<input.Count-1; i++)
        {
            ratioList.Add(new List<float>());
            
            
            ratioList[i].Add((input[i+1].x-input[i].x));
            
            ratioList[i].Add((input[i+1].y-input[i].y));
            
            ratioList[i].Add((input[i+1].z-input[i].z));
        }
 
        float tempX=0;
        float tempY=0;
        float tempZ=0;
        
        
        //  selectedNodes.Add(0);
        for(int i=0;i<ratioList.Count; i++)
        {
            // i and i+1's differences
            float theX = ratioList[i][0];
            float theY = ratioList[i][1];
            float theZ= ratioList[i][2];


            if (   ( Math.Abs(theX - tempX)+ Math.Abs(theY - tempY)+ Math.Abs(theZ - tempZ) )  > (_desiredMinimumDifferance / (10/simplifyingPower)))
            {
                tempX = theX;
                tempY = theY;
                tempZ = theZ;
                selectedNodes.Add(i);
                continue;
                
            }

           
        }
         
        selectedNodes.Add(input.Count-1);

        foreach (var e in selectedNodes)
        {
            result.Add(input[e]);
        }

        // Debug.Log("OLD NODE COUNT IS : "+input.Count);
        
        return result;
    }
    private void GETFirstNodeOffSet(float h,float v)// rename as getOffsetFromFirstNode
    {
        firstNodeOffset[0] = h * -1;
        firstNodeOffset[1] = v * -1;
        
        
        //getFirstNodeOffSet makes first touched point saved for later to be
        //moved to origin with along other nodes moving accordingly this is
        //not an instant action
    }
    private List<Vector3> MakeFirstNodeTheCenter(List<Vector3> input) 
    {
        List<Vector3> result=new List<Vector3>();

        foreach (var e in input)
        {
            result.Add(MakeNode((GETAxis('h',e)+firstNodeOffset[0]),(GETAxis('v',e)+firstNodeOffset[1]),forsakenAxisDefault));
             
            
        }

        return result;

    }
    private List<Vector3> ReDescribeNodesForCreation(List<Vector3> input, float horizontalMax,float verticalMax)
    {
        DrawingIntegrity(horizontalPrimaryFactor);

        float hReverse = reverseHorizontal ? -1 : 1;
        float vReverse = reverseVertical ? -1 : 1;
        
        float oneHUnit = (_hMostLimit-_hLeastLimit) / horizontalMax*hReverse;
        float oneVUnit = (_vMostLimit-_vLeastLimit) / verticalMax*vReverse;

         
         
      //  scaleFactor = limitedScaleFactor(scaleFactor,Math.Abs(oneHUnit),Math.Abs(oneVUnit));
         
        
        
        List<Vector3> result=new List<Vector3>();

        foreach (var e in input)
        {
            result.Add( MakeNode(((GETAxis('h',e)/oneHUnit)*scaleFactor),((GETAxis('v',e)/oneVUnit)*scaleFactor),forsakenAxisDefault)); 
           
        }

        return result;
        
    }
    //----------------------------------------------
    // data saving states ----
    
    // start saving preparations 
    private void StartIntake()
    {
        if (Working&&!drawingActive&&ValidateInput('h')&&ValidateInput('v'))
        {
            float h = Input.mousePosition.x;
            float v = Input.mousePosition.y;
            GETFirstNodeOffSet(h, v);
            tempNodePoint=MakeNode(h,v,forsakenAxisDefault);
            tempNodeArray.Add(tempNodePoint);
            drawingActive = true;
        }
    }
    private void WhileMoving()
    {

        if (Working&&drawingActive)
        {
            TakeNode();
        }
        if (LimitBreach())
        {
            drawingActive = false;
            FinalizeIt();
        }

    }
    private bool LimitBreach()
    {

        return Working && drawingActive && useNodeLimit && tempNodeArray.Count > nodeLimit;
        
    }
    public void CompleteAction()
    {
        if (Working&&drawingActive)
        { 
            drawingActive = false;
            TakeNode();
            FinalizeIt();
        }
    }
    //--------------------------------------------------
    // data acquiring and  saving methods ----
    private void TakeNode()
    {
        float theH = LimitationCorrectInput('H');
        float theV = LimitationCorrectInput('V');
        float tempH = GETAxis('h',tempNodePoint);
        float tempV = GETAxis('v',tempNodePoint);

        if ((Math.Abs(theH - tempH) > _desiredMinimumDifferance)||(Math.Abs(theV - tempV) > _desiredMinimumDifferance  ))
        {
            Vector3 newNode = MakeNode(theH,theV,forsakenAxisDefault);
            
            tempNodeArray.Add(newNode);
            tempNodePoint = newNode;
            Vector3 copyNode = newNode;
            var temptemplist = new List<Vector3>();
            temptemplist.Add(copyNode);
            temptemplist = MakeFirstNodeTheCenter(temptemplist);
            temptemplist = ReDescribeNodesForCreation(temptemplist, maxHorizontalResult,maxVerticalResult);
            temptemplist = AddCurrentLocation(temptemplist);
            temptemplist = RealWorldLimitation(temptemplist);

           /* for (int i = 0; i < temptemplist.Count; i++)
            {
                Vector3 list = temptemplist[i];
                valueControl = new Vector3(leftBoundary.position.x, 0, rightBoundary.position.x);
                value = temptemplist[0];
                Vector3 boundary;
                float xBoundary;
                float zBoundary;
                xBoundary = Mathf.Clamp(list.x, leftBoundary.position.x, rightBoundary.position.x);
                zBoundary = Mathf.Clamp(list.z, backBoundary.position.z, frontBoundary.position.z);

                boundary = new Vector3(xBoundary, list.y,zBoundary);
                temptemplist[0]=boundary;
                value2 = new Vector3(xBoundary,0,0);
            }
            value3= temptemplist[0];*/
            pointerObject.transform.position =new Vector3( temptemplist[0].x,temptemplist[0].y +.8f,temptemplist[0].z);
           // value = temptemplist[0];


        }
    }
    //--------------------------------------------------
    // data testing --------
    // evaluate new node data as if in it decided panel or not
    private bool ValidateInput(char A)
    {
         
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
    //limitationCorrectInput take and make input data record as in panel limits
    private float LimitationCorrectInput(char A)
    {
        float result=0;
        
        
        switch (A)
        {
                
            case 'h':
            case 'H':
                if (Input.mousePosition.x > _hMostLimit)
                    return _hMostLimit;
                else if (Input.mousePosition.x < _hLeastLimit)
                    return _hLeastLimit;
                else result= Input.mousePosition.x;
                break;
            case 'v':
            case 'V':
                if (Input.mousePosition.y > _vMostLimit)
                    return _vMostLimit;
                else if (Input.mousePosition.y < _vLeastLimit)
                    return _vLeastLimit;
                else result= Input.mousePosition.y;
                break;
        }
        
        return result;
    }
    private Vector3 MakeNode(float Horizontal,float Vertical,float Forsaken)
    {
       Vector3 result = new Vector3(0,0,0);
       result=SetAxis('h', result, Horizontal);
       result=SetAxis('v', result, Vertical);
       result = SetAxis('f', result, Forsaken);
       return result;

    }
    private Vector3 SetAxis(char A,Vector3 input,float value)
    {
        if ((A == 'H')||(A == 'h')) A = horizontalEquivalent;
        if ((A == 'v')||(A == 'V')) A = verticalEquivalent; 
        if ((A == 'f')||(A == 'F')) A = GETForsakenAxis(); 
        

        switch (A)
        {
            case 'x':case 'X':
            { 
                input.x = value;
                break;
            }
            case 'y':case 'Y':
            {
                 
                input.y = value;
                break;
            }
            case 'z':case 'Z':
            {
                 
                input.z = value;
                break;
            }
        }

        return input;

    }
    //-----------------------------------------------
    // state controlling ------ 
    private void on_Or_Off(bool do_Or_Dont)
    {
        if (do_Or_Dont)
        {
            Working = true;
            on_Or_OffVisual(true);
            scaleFactor = _scaleFactor;
            GameManager.Instance.camerafollow = true;
        }
        else
        {
            Working = false;
        
            on_Or_OffVisual(false);
            GameManager.Instance.camerafollow = false;
        }
        
            
      
    }
    private void on_Or_OffVisual(bool do_Or_Dont)
    {
        if (do_Or_Dont)
        { 
            visual.on_Or_Off(true);
            UI[0].gameObject.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            visual.on_Or_Off(false);
            UI[0].gameObject.transform.parent.gameObject.SetActive(false);
        }
        
    }
    //-------------------------------------------
    // stored data categorizing and electing
    private char GETForsakenAxis()
    {
        char result = 'x';

        if ((horizontalEquivalent == 'x' || horizontalEquivalent == 'X') ||
            (verticalEquivalent == 'x' || verticalEquivalent == 'X'))
        {
            result = 'y';

        }

        if ((horizontalEquivalent == 'y' || horizontalEquivalent == 'Y') ||
            (verticalEquivalent == 'y' || verticalEquivalent == 'Y'))
        {
            
            result = 'z';
        }
         


        return result;
    }
    private float GETAxis(char A,Vector3 input)
    {

        float result = 0;

        if ((A == 'H')||(A == 'h')) A = horizontalEquivalent;
        if ((A == 'v')||(A == 'V')) A = verticalEquivalent; 
        if ((A == 'f')||(A == 'F')) A = GETForsakenAxis(); 
        
        switch (A)
        {
            case 'x':case 'X':
            { 
                    result = input.x;
                break;
            }
            case 'y':case 'Y':
            {
                 
                    result = input.y;
                break;
            }
            case 'z':case 'Z':
            {
                 
                    result = input.z;
                break;
            }
        }


        return result;



    }

}
