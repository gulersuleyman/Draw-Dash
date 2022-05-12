using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lookPlayer : MonoBehaviour
{
    [SerializeField] private GameObject player;
    
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        Vector3 position = new Vector3(player.transform.position.x,transform.position.y,player.transform.position.z);
        transform.LookAt(position);
        
    }
}
