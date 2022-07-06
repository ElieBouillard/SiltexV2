using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileBracketBehaviour : MonoBehaviour
{
    [SerializeField] private RawImage _profileImage;
    [SerializeField] private TMP_Text _profileNameText;

    public void Initialize(string name)
    {
        _profileNameText.text = name;
    }
}
