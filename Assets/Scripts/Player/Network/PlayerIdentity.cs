using UnityEngine;

public class PlayerIdentity : MonoBehaviour
{
    public ushort Id;
    public int ColorIndex = 0;
    public ulong SteamId;
    public string SteamName;
    public bool IsLocalPlayer { get; private set; }

    private Renderer[] _renderers;
    
    private void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>();
    }

    public void SetAsLocalPlayer()
    {
        IsLocalPlayer = true;
        
        if(CameraController.Instance == null) return;
        CameraController.Instance.SetCameraTarget(gameObject.transform);
    }

    public void ChangeColor(int colorIndex)
    {
        ColorIndex = colorIndex;

        Color color = NetworkColorsManager.Instance.Colors[colorIndex];
        
        for (int i = 0; i < _renderers.Length; i++)
        {
            if (i == 0)
            {
                _renderers[i].materials[1].SetColor("_EmissionColor", color);
            }
            
            if (i == 1)
            {
                _renderers[i].materials[0].SetColor("_EmissionColor", color);
            }   
        }

        // if (TryGetComponent<PlayerHealthController>(out PlayerHealthController healthController))
        // {
        //     healthController.HealthFillImage.color = color;
        // }

        if(!IsLocalPlayer) return;
        ColorSelectionManager.Instance.ChangeImageColor(colorIndex);
    }
}