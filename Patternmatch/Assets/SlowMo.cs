using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMo : MonoBehaviour
{
    private bool timeSlowed;
    

    private void Update()
    {
       
            if (Input.GetMouseButtonDown(0))
            {
                Time.timeScale = 0.2f;
            }
            if (Input.GetMouseButtonUp(0))
            {
                timeSlowed = false;
                Time.timeScale = 1f;
            }
    }
    
}
