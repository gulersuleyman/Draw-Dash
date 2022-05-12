using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadCut : MonoBehaviour
{

    RagdollDismembermentVisual dismemberment;
    CharacterJoint joint;

    // Start is called before the first frame update
    void Start()
    {
        dismemberment = GetComponentInParent<RagdollDismembermentVisual>();
        joint = GetComponent<CharacterJoint>();
        joint.breakForce = 0;
    }

    // Update is called once per frame
    private void OnJointBreak(float breakForce)
    {
        dismemberment.Dismember("Head");
    }
}
