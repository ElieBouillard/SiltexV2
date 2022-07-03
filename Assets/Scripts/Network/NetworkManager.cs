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
        DontDestroyOnLoad(gameObject);

        ClientMessage = gameObject.AddComponent<NetworkClientMessage>();
        ServerMessage = gameObject.AddComponent<NetworkServerMessage>();

        SceneManager.sceneLoaded += OnClientChangeScene;

        //InitializeSteam
        if (UseSteam) gameObject.AddComponent<SteamManager>();
        GetComponent<SteamLobbyManager>().enabled = UseSteam;
    }

    private void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

        SteamServer steamServer = new SteamServer();
        
        EnableClient(steamServer);
        EnableServer(steamServer);
    }

    private void FixedUpdate()
    {
        Client.Tick();

        if (Server.IsRunning) Server.Tick();
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
    }

    private void ServerOnClientDisconnected(object sender, ClientDisconnectedEventArgs e)
    {
        
    }
    #endregion

    #region Client
    private void ClientOnConnected(object sender, EventArgs e)
    {
        UiManager.Instance.SetLobbyPannel();

        ulong steamId = new ushort();
        if (UseSteam) steamId = (ulong)SteamUser.GetSteamID();
        
        ClientMessage.SendOnClientConnected(steamId);
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
        
        if(UseSteam) SteamLobbyManager.Instance.LeaveLobby();
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
            //Reorganize Lobby Placement
            int index = 0;
            foreach (var item in Players)
            {
                item.Value.gameObject.transform.position = _lobbySpawnPoints[index].transform.position;
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

        Server.Stop();

        ClientOnDisconnected(new object(), EventArgs.Empty);
    }

    public void AddPlayerToLobby(ushort id, ulong steamId, int colorIndex)
    {
        GameObject playerInstance = Instantiate(_lobbyPlayerPrefab, _lobbySpawnPoints[Players.Count].transform.position, Quaternion.identity);
        PlayerIdentity playerIdentity = playerInstance.GetComponent<PlayerIdentity>();
        playerIdentity.Id = id;

        if (UseSteam)
        {
            playerIdentity.SteamId = steamId;
            playerIdentity.SteamName = SteamFriends.GetFriendPersonaName((CSteamID) steamId);
        }

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

        if (UseSteam)
        {
            playerInstance.name = playerIdentity.SteamName;
            playerInstance.GetComponentInChildren<TMP_Text>().text = playerIdentity.SteamName;
        }
        else
        {
            playerInstance.name = $"Player : {playerIdentity.Id}";
            playerInstance.GetComponentInChildren<TMP_Text>().text = $"Player : {playerIdentity.Id}";
        }
        
        playerIdentity.ChangeColor(colorIndex);
        
        Players.Add(playerIdentity.Id, playerIdentity);
    }

    public void StartGame()
    {
        _isRunningGame = true;
        SceneManager.LoadScene("GameplayScene");
    }

    private void OnClientChangeScene(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == "LobbyScene")
        {
            _lobbySpawnPoints = SpawnPointsManager.Instance.GetSpawnPoints();
        }
        
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
            playerIdentityTemp.SteamId = player.Value.SteamId;
            playerIdentityTemp.ChangeColor(player.Value.ColorIndex);

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
