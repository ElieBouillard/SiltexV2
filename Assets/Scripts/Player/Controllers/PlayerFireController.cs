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
    [SerializeField] private GameObject _projectileMesh;

    private NavMeshAgent _agent;

    private bool _canShoot = true;
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && _canShoot)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _floorMask))
            {
                Vector3? dir = (new Vector3(hit.point.x, 0, hit.point.z) - new Vector3(transform.position.x, 0f, transform.position.z)).normalized;

                InitializeShoot(ray, dir.Value);
            }
        }
    }

    private void InitializeShoot(Ray ray, Vector3 dir)
    {                
        _agent.isStopped = true;
                
        transform.forward = dir;
        
        Shoot(ray);

        _canShoot = false;
        _projectileMesh.SetActive(false);
        
    }
    
    private void Shoot(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _floorMask))
        {
            GameObject projectileInstance  = Instantiate(_projectile, _launchPos.position, Quaternion.identity);
            projectileInstance.transform.forward = (new Vector3(hit.point.x, 0f,hit.point.z) - new Vector3(transform.position.x, 0f, transform.position.z)).normalized;
            StartCoroutine(EndShoot());
        }
        
    }

    private IEnumerator EndShoot()
    {
        yield return new WaitForSeconds(0.2f);
        _agent.isStopped = false;

        _canShoot = true;
        _projectileMesh.SetActive(true);
    }
}