using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BracketPannel : MonoBehaviour
{
    #region Singleton
    private static BracketPannel _instance;
    public static BracketPannel Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
                _instance = value;
            else if (_instance != value)
            {
                Debug.Log($"{nameof(BracketPannel)} instance already exists, destroying object!");
                Destroy(value);
            }
        }
    }
    #endregion
    
    [SerializeField] private GameObject _profileDisplay;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private Transform[] _semiFinals;
    [SerializeField] private Transform[] _finals;

    private List<GameObject> _playersDiplay = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    public void InstantiatePlayerInSpawnBracket(ushort playerId, Vector2 placeId)
    {
        string playername = "";
        
        foreach (var player in NetworkManager.Instance.Players)
        {
            if (player.Key == playerId)
            {
                playername = player.Value.name;
            }
        }
        
        if (placeId.x == 0)
        {
            GameObject _profileInstance = Instantiate(_profileDisplay, transform);
            _profileInstance.GetComponent<ProfileBracketBehaviour>().Initialize(playername);
            _profileInstance.transform.position = _spawnPoints[(int)placeId.y].position;
            _playersDiplay.Add(_profileInstance);
        }

        if (placeId.x == 1)
        {
            
        }

        if (placeId.x == 2)
        {
            
        }
    }

}
