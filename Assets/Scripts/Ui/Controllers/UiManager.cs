using UnityEngine;

public class UiManager : MonoBehaviour
{
    #region Singleton
    private static UiManager _instance;
    public static UiManager Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
                _instance = value;
            else if (_instance != value)
            {
                Debug.Log($"{nameof(UiManager)} instance already exists, destroying object!");
                Destroy(value);
            }
        }
    }
    #endregion

    public GameObject LobbyPannel;
    public GameObject ConnectionPannel;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetConnectionPannel();
    }

    public void SetLobbyPannel()
    {
        LobbyPannel.SetActive(true);
        ConnectionPannel.SetActive(false);
    }

    public void SetConnectionPannel()
    {
        ConnectionPannel.SetActive(true);
        LobbyPannel.SetActive(false);
    }
}
