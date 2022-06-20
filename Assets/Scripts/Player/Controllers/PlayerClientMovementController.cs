using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClientMovementController : MonoBehaviour
{
    private Animator _animator;
    
    private Vector3? _targetPos;
    private float? _targetRotY;
    private float? _targetSpeed;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (_targetPos != null) transform.position = Vector3.Lerp(transform.position, _targetPos.Value, Time.deltaTime * 15f);
        
        if (_targetRotY != null)
        {
            Quaternion targetRotation = new Quaternion();
            targetRotation = Quaternion.Euler(new Vector3(0,_targetRotY.Value,0));
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 15f);
        }

        if (_targetSpeed != null)
        {
            float speed = Mathf.Lerp(_animator.GetFloat("Speed"), _targetSpeed.Value, Time.deltaTime * 10f);
            _animator.SetFloat("Speed", speed);
        }
    }

    public void Move(Vector3 targetPos, float targetRotY, float speed)
    {
        _targetPos = targetPos;
        _targetRotY = targetRotY;
        _targetSpeed = speed;
    }
}
