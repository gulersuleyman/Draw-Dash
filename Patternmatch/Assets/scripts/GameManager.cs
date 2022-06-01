using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ElephantSDK;
using GameAnalyticsSDK;

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
        GameAnalytics.Initialize();

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
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, PlayerPrefs.GetInt("level").ToString());
        Elephant.LevelStarted(PlayerPrefs.GetInt("level"));
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
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, PlayerPrefs.GetInt("level").ToString());
        Elephant.LevelFailed(PlayerPrefs.GetInt("level"));
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void LoadNextLevel()
    {
        Elephant.LevelCompleted(PlayerPrefs.GetInt("level"));
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, PlayerPrefs.GetInt("level").ToString());
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