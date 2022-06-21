using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerFireController : MonoBehaviour
{
    [SerializeField] private Transform _launchPos;
    [SerializeField] private GameObject _projectile;
    [SerializeField] private LayerMask _floorMask;

    private NavMeshAgent _agent;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        // if(Physics.Raycast(ray, ))

        if (Input.GetKeyDown(KeyCode.A))
        {
            Vector3? dir;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _floorMask))
            {
                dir = (new Vector3(hit.point.x, 0, hit.point.z) - new Vector3(_launchPos.position.x, 0f, _launchPos.position.z)).normalized;
                transform.forward = dir.Value;
                
                GetComponentInChildren<Animator>().SetTrigger("Attack");
                
                _agent.isStopped = true;
                
                Vector3 realPos = hit.point - Camera.main.transform.forward * _launchPos.transform.position.y;

                StartCoroutine(Shoot(realPos));
            }
        }
    }

    private IEnumerator Shoot(Vector3 targetPos)
    {
        yield return new WaitForSeconds(0.25f);
        GameObject projectileInstance  = Instantiate(_projectile, _launchPos.position, Quaternion.identity);
        projectileInstance.transform.forward = (new Vector3(targetPos.x, 0f,targetPos.z) - transform.position).normalized;
        StartCoroutine(EndShoot());
    }

    private IEnumerator EndShoot()
    {
        yield return new WaitForSeconds(0.25f);
        _agent.isStopped = false;
    }
}