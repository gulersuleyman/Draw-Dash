using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG;
using DG.Tweening;
using TMPro;

public class ComboCalculator : MonoBehaviour
{
    private Text _comboText;
    private int comboCoefficent = 1;
    public static float comboCount = 0f;
    private Animator _animator;
    public List<Color> colors;
    private int colorIndex = 0;
    
    
    private void Start()
    {
        _comboText = this.GetComponent<Text>();
        _comboText.text = "";
        _animator = GetComponent<Animator>();
    }
    
    public void RenderComboText()
    {
        Math.Round(comboCount, 1);
        _comboText.text = "x" + comboCount.ToString("F1");
        AnimateText();
        //ChangeColorRandom();
    }


    public void ResetCombo()
    {
        _comboText.text = "";
        colorIndex = 0;
        comboCount = 0;
    }

    public void UpdateCounter()
    {
        comboCount += 0.1f * comboCoefficent;
        _animator.SetTrigger("trigger");
        comboCoefficent++;
    }

    public void ChangeColorRandom()
    {
        _comboText.color = colors[colorIndex];
        if (colorIndex <9)
        {
            colorIndex++;   
        }
    }

    public void AnimateText()
    {
        if (transform.localScale.x<1.3f)
        {
            transform.localScale *= 1.1f;
        }
        
    }
}
