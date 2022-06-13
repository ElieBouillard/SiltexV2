using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class PlayerIdentity : MonoBehaviour
{
    public ushort Id;
    public bool IsLocalPlayer { get; private set; }

    public void SetAsLocalPlayer()
    {
        IsLocalPlayer = true;
        
        if(CameraController.Instance == null) return;
        CameraController.Instance.SetCameraTarget(gameObject.transform);
    }

    public void ChangeColor(Color color)
    {
        Material[] mats = gameObject.GetComponentInChildren<Renderer>().materials;
        foreach (var mat in mats)
        {
            mat.color = color;
        }
    }
}
