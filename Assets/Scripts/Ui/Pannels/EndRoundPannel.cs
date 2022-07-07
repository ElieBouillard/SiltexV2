using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndRoundPannel : MonoBehaviour
{
    public static EndRoundPannel Instance;
    
    [SerializeField] private TMP_Text _text;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Enable(false);
    }

    public void Enable(bool active, bool isWin = false)
    {
        if (isWin)
        {
            _text.text = "VICTORY !";
        }
        else
        {
            _text.text = "DEFEAT !";
        }
        
        _text.gameObject.SetActive(active);
    }
}
