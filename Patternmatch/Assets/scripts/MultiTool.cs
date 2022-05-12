using System; 
using System.Collections.Generic; 
using UnityEngine;

public class MultiTool : MonoBehaviour
{
    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;
    
 /* // look up to the PhysicsHelper
//-----------------------------------------------------------
    check out invoke method 
    
     to get all components in all child objects(grand children too) use this.GetComponentsInChildren<Collider>();
   
   private void OnCollisionEnter(Collision collision) // collision.collider etc.. 
   
   private void OnTriggerEnter(Collider other)
   private void OnTriggerStay(Collider other) // works while collision exist
   private void OnTriggerExit(Collider other) // when an object exits it works once
    
//-----------------------------------------------------------*/
    
    public GameObject findNamedObject( string word ) // finding object by  name first if not with tag if  not with layer
    { 
        var  result = GameObject.Find(word);
        if(result==null)
            result=GameObject.FindWithTag(word);
        if(result==null)
            result=findLayeredObjects(word)[0];
        
        return result;
    }

    public GameObject findPlayer()// find player object by name 
    {
       GameObject result = findNamedObject("player");
       if (result==null)
           result = findNamedObject("Player");
       else if (result==null)
           result = findNamedObject("PLAYER");
       
       return result;
 
    } 
    
    public GameObject []  findTaggedObjects(string Tag) // finding objects with tag 
    {
        return GameObject.FindGameObjectsWithTag(Tag); 
    }
   
    public GameObject  findTaggedObject (string Tag) // finding object with tag 
    {
        return GameObject.FindGameObjectWithTag(Tag); 
    }
   
    public GameObject findLayeredObject (string layer)// finding objects with layer 
    {
        return findLayeredObjects(layer)[0];
    }
    public GameObject []  findLayeredObjects(string layer) // finding objects with layer
    {
        
        GameObject[] allAbjects = findAllObjects();
        List <GameObject> matchedObjects = new List <GameObject>();

        foreach (var e in allAbjects)
        {
            if(LayerMask.LayerToName(e.layer)==layer) // to change layer to name
                matchedObjects.Add(e);
        }
        
        
        return GameObj_LtoA(matchedObjects);
    }                                 // TO DO ARRAY LIST CONFLICT
    public GameObject[] findTag_LayerObjects(string Tag,string layer,char Both_Any)  // find objects by layer and tag all or any match its optional 
    {
        GameObject[] allAbjects = findAllObjects();
        List <GameObject> matchedObjects = new List <GameObject>();

        foreach (var e in allAbjects)
        {
            bool checker=false;
            if (Both_Any=='B'||Both_Any=='b')
            {
                checker = (e.CompareTag(Tag) || LayerMask.LayerToName(e.layer) == layer);
            }
            if (Both_Any=='A'||Both_Any=='a')
            {
                checker = (e.CompareTag(Tag) && LayerMask.LayerToName(e.layer) == layer);
            }
            
            if(  checker) // to change layer to name
                matchedObjects.Add(e);
        }
         
        return GameObj_LtoA(matchedObjects);
    }  // TO DO ARRAY LIST CONFLICT

    public GameObject findTag_LayerObject(string Tag, string layer,char Both_Any)
    {
        return findTag_LayerObjects(Tag, layer,Both_Any)[0];
    }

    public GameObject []  findAllObjects() // finding ALL objects   
    {
        return GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
        
    }

    public void objectActivity(GameObject subject,bool newState) // setting object Activity
    {
        subject.SetActive(newState);
    }
    
    public void objectActivity(GameObject [] subject,bool newState)  // setting object Activity 
    {
        foreach (var e in subject)
            objectActivity( e ,newState);
    }
    
    public void objectActivity(GameObject []  subject,bool newState,bool term ) // setting object Activity 
    {
        if (!term) return;
         
        objectActivity(subject,newState);

    }
     
    public void objectActivity(GameObject subject,bool newState,bool term ) // setting object Activity 
    {
        if (!term) return;
         
        objectActivity(subject,newState);

    }

    public void destroyObjects(GameObject toDestroyObjects) // destroying object
    {
        Destroy(toDestroyObjects); // if you want to destroy immediatly, use DestroyImmediate(x);
    }
    
