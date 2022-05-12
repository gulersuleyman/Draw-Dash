using System;
using UnityEngine;

 
 
    public class setLastLevel : MonoBehaviour
    {

        [SerializeField] private String levelHolderPrefName ="lastLevel";
        public bool changeIt = false;
        public int toLevel;
 
        // Update is called once per frame
        void Update()
        {
            if (changeIt)
            {
                changeIt = false;
                PlayerPrefs.SetInt(levelHolderPrefName,toLevel); 
            }
        }
    }
 
