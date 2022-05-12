using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class returning : MonoBehaviour
{
    [SerializeField] private bool work;
    [SerializeField] private bool playerwait;
    [SerializeField] private Vector3 target =new Vector3();
    [SerializeField] private float time=10;
    
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerwait)
        {
            if (GameManager.Instance.camerafollow)
            {
                work = true;
            }
            else
            {
                work = false;
            }
        }
        if (work)
        {
            target=new Vector3(transform.position.x,transform.position.y,target.z);

            transform.DOMove(target, time, true);
             
        }
    }
}
