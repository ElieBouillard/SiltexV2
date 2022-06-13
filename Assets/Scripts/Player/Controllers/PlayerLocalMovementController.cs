using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerLocalMovementController : MonoBehaviour
{
    [SerializeField] private LayerMask _floorMask;
    private NavMeshAgent _agent;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _floorMask))
            {
                _agent.SetDestination(hit.point);
            }
        }
    }

    private void FixedUpdate()
    {
        NetworkManager.Instance.ClientMessage.SendOnMovement(transform.position, transform.GetChild(0).rotation.eulerAngles.y);
    }
}
