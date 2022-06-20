using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausePannel : Pannel
{
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _leaveButton;

    public override void SetButtonsReferences()
    {
        _resumeButton.onClick.AddListener(delegate { InGameUiManager.Instance.SetPause(false); });
        _leaveButton.onClick.AddListener(NetworkManager.Instance.LeaveGame);
    }
}
