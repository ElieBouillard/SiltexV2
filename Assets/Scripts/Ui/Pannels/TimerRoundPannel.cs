using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerRoundPannel : MonoBehaviour
{
    #region Singleton
    private static TimerRoundPannel _instance;
    public static TimerRoundPannel Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
                _instance = value;
            else if (_instance != value)
            {
                Debug.Log($"{nameof(TimerRoundPannel)} instance already exists, destroying object!");
                Destroy(value);
            }
        }
    }
    #endregion

    [SerializeField] private Image _timerImage;
    [SerializeField] private TMP_Text _timerText;

    private float _timer = -1f;
    private void Awake()
    {
        Instance = this;
    }

    public void InitializeCouldown()
    {
        _timerImage.gameObject.SetActive(true);
        _timerText.gameObject.SetActive(true);

        _timerImage.fillAmount = 1f;
        _timerText.text = "3";

        _timer = 3f;

        foreach (var player in NetworkManager.Instance.Players)
        {
            player.Value.GetComponent<PlayerProfileController>().EnableProfile(true);
        }
    }
    
    private void Update()
    {
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
            _timerText.text = (Mathf.FloorToInt(_timer) + 1).ToString();
            _timerImage.fillAmount = _timer / 3;
        }
        else if(_timer != -1)
        {
            _timer = -1;
            OnRoundStart();
        }
    }

    private void OnRoundStart()
    {
        _timerText.gameObject.SetActive(false);
        _timerImage.gameObject.SetActive(false);
        
        foreach (var player in NetworkManager.Instance.Players)
        {
            player.Value.GetComponent<PlayerProfileController>().EnableProfile(false);
        }
        
        NetworkManager.Instance.LocalPlayer.Initialize(true);
    }
}
