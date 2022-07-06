using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using DG.Tweening;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class SendInviteFriendsPannel : Pannel
{
    [SerializeField] private Button _inviteFriendButton;
    [SerializeField] private Button _refreshButton;
    [SerializeField] private Button _quitPannelButton;
    [SerializeField] private GameObject _friendDisplayPrefab;
    [SerializeField] private GameObject _barSectionDisplayPrefab;
    [SerializeField] private RectTransform _content;

    private void Start()
    {
        GrabFriends();
    }

    public override void SetButtonsReferences()
    {
        _refreshButton.onClick.AddListener(OnClickRefreshButton);
        _quitPannelButton.onClick.AddListener(OnClickQuitButton);
    }

    private void OnClickRefreshButton()
    {
        int friendCount = _content.childCount;

        for (int i = 0; i < friendCount; i++)
        {
            Destroy(_content.GetChild(i).gameObject);
        }

        offsetY = -25f;
        
        _content.sizeDelta = new Vector2(_content.sizeDelta.x, 25f);
        
        GrabFriends();
    }
    
    private void OnClickQuitButton()
    {
        this.transform.DOLocalMoveY(850f, 0.25f);
        _inviteFriendButton.gameObject.SetActive(true);
    }
    
    private float offsetY = -25f;
    [ContextMenu("GrabFriends")]
    public void GrabFriends()
    {
        int friendsCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagAll);
        List<CSteamID> friends = new List<CSteamID>();
        for (int i = 0; i < friendsCount; i++)
        {
            CSteamID friendId = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagAll);

            friends.Add(friendId);
        }

        for (int i = 0; i < friends.Count; i++)
        {
            if (SteamFriends.GetFriendGamePlayed(friends[i], out FriendGameInfo_t friendGameInfo))
            {
                if (friendGameInfo.m_gameID == new CGameID((ulong) 480))
                {
                    GameObject friendDisplayInstance = Instantiate(_friendDisplayPrefab, _content);
                    _content.sizeDelta = new Vector2(_content.sizeDelta.x, _content.sizeDelta.y + 100f);
                    friendDisplayInstance.transform.localPosition = new Vector3(friendDisplayInstance.transform.localPosition.x,offsetY,friendDisplayInstance.transform.localPosition.z);
                    offsetY -= 100;
            
                    friendDisplayInstance.GetComponent<InviteFriendBehaviour>().Initialize(friends[i]);
                }
            }
        }
        
        if(_content.sizeDelta.y != 25f) AddBarSection();
        
        for (int i = 0; i < friends.Count; i++)
        {
            if (SteamFriends.GetFriendPersonaName(friends[i]) != "[unknown]")
            {
                GameObject friendDisplayInstance = Instantiate(_friendDisplayPrefab, _content);
                _content.sizeDelta = new Vector2(_content.sizeDelta.x, _content.sizeDelta.y + 100f);
                friendDisplayInstance.transform.localPosition = new Vector3(friendDisplayInstance.transform.localPosition.x,offsetY,friendDisplayInstance.transform.localPosition.z);
                offsetY -= 100;
            
                friendDisplayInstance.GetComponent<InviteFriendBehaviour>().Initialize(friends[i]);
            }
        }
    }
    
    [ContextMenu("AddFriendInContent")]
    public void AddFriendInContent()
    {
        GameObject a = Instantiate(_friendDisplayPrefab, _content);
        _content.sizeDelta = new Vector2(_content.sizeDelta.x, _content.sizeDelta.y + 100f);
        a.transform.localPosition = new Vector3(a.transform.localPosition.x,offsetY,a.transform.localPosition.z);
        offsetY -= 100;
    }

    [ContextMenu("AddBarSectionInContent")]
    public void AddBarSection()
    {
        GameObject barInstance = Instantiate(_barSectionDisplayPrefab, _content);
        _content.sizeDelta = new Vector2(_content.sizeDelta.x, _content.sizeDelta.y + 30f);
        barInstance.transform.localPosition = new Vector3(barInstance.transform.localPosition.x,offsetY,barInstance.transform.localPosition.z);
        offsetY -= 30f;
    }
}
