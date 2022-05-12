using UnityEngine;

namespace Scenes.Levels.example.input.AxisManipulation
{
    public class OneAxisManipulator : MonoBehaviour
    {

        [SerializeField] private bool working = true;
        [SerializeField] private bool inputHorizontalElseVertical=true;
        [SerializeField] private char worldAxis='x';
        [SerializeField] private bool reverseControls = false;
        [SerializeField] private bool onlyMoverElseExtraFactor=true;
        [SerializeField] private bool allowWastedTouch=false;
        [SerializeField] private float basePixelPercent=1f;
        [SerializeField] private float worldEquivalent=0.1f;
        [SerializeField] private bool useWorldLimit=false ;
        [SerializeField] private bool makeScreenMaxEqualsToLimits=false ;  
        [SerializeField] private float [] limitMinMax=new float[2];
     
        //-------------------------------------------------------------

        private float reverseFactor = 1f;
    
        private float _firstWorld;
        private float _currentWorld;
    
        private float _firstScreen;
        // private float _currentScreen;

        private bool _onTheMove = false;

        private float _basePixelCount;
        // private float _baseWorldDistance;
        void Start()
        {
            if (reverseControls)
                reverseFactor = -1;

            limitMinMax = checkOrder(limitMinMax);

            calculateRates();

        }

     
        void FixedUpdate()
        {
            if (working)
            {
                reverseFactor = reverseControls ? -1 : 1;

                if (firstTouch())
                {
                    resetValues();
                }

                if (fingerMove())
                {
                    if (!allowWastedTouch)
                    {
                        discardExtra();
                    }
                
                
                    move();
                }

                 
            }
            
        }

      
        private void move()
        {

            if (onlyMoverElseExtraFactor)
                soloMove();
            else extraMove();



        }
        private void soloMove()
        {

            if ( ( ! maxLimitBreach()&&!minLimitBreach() ) || ( maxLimitBreach()&&asksForDecrease() ) || ( minLimitBreach()&& asksForIncrease() ) )
            {
                var addition = ((getCurrentScreen()-_firstScreen)/_basePixelCount)*worldEquivalent*reverseFactor;
                var axisMust = (_firstWorld + addition);

                if (useWorldLimit)
                {
                    axisMust = axisMust < limitMinMax[0] ? limitMinMax[0] : axisMust;
                    axisMust = axisMust > limitMinMax[1] ? limitMinMax[1] : axisMust;
                }
            
                transform.position = assignAxis(worldAxis, axisMust, transform);
            }



        }
        private void extraMove()
        {

            if ( ( ! maxLimitBreach()&&!minLimitBreach() ) || ( maxLimitBreach()&&asksForDecrease() ) || ( minLimitBreach()&& asksForIncrease() ) )
            {
                var baseExtra=(getCurrentScreen()-_firstScreen)/_basePixelCount;

                if (Mathf.Abs(baseExtra)>=1)
                {
                    var extra = baseExtra * worldEquivalent * reverseFactor;

                    float position = getCurrentWorld(worldAxis, transform);

                    if ( ( (position+extra)>limitMinMax[1] ) || ( (position+extra)<limitMinMax[0] ) )  
                    {
                        resetValues();
                        return;
                    }
                
                
                    transform.position +=  extraToAxis(worldAxis, extra);
                    resetValues();
                }
            }
        }
        private void resetValues()
        {

            _firstScreen = getCurrentScreen();
            _firstWorld = getCurrentWorld(worldAxis,transform);


        }

        

        private bool firstTouch()
        {
            if(!_onTheMove&& (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began || Input.GetMouseButton(0)))
            {
                _onTheMove = true;
                return true;
            }
            return false;
        }
        private bool fingerMove()
        {
            if (_onTheMove&&(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetMouseButton(0)))
            {
           
                return true;
            }
            else if(_onTheMove)
                _onTheMove = false;
        
            return false;
        }
    
    
        private float getCurrentScreen()
        {
            return inputHorizontalElseVertical ? Input.mousePosition.x : Input.mousePosition.y;
        }
    
