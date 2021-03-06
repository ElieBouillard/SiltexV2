using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPannel : Pannel
{
    [SerializeField] private RectTransform _inviteFriendPannel;
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _inviteFriendsButton;
    [SerializeField] private Button _leaveGameButton;

    public override void SetButtonsReferences()
    {
        _startGameButton.onClick.AddListener(OnClickStartGameButton);
        _inviteFriendsButton.onClick.AddListener(OnClickInviteFriendButton);
        _leaveGameButton.onClick.AddListener(OnClickLeaveGameButton);
    }

    private void OnClickStartGameButton()
    {
        NetworkManager.Instance.ClientMessage.SendOnStartGame();
    }

    private void OnClickInviteFriendButton()
    {
        _inviteFriendsButton.gameObject.SetActive(false);
        _inviteFriendPannel.DOLocalMoveY(250f, 0.25f);
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
