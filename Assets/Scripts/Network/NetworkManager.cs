using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using RiptideNetworking;
using RiptideNetworking.Transports.SteamTransport;
using RiptideNetworking.Utils;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using SteamClient = RiptideNetworking.Transports.SteamTransport.SteamClient;

public class NetworkManager : MonoBehaviour
{
    #region Singleton
    private static NetworkManager _instance;
    public static NetworkManager Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
                _instance = value;
            else if (_instance != value)
            {
                Debug.Log($"{nameof(NetworkManager)} instance already exists, destroying object!");
                Destroy(value);
            }
        }
    }
    #endregion

    #region Properties
    public Server Server;
    public Client Client;
    [HideInInspector] public NetworkClientMessage ClientMessage;
    [HideInInspector] public NetworkServerMessage ServerMessage;

    public Dictionary<ushort, PlayerIdentity> Players = new Dictionary<ushort, PlayerIdentity>();
     public PlayerIdentity LocalPlayer;
    #endregion

    #region Inspector
    public bool UseSteam;
    [SerializeField] private ushort _port = 7777;
    [SerializeField] private ushort _maxPlayer = 4;
    [SerializeField] private GameObject _lobbyPlayerPrefab;
    [SerializeField] private GameObject _localPlayerPrefab;
    [SerializeField] private GameObject _clientPlayerPrefab;
    [SerializeField] private Transform[] _lobbySpawnPoints;
    #endregion

    private bool _isRunningGame = false;
    
    #region Unity
    private void Awake()
    {
        Instance = this;

        ClientMessage = gameObject.AddComponent<NetworkClientMessage>();
        ServerMessage = gameObject.AddComponent<NetworkServerMessage>();
        
        CheckForSteamLobby();
        
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

        SteamServer steamServer = new SteamServer();
        
        EnableClient(steamServer);
        EnableServer(steamServer);

        SceneManager.sceneLoaded += OnClientChangeScene;
    }

    private void FixedUpdate()
    {
        Client.Tick();

        if (!Server.IsRunning) return;
        Server.Tick();
    }

    private void OnApplicationQuit()
    {
        LeaveGame();
    }

    private void EnableServer(SteamServer steamServer)
    {
        if (UseSteam) Server = new Server(steamServer);
        else Server = new Server();

        Server.ClientConnected += ServerOnClientConnected;
        Server.ClientDisconnected += ServerOnClientDisconnected;
    }
    
    private void EnableClient(SteamServer steamServer)
    {
        if (UseSteam) Client = new Client(new SteamClient(steamServer));
        else Client = new Client();

        Client.Connected += ClientOnConnected;
        Client.Disconnected += ClientOnDisconnected;
        Client.ConnectionFailed += ClientOnConnectionFailed;
        Client.ClientConnected += ClientOnPlayerJoin;
        Client.ClientDisconnected += ClientOnPlayerLeft;
    }
    #endregion

    #region Server
    private void ServerOnClientConnected(object sender, ServerClientConnectedEventArgs e)
    {
        if (_isRunningGame)
        {
            Server.DisconnectClient(e.Client.Id);
            return;
        }
        
        ServerMessage.ServerSendOnClientConnected(e.Client.Id);
    }

    private void ServerOnClientDisconnected(object sender, ClientDisconnectedEventArgs e)
    {
        
    }
    #endregion

    #region Client
    private void ClientOnConnected(object sender, EventArgs e)
    {
        UiManager.Instance.SetLobbyPannel();
    }

    private void ClientOnDisconnected(object sender, EventArgs e)
    {
        if (_isRunningGame)
        {
            SceneManager.LoadScene("LobbyScene");
        }
        else
        {
            UiManager.Instance.SetConnectionPannel();
        }

        foreach (var item in Players)
        {
           Destroy(item.Value.gameObject); 
        }
        Players.Clear();

        _isRunningGame = false;
        
        // if(UseSteam) SteamLobbyManager.Instance.LeaveLobby();
    }

    private void ClientOnConnectionFailed(object sender, EventArgs e)
    {
        UiManager.Instance.SetConnectionPannel();
    }

    private void ClientOnPlayerJoin(object sender, ClientConnectedEventArgs e)
    {
        
    }

    private void ClientOnPlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        foreach (var item in Players)
        {
            if (item.Value.Id == e.Id)
            {
                Destroy(item.Value.gameObject);
                Players.Remove(item.Key);
                break;
            }
        }

        if (_isRunningGame)
        {
            
        }
        else
        {
            int index = 0;
            foreach (var item in Players)
            {
                item.Value.gameObject.transform.position = _lobbySpawnPoints[index].position;
                index++;
            }
        }
    }
    #endregion

    #region Functions
    public void StartHost()
    {
        if (UseSteam)
        {
            SteamLobbyManager.Instance.CreateLobby();
        }
        else
        {   
            Server.Start(_port, _maxPlayer);
            Client.Connect($"127.0.0.1:{_port}");
        }
    }

    public void JoinLobby()
    {
        if(UseSteam) return;
        Client.Connect($"127.0.0.1:{_port}");
    }

    public void LeaveGame()
    {
        Client.Disconnect();
        ClientOnDisconnected(new object(), EventArgs.Empty);

        if (Server.IsRunning) Server.Stop();
    }

    public void AddPlayerToLobby(ushort id)
    {
        GameObject playerInstance = Instantiate(_lobbyPlayerPrefab, _lobbySpawnPoints[Players.Count].position, Quaternion.identity);
        PlayerIdentity playerIdentity = playerInstance.GetComponent<PlayerIdentity>();
        playerIdentity.Id = id;

        if (playerIdentity.Id == Client.Id)
        {
            playerIdentity.SetAsLocalPlayer();
            LocalPlayer = playerIdentity;

            if (id == 1)
            {
                UiManager.Instance.LobbyPannel.GetComponent<LobbyPannel>().EnableStartGameButton(true);
            }
            else
            {
                UiManager.Instance.LobbyPannel.GetComponent<LobbyPannel>().EnableStartGameButton(false);
            }
        }

        playerInstance.name = $"Player : {playerIdentity.Id}";
        playerInstance.GetComponentInChildren<TMP_Text>().text = $"Player : {playerIdentity.Id}";

        Players.Add(playerIdentity.Id, playerIdentity);
    }

    public void StartGame()
    {
        _isRunningGame = true;
        SceneManager.LoadScene("GameplayScene");
    }

    private void OnClientChangeScene(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == "GameplayScene")
        {
            InitializeGameplay();
        }
    }

    private void InitializeGameplay()
    {
        Dictionary<ushort, PlayerIdentity> playersTemp = new Dictionary<ushort, PlayerIdentity>();
        foreach (var player in Players)
        {
            GameObject playerTemp = null;
            PlayerIdentity playerIdentityTemp = null;

            if (player.Key == Client.Id)
            {
                playerTemp = Instantiate(_localPlayerPrefab, Vector3.zero, Quaternion.identity);
                playerTemp.name = $"Player Local : {player.Key}";
            }
            else
            {
                playerTemp = Instantiate(_clientPlayerPrefab, Vector3.zero, Quaternion.identity);
                playerTemp.name = $"Player Client : {player.Key}";
            }
            
            playerIdentityTemp = playerTemp.GetComponent<PlayerIdentity>();
            playerIdentityTemp.Id = player.Key;

            if (playerIdentityTemp.Id == Client.Id)
            {
                playerIdentityTemp.SetAsLocalPlayer();
                LocalPlayer = playerIdentityTemp;
            }

            playersTemp.Add(playerIdentityTemp.Id, playerIdentityTemp);
        }
        
        Players.Clear();
        Players = playersTemp;
    }
    
    private void CheckForSteamLobby()
    {
        SteamLobbyManager lobbyManager = FindObjectOfType<SteamLobbyManager>();
        
        if (lobbyManager == null) return;
        
        if (UseSteam)
        {
            gameObject.AddComponent<SteamManager>();
            lobbyManager.enabled = true;
        }
        else
        {
            lobbyManager.enabled = false;
        }
    }
    #endregion

    #region Debug
    [ContextMenu("DebugPlayersList")]
    public void DebugPlayersList()
    {
        foreach (var item in Players)
        {
            Debug.Log(item.Value.gameObject);
        }
    }
    #endregion
}
