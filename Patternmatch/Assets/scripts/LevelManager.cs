using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scenes.Levels.level0
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private String levelHolderPrefName ="lastLevel";
        
         
        private static LevelManager _instance;
        public static LevelManager Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<LevelManager>();
                }

                return _instance;
            }
        }

        private void Awake()
        {
            if (PlayerPrefs.GetInt(levelHolderPrefName)== 0)
            {
                PlayerPrefs.SetInt(levelHolderPrefName,1);
            }

            SceneManager.LoadScene(PlayerPrefs.GetInt(levelHolderPrefName));
        }
    }
}

