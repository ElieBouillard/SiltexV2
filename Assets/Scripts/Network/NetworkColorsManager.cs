using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkColorsManager : MonoBehaviour
{
    #region Singleton
    private static NetworkColorsManager _instance;
    public static NetworkColorsManager Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
                _instance = value;
            else if (_instance != value)
            {
                Debug.Log($"{nameof(NetworkColorsManager)} instance already exists, destroying object!");
                Destroy(value);
            }
        }
    }
    #endregion
    
    public Color[] Colors;

    private void Awake()
    {
        Instance = this;
    }
}
