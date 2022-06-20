using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerLocalMovementController : MonoBehaviour
{
    [SerializeField] private LayerMask _floorMask;
    [SerializeField] private ParticleSystem _moveClickFx;
    private NavMeshAgent _agent;
    private Animator _animator;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();
        _moveClickFx.transform.parent = null;
    }

    private void Update()
    {
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
    }

    private void FixedUpdate()
    {
        if(NetworkManager.Instance == null) return;
        NetworkManager.Instance.ClientMessage.SendOnMovement(transform.position, transform.GetChild(0).rotation.eulerAngles.y, _animator.GetFloat("Speed"));
    }
}
