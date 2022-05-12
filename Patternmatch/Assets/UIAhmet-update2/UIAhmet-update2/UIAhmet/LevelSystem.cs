using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSystem : MonoBehaviour
{
    public bool DidYouReturnPanel;
    public bool DidYouNextLevelPanel;

    private int levelsIndexCount;

   [SerializeField] private bool levelIndexWork=true;

    

    private float _previousTimeScale;

    public bool mouseDown;

    #region Singleton

    public static LevelSystem Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("EXTRA : " + this + "  SCRIPT DETECTED RELATED GAME OBJ DESTROYED");
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    #endregion
    

    private void Start()
    {
        
      
        //level kayit ayarlandı
        Debug.Log("kayıtlı olan level"+PlayerPrefs.GetInt("kacincilevel"));
        DontDestroyOnLoad(this.gameObject);
        levelsIndexCount = SceneManager.sceneCountInBuildSettings-1;
//        Debug.Log(levelsIndexCount);
    }

    private void Update()
    {
       
        _previousTimeScale = (Time.timeScale != 0) ? (Time.timeScale) : (_previousTimeScale);
        
       // Debug.Log("scale"+_previousTimeScale);
        
       
       // Debug.Log(PlayerPrefs.GetInt("kacincilevel", SceneManager.GetActiveScene().buildIndex));
        
        
        MouseClick();

        if (SceneManager.GetActiveScene().buildIndex >= 1 && levelIndexWork  )
        {
           
            if (!mouseDown)
            {
                TapToPanelOpen();
                this.gameObject.transform.GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text= "LEVEL" + " " + SceneManager.GetActiveScene().buildIndex;
                Invoke("LevelPanelOpen",.01f);
            }
            if(mouseDown)
            {
                TapToPanelClosed();
                 LevelPanelClosed();
            }
           
            
            
        }
        if (DidYouReturnPanel == true)
        {
            LevelPanelClosed();
            ReturnPanelOpen();    
        }
        else if(DidYouReturnPanel==false)
        {
            ReturnPanelClosed();
            

        }

        if (DidYouNextLevelPanel == true) 
        {
            NextLevelPanelOpen();
            LevelPanelClosed();

        }
        else if(DidYouNextLevelPanel==false)
        {
           
            NextLevelPanelClosed();
            

        }
    }

    /// <summary>
    /// Panel acma-kapama metodları ayarlandi
    /// </summary>
    
    public void ReturnPanelOpen()
    {
        
        transform.GetChild(0).gameObject.SetActive(true);
    } 
    public void ReturnPanelClosed()
    {
        
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void NextLevelPanelOpen()
    {
        transform.GetChild(1).gameObject.SetActive(true);
    }
    public void NextLevelPanelClosed()
    {
        transform.GetChild(1).gameObject.SetActive(false);
    }

    public void LevelPanelOpen()
    {
       
        transform.GetChild(2).gameObject.SetActive(true);
        transform.GetChild(2).GetChild(0).gameObject.transform.DOScale(new Vector3(.74f, .91f, 1),.3f);
    }
    public void TapToPanelOpen()
    {
       
        transform.GetChild(3).gameObject.SetActive(true);
        
    } 
    public void TapToPanelClosed()
    {
       
        transform.GetChild(3).gameObject.SetActive(false);
        
    }  
   
    public void LevelPanelClosed()
    {
        
        transform.GetChild(2).GetChild(0).gameObject.transform.DOScale(Vector3.zero, .001f).OnComplete(() =>
        {
            transform.GetChild(2).gameObject.SetActive(false);
            
        });
        
        
    }

   
    
    // <summary>
    /// Buton click ayarları yapıldı
    /// </summary>

    public void ReturnBtnClick()
    {
       
        mouseDown = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        DidYouReturnPanel = false;
        ReturnPanelClosed();

        Time.timeScale = 1;
    }
 
    public void NextBtnClick()
    {
       
        mouseDown = false;
        if (SceneManager.GetActiveScene().buildIndex ==levelsIndexCount )
        {
           
            SceneManager.LoadScene(1);
            DidYouNextLevelPanel = false;
            NextLevelPanelClosed();
            Time.timeScale = 1;
        }
        else
        {
            //PlayerPrefs.SetInt("kacincilevel", SceneManager.GetActiveScene().buildIndex);
            
            PlayerPrefs.SetInt("kacincilevel", SceneManager.GetActiveScene().buildIndex + 1);
            SceneManager.LoadScene(PlayerPrefs.GetInt("kacincilevel"));
            DidYouNextLevelPanel = false;
            NextLevelPanelClosed();


        }
    }

    
  


    void MouseClick()
    {
        if (Input.GetMouseButton(0))
        {
            mouseDown = true;
        }
      
    }
}
