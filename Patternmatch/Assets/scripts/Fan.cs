using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Fan : MonoBehaviour
{
    public bool xRot;

    public bool yRot;

    public bool zRot;

    public float rotationScale;

    


    void Update()
    {
        if (xRot)
        {
            transform.Rotate(new Vector3(rotationScale, 0, 0));
        }
        if (yRot)
        {
            transform.Rotate(new Vector3(0, rotationScale, 0));
        }
        if (zRot)
        {
            transform.Rotate(new Vector3(0, 0, rotationScale));
        }
        
    }
}


