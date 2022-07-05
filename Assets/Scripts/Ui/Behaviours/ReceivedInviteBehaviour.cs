using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReceivedInviteBehaviour : Pannel
{
    [SerializeField] private TMP_Text _profileNameText;
    [SerializeField] private RawImage _profileImage;
    [SerializeField] private Button _acceptButton;
    [SerializeField] private Button _declineButton;

    private ulong _friendSteamId;
    private ulong _lobbyId;

    private ReceiveInvitePannel _invitationPannel;
    
    protected Callback<AvatarImageLoaded_t> ImageLoaded;

    private void Start()
    {
        _invitationPannel = FindObjectOfType<ReceiveInvitePannel>();
        ImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnPlayerAvatarLoaded);
    }

    public override void SetButtonsReferences()
    {
        _acceptButton.onClick.AddListener(AcceptInvite);
        _declineButton.onClick.AddListener(DeclineInvite);
    }

    public void Initialize(LobbyInvite_t lobbyInvite)
    {
        _friendSteamId = lobbyInvite.m_ulSteamIDUser;
        _lobbyId = lobbyInvite.m_ulSteamIDLobby;
        
        _profileNameText.text = SteamFriends.GetFriendPersonaName((CSteamID)_friendSteamId);
        
        LoadFriendAvatar();
    }

    private void AcceptInvite()
    {
        NetworkManager.Instance.LeaveGame();
        SteamMatchmaking.JoinLobby((CSteamID)_lobbyId);
        DeclineInvite();
    }

    private void DeclineInvite()
    {
        ReceiveInvitePannel.Instance.DeleteInvite(this);
        ReceiveInvitePannel.Instance.RefreshListDisplay();
    }

    public void LoadFriendAvatar()
    {
        int _playerAvatarId = SteamFriends.GetLargeFriendAvatar((CSteamID)_friendSteamId);
        
        if(_playerAvatarId == -1)  {Debug.Log("Error loading image"); return;}

        _profileImage.texture = GetSteamImageAsTexture(_playerAvatarId);
    }
    
    private void OnPlayerAvatarLoaded(AvatarImageLoaded_t callback)
    {
        if (callback.m_steamID == (CSteamID)_friendSteamId)
        { 
            _profileImage.texture = GetSteamImageAsTexture(callback.m_iImage);
        }
    }
    
    private Texture2D GetSteamImageAsTexture(int image)
    {
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(image, out uint width, out uint height);

        if (isValid)
        {
            byte[] imageTemp = new byte[width * height * 4];

            isValid = SteamUtils.GetImageRGBA(image, imageTemp,(int)width * (int)height * 4);

            if (isValid)
            {
                texture = new Texture2D((int) width, (int) height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(imageTemp);
                texture.Apply();
            }
        }
        return texture;
    }
}
