using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Singleton
    private static CameraController _instance;
    public static CameraController Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
                _instance = value;
            else if (_instance != value)
            {
                Debug.Log($"{nameof(CameraController)} instance already exists, destroying object!");
                Destroy(value);
            }
        }
    }
    #endregion
    
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offset;

    private void Awake()
    {
        _instance = this;
    }
    
    private void Update()
    {
        if (_target == null) return;
        transform.position = _target.position + _offset;
    }
    
    public void SetCameraTarget(Transform target)
    {
        _target = target;
    }
}