        private float getCurrentWorld(char A,Transform subject)
        {
            float result=0;

            switch (A)
            {
                case 'x':case 'X':

                    result = subject.position.x;
                    break;
                case 'y':case 'Y':
                
                    result= subject.position.y;
                    break;
                case 'z':case 'Z':
                
                    result = subject.position.z;
                    break;
            }

            return result;
        
        }

        private Vector3 assignAxis(char A,float replacement, Transform subject)
        {
            var position = subject.position;
        
            var result= new Vector3(position.x,position.y,position.z);

            switch (A)
            {
                case 'x':case 'X':

                    result.x = replacement;
                    break;
                case 'y':case 'Y':
                
                    result.y = replacement;
                    break;
                case 'z':case 'Z':
                
                    result.z = replacement;
                    break;
            }


            return result;
        }
        private Vector3 extraToAxis(char A,float extra )
        {
        
            var result= new Vector3();

            switch (A)
            {
                case 'x':case 'X':

                    result.x = extra;
                    break;
                case 'y':case 'Y':
                
                    result.y = extra;
                    break;
                case 'z':case 'Z':
                
                    result.z = extra;
                    break;
            }


            return result;
        }
    
    
        private void discardExtra()
        { 
            if ((maxLimitBreach()&&asksForIncrease())||(minLimitBreach()&&asksForDecrease()) )
                resetValues(); 
        }

        private bool maxLimitBreach()
        { 
            return !useWorldLimit || ((limitMinMax[1]-getCurrentWorld(worldAxis, transform) )<=0);
 
        }
        private bool minLimitBreach()
        { 
            return !useWorldLimit || ((getCurrentWorld(worldAxis,transform)-limitMinMax[0])<=0);
 
        }
        private bool asksForIncrease()
        { 
            var addition = ((getCurrentScreen()-_firstScreen)/_basePixelCount)*worldEquivalent*reverseFactor;
               
            var axisMust = (_firstWorld + addition);                         
        
             
            return  (axisMust > getCurrentWorld(worldAxis,transform)) ;
 
        }
        private bool asksForDecrease()
        { 
             var addition = ((getCurrentScreen()-_firstScreen)/_basePixelCount)*worldEquivalent*reverseFactor;
                           
             var axisMust = (_firstWorld + addition);                         
                    
                         
             return  (axisMust < getCurrentWorld(worldAxis,transform)) ;
 
        }
    
        public void setOff()
        {
            working = false;
            _onTheMove = false;
        

        }

        public void setOn()
        { 
            working = true;
        }
    
        private void calculateRates()
        {

            float totalPixelCount = Screen.width; // if vertical maybe use the height  ? 

            _basePixelCount = (totalPixelCount / 100) * basePixelPercent;

            if (makeScreenMaxEqualsToLimits)
            {
                float nineOfTen = totalPixelCount - (totalPixelCount / 10);

                float limitWidth = limitMinMax[1] - limitMinMax[0];

                worldEquivalent = ((int )((limitWidth / (nineOfTen / _basePixelCount))*100)) /100f;

            }
         


        }
        
        
        private float [] checkOrder(float[] input) // being fancy eh ? 
        {
                    if (!(input[0] > input[1])) return input;
                
                    input[0] += input[1];
                    input[1] = input[0] - input[1];
                    input[0] = input[0] - input[1];
                            
                    return input;          
        }
        

 private bool firstTouchAlternate()
        {
            if(!_onTheMove&&  Input.GetMouseButton(0))
            {
                _onTheMove = true;
                return true;
            }
            return false;
        }
        private bool fingerMoveAlternate()
        {
            if (_onTheMove&&Input.GetMouseButton(0))
            {
           
                return true;
            }
            else if(_onTheMove)
                _onTheMove = false;
        
            return false;
        }


    }
    
}
