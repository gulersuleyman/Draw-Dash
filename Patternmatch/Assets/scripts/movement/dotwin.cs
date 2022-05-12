using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class dotwin : MonoBehaviour
{
    [SerializeField] private bool work;
    [SerializeField] private float speed;
    
     
    void Update()
    {
        speed = GameManager.Instance.roadSpeed;
        if (work&&!GameManager.Instance.levelFinished)
        {
            float step =  speed * Time.deltaTime; // calculate distance to move
            transform.Translate(Vector3.back * step,Space.World);
        }
    }
}