    public void destroyObjects(GameObject [] toDestroyObjects)  // destroying objects 
    {
        foreach (var e in toDestroyObjects)
            destroyObjects( e );
    }
    
    private bool firstTouch()
    {

        return Input.GetMouseButtonDown(0);

    }
    
    private bool fingerHasMoved()
    {
         
        return (Input.GetAxis("Mouse X") != 0) || (Input.GetAxis("Mouse Y") != 0);
    }

    private bool touchEnded()
    {

        return Input.GetMouseButtonUp(0);
    }
    
    private float getCurrentScreen(bool inputHorizontalElseVertical)
    {
        return inputHorizontalElseVertical ? Input.mousePosition.x : Input.mousePosition.y;
    }
    
    public bool validateMouseInput(char A,float _hLeastLimit,float _hMostLimit,float _vLeastLimit,float _vMostLimit )
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
    public float fixedMouseInput(char A,float _hLeastLimit,float _hMostLimit,float _vLeastLimit,float _vMostLimit )
    {
        float result = 0;
        switch (A)
        {
            case 'h':case 'H':
                if (Input.mousePosition.x > _hMostLimit)
                    result=_hMostLimit;
                else if (Input.mousePosition.x < _hLeastLimit)
                    result = _hLeastLimit;
                else result = Input.mousePosition.x;
                 
                break;
            case 'v':case 'V':
                if (Input.mousePosition.y > _vMostLimit)
                    result=_vMostLimit;
                else if (Input.mousePosition.y < _vLeastLimit)
                    result=_vLeastLimit;
                else result = Input.mousePosition.y;
                break;
            default:
             break;
        }

        return result;
    }

    private bool itIsInLimit(char checkMaxLeastBoth,Vector3 LeastLimits,Vector3 MaxLimits,Vector3 subject,string Axes,bool checkIfItsFitsAtLeastOneAxis=false)
    {

        
        bool checkX = checkChar('X', Axes);
        bool checkY = checkChar('Y', Axes);
        bool checkZ = checkChar('Z', Axes);
        bool checkMax = checkChar('b', checkMaxLeastBoth) || checkChar('m', checkMaxLeastBoth);
        bool checkLeast = checkChar('b', checkMaxLeastBoth) || checkChar('l', checkMaxLeastBoth);
        
        bool result = true;
        
        if (checkIfItsFitsAtLeastOneAxis)
        {
            bool output = false;
            if (checkX)
            {
                if (itIsInLimit( checkMaxLeastBoth, LeastLimits, MaxLimits, subject,"x"))
                {
                    return true;
                }
            }
            if (checkY)
            {
                if (itIsInLimit( checkMaxLeastBoth, LeastLimits, MaxLimits, subject,"y"))
                {
                    return true;
                }
            }
            if (checkZ)
            {
                if (itIsInLimit( checkMaxLeastBoth, LeastLimits, MaxLimits, subject,"z"))
                {
                    return true;
                }
            }
        }

        
        

        return result;

    }
    private bool itIsInLimit(char checkMaxLeastBoth,float LeastLimit,float maxLimit,Vector3 subject,string Axes,bool checkIfItsFitsAtLeastOneAxis=false)
    {
        Vector3 MaxLimits=new Vector3(maxLimit,maxLimit,maxLimit);
        Vector3 LeastLimits=new Vector3(LeastLimit,LeastLimit,LeastLimit);

        return itIsInLimit(checkMaxLeastBoth,  LeastLimits, MaxLimits,subject, Axes,checkIfItsFitsAtLeastOneAxis);



    }
    private bool itIsInLimit(char checkMaxLeastBoth,float LeastLimit,float maxLimit,float subject)
    {
        Vector3 MaxLimits=new Vector3(maxLimit,maxLimit,maxLimit);
        Vector3 LeastLimits=new Vector3(LeastLimit,LeastLimit,LeastLimit);
        Vector3 subjectVector=new Vector3(subject,subject,subject);

        return itIsInLimit(checkMaxLeastBoth,  LeastLimits, MaxLimits,subjectVector, "x");



    }
/* dumb
    private float speedTime(char desiredOutput,float theOther, float distance)
    {
        if (checkChar('s',desiredOutput))
        {
            return distance / theOther;
        }
        if (checkChar('t',desiredOutput))
        {
            return distance / theOther;
        }



    }*/

