using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class PlayerHealth : MonoBehaviour
{
   // public ComboCalculator _comboCalculator;
    public ParticleSystem whenPlayerShooted;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("bullet"))
        {
            Camera.main.transform
                .DOShakePosition(.1f, .01f, 1, 10f);
          //  _comboCalculator.ResetCombo();
            whenPlayerShooted.Play();
            Destroy(other.gameObject);
        }
    }
}
