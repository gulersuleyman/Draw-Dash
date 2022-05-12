using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class drawNmove : MonoBehaviour
{
    [SerializeField] private int targetCount = 0;
    [SerializeField] private int stopListCount = 0;
    public Vector3 currentTarget;

    public bool inPathMoveStopped = true;

    public int startIntakeCount = 0;
    
    
    [SerializeField]  private bool InPath = false;
    [SerializeField]  private bool OUTPath = false;
    [SerializeField] private bool whileMoveIntake=false;
    
    
    
    
    [SerializeField] public bool Working;
    [SerializeField] private float returnSpeed=3;
    [SerializeField] private float pathSpeed=10;
    [SerializeField] private float returnCenterZ;
    
    
    [SerializeField] private char horizontalEquivalent='x';
    [SerializeField] private char verticalEquivalent='z';
    [SerializeField] private float forsakenAxisDefault=0;// name it extra
    [SerializeField] private bool keepPlayersExtraAxis;
    
    private bool reverseHorizontal = false;
    private bool reverseVertical=false;
    
    private bool drawingActive = false;//name ıt drawing on
    [SerializeField] private bool drawingOn = false;
    [SerializeField] private bool drawingPassive = true;
    [SerializeField] private Vector3 playerInitialPos;
    [SerializeField] private float [] playerInitialPixelPos=new float[2]; // this will be firstNode offset
    [SerializeField] private float [] firstRealPixelPos=new float[2];
    [SerializeField] private float firstFakePixelPos;// no need
    
    public float _vMostLimit; //pixel cordinates
    public float _vLeastLimit;
    public float _hMostLimit;
    public float _hLeastLimit;
    
    [SerializeField] private GameObject player;
    
    [SerializeField] private visualIndicator visual;
    
    //--------------------
    [SerializeField] private adjustUI [] UI;
    
     
    [SerializeField] private float [] hWorldMinMax=new float[2];
     
    [SerializeField] private float [] vWorldMinMax=new float[2];
    
    
    [SerializeField] private bool extraSimplifyNodes=false; // rename it
    [SerializeField] private float simplifyingPower=10;
    
    [SerializeField] private float desiredMinimumDifferance=1;
    private float _desiredMinimumDifferance=1;
    
    [SerializeField]  private float scaleFactor=1;// no need
    private float _scaleFactor;// no need
    
      private float maxHorizontalResult;
      private float maxVerticalResult ;
    
     
    [SerializeField] private float visualTopLimitScreenPercentage=50;
    [SerializeField] private float visualBottomLimitScreenPercentage=10;
    [SerializeField] private float rVisualSideLimitScreenPercentage=90;
    [SerializeField] private float lVisualSideLimitScreenPercentage=10;
      
     private float [] x_y_Ratio=new float[2];//514  --  302 // will use with limitation ratio 
    
     
     
     
     private float[] center=new float[2];
    
     private float[] firstNodeOffset=new float[2];
    
     private Vector3 tempNodePoint=new Vector3();
     private Vector3 lastNodeOriginal=new Vector3();
      
     [SerializeField] private List<Vector3> stopList= new List<Vector3>();// rename to stops
     private List<Vector3> finalNodeArray= new List<Vector3>(); // no need
     private Vector3[] nodePackage;// what does this do

   
     private int InPathIntakeCount = 0;
     
    void Start()
    {
        fixLimitations();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.levelFinished) return;

       // stopListCount = stopList.Count;
        checkGameManager();
        
        if(!Working) return ;
        
        if (firstTouch())
        {
            startIntake();
        }
        
        if (fingerHasMoved()&&drawingOn)
        {
            whileMoveIntake = true;
            playerMoveInPath();
            if (!drawingPassive)// player moving in path
            {
                
            }
            else // player is not moving in path
            {
             //   playerNotMoveInPath();
            }
          
        }
        
        else
        {
            whileMoveIntake = false;
            InPath = false;
        }
        if (drawingEnded())
        {
            drawingOn = false;
            drawingPassive = true;
            stopList.Clear();
            stopListCount = stopList.Count;
            targetCount = 0;
            inPathMoveStopped = true;
        }

        if (drawingPassive)
        {
            Vector3 target = new Vector3(transform.position.x,transform.position.y,returnCenterZ);
            transform.position = Vector3.MoveTowards(transform.position, target, returnSpeed * Time.deltaTime);
        }
        
    }
    
    private void fixLimitations()// rename and elaborate
    {
        solveConflict();
        
        setBorders();
        
        setBaseElements();
        
        alignSubordinates();
        

         
        // UI.getPosition(_xMostLimit,_xLeastLimit,_yMostLimit,_yLeastLimit,center);
    }
    
    private void solveConflict()
    {
        if (scaleFactor<=0)
            scaleFactor = 1;
        if (simplifyingPower<=0)
            simplifyingPower = 1;
        if (simplifyingPower>20)
            simplifyingPower = 20;
    }
    private void setBorders()
    {
        x_y_Ratio[0] = hWorldMinMax[1] - hWorldMinMax[0];
        x_y_Ratio[1] = vWorldMinMax[1] - vWorldMinMax[0];

        _vMostLimit = (Screen.height / 100 )* visualTopLimitScreenPercentage;  //  Mathf.Round(Screen.height / 100) * yMostLimit;
        _vLeastLimit =( Screen.height / 100) * visualBottomLimitScreenPercentage;
        _hMostLimit = ((Screen.width / 100) * rVisualSideLimitScreenPercentage)+(( Screen.width- ((Screen.width / 100)*100))/2);
        _hLeastLimit= ((Screen.width / 100) * lVisualSideLimitScreenPercentage)+(( Screen.width- ((Screen.width / 100)*100))/2);

        
         
        float width = _hMostLimit - _hLeastLimit;
        float oneUnit = (width / x_y_Ratio[0]);
        _vMostLimit = _vLeastLimit + (oneUnit * x_y_Ratio[1]);
         

        
        Debug.Log("horizontal / vertical  lenght : " +((_hMostLimit-_hLeastLimit)/ (_vMostLimit-_vLeastLimit)));
        
    }
    
    private void setBaseElements()
    {
        _desiredMinimumDifferance=(Screen.width / 100) * desiredMinimumDifferance;

        center=getPanelCenter();

        if (keepPlayersExtraAxis)
        {
            forsakenAxisDefault = getAxis('f', player.transform.position);
            Debug.Log("FORSAKEN AXIS TAKEN FROM PLAYER IT IS  :  "+forsakenAxisDefault);
        }

        _scaleFactor = scaleFactor;

        maxHorizontalResult = hWorldMinMax[1] - hWorldMinMax[0];
        maxVerticalResult = vWorldMinMax[1] - vWorldMinMax[0];

    }
    
    private float[] getPanelCenter()
    {
        float [] result=new float[2];
        result[0] = (_hMostLimit + _hLeastLimit)/2;
        result[1] = (_vMostLimit +_vLeastLimit)/2;
        return result;
    }
    
    private float getAxis(char A,Vector3 input)
    {

        float result = 0;

        if ((A == 'H')||(A == 'h')) A = horizontalEquivalent;
        if ((A == 'v')||(A == 'V')) A = verticalEquivalent; 
        if ((A == 'f')||(A == 'F')) A = getForsakenAxis(); 
        
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
    private char getForsakenAxis()
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
    
    private void alignSubordinates()
    {
        
        visual.setLimits(_hMostLimit,_hLeastLimit,_vMostLimit,_vLeastLimit); // gives the right borders to the visual indicator
        
        
        foreach (var e in UI)// sets all uı to the shape
        {
            e.getPosition(_hMostLimit,_hLeastLimit,_vMostLimit,_vLeastLimit,center);
        }
        

    }
    
    private void checkGameManager() // change to draw script on of 
    {
        if (!GameManager.Instance.getPathActive())
        {
            GameManager.Instance.setPathActive(true);
            on_Or_Off(true);
        }
    }
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
            //  UI[0].gameObject.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            visual.on_Or_Off(false);
            //  UI[0].gameObject.transform.parent.gameObject.SetActive(false);
        }
        
            
      
    }


    private bool firstTouch()
    {

        return (Input.GetMouseButtonDown(0)&&!drawingOn );

    }
    private bool fingerHasMoved()
    {
         
        return (Input.GetAxis("Mouse X") != 0) || (Input.GetAxis("Mouse Y") != 0);
    }

    private bool drawingEnded()
    {

        return Input.GetMouseButtonUp(0);
    }
    private void playerMoveInPath()
    {
        InPath = true;
        OUTPath = false;
        
        float h = Input.mousePosition.x;
        float v = Input.mousePosition.y;
        //Debug.Log("CURRENT MOUSE  : "+h+"  "+v);
        h = h  - playerInitialPixelPos[0];// use all ways
        v = v  - playerInitialPixelPos[1]; // use all ways
       // Debug.Log("LATER MOUSE  : "+h+"  "+v);
       Debug.Log(" ");

        float theH = limitationCorrectInput('H',h,v);
        float theV = limitationCorrectInput('V',h,v);
        
        float tempH = getAxis('h',lastNodeOriginal);
        float tempV = getAxis('v',lastNodeOriginal);
        Debug.Log("limitationCorrectInput  : "+theH+"  "+theV);
        Debug.Log("LAST NODE ORIGINAL  : "+lastNodeOriginal);

        if ((Math.Abs(theH - tempH) > _desiredMinimumDifferance)||(Math.Abs(theV - tempV) > _desiredMinimumDifferance  ))
        {
            tempNodePoint = makeNode(theH,theV,forsakenAxisDefault);
            
            lastNodeOriginal = makeNode(theH,theV,forsakenAxisDefault);
            
            
            tempNodePoint = makePixelNodeToRealPos(tempNodePoint);
            
            
            stopList.Add(tempNodePoint);
            
            Debug.Log(" new NODE POINT IS   : "+tempNodePoint);

            if (inPathMoveStopped)
            {
               // targetCount++;
                stopListCount = stopList.Count;
                movement();
                inPathMoveStopped = false;

            }
                

        }
     /*   else if (stopListCount==0)
        {
            
         
            Debug.Log(" ITS ALL THE SAMEEEEE");
        }*/

    }

    private void playerNotMoveInPath()
    {
        InPath = false;
        OUTPath = true;
        
        float h = Input.mousePosition.x;
        float v = Input.mousePosition.y;

        float h2 = h;
        float v2 = v;
        
        h = h - (h - playerInitialPixelPos[0]);// use all ways
        v = v - (v - playerInitialPixelPos[1]); // use all ways

        float theH = limitationCorrectInput('H',h,v);
        float theV = limitationCorrectInput('V',h,v);
        
        float tempH = getAxis('h',lastNodeOriginal);
        float tempV = getAxis('v',lastNodeOriginal);

        if ((Math.Abs(theH - tempH) > _desiredMinimumDifferance)||(Math.Abs(theV - tempV) > _desiredMinimumDifferance  ))
        {
            drawingPassive = false;
            
            firstRealPixelPos[0] = h2;
            firstRealPixelPos[1] = v2;
            
            playerInitialPos= getPlayerInitialPos();
            playerInitialPixelPos = getPlayerInitialPixelPos(playerInitialPos);
            
            h2 = h2 - (h2 - playerInitialPixelPos[0]);// use all ways
            v2 = v2 - (v2 - playerInitialPixelPos[1]);
            
            getFirstNodeOffSet(h2, v2);
            
            tempNodePoint = makeNode(h2,v2,forsakenAxisDefault);
            lastNodeOriginal =  makeNode(h2,v2,forsakenAxisDefault);
            
            
            tempNodePoint = makePixelNodeToRealPos(tempNodePoint);
            
            
            stopList.Add(tempNodePoint);
            Debug.Log(" new NODE POINT IS   : "+tempNodePoint);
            
            targetCount = 0;

            movement();

        }
    } // not in use
    private float limitationCorrectInput(char A,float mouseX,float mouseY)
    {
        float result=0;
        
        
        switch (A)
        {
                
            case 'h':
            case 'H':
                if (mouseX > _hMostLimit)
                    return _hMostLimit;
                else if (mouseX < _hLeastLimit)
                    return _hLeastLimit;
                else result= mouseX;
                break;
            case 'v':
            case 'V':
                if (mouseY > _vMostLimit)
                    return _vMostLimit;
                else if (mouseY < _vLeastLimit)
                    return _vLeastLimit;
                else result= mouseY;
                break;
            default:
                // Debug.Log("PROBLEM HAPPENED  2");
                break;
        }
        
        return result;
    }
    private void startIntake()
    {
        startIntakeCount++;
        Debug.Log("START IN TAKE COUNT  : "+startIntakeCount);

        drawingOn = true;
        drawingPassive = false;
        
        float h = Input.mousePosition.x;
        float v = Input.mousePosition.y;
        firstRealPixelPos[0] = h;
        firstRealPixelPos[1] = v;

        playerInitialPos= getPlayerInitialPos();
        playerInitialPixelPos = getPlayerInitialPixelPos(playerInitialPos);
        
        h = h - (h - playerInitialPixelPos[0]);// use all ways
        v = v - (v - playerInitialPixelPos[1]); // use all ways

        getFirstNodeOffSet(h, v);

     //   Debug.Log(" TEMP NODE POINT IS 1 : "+tempNodePoint);
     /*
        
        tempNodePoint=makeNode(h,v,forsakenAxisDefault);
        
     //   Debug.Log(" TEMP NODE POINT IS 2 : "+tempNodePoint);
        
        lastNodeOriginal=makeNode(h,v,forsakenAxisDefault);// fixed limitation might needed

    //    Debug.Log(" TEMP NODE POINT IS 3 : "+tempNodePoint);
        
        tempNodePoint = makePixelNodeToRealPos(tempNodePoint);

      Debug.Log(" TEMP NODE POINT IS 4 : "+tempNodePoint);

        tempNodeArray.Add(tempNodePoint);*/

        targetCount = 0;

      //  movement();

    }

    private void movement()
    {
        Vector3 currentPos = transform.position;
        float distance = Vector3.Distance(currentPos, stopList[targetCount]) ;
        float perTime = distance / pathSpeed;

        Debug.Log(" TEMP NODE ARRAY COUNT IS : "+stopList.Count);
        Debug.Log("TARGET COUNT IS  : "+targetCount);
        Debug.Log("TARGET  IS  : "+stopList[targetCount]);

        currentTarget = stopList[targetCount];
        this.transform.DOMove(stopList[targetCount], perTime, false).OnComplete(() =>
        {
            
            if (targetCount>=stopList.Count)
            {
                inPathMoveStopped = true;
            }
            else
            {
                targetCount++;
                movement();
            }
                
        });


    }

    private Vector3 makePixelNodeToRealPos(Vector3 input)
    {
        List<Vector3> inputList=new List<Vector3>();
        inputList.Add(input);
   //     Debug.Log("inputList.count 1  : "+inputList.Count);
        inputList = makeFirstNodeTheCenter(inputList);
   //     Debug.Log("inputList.count 2  : "+inputList.Count);
        inputList = reDescribeNodesForCreation(inputList, maxHorizontalResult, maxVerticalResult);
//        Debug.Log("inputList.count 3  : "+inputList.Count);
        inputList = addCurrentLocation(inputList, playerInitialPos);
    //    Debug.Log("inputList.count 4 : "+inputList.Count);
        return inputList[0];
    }
    private List<Vector3>  addCurrentLocation(List<Vector3> input,Vector3 playerPos)
    {
        List<Vector3> result=new List<Vector3>();
         

    //    Debug.Log("playerpos TEMPLATE IS : " +playerPos);
        
        playerPos = resetForsakenAxis(playerPos);

//        Debug.Log("playerpos TEMPLATE IS : " +playerPos);
    //    Debug.Log("forsakenAxis Default  :  "+forsakenAxisDefault);
        
        for(int i=0;i<input.Count; i++)
        {
            Vector3 temp = combineVectorAndNode(playerPos, input[i]);
            result.Add(temp);
             
        }

        return result;
    }
    
    private Vector3 resetForsakenAxis(Vector3 input)
    {
        Vector3 result=new Vector3(0,0,0);
 
        result=addAxisToVector('h', result,input);
        result=addAxisToVector('v', result,input);

        //  return   setAxis('f', input, 0);

        return result;

    }
    private Vector3 addAxisToVector(char A, Vector3 baseVector3 ,Vector3 subject)
    {

        
        
        if ((A == 'H')||(A == 'h')) A = horizontalEquivalent;
        if ((A == 'v')||(A == 'V')) A = verticalEquivalent; 
        if ((A == 'f')||(A == 'F')) A = getForsakenAxis(); 
        
        switch (A)
        {
            case 'x':case 'X':
            { 
                baseVector3.x += getAxis(A,subject);
                break;
            }
            case 'y':case 'Y':
            {
                 
                baseVector3.y += getAxis(A,subject);
                break;
            }
            case 'z':case 'Z':
            {
                 
                baseVector3.z += getAxis(A,subject);
                break;
            }
        }

        return baseVector3;


    }
    private Vector3 combineVectorAndNode(Vector3 primary,Vector3 node)// combines only selected node properties
    {
        Vector3 additive = new Vector3(0,0,0);

        additive=addAxisToVector('h', additive,node);
        additive=addAxisToVector('v', additive,node);
        additive=addAxisToVector('f', additive,node);

        return (primary + additive);



    }

   

    private Vector3 getPlayerInitialPos()
    {
        Vector3 result=new Vector3();

        var position = player.transform.position;
        result.x = position.x;
        result.y = position.y;
        result.z = position.z;
        return result;
    }
    private float[] getPlayerInitialPixelPos(Vector3 input)
    {
        float oneHUnit = (_hMostLimit-_hLeastLimit) / maxHorizontalResult ;
        float oneVUnit = (_vMostLimit-_vLeastLimit) / maxVerticalResult ;
        
        
        float [] result = new float[2];



        result[0] = _hLeastLimit + ((getAxis('h', input) - hWorldMinMax[0]) * oneHUnit);
        result[1] = _vLeastLimit + ((getAxis('v', input) - vWorldMinMax[0]) * oneVUnit);
            
        return result;
    }


    private void getFirstNodeOffSet(float h,float v)// rename as getOffsetFromFirstNode
    {
        firstNodeOffset[0] = h * -1;
        firstNodeOffset[1] = v * -1;
        
        
        //getFirstNodeOffSet makes first touched point saved for later to be
        //moved to origin with along other nodes moving accordingly this is
        //not an instant action
    }
    
    private List<Vector3> makeFirstNodeTheCenter(List<Vector3> input) 
    {
        List<Vector3> result=new List<Vector3>();

        foreach (var e in input)
        {
            result.Add(makeNode((getAxis('h',e)+firstNodeOffset[0]),(getAxis('v',e)+firstNodeOffset[1]),forsakenAxisDefault));
             
            
        }

        return result;

    }
    
    
    private List<Vector3> reDescribeNodesForCreation(List<Vector3> input, float horizontalMax,float verticalMax)
    {
       // drawingIntegrity(horizontalPrimaryFactor);

        float hReverse = reverseHorizontal ? -1 : 1;
        float vReverse = reverseVertical ? -1 : 1;
        
        float oneHUnit = (_hMostLimit-_hLeastLimit) / horizontalMax*hReverse;
        float oneVUnit = (_vMostLimit-_vLeastLimit) / verticalMax*vReverse;

         
         
      // scaleFactor = limitedScaleFactor(scaleFactor,Math.Abs(oneHUnit),Math.Abs(oneVUnit));
         
        
        
        List<Vector3> result=new List<Vector3>();

        foreach (var e in input)
        {
            result.Add( makeNode(((getAxis('h',e)/oneHUnit)*scaleFactor),((getAxis('v',e)/oneVUnit)*scaleFactor),forsakenAxisDefault)); 
           
        }

        return result;

    }
    
    private Vector3 makeNode(float Horizontal,float Vertical,float Forsaken)
    {
        Vector3 result = new Vector3(0,0,0);

        result=setAxis('h', result, Horizontal);
        result=setAxis('v', result, Vertical);
        result = setAxis('f', result, Forsaken);

        return result;

    }
    private Vector3 setAxis(char A,Vector3 input,float value)
    {
        if ((A == 'H')||(A == 'h')) A = horizontalEquivalent;
        if ((A == 'v')||(A == 'V')) A = verticalEquivalent; 
        if ((A == 'f')||(A == 'F')) A = getForsakenAxis(); 
        

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
    
    private bool validateInput(char A) // might needed not now input can be given at the start
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
    
    
    
}
