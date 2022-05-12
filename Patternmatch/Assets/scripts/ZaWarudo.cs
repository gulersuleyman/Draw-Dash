using System;
using UnityEngine;

namespace Scenes.Levels.level1
{
    public class ZaWarudo : MonoBehaviour
    {
        [SerializeField] private String triggerKey ="space";
        [SerializeField] private String jumpKey ="escape";
        
        [SerializeField] private bool theWorld;
    
        [SerializeField] private bool isStopped=false;
    
    
        // Update is called once per frame
        void Update()
        {

            if (theWorld||Input.GetKeyUp(triggerKey))
            {
                if (Math.Abs(Time.timeScale - 1.0f) < 0.1f)
                {
                    Time.timeScale = 0.0f;        
                    isStopped = true;
                }

                else
                {
                    Time.timeScale = 1.0f;
                    isStopped = false;
                }
                
                theWorld = false;
            }

            if (theWorld || Input.GetKeyUp(jumpKey))
            {
                if (!(Math.Abs(Time.timeScale - 1.0f) < 0.1f))
                {
                    Time.timeScale = 1.0f;
                    isStopped = false;
                    Invoke(nameof(jump),0.02f);
                }
            }
        
        
        }

        private void jump()
        {

            Time.timeScale = 0.0f;        
            isStopped = true;



        }
    }
}
