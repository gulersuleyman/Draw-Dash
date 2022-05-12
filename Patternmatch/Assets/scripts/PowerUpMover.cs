using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class PowerUpMover : MonoBehaviour
{



    float firstYPos;
    // Start is called before the first frame update
    void Start()
    {
        firstYPos = transform.position.y;
        Move();
    }
    private void Move()
    {
        transform.DOMoveY(5f, 1f).OnComplete(() =>
         {
             transform.DOMoveY(firstYPos, 1f).OnComplete(() =>
              {
                  Move();
              });
         });
        
    }


}
