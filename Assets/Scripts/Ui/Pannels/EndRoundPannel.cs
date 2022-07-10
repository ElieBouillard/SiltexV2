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
            _text.color = Color.green;
        }
        else
        {
            _text.color = Color.red;
            _text.text = "DEFEAT !";
        }
        
        _text.gameObject.SetActive(active);

        if (active) StartCoroutine(DisableText());
    }

    IEnumerator DisableText()
    {
        yield return new WaitForSeconds(2f);
        _text.gameObject.SetActive(false);
    }
}
