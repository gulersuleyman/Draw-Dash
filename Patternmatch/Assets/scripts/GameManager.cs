using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
     
    
    private static GameManager _instance;

    #region Public Variables

    
    
    public int levelIdentity;

    public float nextSceneSkipTime = 5f;

    [SerializeField] private bool pathOnAction = false;
     
     
    [SerializeField] private float passiveMoveSpeed;

    public bool camerafollow = false;
     
    public bool isMoving = false;
    public bool levelFinished = false;

    public float roadSpeed;


    public float enemyFireDistance = 40;

    public float enemyFireRateIn1Min=60;
    


    #endregion

    public static GameManager Instance { get { return _instance; } }

    private void Awake()
    {

        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        
    }

    private void Start()
    {
        levelIdentity = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt("lastLevel",levelIdentity);
    }

    public bool getPathActive()
    {
        return pathOnAction;
    }

    public void setPathActive(bool input)
    {
        pathOnAction=input;
        
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void LoadNextLevel()
    {
        levelIdentity += 1;
        if (levelIdentity == (SceneManager.sceneCountInBuildSettings))
        {
            SceneManager.LoadScene(1);
             
        }
        else
        {
            SceneManager.LoadScene(levelIdentity);
             
        }
    }

    
}