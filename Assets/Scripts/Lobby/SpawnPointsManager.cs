using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointsManager : MonoBehaviour
{
    #region Singleton
    private static SpawnPointsManager _instance;
    public static SpawnPointsManager Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
                _instance = value;
            else if (_instance != value)
            {
                Debug.Log($"{nameof(SpawnPointsManager)} instance already exists, destroying object!");
                Destroy(value);
            }
        }
    }
    #endregion

    [SerializeField] private Transform[] _spawnPoints;
    
    private void Awake()
    {
        Instance = this;
    }

    public Transform[] GetSpawnPoints()
    {
        return _spawnPoints;
    }
    
}
