using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Tutorial : MonoBehaviour
{
    [SerializeField] GameObject hand;

    [SerializeField] Transform linePoint1;
    [SerializeField] Transform linePoint2;
    [SerializeField] RectTransform handPoint1;
    [SerializeField] RectTransform handPoint2;

    public float duration=0.5f;
    TrailRenderer _trail;

    Vector3 firstPos;
    Vector3 firstHandPos;
    // Start is called before the first frame update
    void Start()
    {
        _trail = GetComponent<TrailRenderer>();
        firstPos = transform.position;
        firstHandPos = hand.transform.position;

        mover();
        handMover();
    }

    private void mover()
    {
        _trail.enabled = true;
        transform.DOMove(linePoint1.position, duration).OnComplete(() =>
         {
             transform.DOMove(linePoint2.position, duration).OnComplete(() =>
              {
                  _trail.enabled = false;
                  transform.DOMove(firstPos, 0.3f).OnComplete(() =>
                   {
                       mover();
                       handMover();
                   });
                  
                  
              });
         });
       
    }
	private void Update()
	{
		if(Input.GetMouseButtonDown(0))
		{
            this.gameObject.SetActive(false);
            hand.gameObject.SetActive(false);
		}
	}
	void handMover()
	{
        hand.transform.DOMoveX(handPoint1.position.x, duration - 0.1f).OnComplete(() =>
        {
            hand.transform.DOMoveX(handPoint2.position.x, duration - 0.1f).OnComplete(() =>
            {

                hand.transform.position = firstHandPos;

            });
        });
        hand.transform.DOMoveY(handPoint1.position.y, duration - 0.1f).OnComplete(() =>
        {
            hand.transform.DOMoveY(handPoint2.position.y, duration - 0.1f).OnComplete(() =>
            {

                hand.transform.position = firstHandPos;

            });
        });
    }
        
}
