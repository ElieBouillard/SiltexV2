using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class PlayerLocalMovementController : MonoBehaviour
{
    [SerializeField] private LayerMask _floorMask;
    [SerializeField] private ParticleSystem _moveClickFx;
    private NavMeshAgent _agent;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _moveClickFx.transform.parent = null;
    }

    private void Update()
    {
        if (!_agent.enabled) return;
        
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _floorMask))
            {
                _agent.SetDestination(hit.point);
                _moveClickFx.Stop();
                _moveClickFx.transform.position = hit.point + new Vector3(0,0.1f,0);
                _moveClickFx.Play();
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            _agent.ResetPath();
        }
    }

    private void FixedUpdate()
    {
        if(NetworkManager.Instance == null) return;
        NetworkManager.Instance.ClientMessage.SendOnMovement(transform.position);
    }
}