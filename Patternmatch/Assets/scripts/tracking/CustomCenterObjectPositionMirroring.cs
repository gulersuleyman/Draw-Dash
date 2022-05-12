using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCenterObjectPositionMirroring : MonoBehaviour
{
    public Transform mirroringCenter;  
    public Transform objectToMirror;
    public float trackSpeed = 10000f;
     
    public float xfactorToDistance = 1; // make it 2 and mirroring object will be x2 distance to the center than original object

  
     
    void FixedUpdate()
    {
        float M =  xfactorToDistance;

        transform.position = Vector3.MoveTowards(transform.position, new Vector3(-M * objectToMirror.position.x + 2 * mirroringCenter.position.x, mirroringCenter.position.y, -M * objectToMirror.position.z + 2 * mirroringCenter.position.z), 0.1F * trackSpeed * Time.fixedDeltaTime);
        

    }

}
