using UnityEngine;

namespace Scenes.Levels.example.input.fingerTracingOnPlane
{
    public class SecondaryFingerTracing : MonoBehaviour
    {
        [SerializeField] private bool working = true;

        [SerializeField] private bool allowWastedTouch = true;
    
        [SerializeField] private GameObject target;
        [SerializeField] private Vector3 offsetsToTarget=new Vector3();
    
        [SerializeField] private GameObject limitExample;// if added will be preferred limit reference
        [SerializeField] private bool useXLimit; 
        [SerializeField] private bool useYLimit; 
        [SerializeField] private bool useZLimit; 
        [SerializeField] private Vector3 mostLimits=new Vector3();
        [SerializeField] private Vector3 leastLimits=new Vector3();
        
        [SerializeField] private bool xIsFixed;
        [SerializeField] private bool yIsFixed;
        [SerializeField] private bool zIsFixed;
        [SerializeField] private Vector3 fixedPositions= new Vector3(0,0,0);
        
        private bool firstTime = true;

     
        //------------------------------------------------------------------------------------------------

        [SerializeField] private string TargetProperties="down from here is for primary tracker";
    
        [SerializeField] private Vector3 offset=new Vector3();
        [SerializeField] private bool xIssFixed;
        [SerializeField] private bool yIssFixed;
        [SerializeField] private bool zIssFixed;
        [SerializeField] private Vector3 targetFixedPositions= new Vector3(0,0,0);
        [SerializeField] private string detectionLayer="mouseTrack";
        private Camera _camera;
    
        
    
    
        private void Awake()
        {
            _camera = Camera.main;
        
            var transformTarget = target.transform;
        
            var editorPositionTarget = transformTarget.position;
         
            transformTarget.position = positionAline(new Vector3(editorPositionTarget.x, editorPositionTarget.y , editorPositionTarget.z));
            
        }
    
    
        void Start()
        {
         
            if(limitExample!=null)
                getExampleLimitations(limitExample,useXLimit,useYLimit,useZLimit);
            offsetsToTarget = getOffsets(offsetsToTarget);

        }

     
        void FixedUpdate()
        {
            if (!working) return;
        
             
             
                if (/* firstTouch() ||*/ ( firstTime&&      Input.GetMouseButton(0)   )   )// first touch
                {
                    fingerTrack();
                    resetTargetOffsets(true,true,true);
                    firstTime = false;

                }

                if (/*touchMoved() || */(  !firstTime && Input.GetMouseButton(0))  ) //while touching
                {
                    fingerTrack();
                    trackTarget();
                }

                if (!firstTime&&!Input.GetMouseButton(0))
                {
                    firstTime = true;
                  
                }
          
        }

        public void setOff()
        {

            firstTime = true;
            working = false;



        }
        public void setOn()
        {

         
            working = true;



        }

