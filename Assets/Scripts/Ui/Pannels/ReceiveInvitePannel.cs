using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Steamworks;
using UnityEngine;

public class ReceiveInvitePannel : MonoBehaviour
{
    #region Singleton
    private static ReceiveInvitePannel _instance;
    public static ReceiveInvitePannel Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
                _instance = value;
            else if (_instance != value)
            {
                Debug.Log($"{nameof(ReceiveInvitePannel)} instance already exists, destroying object!");
                Destroy(value);
            }
        }
    }
    #endregion
    
    [SerializeField] private RectTransform _content;
    [SerializeField] private GameObject _invitationDisplay;

    public List<ReceivedInviteBehaviour> _invitations = new List<ReceivedInviteBehaviour>();

    protected Callback<LobbyInvite_t> InviteLobby;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InviteLobby = Callback<LobbyInvite_t>.Create(OnInvite);
    }
    
    private void OnInvite(LobbyInvite_t lobbyInvite)
    {
        if (_content == null) return;
        GameObject invitationInstance = Instantiate(_invitationDisplay, _content);
        ReceivedInviteBehaviour invitation = invitationInstance.GetComponent<ReceivedInviteBehaviour>();

        invitation.Initialize(lobbyInvite);
        
        _invitations.Add(invitation);

        RefreshListDisplay();
        
        EnablePannel(true);
    }

    public void RefreshListDisplay()
    {
        if (_invitations.Count == 0)
        {
            EnablePannel(false);
            return;
        }
        
        _content.sizeDelta = new Vector2(_content.sizeDelta.x,  100 * _invitations.Count);
        
        for (int i = 0; i < _invitations.Count; i++)
        {
            _invitations[i].gameObject.transform.localPosition = new Vector3(0, -100 * i, 0);
        }
    }

    public void DeleteInvite(ReceivedInviteBehaviour inviteBehaviour)
    {
        if (!_invitations.Contains(inviteBehaviour)) return;

        Destroy(inviteBehaviour.gameObject);

        _invitations.Remove(inviteBehaviour);
    }
    
    public void EnablePannel(bool value)
    {
        if (value)
        {
            transform.DOMoveX(0,0.25f);
        }
        else
        {
            transform.DOMoveX(-500f, 0.25f);
        }
    }
}
