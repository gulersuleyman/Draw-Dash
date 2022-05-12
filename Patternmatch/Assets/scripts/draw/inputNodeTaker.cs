using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class inputNodeTaker : MonoBehaviour
{
    //public float currentX;
  //  public float currentY;
 // private bool isValidated = false;

// [SerializeField] private GenerateMesh meshMake;
 
 public GameObject drawToPlay;
    public adjustUI [] UI;
    public exclusiveTrack specialTrack;


    [SerializeField] private bool extraSimplifyNodes=false;
    [SerializeField] private float simplifyingPower=10;
    
    [SerializeField] private bool useNodeLimit=false;
    [SerializeField] private int nodeLimit=10000;
    [SerializeField] private int nodeDownLimit=10;
    

    
    //---------------------------------------------------
    
    [SerializeField] public bool Working;
    
    [SerializeField] private float yMostLimit=50;
    [SerializeField] private float yLeastLimit=10;
    [SerializeField] private float xMostLimit=90;
    [SerializeField] private float xLeastLimit=10;
    [SerializeField] private bool useFixedRatio=false;
    [SerializeField] private int [] x_y_Ratio=new int[2];
    
    
    private float defaultTransformY=0;
    [SerializeField] private float scaleFactor=1;
    [SerializeField] private int shrinkBy = 10;
    
      private float _yMostLimit; //pixel cordinates
      private float _yLeastLimit;
      private float _xMostLimit;
      private float _xLeastLimit;
      
    [SerializeField] private float desiredMinimumDifferance=1;
    private float _desiredMinimumDifferance=1;
    
    
    private bool drawingActive = false;
    
    //------------------------------------------
    
    private float[] center=new float[2];
    
    private float[] firstNodeOffset=new float[2];
    
    
     

   //-------------------------------------------
   private Vector3 tempNodePoint=new Vector3(); 
   private List<Vector3> tempNodeArray= new List<Vector3>();
   private List<Vector3> finalNodeArray= new List<Vector3>();
   private Vector3[] nodePackage; 
   
    void Start()
    {
        fixLimitations();
        
    }
    
    // Update is called once per frame
    void Update()
    {
      //  fixLimitations();
      if (Input.GetMouseButtonDown(0))
      {
          startIntake();
      }

      if (HasMouseMoved())
      {
          whileMoving();

           //private void debugging();
          
      }
      if (Input.GetMouseButtonUp(0))
      {
          completeAction();
      }

      
      
      
    }

   /* private void debugging()
    {
        currentX=Input.mousePosition.x;
        currentY=Input.mousePosition.y;
          
        if (validateInput('x')&&validateInput('y') )
        {
            if(!isValidated)
                Debug.Log(" --------  CONFIRMED  ---------  CONFIRMED  --------  CONFIRMED-----");
            isValidated = true;
              
        }
        else
        {
            if(isValidated)
                Debug.Log(" --------  DENIED ---------   DENIED --------    DENIED ------");
            isValidated = false;
        }
    }*/
    private bool HasMouseMoved()
    {
         
        return (Input.GetAxis("Mouse X") != 0) || (Input.GetAxis("Mouse Y") != 0);
    }
    
    private void getFirstNodeOffSet(float x,float y)
    {
        firstNodeOffset[0] = x * -1;
        firstNodeOffset[1] = y * -1;
    }

    private void fixLimitations()
    {
        _yMostLimit = (Screen.height / 100 )* yMostLimit;  //  Mathf.Round(Screen.height / 100) * yMostLimit;
        _yLeastLimit =( Screen.height / 100) * yLeastLimit;
        _xMostLimit = ((Screen.width / 100) * xMostLimit)+(( Screen.width- ((Screen.width / 100)*100))/2);
        _xLeastLimit= ((Screen.width / 100) * xLeastLimit)+(( Screen.width- ((Screen.width / 100)*100))/2);

        if (useFixedRatio)
        {
            float width = _xMostLimit - _xLeastLimit;
            float oneUnit = (width / x_y_Ratio[0]);
            _yMostLimit = _yLeastLimit + (oneUnit * x_y_Ratio[1]);
        }
        
        _desiredMinimumDifferance=(Screen.width / 100) * desiredMinimumDifferance;
        center=getPanelCenter();
        
        if (scaleFactor<=0)
            scaleFactor = 1;
        if (simplifyingPower<=0)
            simplifyingPower = 1;
        if (simplifyingPower>20)
            simplifyingPower = 20;
        
        
        foreach (var e in UI)
        {
            e.getPosition(_xMostLimit,_xLeastLimit,_yMostLimit,_yLeastLimit,center);
        }
        
        //
        //UI.getPosition(_xMostLimit,_xLeastLimit,_yMostLimit,_yLeastLimit,center);
    }
    
    private float[] getPanelCenter()
    {
        float [] result=new float[2];
        result[0] = (_xMostLimit + _xLeastLimit)/2;
        result[1] = (_yMostLimit +_yLeastLimit)/2;
        return result;
    }
    public bool validateInput(char A)
    {
        switch (A)
        {
            case 'x':case 'X':
                if (Input.mousePosition.x > _xMostLimit)
                    return false;
                else if (Input.mousePosition.x < _xLeastLimit)
                    return false;
                break;
            case 'y':case 'Y':
                if (Input.mousePosition.y > _yMostLimit)
                    return false;
                else if (Input.mousePosition.y < _yLeastLimit)
                    return false;
                break;
            default:
                Debug.Log("PROBLEM HAPPENED  1");
                return false;
        }

        return true;
    }

    private float limitationCorrectInput(char A)
    {
        float result=0;
        switch (A)
        {
                
            case 'x':
            case 'X':
                if (Input.mousePosition.x > _xMostLimit)
                    return _xMostLimit;
                else if (Input.mousePosition.x < _xLeastLimit)
                    return _xLeastLimit;
                else result= Input.mousePosition.x;
                break;
            case 'y':
            case 'Y':
                if (Input.mousePosition.y > _yMostLimit)
                    return _yMostLimit;
                else if (Input.mousePosition.y < _yLeastLimit)
                    return _yLeastLimit;
                else result= Input.mousePosition.y;
                break;
            default:
                Debug.Log("PROBLEM HAPPENED  2");
                break;
        }
        
        return result;
    }

    void startIntake()
    {   
        Debug.Log("------------------------------MOUSE DOWN  BEEN CALLED--------------------------------- ");
        if (Working&&!drawingActive&&validateInput('x')&&validateInput('y'))
        {
            Debug.Log("------------------------------NODE TAKING HAS START--------------------------------- ");
            float x = Input.mousePosition.x;
            float y = Input.mousePosition.y;
            
            getFirstNodeOffSet(x, y);
            tempNodePoint=new Vector3(x,defaultTransformY,y);

            drawingActive = true;

            closeDrawToPlay();
        }
        
         
    }

    private void closeDrawToPlay()
    {
        
        drawToPlay.SetActive(false);
        
        
    }
    void whileMoving()
    {

        if (Working&&drawingActive&&useNodeLimit && tempNodeArray.Count > nodeLimit)
        {
            drawingActive = false;
            finalizeIt();
        }
            
        if (Working&&drawingActive)
        {
            takeNode();
        }
     

    }

    private void takeNode()
    {
        float theX = limitationCorrectInput('x');
        float theY = limitationCorrectInput('y');
        float tempX = tempNodePoint.x;
        float tempY = tempNodePoint.z;

        if ((Math.Abs(theX - tempX) > _desiredMinimumDifferance)||(Math.Abs(theY - tempY) > _desiredMinimumDifferance  ))
        {
            Vector3 newNode = new Vector3(theX, defaultTransformY, theY);
            tempNodeArray.Add(newNode);
            tempNodePoint = newNode;

        }
    }

    public void completeAction()
    {
        Debug.Log("------------------------------MOUSE UP  BEEN CALLED--------------------------------- ");
        if (Working&&drawingActive)
        { 
            drawingActive = false;
            takeNode();
            finalizeIt();
        }
        
       
    }

    private void finalizeIt()
    {
        
        tempNodeArray = makeFirstNodeTheCenter(tempNodeArray);
        //  tempNodeArray = makeFirstNodeCenter(tempNodeArray); round inputs or not ?


        if(extraSimplifyNodes)
            tempNodeArray = nodeSimplification(tempNodeArray);
        
        
        tempNodeArray = reDescribeNodesForCreation(tempNodeArray, shrinkBy);

        finalNodeArray = tempNodeArray;
       
       
       /* foreach (var e in finalNodeArray)
        {
            //   Debug.Log(" the nodes we acquired  :  "+e);
        }*/

      //  Debug.Log("NODE COUNT IS : "+finalNodeArray.Count);
      //  Debug.Log("maximum width  : "+Screen.width);
      //  Debug.Log("maximum height  : "+Screen.height);
       
       
       
       
        if (useNodeLimit)
        {
            if (nodeDownLimit <finalNodeArray.Count)
            {
                
                Debug.Log(" NEW OBJECT CREATED ");
            }

        }
        else
        {
           
            Debug.Log(" NEW OBJECT CREATED ");
        }
        tempNodeArray=new List<Vector3>();
        

        closeDrawing();

    }

    private void closeDrawing()
    {
        Working = false;
        if (specialTrack!=null)
        {
            specialTrack.enabled = true;
        }

       
        UI[0].gameObject.transform.parent.gameObject.SetActive(false);
            

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

        Debug.Log("OLD NODE COUNT IS : "+input.Count);
        
        return result;
    }

    private List<Vector3> makeFirstNodeTheCenter(List<Vector3> input)
    {
        List<Vector3> result=new List<Vector3>();

        foreach (var e in input)
        {
            result.Add(new Vector3(e.x+firstNodeOffset[0],e.y,e.z+firstNodeOffset[1]));
        }

        return result;

    }

    private List<Vector3> reDescribeNodesForCreation(List<Vector3> input, int normalizeBy)
    {
        //float oneXUnit = Screen.width / scale;
        //float oneYUnit = Screen.height / scale;
        
        float oneXUnit = (_xMostLimit-_xLeastLimit) / normalizeBy;
        //float oneYUnit = (_yMostLimit-_yLeastLimit) / scale;
        float oneYUnit = (_xMostLimit-_xLeastLimit) / normalizeBy;
        
        List<Vector3> result=new List<Vector3>();

        foreach (var e in input)
        {
            result.Add(new Vector3((e.x/oneXUnit)*scaleFactor,(e.y)*scaleFactor,(e.z/oneYUnit)*scaleFactor));
        }

        return result;

    }

    public void FixCenter()
    {
        //----reCenter();
        
        /*old tracker3 version:
        //setAline(tracker3.objectB.gameObject, 'x', 'l');
        //fixCenter Fixer = tracker3.transform.gameObject.GetComponent<fixCenter>();
        //Fixer.reCenter();
       // tracker3.teleportToTarget();
         */
    }

    private void setAline(GameObject obj,char A,char LOCAL_GLOBAL)
    {
        var position = obj.transform.position;
        Vector3 oldPos=new Vector3(position.x,position.y,position.z);
        var localPosition = obj.transform.localPosition;
        Vector3 oldLocalPos=new Vector3(localPosition.x,localPosition.y,localPosition.z);
        
         
        
        if (LOCAL_GLOBAL=='l'||LOCAL_GLOBAL=='L')
        {
            oldLocalPos = resetAxis(oldLocalPos, A);
            obj.transform.localPosition = oldLocalPos;
        }
        else
        {
            oldPos = resetAxis(oldPos, A);
            obj.transform.position = oldPos;
        }
        
        
        //tracker3.objectB.transform.localPosition=new Vector3(0,0,0);
    }

    private Vector3 resetAxis(Vector3 input,char A)
    {
        Vector3 result=new Vector3(input.x,input.y,input.z);
        switch (A)
        {
            case 'x': case 'X':
                result=new Vector3(0,result.y,result.z);
                break;
            case 'y': case 'Y':
                result=new Vector3(result.x,0,result.z);
                break;
            case 'z': case 'Z':
                result=new Vector3(result.x,result.y,0);
                break;
        }

        return result;
    }

    private float findMost(char A,List<Vector3> input)
    {
        float result=0;

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
    
    
    
    
    
    //-------------------------------
    
   

    private void recenterLocal(float offsetX,float offsetY, float offsetZ)
    {
      //  var transformLocalPosition = transform.localPosition;
        
      //  transformLocalPosition.x = offsetX;
        Debug.Log(" offsetX  : "+offsetX);
      //  transformLocalPosition.y = offsetY;
        Debug.Log(" offsetY  : "+offsetY);
      //  transformLocalPosition.z = offsetZ;
        Debug.Log(" offsetZ  : "+offsetZ);
        
        transform.localPosition = new Vector3(offsetX, offsetY, offsetZ);
    }
    /*-------------------------
     
     
      public void reCenter()
    {
        
 
        Vector3 mostX=findMaxCurrentNode('x');
        Vector3 mostY=findMaxCurrentNode('y');
        Vector3 mostZ=findMaxCurrentNode('z');
        Vector3 leastX=findMinCurrentNode('x');
        Vector3 leastY=findMinCurrentNode('y');
        Vector3 leastZ=findMinCurrentNode('z');
            

        float xCenter = (mostX.x + leastX.x) / 2;
        float yCenter = (mostY.y + leastY.y) / 2;
        float zCenter = (mostZ.z + leastZ.z) / 2;

        var position = transform.position;
        float xOffset = position.x-xCenter;
        float yOffset = position.y-yCenter;
        float zOffset = position.z-zCenter;
        
        Debug.Log("X  MOST  :  "+mostX.x+"    X  LEAST :  "+leastX.x);
        Debug.Log("X  CENTER  :  "+xCenter+"   Position X  :  "+position.x+"  Offset X  :  "+xOffset);
        Debug.Log("Y  MOST  :  "+mostY.y+"    Y  LEAST :  "+leastY.y);
        Debug.Log("Y CENTER  :  "+yCenter+"  Position Y  :  "+position.y+"  Offset Y :  "+yOffset);
        Debug.Log("Z MOST  :  "+mostZ.z+"    Z  LEAST :  "+leastZ.z);
        Debug.Log("Z  CENTER  :  "+zCenter+"  Position Z :  "+position.z+"  Offset Z :  "+zOffset);
        
       // tracker4.resetOffset();
       // tracker4.calibrateOffset(xOffset,yOffset,zOffset);
       // tracker4.teleportToTarget();

          recenterLocal(xOffset,yOffset,zOffset);

         
    }
     
     
    private Vector3 findMaxCurrentNode(char A)
    {
        Vector3[] preCursor = new Vector3[GenerateMesh.drawPositions.Count];

        for (int i=0;i<GenerateMesh.drawPositions.Count;i++)
        {
            preCursor[i] = GenerateMesh.drawPositions[i];

        }
            
            
        //  Vector3 result = new Vector3(transform.position.x,transform.position.y,transform.position.z);
      
      Vector3 result = preCursor[0];
        foreach (var e in preCursor)
        {
            switch (A)
            {
                case 'x':
                    
                    if(e.x>result.x)
                        result = e;
                    break;
                case 'y':
                    if(e.y>result.y)
                        result = e;
                    break;
                case 'z':
                    if(e.z>result.z)
                        result = e;
                    break;
            }

            

        }

        result=new Vector3(result.x*(transform.localScale.x),result.y*(transform.localScale.y),result.z*(transform.localScale.z));
        result=new Vector3(result.x+transform.position.x,result.y+transform.position.y,result.z+transform.position.z);
        
        return result;
    }
    private Vector3 findMinCurrentNode(char A)
    { 
        Vector3[] preCursor = new Vector3[GenerateMesh.drawPositions.Count];

        for (int i=0;i<GenerateMesh.drawPositions.Count;i++)
        {
            preCursor[i] = GenerateMesh.drawPositions[i];

        }
            
            
        //  Vector3 result = new Vector3(transform.position.x,transform.position.y,transform.position.z);
      
        Vector3 result = preCursor[0];
        foreach (var e in preCursor) 
        {
            switch (A)
            {
                case 'x':
                    
                    if(e.x<result.x)
                        result = e;
                    break;
                case 'y':
                    if(e.y<result.y)
                        result = e;
                    break;
                case 'z':
                    if(e.z<result.z)
                        result = e;
                    break;
            }

            

        }

        result=new Vector3(result.x*(transform.localScale.x),result.y*(transform.localScale.y),result.z*(transform.localScale.z));
        result=new Vector3(result.x+transform.position.x,result.y+transform.position.y,result.z+transform.position.z);
        return result;
    } */
    
    
    
    
}



/*
 
 tempNodeArray = makeFirstNodeTheCenter(tempNodeArray);
       //  tempNodeArray = makeFirstNodeCenter(tempNodeArray); round inputs or not ?
       // tempNodeArray = makeFirstNodeCenter(tempNodeArray); select out unnecessary nodes
       tempNodeArray = reDescribeNodesForCreation(tempNodeArray, 10);
       
       finalNodeArray = tempNodeArray;
       GenerateMesh.drawPositions.Clear(); 
       GenerateMesh.drawPositions = finalNodeArray;
       GameManager.Instance.newObjectReady = true;
       GameManager.Instance.inputTakenCorrectly = true;
       tempNodeArray=new List<Vector3>();

      
     //  nodePackage = finalNodeArray.ToArray();
       
       // find horizontal center of the object and if is it right side or the left and the distance between it and the panel center we will use this in tracer offset
       // find width of the object use it to reSet swipe limit 
       // 
       
       
       
       // call the make new object function ( )
       foreach (var e in finalNodeArray)
       {
           Debug.Log(" the nodes we acquired  :  "+e);
       }

       Debug.Log("node count "+finalNodeArray.Count);
       Debug.Log("maximum width  : "+Screen.width);
       Debug.Log("maximum height  : "+Screen.height);
 
 */