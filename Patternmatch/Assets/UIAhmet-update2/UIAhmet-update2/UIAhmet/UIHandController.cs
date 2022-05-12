using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIHandController : MonoBehaviour
{
    [SerializeField] private Ease ease;
    [SerializeField] private bool run = true;
    [SerializeField] private float handScaleIndex=1;
    [SerializeField] private Sprite[] handSprites;

    Image image;
    Vector3 handScale;
    // Update is called once per frame
    private void Awake()
    {
        image = GetComponent<Image>();
        handScale = new Vector3(handScaleIndex, handScaleIndex, 1);
    }
    void Update()
    {
        if (run)
        {
            run = false;
            image.sprite = handSprites[1];
            transform.DOScale(gameObject.transform.localScale+handScale, .5f).SetEase(ease).OnComplete((() =>
            {
                image.sprite = handSprites[0];
                transform.DOScale(gameObject.transform.localScale-handScale, .5f).SetEase(ease).OnComplete((() =>
                {
                    run = true;
                }));
            }));
        }
    }
}