    public Vector3 addToTheVector3(Vector3 baseVector, float additionToX, float additionToY, float additionToZ)
    {
        return new Vector3(baseVector.x + additionToX, baseVector.y + additionToY, baseVector.z + additionToZ);

    } //-------------------------VECTOR 3
   
    public Vector3 addToTheVector3(Vector3 baseVector, char Axis, float addition )
    {
        float x=0, y=0, z=0;
        
        switch (Axis)
        {
            case 'x' : case'X':
                x = addition;
                break;
            case 'y' : case'Y':
                y = addition;
                break;
            case 'z' : case'Z':
                z = addition;
                break;
            
        }
        return addToTheVector3(baseVector, x, y, z);
    }

    public Vector3 addToTheVector3(Vector3 baseVector, Vector3 additionalVector)
    {
       return addToTheVector3(baseVector, additionalVector.x ,additionalVector.y,additionalVector.z );

    }
    
    private float findMostValueInVectors(char Axis,List<Vector3> input)
    {
        float result=0;

        foreach (var e in input)
        {
            switch (Axis)
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
    
    private float findLeastValueInVectors(char Axis,List<Vector3> input)
    {
        float result=0;

        foreach (var e in input)
        {
            switch (Axis)
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
    
    private Vector3 findMostVector(char Axis,List<Vector3> input)
    {
        Vector3 result=new Vector3();
        float temp=0;

        foreach (var e in input)
        {
            switch (Axis)
            {
                case 'x':case 'X':
                {
                    if (e.x > temp)
                    {
                        temp = e.x;
                        result = e;
                    }
                        
                    
                    break;
                }
                case 'y':case 'Y':
                {
                    if (e.y > temp)
                    {
                        temp = e.y;
                        result = e;
                    }
                        
                    break;
                }
                case 'z':case 'Z':
                {
                    if (e.z > temp)
                    {
                        temp = e.z;
                        result = e;
                    }
                        
                    break;
                }
            }
        }

        return result;
    }
    
    private Vector3 findLeastVector(char Axis,List<Vector3> input)
    {
        Vector3 result=new Vector3();
        float temp=0;

        foreach (var e in input)
        {
            switch (Axis)
            {
                case 'x':case 'X':
                {
                    if (e.x < temp)
                    {
                        temp = e.x;
                        result = e;
                    }
                        
                    break;
                }
                case 'y':case 'Y':
                {
                    if (e.y < temp)
                    {
                        temp = e.y;
                        result = e;
                    }
                        
                    break;
                }
                case 'z':case 'Z':
                {
                    if (e.z < temp)
                    {
                        temp = e.z;
                        result = e;
                    }
                        
                    break;
                }
            }
        }

        return result;
    }
  
    private bool checkChar(char judge,char Example)// checks char if matching
    {
        char judgeOther;
        if (Char.IsUpper(judge))
        {
            judgeOther = Char.ToLower(judge);
        }
        else
        {
            judgeOther = Char.ToUpper(judge);
        }
        char other;
        if (Char.IsUpper(Example))
        {
            other = Char.ToLower(Example);
        }
        else
        {
            other = Char.ToUpper(Example);
        }

        if (  (judge==Example||judge==other) || (judgeOther==Example||judgeOther==other )  )
        {
            return true;
        }
        return false;


    }
    
    public bool checkChar(char judge,String Example) // checks string if it has matching char 
    {
         
        char [] array = Example.ToCharArray();

        
        foreach (var e in array)
        {
            if (checkChar(judge, e) )
            {
                return true;
            }
        }
        
        return false;
    } 
    
   

    public float distanceBetween(GameObject a, GameObject b) //gives the distance between 
    {
        return Vector3.Distance(a.transform.position, b.transform.position);

    }
    public float distanceBetween(GameObject a, Transform b) //gives the distance between 
    {
        return Vector3.Distance(a.transform.position, b.position);

    }
    public float distanceBetween(Transform a, GameObject b) //gives the distance between 
    {
        return Vector3.Distance(a.position, b.transform.position);

    }
    public float distanceBetween(Transform a, Transform b) //gives the distance between 
    {
        return Vector3.Distance(a.position, b.position);

    }
    
    private List<Vector3>  updateVectors(List<Vector3> main,Vector3 addition,char Plus_Minus,bool additionPrimary = false)
    {
        List<Vector3> result=new List<Vector3>();


        for(int i=0;i<main.Count-1; i++)
        {
            Vector3 temp =  main[i]+addition;
            if (Plus_Minus=='m'||Plus_Minus=='M')
            {
                temp = main[i] - addition;
                if (additionPrimary)
                {
                    temp = addition-main[i] ;
                }
            }
            result.Add(temp);
             
        }

        return result;
    }
    private Vector3 addAxisToAxis(char baseAxis, Vector3 baseVector3 ,char additionAxis,Vector3 additiveVector)
    {
        return setAxis(baseAxis, baseVector3, getAxis(additionAxis, additiveVector));
    }
    
    private float getAxis(char A,Vector3 input)
    {
        float result = 0;

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
    
    private Vector3 setAxis(char Axis,Vector3 input,float value)
    {
        switch (Axis)
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

    private Vector3 [] DuplicateArray(Vector3 [] input) // copies vector3 Array
    { 
        Vector3 [] output =new Vector3[input.Length];
        for (int i = 0; i < output.Length; i++)
        {
            output[i] = input[i];
        }

        return output;
    }
    
    private int [] DuplicateArray(int [] input)  // copies int Array
    { 
        int [] output =new int[input.Length];
        for (int i = 0; i < output.Length; i++)
        {
            output[i] = input[i];
        }

        return output;
    }
    
    private float [] DuplicateArray(float [] input)  // copies float Array
    { 
        float [] output =new float[input.Length];
        for (int i = 0; i < output.Length; i++)
        {
            output[i] = input[i];
        }

        return output;
    }
    
    public Vector3[] listToArray(List<Vector3> input)  // TO DO DOES THIS WORK ?
    {   
        Vector3[]  result = new Vector3 [input.Count];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = input[i];
        }

        return result;
    }
    
    public int[] listToArray(List<int> input)  // TO DO DOES THIS WORK ?
    {   
        int[]  result = new int [input.Count];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = input[i];
        }

        return result;
    }

    public List<int> arrayToList(int[] input)  // TO DO DOES THIS WORK ?
    {
        List<int> output=new List<int>();
        foreach (var e in input)
        {
            output.Add(e);
        }

        return output;
    }
    
    public List<Vector3> arrayToList(Vector3[] input)  // TO DO DOES THIS WORK ?
    {
        List<Vector3> output=new List<Vector3>();
        foreach (var e in input)
        {
            output.Add(e);
        }
        
        return output;
    }
    public GameObject[] GameObj_LtoA(List<GameObject> input)   // TO DO DOES THIS WORK ?  
    {   GameObject [] result = new GameObject[input.Count];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = input[i];
        }

        return result;
    }        
    
    public List<GameObject> GameObj_AtoL( GameObject[] input)   // TO DO DOES THIS WORK ? 
    {   List<GameObject> result = new List<GameObject>();
        foreach (var e in input)
        {
            result.Add(e);
        }

        return result;
    }

    public List<Vector3> combineCollections(List<Vector3> primary, Vector3[] secondary)  // TO DO DOES THIS WORK ?
    {
        List<Vector3> output=new List<Vector3>();
        
        foreach (var e in primary)
        {
            output.Add(e);
        }
        foreach (var e in secondary)
        {
            output.Add(e);
        }
        return output;
    }
    
    public List<Vector3> combineCollections(List<Vector3> primary, List<Vector3>  secondary)  // TO DO DOES THIS WORK ?
    {
        List<Vector3> output=new List<Vector3>();

        foreach (var e in primary)
        {
            output.Add(e);
        }
        foreach (var e in secondary)
        {
            output.Add(e);
        }
        
        return output;
    }
    
    public Vector3[]combineCollections(Vector3[]  primary, Vector3[] secondary)  // TO DO DOES THIS WORK ?
    {
        Vector3[] output=new Vector3[primary.Length+secondary.Length];

        int x = 0;
        
        foreach (var t in primary)
        {
            output[x] = t;
            x++;
        }
        foreach (var e in secondary)
        {
            output[x] = e;
            x++;
        }
        
        return output;
    }
    
    public Vector3[]combineCollections(Vector3[] primary,List<Vector3> secondary )  // TO DO DOES THIS WORK ?
    {
        Vector3[] output=new Vector3[primary.Length+secondary.Count];
        
        int x = 0;
        
        foreach (var t in primary)
        {
            output[x] = t;
            x++;
        }
        foreach (var e in secondary)
        {
            output[x] = e;
            x++;
        }
        
        return output;
    }
    
    public int[]combineCollections(int[] primary,int[] secondary )  // TO DO DOES THIS WORK ?
    {
        int[] output=new int[primary.Length+secondary.Length];
        
        int x = 0;
        
        foreach (var t in primary)
        {
            output[x] = t;
            x++;
        }
        foreach (var e in secondary)
        {
            output[x] = e;
            x++;
        }
        
        return output;
    }
    
    public int[]combineCollections(int[] primary,List<int> secondary )  // TO DO DOES THIS WORK ? 
    {
        int[] output=new int[primary.Length+secondary.Count];
        
        int x = 0;
        
        foreach (var t in primary)
        {
            output[x] = t;
            x++;
        }
        foreach (var e in secondary)
        {
            output[x] = e;
            x++;
        }
        
        return output;
    }
    
    public void useImpulsiveForce  (GameObject subject, String direction,float power,float massFactor) // TO DO DOES THIS WORK ? 
    {
        Vector3 impulse= new Vector3(0, -0.01f, 0);
        switch ( direction.ToCharArray()[0])
        {
            case 'r' : case'R':
                impulse= new Vector3(0.01f, 0, 0);
                break;
            case 'l' : case'L':
                impulse = new Vector3(-0.01f, 0, 0);
                break;
            case 'f' : case'F':
                impulse = new Vector3(0, 0, 0.01f);;
                break;
            case 'b' : case'B':
                impulse= new Vector3(0, 0, -0.01f);
                break;
            case 'u' : case'U':
                impulse = new Vector3(0, 0.01f, 0);
                break;
            case 'd' : case'D':
                impulse = new Vector3(0, -0.01f, 0);
                break;
            
        }
        
        Rigidbody rb = subject.GetComponent<Rigidbody>();
        rb.AddForce(impulse * power*rb.mass, ForceMode.Impulse);
    }

    public void objectKinematic(GameObject subject,bool make ) // TO DO  DOES THIS WORK ?
    {
        Rigidbody rb = subject.GetComponent<Rigidbody>();
        rb.isKinematic = make;

    }
    
    public void objectsKinematic(GameObject [] subjects,bool make) // TO DO  DOES THIS WORK ?
    {
        foreach (var e in subjects)
        {
            objectKinematic(e,make);
        }

    }
    
    public void rbReleaseConstraints(GameObject subject) // TO DO  DOES THIS WORK ?
    {
        Rigidbody rb = subject.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.None;
    }
    
    public char swipeDetection()// TO DO IT DOESNT WORK I THINK
    {
        if (Input.GetMouseButtonDown(0))
        {
            //save began touch 2d point
            firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
           
        }

        if (Input.GetMouseButtonUp(0))
        {
            //save ended touch 2d point
            secondPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            //create vector from the two points
            currentSwipe = new Vector2(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

            //normalize the 2d vector
            currentSwipe.Normalize();



            //swipe upwards
            if ((currentSwipe.y > 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f))
            {
                Debug.Log("up swipe");
                return 'U';
            }

            //swipe down

            if ((currentSwipe.y < 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f))
            {
                Debug.Log("down swipe");
                return 'D';
            }

            //swipe left               
            if ((currentSwipe.x < 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f))
            {
                Debug.Log("left swipe");
                return 'L';
            }


            //swipe right

            if ((currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f))
            {

                Debug.Log("right swipe");
                return 'R';

            }
            

        }
        return 'N';
        
        
    }
    
    
}
// // TO DO  DOES THIS WORK ?
/*
    public void invoker(String methodName, float firstWaitingTime, float timesBetween, int totalTurns)
    {    
        float extraTime = timesBetween;
        for (int i = 0; i < totalTurns; i++)
        {
            Invoke(methodName, (firstWaitingTime+extraTime));
            extraTime += timesBetween;
        }
        
    }*/