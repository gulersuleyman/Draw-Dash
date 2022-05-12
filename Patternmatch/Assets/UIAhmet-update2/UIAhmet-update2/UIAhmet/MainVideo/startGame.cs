using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class startGame : MonoBehaviour
{
    private void Start()
    {    
        StartCoroutine(StartGame());
    }


    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(0.2f);
        if (PlayerPrefs.GetInt("kacincilevel") == 0)
        {
            PlayerPrefs.SetInt("kacincilevel", SceneManager.GetActiveScene().buildIndex + 1);

            SceneManager.LoadScene(1);
        }
        else if (PlayerPrefs.GetInt("kacincilevel") != 0)
        {
            SceneManager.LoadScene(PlayerPrefs.GetInt("kacincilevel"));
            
        }
        //PlayerPrefs.SetInt("kacincilevel", SceneManager.GetActiveScene().buildIndex + 1);
        
    }
}
