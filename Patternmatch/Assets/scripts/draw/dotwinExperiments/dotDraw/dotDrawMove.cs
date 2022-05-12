using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dotDrawMove : MonoBehaviour
{
   
    [SerializeField] public bool Working;
    [SerializeField] private char horizontalEquivalent='x';
    [SerializeField] private char verticalEquivalent='z';
    [SerializeField] private float forsakenAxisDefault=0;
    [SerializeField] private bool keepPlayersExtraAxis;
    
    private bool reverseHorizontal = false;
    private bool reverseVertical=false;
    
    private bool drawingActive = false;

    [SerializeField] private GameObject player;
    [SerializeField] private dotStopList mover;
    [SerializeField] private visualIndicator visual;

    
    //--------------------
    [SerializeField] private adjustUI [] UI;
     
    //----
    //   [SerializeField] private bool manageStartText;
    //   [SerializeField] private GameObject drawToPlay; // rename it
    //-----
    
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
    
    private float[] firstNodeOffset = new float[2];
    
    private Vector3 tempNodePoint=new Vector3(); 
    private List<Vector3> tempNodeArray= new List<Vector3>();
    private List<Vector3> finalNodeArray= new List<Vector3>();
    private Vector3[] nodePackage; 
    
    
   
    
    
    
    
    // player position must be take consideration here 
    void Start()
    {
        fixLimitations();
    }

     
    void Update()
    {
        checkGameManager();
        
        if (firstTouch())
        {
            startIntake();
        }
        
        if (fingerHasMoved())
        {
            whileMoving();

            //private void debugging();
          
        }
        if (drawingEnded())
        {
            completeAction();
        }
        
    }
    
    //-----------------------------
    // conflict solving data altering

    private void checkGameManager()
    {
        if (!GameManager.Instance.getPathActive())
        {
            GameManager.Instance.setPathActive(true);
            on_Or_Off(true);
        }
    }

    private float limitedScaleFactor(float scale,float hUnit,float vUnit)
    {
        bool hPositive=false, vPositive=false ;
        if ((!horizontalWorldLimitation)&&(!verticalWorldLimitation)) return scale;
        
        float correctionFactor = 1;

        float hScaleTemp=1/underLimitingMaxPossibleScale;
        float vScaleTemp=1/underLimitingMaxPossibleScale;


        float nodePotentPositiveH = getDrawingExtent(false, 'H', 'P', hUnit, vUnit);    
        float nodePotentNegativeH = getDrawingExtent(false, 'H', 'N', hUnit, vUnit); 
       
        
        float nodePotentPositiveV =getDrawingExtent(false, 'V', 'P', hUnit, vUnit); 
        float nodePotentNegativeV =getDrawingExtent(false, 'V', 'N', hUnit, vUnit); 
        

        Vector3 playerPos = player.transform.position;
        
        float playerPotentPositiveH = getPlayerLimit('t', 'h', playerPos);
        float playerPotentNegativeH=getPlayerLimit('b', 'h', playerPos);
        
        float playerPotentPositiveV=getPlayerLimit('t', 'v', playerPos);
        float playerPotentNegativeV=getPlayerLimit('b', 'v', playerPos);

        if (horizontalWorldLimitation)
        {
            if ((nodePotentPositiveH / playerPotentPositiveH) >
                (nodePotentNegativeH / playerPotentNegativeH))
                hPositive = true;
             
            
            hScaleTemp =
                ((nodePotentPositiveH / playerPotentPositiveH) >
                 (nodePotentNegativeH / playerPotentNegativeH))
                    ? (nodePotentPositiveH / playerPotentPositiveH)
                    : (nodePotentNegativeH / playerPotentNegativeH);
        }

        if (verticalWorldLimitation)
        {
            if ((nodePotentPositiveV / playerPotentPositiveV) >
                (nodePotentNegativeV / playerPotentNegativeV))
                vPositive = true;
             
            vScaleTemp =
                ((nodePotentPositiveV / playerPotentPositiveV) >
                 (nodePotentNegativeV / playerPotentNegativeV))
                    ? (nodePotentPositiveV / playerPotentPositiveV)
                    : (nodePotentNegativeV / playerPotentNegativeV);
        }

        if ((hScaleTemp > vScaleTemp))
        {
           float panelPotentPositiveH = getDrawingExtent(true, 'H', 'P', hUnit, vUnit);  
           float panelPotentNegativeH = getDrawingExtent(true, 'H', 'N', hUnit, vUnit);
            
            if (hPositive)
            {
                correctionFactor = panelPotentPositiveH / playerPotentPositiveH;
            }
            else
            {
                correctionFactor = panelPotentNegativeH / playerPotentNegativeH;
            }
        }
        else
        {
            float panelPotentPositiveV =getDrawingExtent(true, 'V', 'P', hUnit, vUnit);
            float panelPotentNegativeV =getDrawingExtent(true, 'V', 'N', hUnit, vUnit);
            
            if (vPositive)
            {
                correctionFactor = panelPotentPositiveV / playerPotentPositiveV;
            }
            else
            {
                correctionFactor = panelPotentNegativeV / playerPotentNegativeV;
            }
        }

        Debug.Log("Scale temp  : "+hScaleTemp);

        float result = (scale / correctionFactor);
        
        if (horizontalWorldLimitation)
        {
            if (playerPotentPositiveH<=0.1 && ((nodePotentPositiveH>0 )) )
            {
                result = 0;
            }
       
            if (playerPotentNegativeH<=0.1 && ( (nodePotentNegativeH>0 )) )
            {
                result = 0;
            }
        }

        if (verticalWorldLimitation)
        {
            if (playerPotentPositiveV<=0.1 && ((nodePotentPositiveV>0 )) )
            {
                result = 0;
            }
            if (playerPotentNegativeV<=0.1 && ( (nodePotentNegativeV>0 )) )
            {
                result = 0;
            }

        }
        
      
        return result;
    }

    

    //-------------------------------
    //start processes ----
    
   // panel position setting of visual indicators
   // and limit reference assigning as in pixel forms
   //conflict resolving 
    private void fixLimitations()// rename and elaborate
    {
        solveConflict();
        
        setBorders();
        
        setBaseElements();
        
        alignSubordinates();
        

         
       // UI.getPosition(_xMostLimit,_xLeastLimit,_yMostLimit,_yLeastLimit,center);
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

        
        Debug.Log("horizontal / vertical  lenght : " +((_hMostLimit-_hLeastLimit)/ (_vMostLimit-_vLeastLimit)));
        
    }

    private void alignSubordinates()
    {
        
        visual.setLimits(_hMostLimit,_hLeastLimit,_vMostLimit,_vLeastLimit); // gives the right borders to the visual indicator
        
        
         foreach (var e in UI)// sets all uı to the shape
        {
            e.getPosition(_hMostLimit,_hLeastLimit,_vMostLimit,_vLeastLimit,center);
        }
        

    }

    private void drawingIntegrity( bool PrimaryFactorHorizontal) //conflict resolving 
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

    private float[] getPanelCenter()
    {
        float [] result=new float[2];
        result[0] = (_hMostLimit + _hLeastLimit)/2;
        result[1] = (_vMostLimit +_vLeastLimit)/2;
        return result;
    }
    
    //--------------------------------------------
    //            input taking state evaluating ---
    private bool firstTouch()
    {

        return Input.GetMouseButtonDown(0);

    }
    private bool fingerHasMoved()
    {
         
        return (Input.GetAxis("Mouse X") != 0) || (Input.GetAxis("Mouse Y") != 0);
    }

    private bool drawingEnded()
    {

        return Input.GetMouseButtonUp(0);
    }
    
    //----------------------------------------------
    // post drawing data altering methods---
    
    private void finalizeIt()
    {
        if (tempNodeArray==null)
        {
            tempNodeArray=new List<Vector3>();
            return;
        }
        
        tempNodeArray = makeFirstNodeTheCenter(tempNodeArray);
        //  tempNodeArray = makeFirstNodeCenter(tempNodeArray); round inputs or not ?

        
      
        

        if(extraSimplifyNodes)
            tempNodeArray = nodeSimplification(tempNodeArray);
        
        
        tempNodeArray = reDescribeNodesForCreation(tempNodeArray, maxHorizontalResult,maxVerticalResult);
        
        
        tempNodeArray = addCurrentLocation(tempNodeArray);

        tempNodeArray = realWorldLimitation(tempNodeArray);

       // tempNodeArray = addExtraNodeForRotationCorrection(tempNodeArray, 'z',  1.2f);
            

        finalNodeArray = tempNodeArray;
       
       
       /* foreach (var e in finalNodeArray)// debug only
        {
           //  Debug.Log(" current location added NODES :  "+e);}
        }*/

        // Debug.Log("NODE COUNT IS : "+finalNodeArray.Count);
      //  Debug.Log("maximum width  : "+Screen.width);
      //  Debug.Log("maximum height  : "+Screen.height);
       
       
       
       
        if (useNodeLimit)// this one missing parts
        {
            if (nodeDownLimit <finalNodeArray.Count)
            {
                mover.setNewStops(finalNodeArray);
            }

        }
        else
        {
           mover.setNewStops(finalNodeArray);
            
        }
        tempNodeArray=new List<Vector3>();

        on_Or_Off(false);

    }

    private List<Vector3> realWorldLimitation(List<Vector3> input)
    {
        List<Vector3> result=new List<Vector3>();

        foreach (var e in input)
        {
            Vector3 temp = makeNode(getAxis('h',e),getAxis('v',e),getAxis('f',e));
            if (horizontalWorldLimitation)
            {
                if (getAxis('h',temp)>hWorldMinMax[1])
                {
                    temp = setAxis('h', temp, hWorldMinMax[1]);
                }
                else if (getAxis('h',temp)<hWorldMinMax[0])
                {
                    temp = setAxis('h', temp, hWorldMinMax[0]);
                }
            }
            if (verticalWorldLimitation)
            {
                if (getAxis('v',temp)>vWorldMinMax[1])
                {
                    temp = setAxis('v', temp, vWorldMinMax[1]);
                }
                else if (getAxis('v',temp)<vWorldMinMax[0])
                {
                    temp = setAxis('v', temp, vWorldMinMax[0]);
                }
            }
            
            
            
            result.Add( temp); 
           
        }

        return result;



    }

    private List<Vector3> addExtraNodeForRotationCorrection(List<Vector3> input, char A,float addition)
    {
        Vector3 lastNode = new Vector3(input[input.Count - 1].x, input[input.Count - 1].y, input[input.Count - 1].z);
       // Vector3 willBeLastNode =new Vector3(input[input.Count - 1].x, input[input.Count - 1].y, input[input.Count - 1].z);
        
        switch (A)
        {
            case 'x':case 'X':
            {
                lastNode.x += addition;
            //    willBeLastNode.x += addition*2;
                break;
            }
            case 'y':case 'Y':
            {
                 
                lastNode.y += addition;
              //  willBeLastNode.x += addition*2;
                break;
            }
            case 'z':case 'Z':
            {
                 
                lastNode.z += addition;
            //    willBeLastNode.x += addition*2;
                break;
            }
        }
        input.Add(lastNode);
        
      //  input.Add(willBeLastNode);

        return input;

    }

    private List<Vector3>  addCurrentLocation(List<Vector3> input)
    {
        List<Vector3> result=new List<Vector3>();
        Vector3 playerPos = player.transform.position;

        Debug.Log("playerpos TEMPLATE IS : " +playerPos);
        
        playerPos = resetForsakenAxis(playerPos);

        Debug.Log("playerpos TEMPLATE IS : " +playerPos);
        Debug.Log("forsakenAxis Default  :  "+forsakenAxisDefault);
        
        for(int i=0;i<input.Count-1; i++)
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

    private Vector3 combineVectorAndNode(Vector3 primary,Vector3 node)// combines only selected node properties
    {
        Vector3 additive = new Vector3(0,0,0);

        additive=addAxisToVector('h', additive,node);
        additive=addAxisToVector('v', additive,node);
        additive=addAxisToVector('f', additive,node);

        return (primary + additive);



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

    private List<Vector3> nodeSimplification(List<Vector3> input)
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
        drawingIntegrity(horizontalPrimaryFactor);

        float hReverse = reverseHorizontal ? -1 : 1;
        float vReverse = reverseVertical ? -1 : 1;
        
        float oneHUnit = (_hMostLimit-_hLeastLimit) / horizontalMax*hReverse;
        float oneVUnit = (_vMostLimit-_vLeastLimit) / verticalMax*vReverse;

         
         
      //  scaleFactor = limitedScaleFactor(scaleFactor,Math.Abs(oneHUnit),Math.Abs(oneVUnit));
         
        
        
        List<Vector3> result=new List<Vector3>();

        foreach (var e in input)
        {
            result.Add( makeNode(((getAxis('h',e)/oneHUnit)*scaleFactor),((getAxis('v',e)/oneVUnit)*scaleFactor),forsakenAxisDefault)); 
           
        }

        return result;

    }
    //----------------------------------------------
    // data saving states ----
    
    // start saving preparations 
    private void startIntake()
    {   
     //   Debug.Log("------------------------------MOUSE DOWN  BEEN CALLED--------------------------------- ");
        if (Working&&!drawingActive&&validateInput('h')&&validateInput('v'))
        {
           // Debug.Log("------------------------------NODE TAKING HAS START--------------------------------- ");
            float h = Input.mousePosition.x;
            float v = Input.mousePosition.y;
            
            getFirstNodeOffSet(h, v);
            
            tempNodePoint=makeNode(h,v,forsakenAxisDefault);
            
            tempNodeArray.Add(tempNodePoint);//------------------

            drawingActive = true;
           
           // closeDrawToPlay();
        }
        
         
    }
    
    
    private void whileMoving()
    {

        if (Working&&drawingActive)
        {
            takeNode();
        }
        if (limitBreach())
        {
            drawingActive = false;
            finalizeIt();
        }

    }

    private bool limitBreach()
    {

        return Working && drawingActive && useNodeLimit && tempNodeArray.Count > nodeLimit;
        
    }

    public void completeAction()
    {
      //  Debug.Log("------------------------------MOUSE UP  BEEN CALLED--------------------------------- ");
        if (Working&&drawingActive)
        { 
            drawingActive = false;
            takeNode();
            finalizeIt();
        }
        
       
    }
    
    //--------------------------------------------------
    
    // data acquiring and  saving methods ----

    private float getDrawingExtent(bool doItSafe, char Axis, char Positive_Negative, float unitH, float unitV)
    {

        float result = 0;
        float backUp = 0;

        // h p    (_hMostLimit- Math.Abs(firstNodeOffset[0])) / hUnit  
        // h n    (Math.Abs(firstNodeOffset[0]) - _hLeastLimit)/ unitH

        // v p    (_vMostLimit- Math.Abs(firstNodeOffset[1])) / vUnit 
        // v n    (Math.Abs(firstNodeOffset[1]) - _vLeastLimit)/ vUnit



        if (doItSafe)
        {
              
            switch (Axis)
            {
                case 'h':
                case 'H':
                {
                    if (Positive_Negative == 'P' || Positive_Negative == 'p')
                    {
                        result = (_hMostLimit - Math.Abs(firstNodeOffset[0])) / unitH;
                    }

                    else
                    {
                        result = (Math.Abs(firstNodeOffset[0]) - _hLeastLimit) / unitH;
                    }

                    break;
                }
                case 'v':
                case 'V':
                {

                    if (Positive_Negative == 'P' || Positive_Negative == 'p')
                    {
                        result = (_vMostLimit - Math.Abs(firstNodeOffset[1])) / unitV;
                    }

                    else
                    {
                        result = (Math.Abs(firstNodeOffset[1]) - _vLeastLimit) / unitV;
                    }

                    break;
                }

            }  
              
            return result;
        }



        List<Vector3> subjectArray = tempNodeArray;
         
        switch (Axis)
        {
            case 'h':case 'H':
            {
                if (Positive_Negative == 'P' || Positive_Negative == 'p')
                {
                    float maxHAnNodeReached = findMost('H', subjectArray);
                        result = Math.Abs(maxHAnNodeReached) / unitH;
                      //  Debug.Log(" maxHAnNodeReached  : "+result);
                      //  result = (maxHAnNodeReached - Math.Abs(firstNodeOffset[0])) / unitH;
                }

                else
                {
                    float minHAnNodeReached = findLeast('H', subjectArray);
                    result = Math.Abs(minHAnNodeReached)/unitH;
                  //  Debug.Log(" minHAnNodeReached  : "+result);
                   // result = (Math.Abs(firstNodeOffset[0]) - minHAnNodeReached) / unitH;
                }
                
                break;
            }
            case 'v':case 'V':
            {

                if (Positive_Negative == 'P' || Positive_Negative == 'p')
                {
                    float maxVAnNodeReached = findMost('V', subjectArray);
                    result = Math.Abs(maxVAnNodeReached)/unitV;
                  //  Debug.Log(" maxVAnNodeReached  : "+result);
                    //result = (maxVAnNodeReached - Math.Abs(firstNodeOffset[1])) / unitV;
                }

                else
                {
                    float minVAnNodeReached = findLeast('V', subjectArray);
                    result = Math.Abs(minVAnNodeReached)/unitV;
                  //  Debug.Log(" minVAnNodeReached  : "+result);
                    //result = (Math.Abs(firstNodeOffset[1]) - minVAnNodeReached) / unitV;
                }
                
                break;
            }
        
        }

        
        return result;

    }

    private float getPlayerLimit(char Top_Bottom,char Axis,Vector3 position)
    {
        float result = 0;
        float max=1,min=1;

        if ((Axis == 'H') || (Axis == 'h'))
        {
            Axis = horizontalEquivalent;
            max = hWorldMinMax[1];
            min = hWorldMinMax[0];
        }

        if ((Axis == 'V') || (Axis == 'v'))
        {
            Axis = verticalEquivalent;
            max = vWorldMinMax[1];
            min = vWorldMinMax[0];
        }
        
        switch (Axis)
        {
            case 'x':case 'X':
            { 
                if(Top_Bottom=='T'||Top_Bottom=='t') result = max-position.x;
                    
                else result = position.x-min ;
                
                break;
            }
            case 'y':case 'Y':
            {
                 
                if(Top_Bottom=='T'||Top_Bottom=='t') result = max-position.y;
                    
                else result = position.y-min ;
                
                break;
            }
            case 'z':case 'Z':
            {
                 
                if(Top_Bottom=='T'||Top_Bottom=='t') result = max-position.z;
                    
                else result = position.z-min ;
                
                break;
            }
        }

        if (result ==0)
        {
            if (findLeast(Axis,tempNodeArray)==0||findMost(Axis,tempNodeArray)==0)
            {
                result += 1f;
            }
            
            
            
            
            
        }
            

        return result;

    }

    private void takeNode()
    {
        float theH = limitationCorrectInput('H');
        float theV = limitationCorrectInput('V');
        
        float tempH = getAxis('h',tempNodePoint);
        float tempV = getAxis('v',tempNodePoint);

        if ((Math.Abs(theH - tempH) > _desiredMinimumDifferance)||(Math.Abs(theV - tempV) > _desiredMinimumDifferance  ))
        {
            Vector3 newNode = makeNode(theH,theV,forsakenAxisDefault);
            tempNodeArray.Add(newNode);
            tempNodePoint = newNode;

        }
    }
    
    //--------------------------------------------------
    // data testing --------
    
    // evaluate new node data as if in it decided panel or not
    private bool validateInput(char A)
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
    private float limitationCorrectInput(char A)
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
            default:
               // Debug.Log("PROBLEM HAPPENED  2");
                break;
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
          //  UI[0].gameObject.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            visual.on_Or_Off(false);
          //  UI[0].gameObject.transform.parent.gameObject.SetActive(false);
        }
        
            
      
    }
    
    
    private void closeDrawToPlay()// rename
    {
      //  if(manageStartText)
          //  drawToPlay.SetActive(false);
        
        
    }
    
    //-------------------------------------------
    // stored data categorizing and electing
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

    private float findMost(char A,List<Vector3> input)
    {
        float result=0;

        if ((A == 'H')||(A == 'h')) A = horizontalEquivalent;
        if ((A == 'V')||(A == 'v')) A = verticalEquivalent;

        foreach (var e in input)
        {
            switch (A)
            {
                case 'x':case 'X':
                {
                    if (e.x > result)
                        result = e.x;
                    break;
                }
                case 'y':case 'Y':
                {
                    if (e.y > result)
                        result = e.y;
                    break;
                }
                case 'z':case 'Z':
                {
                    if (e.z > result)
                        result = e.z;
                    break;
                }
            }
        }

        return result;
    }
    
    private float findLeast(char A,List<Vector3> input)
    {
        float result=0;
        
        if ((A == 'H')||(A == 'h')) A = horizontalEquivalent;
        if ((A == 'V')||(A == 'v')) A = verticalEquivalent;

        foreach (var e in input)
        {
            switch (A)
            {
                case 'x':case 'X':
                {
                    if (e.x < result)
                        result = e.x;
                    break;
                }
                case 'y':case 'Y':
                {
                    if (e.y < result)
                        result = e.y;
                    break;
                }
                case 'z':case 'Z':
                {
                    if (e.z < result)
                        result = e.z;
                    break;
                }
            }
        }

        return result;
    }
    
}
