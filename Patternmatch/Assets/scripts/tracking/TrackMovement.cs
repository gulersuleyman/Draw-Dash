using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackMovement : MonoBehaviour
{
    
    [SerializeField] private bool work=true;
    
    [SerializeField] private Transform objectB;
    [SerializeField] private bool willTrack_elseCarry_ =true;
    
    [SerializeField] private Vector3 offset = new Vector3();
    [SerializeField] private bool getSceneOffsets;
    private Vector3 firstOffset = new Vector3();
    
    //--------------------------------------------------------------
    
    [SerializeField] private bool trackX = false;
    [SerializeField] private bool trackY = false;
    [SerializeField] private bool trackZ = false;

    public string SpecialConditions;
    [SerializeField] private bool xLimitForTarget;
    [SerializeField] private bool yLimitForTarget;
    [SerializeField] private bool zLimitForTarget;
    
    
    
    //--------------------------------------------------------------
    
    
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
