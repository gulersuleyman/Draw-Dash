using UnityEngine;

namespace Scenes.Levels.example.tracking
{
    public class TrackRotation : MonoBehaviour
    {
        [SerializeField] private bool work=true;
        private  GameObject objectA;
         
        [SerializeField] private  GameObject objectB;
         
        [SerializeField] private bool willTrack_elseCarry_ =true;
        
        [SerializeField] private bool mimicTargetRotation = false;
        [SerializeField] private bool faceTheTarget = true;
        [SerializeField] private string acceptMovementInDimensions="xyz";
        [SerializeField] private float facingSpeed = 20;
        
        
        private bool faceX;
        private bool faceY;
        private bool faceZ;
        
        
        private bool _faceTheTarget = false;
        private bool _mimicTargetRotation = false;
        
        void Start()
        {
            
            adjustments();

            exceptAxes(acceptMovementInDimensions);

        }

        
        void FixedUpdate()
        {

            resolveContrast();
            facingTarget(_faceTheTarget);
            mimicTarget(_mimicTargetRotation);

        }

         

        private void resolveContrast()
        {

            if (_mimicTargetRotation!=mimicTargetRotation)
            {
                if (mimicTargetRotation)
                {
                    faceTheTarget = false;
                    _faceTheTarget = false;
                    
                }
                _mimicTargetRotation = mimicTargetRotation;
            }
            if (_faceTheTarget!=faceTheTarget)
            {
                if (faceTheTarget)
                {
                    mimicTargetRotation = false;
                    _mimicTargetRotation = false;
                    
                }
                _faceTheTarget = faceTheTarget;
            }


        }

        private void exceptAxes(string input)
        {
            bool x = false, y = false, z = false;
            

            char[] charList = input.ToCharArray();
            foreach (var e in charList)
            {
                switch (e)
                {
                    case 'x':
                    case 'X':
                        x = true;
                        break;
                    case 'y':
                    case 'Y':
                        y = true;
                        break;
                    case 'z':
                    case 'Z':
                        z = true;
                        break;
                }
            }

            faceX = x;
            faceY = y;
            faceZ = z;

        }

        private void adjustments()
        {
            if (objectA == null)
                objectA = this.gameObject; 
            
            if (!willTrack_elseCarry_ )
            {
                objectA = objectB;
                objectB=this.gameObject;
            }
            
            //---------------------------------------
            
            if (mimicTargetRotation)
            {
                _mimicTargetRotation = true;
                faceTheTarget = false;
            }
                
            _faceTheTarget = faceTheTarget;

        }

        private void mimicTarget(bool doOrNot)
        {

            if(doOrNot)
                objectA.transform.rotation = objectB.transform.rotation;


        }

        private void facingTarget(bool doOrNot)
        {

            if (doOrNot)
            {
                float speed = facingSpeed;
                Vector3 from = objectA.transform.position;
                Vector3 to = objectB.transform.position;

                if (!faceX)
                    to=new Vector3(from.x,to.y,to.z);
                if (!faceY)
                    to=new Vector3(to.x,from.y,to.z);
                if (!faceZ)
                    to=new Vector3(to.x,to.y,from.z);

                Quaternion lookRotation = Quaternion.LookRotation((to - from).normalized);

          

                objectA.transform.rotation = Quaternion.Slerp(objectA.transform.rotation, lookRotation, Time.deltaTime * speed);
            }
        
        
        }

    }
    
   



}
