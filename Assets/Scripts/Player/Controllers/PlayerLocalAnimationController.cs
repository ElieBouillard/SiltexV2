using System;
using UnityEngine;
using UnityEngine.AI;

public class PlayerLocalAnimationController : MonoBehaviour
{
    private Animator _animator;
    private NavMeshAgent _agent;
    private void Awake()
    {
        _animator = gameObject.GetComponent<Animator>();
        _agent = gameObject.GetComponentInParent<NavMeshAgent>();
    }

    private void Update()
    {
        _animator.SetFloat("Speed", _agent.velocity.magnitude);
    }
}
