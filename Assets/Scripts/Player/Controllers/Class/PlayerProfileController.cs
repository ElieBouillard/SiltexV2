using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Steamworks;
using TMPro;
using UnityEngine;

public class PlayerProfileController : MonoBehaviour
{
    [SerializeField] private GameObject _profileDisplay;
    [SerializeField] private TMP_Text _nameText;

    private void Start()
    {
        _nameText.text = SteamFriends.GetFriendPersonaName((CSteamID) GetComponent<PlayerIdentity>().SteamId);
    }

    public void EnableProfile(bool value)
    {
        _profileDisplay.SetActive(value);   
    }
}
