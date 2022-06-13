using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClientMovementController : MonoBehaviour
{
    private Vector3? _targetPos;
    private float? _targetRotY;
    
    private void Update()
    {
        if (_targetPos != null) transform.position = Vector3.Lerp(transform.position, _targetPos.Value, Time.deltaTime * 25f);
        
        if (_targetRotY != null)
        {
            Quaternion targetRotation = new Quaternion();
            targetRotation = Quaternion.Euler(new Vector3(0,_targetRotY.Value,0));
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 15f);
        }
    }

    public void Move(Vector3 targetPos, float targetRotY)
    {
        _targetPos = targetPos;
        _targetRotY = targetRotY;
    }
}
