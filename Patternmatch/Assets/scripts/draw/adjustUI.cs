using UnityEngine;

 
    public class adjustUI : MonoBehaviour
    {
     
        [SerializeField] private RectTransform panel;
        [SerializeField] private bool tryIt;

        private float xM;
        private float xL;
        private float yM;
        private float yL;
        private float[] C;
    
    
        void Start()
        {
          //  panel = GetComponent<RectTransform>();
         
        }

        // Update is called once per frame
        void Update()
        {
            if (tryIt)
            {
                tryIt = false;
                getPosition(xM,xL,yM,yL,C);
            }
        }

        public void getPosition(float xMost,float xLeast,float yMost,float yLeast,float[] center)
        {  
        
            float first=(yMost-yLeast);// height lenght vertical
            float second = (xMost - xLeast);
        
            panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,first);// for y
            transform.position=new Vector3(transform.position.x,center[1],transform.position.z);// for y height
        
            //  panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,second); // for x
        
            panel.offsetMin = new Vector2(xLeast,panel.offsetMin.y);
            panel.offsetMax = new Vector2(-(Screen.width-xMost),panel.offsetMax.y);
        
        
        

//        Debug.Log("-------------FIRST ONE  ---------------"+first);
            //      Debug.Log("-------------LEFT X  ---------------"+xLeast);
            //     Debug.Log("-------------RIGHT X  ---------------"+(Screen.width-xMost));
            //  Debug.Log("-------------ADDITION ---------------"+(( Screen.width- ((Screen.width / 100)*100))/2));
         



            xM = xMost;
            xL = xLeast;
            yM = yMost;
            yL = yLeast;
            C = center;
        }
    }
 
