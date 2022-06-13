using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPannel : Pannel
{
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _leaveGameButton;

    public override void SetButtonsReferences()
    {
        _startGameButton.onClick.AddListener(OnClickStartGameButton);
        _leaveGameButton.onClick.AddListener(OnClickLeaveGameButton);
    }

    private void OnClickStartGameButton()
    {
        NetworkManager.Instance.ClientMessage.SendOnStartGame();
    }

    private void OnClickLeaveGameButton()
    {
        NetworkManager.Instance.LeaveGame();
    }

    public void EnableStartGameButton(bool value)
    {
        _startGameButton.gameObject.SetActive(value);
    }
}
