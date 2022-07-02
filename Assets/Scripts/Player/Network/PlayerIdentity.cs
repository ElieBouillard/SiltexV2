using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Steamworks;
using TMPro;
using UnityEditor;
using UnityEngine;
using Color = System.Drawing.Color;

public class PlayerIdentity : MonoBehaviour
{
    public ushort Id;
    public int ColorIndex = 0;
    public ulong SteamId;
    public string SteamName;
    public bool IsLocalPlayer { get; private set; }

    private Renderer[] _renderers;
    
    private void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>();
    }

    public void SetAsLocalPlayer()
    {
        IsLocalPlayer = true;
        
        if(CameraController.Instance == null) return;
        CameraController.Instance.SetCameraTarget(gameObject.transform);
    }

    public void ChangeColor(int colorIndex)
    {
        ColorIndex = colorIndex;

        for (int i = 0; i < _renderers.Length; i++)
        {
            if (i == 0)
            {
                _renderers[i].materials[1].SetColor("_EmissionColor", NetworkColorsManager.Instance.Colors[colorIndex]);
            }
            
            if (i == 1)
            {
                _renderers[i].materials[0].SetColor("_EmissionColor", NetworkColorsManager.Instance.Colors[colorIndex]);
            }   
        }

        if(!IsLocalPlayer) return;
        ColorSelectionManager.Instance.ChangeImageColor(colorIndex);
    }
}