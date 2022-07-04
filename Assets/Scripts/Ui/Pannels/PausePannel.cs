using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausePannel : Pannel
{
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _backLobbyButton;
    [SerializeField] private Button _leaveButton;

    private void OnEnable()
    {
        if (NetworkManager.Instance.LocalPlayer.Id == 1) _backLobbyButton.gameObject.SetActive(true);
        else _backLobbyButton.gameObject.SetActive(false);
        
    }

    public override void SetButtonsReferences()
    {
        _resumeButton.onClick.AddListener(delegate { InGameUiManager.Instance.SetPause(false); });
        // _backLobbyButton.onClick.AddListener(NetworkManager.Instance.BackToLobby);
        _leaveButton.onClick.AddListener(NetworkManager.Instance.LeaveGame);
    }
}