        private bool firstTouch()
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began )
                return true;
            return false;
        }

        private bool touchMoved()
        {
        
            if (Input.GetTouch(0).phase == TouchPhase.Moved )
                return true;
            return false;
        }

        private bool touchEnded()
        {
        
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended )
                return true;
            return false;
        
        }

        private void trackTarget()
        {
            var currentPosition = transform.position;
            var targetPosition = target.transform.position;


            float targetX =  targetPosition.x + offsetsToTarget.x ;
            float targetY = targetPosition.y + offsetsToTarget.y;
            float targetZ = targetPosition.z + offsetsToTarget.z;
            
            if (!allowWastedTouch)
            {
                if ( !checkLimitation('x',targetX))
                    resetTargetOffset('x');
                if ( !checkLimitation('y',targetX))
                    resetTargetOffset('y');
                if ( !checkLimitation('z',targetX))
                    resetTargetOffset('z');
                
            }
            
            targetX =(  checkLimitation('x',targetX) ?  targetX : currentPosition.x );
            targetY =(  checkLimitation('y',targetY) ?  targetY: currentPosition.y );
            targetZ =(  checkLimitation('z',targetZ ) ?  targetZ  : currentPosition.z );

            Vector3 newPosition = new Vector3(targetX, targetY, targetZ);

            transform.position = newPosition;

            
 
        }
        private bool checkLimitation(char Axis,float destination)
        {
         
            switch (Axis)
            {
                case 'x':
                    if (useXLimit && (  ( destination < leastLimits.x || destination > mostLimits.x )) )   
                        return false;
                    break;
                
                case 'y':
                    if (useYLimit && (  ( destination < leastLimits.y || destination > mostLimits.y )) )
                        return false;
                    break;
                case 'z':
                    if (useZLimit && (  ( destination < leastLimits.z || destination > mostLimits.z )) )
                        return false;
                    break;
             
            }

            return true;
         
        }

        private Vector3 getOffsets(Vector3 input)
        {
        
            return  (  findDistances(this.gameObject,target) + input);
 
        }

        private void resetTargetOffset(char resetAxis)
        {
            switch (resetAxis)
            {
                case 'x':case 'X':
                    resetTargetOffsets(true,false,false);
                    break;
                case 'y':case 'Y':
                    resetTargetOffsets(false,true,false);
                    break;
                case 'z':case 'Z':
                    resetTargetOffsets(false,false,true);
                    break;
            }

            
        }
       
        private void resetTargetOffsets(bool resetX,bool resetY,bool resetZ)
        { 
            Vector3 tempOffsets=findDistances(this.gameObject, target);

            if (resetX)
                offsetsToTarget.x = tempOffsets.x;
            if (resetY)
                offsetsToTarget.y = tempOffsets.y;
            if (resetZ)
                offsetsToTarget.z = tempOffsets.z;
            
             
 
        }
        private Vector3 findDistances(GameObject objectA,GameObject objectB)
        {
            Vector3 result = new Vector3();

            var aPosition = objectA.transform.position;
            var bPosition = objectB.transform.position;
        
            result.x = aPosition.x - bPosition.x;
            result.y = aPosition.y - bPosition.y;
            result.z = aPosition.z - bPosition.z;


            return result;
        } 
    
        private void getExampleLimitations(GameObject subject,bool useX,bool useY,bool useZ )
        {
            float []  Limits=new float[6];

            float xSize = subject.GetComponent<Renderer>().bounds.size.x;
            float ySize = subject.GetComponent<Renderer>().bounds.size.y;
            float zSize = subject.GetComponent<Renderer>().bounds.size.z;
            Vector3 subjectPosition = subject.transform.position;

            if (useX )
            {
                Limits[0] = subjectPosition.x - xSize/2;
                Limits[1] = subjectPosition.x + xSize/2;
            }

            if (useY)
            {
                Limits[2] = subjectPosition.y - ySize/2;
                Limits[3] = subjectPosition.y + ySize/2;
            }    

            if (useZ)
            {
                Limits[4] = subjectPosition.z - zSize/2;
                Limits[5] = subjectPosition.z + zSize/2;
            }

      
      
      
     
            leastLimits.x = Limits[0];
            mostLimits.x = Limits[1];
            leastLimits.y = Limits[2];
            mostLimits.y = Limits[3];
            leastLimits.z = Limits[4];
            mostLimits.z = Limits[5];

      
            //return Limits
      

        }

//---------------------------------------------------------------------------------------------------

        private void fingerTrack()
        {
            if (_camera is null) return;
        
            var ray = _camera.ScreenPointToRay(Input.mousePosition); //Ray ray

            if( Physics.Raycast(ray , out var hit, 1000, 1 << LayerMask.NameToLayer(detectionLayer))) //it wont see other layers 
          
            {

                target.transform.position = positionAline(new Vector3 (hit.point.x, hit.point.y, hit.point.z));
            }
        }
    
        private Vector3 positionAline(Vector3 input)
        {

            input += offset;
            
            
            if (xIssFixed)
            {
                input.x = targetFixedPositions.x;
            } 
            if (yIssFixed)
            {
                input.y = targetFixedPositions.y;
            } 
            if (zIssFixed)
            {
                input.z = targetFixedPositions.z;
            } 
        
            return input;
        }
    
    }
}
