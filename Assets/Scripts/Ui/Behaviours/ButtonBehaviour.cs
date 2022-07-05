using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform _rectTransform;
    private Button _button;

    private void Start()
    {
        _button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        _rectTransform = GetComponent<RectTransform>();
        _rectTransform.localScale = Vector3.one; 
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _rectTransform.DOKill();
        _rectTransform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.20f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _rectTransform.DOKill();
        _rectTransform.DOScale(new Vector3(1f, 1f, 1f), 0.20f);
    }
}
