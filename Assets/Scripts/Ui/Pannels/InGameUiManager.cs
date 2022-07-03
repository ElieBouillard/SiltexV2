using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUiManager : MonoBehaviour
{
    #region Singleton
    private static InGameUiManager _instance;
    public static InGameUiManager Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
                _instance = value;
            else if (_instance != value)
            {
                Debug.Log($"{nameof(InGameUiManager)} instance already exists, destroying object!");
                Destroy(value);
            }
        }
    }
    #endregion
    
    [SerializeField] private GameObject _pausePannel;

    private void Awake()
    {
        Instance = this;
    }

    private bool _isPause = false;

    private void Start()
    {
        _pausePannel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetPause(!_isPause);
        }
    }

    public void SetPause(bool value)
    {
        _isPause = value;
        _pausePannel.SetActive(value);
    }
}
