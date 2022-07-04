using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClientMovementController : MonoBehaviour
{ 
    private Vector3? _targetPos;
    private float? _targetSpeed;

    private void Update()
    {
        if (_targetPos != null) transform.position = Vector3.Lerp(transform.position, _targetPos.Value, Time.deltaTime * 15f);
        
    }

    public void Move(Vector3 targetPos)
    {
        _targetPos = targetPos;
    }
}