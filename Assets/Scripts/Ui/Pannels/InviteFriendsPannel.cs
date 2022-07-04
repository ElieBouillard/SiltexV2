using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class InviteFriendsPannel : Pannel
{
    [SerializeField] private Button _inviteFriendButton;
    [SerializeField] private Button _quitPannelButton;
    [SerializeField] private GameObject _prefabs;
    [SerializeField] private RectTransform _content;

    private float offsetY = -25f;
    [ContextMenu("uwu")]
    public void Add()
    {
        GameObject a = Instantiate(_prefabs, _content);
        a.transform.localPosition = new Vector3(a.transform.localPosition.x,offsetY,a.transform.localPosition.z);
        _content.sizeDelta = new Vector2(_content.sizeDelta.x, _content.sizeDelta.y + 100f);
        offsetY -= 75;
    }
    
    public override void SetButtonsReferences()
    {
        _quitPannelButton.onClick.AddListener(OnClickQuitButton);
    }

    private void OnClickQuitButton()
    {
        this.transform.DOLocalMoveY(850f, 0.25f);
        _inviteFriendButton.gameObject.SetActive(true);
    }
}
