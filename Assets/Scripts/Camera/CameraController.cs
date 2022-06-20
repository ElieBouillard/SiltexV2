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

    [SerializeField] private float speed = 20f;
    [SerializeField] private float lerpSpeed = 5f;
    [SerializeField] private float screenBorderThickness = 10f;
    [SerializeField] private Vector2 screenXLimits = Vector2.zero;
    [SerializeField] private Vector2 screenZLimits = Vector2.zero;
    
    public bool _isLocked = false;

    private void Awake()
    {
        _instance = this;
    }
    
    private void Update()
    {
        if (_target == null) return;
        
        if (Input.GetKeyDown(KeyCode.Y))
        {
            _isLocked = !_isLocked;
        }

        Vector3 pos = transform.position;

        if (Input.GetKey(KeyCode.Space))
        {
            transform.position = _target.position + _offset;
        }

        if (_isLocked)
        {
            pos = _target.position + _offset;
        }
        else
        {
            Vector2 cursorPosition = Input.mousePosition;

            if (cursorPosition.y >= Screen.height - screenBorderThickness)
            {
                pos = transform.position + new Vector3(transform.forward.x, 0f, transform.forward.z) * speed;
            }
            else if (cursorPosition.y <= screenBorderThickness)
            {
                pos = transform.position - new Vector3(transform.forward.x, 0f, transform.forward.z) * speed;
            }
            if (cursorPosition.x >= Screen.width - screenBorderThickness)
            {
                pos = transform.position + new Vector3(transform.right.x, 0f, transform.right.z) * speed;
            }
            else if (cursorPosition.x <= screenBorderThickness)
            {
                pos = transform.position - new Vector3(transform.right.x, 0f, transform.right.z) * speed;
            }
        } 
        
        transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * lerpSpeed); 
    }
    
    public void SetCameraTarget(Transform target)
    {
        _target = target;
    }
}
