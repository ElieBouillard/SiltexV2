using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class PlayerLocalFireController : MonoBehaviour
{
    [SerializeField] private Transform _launchPos;
    [SerializeField] private GameObject _projectile;
    [SerializeField] private LayerMask _floorMask;
    [SerializeField] private GameObject _projectileMesh;
    [SerializeField] private float _initialCooldown;

    private NavMeshAgent _agent;

    private bool _canShoot = true;

    private float _cooldownShootClock = -1f;

    private List<Shoot> _shootsToSend = new List<Shoot>();
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

        if (_cooldownShootClock > 0)
        {
            _cooldownShootClock -= Time.deltaTime;
        }
        else if(_cooldownShootClock != -1)
        {
            _canShoot = true;
            _cooldownShootClock = -1;
        }
    }

    private void FixedUpdate()
    {
        foreach (Shoot shoot in _shootsToSend)
        {
            NetworkManager.Instance.ClientMessage.SendShoot(shoot.ShootId, shoot.Pos, shoot.Dir);
        }
    }

    private void InitializeShoot(Ray ray, Vector3 dir)
    {                
        _agent.isStopped = true;
                
        transform.forward = dir;
        
        Shoot(ray);

        _canShoot = false;
        _cooldownShootClock = _initialCooldown;
        _projectileMesh.transform.localScale = new Vector3(0,0,0);

        Shoot shoot = new Shoot(_launchPos.position, dir);
        _shootsToSend.Add(shoot);
    }
    
    private void Shoot(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _floorMask))
        {
            GameObject projectileInstance  = Instantiate(_projectile, _launchPos.position, Quaternion.identity);
            projectileInstance.transform.forward = (new Vector3(hit.point.x, 0f,hit.point.z) - new Vector3(transform.position.x, 0f, transform.position.z)).normalized;
            projectileInstance.GetComponentInChildren<ProjectileMeshRotation>().ChangeColor(transform.GetComponent<PlayerIdentity>().ColorIndex);
            StartCoroutine(EndShoot());
        }
    }

    private IEnumerator EndShoot()
    {
        StartCoroutine(EndShoot2());
        yield return new WaitForSeconds(0.2f);
        _agent.isStopped = false;
    }

    private IEnumerator EndShoot2()
    {
        yield return new WaitForSeconds(_initialCooldown / 2);
        _projectileMesh.transform.DOScale(new Vector3(0.5f, 0.05f, 0.5f), _initialCooldown / 2);
    }

    public void ShootSended(int shootId)
    {
        for (int i = 0; i < _shootsToSend.Count; i++)
        {
            if (_shootsToSend[i].ShootId == shootId)
            {
                _shootsToSend[i].ReceivedIndex++;

                if (_shootsToSend[i].ReceivedIndex == NetworkManager.Instance.Players.Count - 1) ;
                _shootsToSend.Remove(_shootsToSend[i]);
                break;
            }
        }
    }
}

public class Shoot
{
    public int ShootId;
    public Vector3 Pos;
    public Vector3 Dir;
    public int ReceivedIndex = 0;

    public Shoot(Vector3 pos, Vector3 dir)
    {
        ShootId = Random.Range(0, 999999);
        Pos = pos;
        Dir = dir;
    }
    
    public Shoot(int shootId, Vector3 pos, Vector3 dir)
    {
        ShootId = shootId;
        Pos = pos;
        Dir = dir;
    }
}