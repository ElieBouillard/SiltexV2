using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorSelectionManager : MonoBehaviour
{
    #region Singleton
    private static ColorSelectionManager _instance;
    public static ColorSelectionManager Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
                _instance = value;
            else if (_instance != value)
            {
                Debug.Log($"{nameof(ColorSelectionManager)} instance already exists, destroying object!");
                Destroy(value);
            }
        }
    }
    #endregion
    
    [SerializeField] private Button _leftSelectionButton;
    [SerializeField] private Button _rightSelectionButton;
    [SerializeField] private Image _colorSelectedImage;
    public int CurrColorIndex = 0;

    private void Awake()
    {
        Instance = this;
        
        _leftSelectionButton.onClick.AddListener(OnClickLeftButton);
        _rightSelectionButton.onClick.AddListener(OnClickRightButton);
    }

    private void OnClickLeftButton()
    {
        AssignColor(CurrColorIndex - 1);
    }

    private void OnClickRightButton()
    {
        AssignColor(CurrColorIndex + 1);
    }

    public void AssignColor(int colorIndex)
    {
        Color[] colors = NetworkColorsManager.Instance.Colors;
        
        if (colorIndex < 0) colorIndex = colors.Length - 1;
        if (colorIndex >= colors.Length) colorIndex = 0;
        
        NetworkManager.Instance.LocalPlayer.ChangeColor(colorIndex);
        
        NetworkManager.Instance.ClientMessage.SendOnClientChangeColor(colorIndex);
    }

    public void ChangeImageColor(int colorIndex)
    {
        _colorSelectedImage.color = NetworkColorsManager.Instance.Colors[colorIndex];
        CurrColorIndex = colorIndex;
    }
}
