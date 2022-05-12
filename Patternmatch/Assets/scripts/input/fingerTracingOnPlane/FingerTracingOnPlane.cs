using UnityEngine;

namespace Scenes.Levels.example.input.fingerTracingOnPlane
{
    public class FingerTracingOnPlane : MonoBehaviour  // this is for keeping track where the finger touch all the time ( ! it wont see the objects only the set ground )
    {
        [SerializeField] private bool working = true;
        [SerializeField] private bool wereWorking ;
         

        [SerializeField] private Vector3 offset=new Vector3();
        
        
        [SerializeField] private bool xIsFixed;
        [SerializeField] private bool yIsFixed;
        [SerializeField] private bool zIsFixed;
    
        [SerializeField] private Vector3 fixedPositions= new Vector3(0,0,0);
        [SerializeField] private string detectionLayer="inputDetectionLayer";
        private Camera _camera;


        private void Awake()
        {
            _camera = Camera.main;

        }

        private void Start()
        {
            wereWorking = working;
            var transformSelf = transform;
        
            var editorPosition = transformSelf.position;
            
            transformSelf.position = positionAline(new Vector3(editorPosition.x, editorPosition.y , editorPosition.z));
        }

        private void FixedUpdate()
        {
            adjustEditorChanges();
           

            if(working&&Input.GetMouseButton(0))
                track();
         
        }

        private void adjustEditorChanges()
        {
            if (working!=wereWorking)
            {
                if (working)
                    setOn();
                else
                    setOff();
                wereWorking = working;
            }
        }
        private void track()
        {
            if (_camera is null) return;
        
            var ray = _camera.ScreenPointToRay(Input.mousePosition); //Ray ray

            if( Physics.Raycast(ray , out var hit, 1000, 1 << LayerMask.NameToLayer(detectionLayer))) //it wont see other layers 
          
            {

                transform.position = positionAline(new Vector3 (hit.point.x, hit.point.y, hit.point.z));
            }
        }

        private Vector3 positionAline(Vector3 input)
        {

            input += offset;
            
            
            if (xIsFixed)
            {
                input.x = fixedPositions.x;
            } 
            if (yIsFixed)
            {
                input.y = fixedPositions.y;
            } 
            if (zIsFixed)
            {
                input.z = fixedPositions.z;
            } 
        
            return input;
        }

        public void setOn()
        {

            working = true;
        }
        public void setOff()
        {

            working = false;
        }


    }
}
 
