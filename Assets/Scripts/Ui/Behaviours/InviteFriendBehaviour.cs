using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class InviteFriendBehaviour : Pannel
{
    [SerializeField] [CanBeNull] private TMP_Text _profileNameText;
    [SerializeField] private RawImage _profileImage;
    [SerializeField] private Button _inviteButton;

    protected Callback<AvatarImageLoaded_t> ImageLoaded;

    private CSteamID _friendSteamId;

    private void Start()
    {
        ImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnPlayerAvatarLoaded);
    }

    public override void SetButtonsReferences()
    {
        _inviteButton.onClick.AddListener(OnClickInviteButton);
    }

    private void OnClickInviteButton()
    {
        SteamMatchmaking.InviteUserToLobby(SteamLobbyManager.Instance.GetLobbyId(), _friendSteamId);
        
    }

    public void Initialize(CSteamID friendSteamID)
    {
        _friendSteamId = friendSteamID;
        
        string friendName = SteamFriends.GetFriendPersonaName(_friendSteamId);
        _profileNameText.text = friendName;

        LoadFriendAvatar(); 
    }
    
    public void LoadFriendAvatar()
    {
        int _playerAvatarId = SteamFriends.GetLargeFriendAvatar(_friendSteamId);
        
        if(_playerAvatarId == -1)  {Debug.Log("Error loading image"); return;}

        _profileImage.texture = GetSteamImageAsTexture(_playerAvatarId);
    }
    
    private void OnPlayerAvatarLoaded(AvatarImageLoaded_t callback)
    {
        if (callback.m_steamID == _friendSteamId)
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
