using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerClientFireController : MonoBehaviour
{
    [SerializeField] private GameObject _projectile;
    [SerializeField] private GameObject _projectileMesh;
    [SerializeField] private float _couldown = 2f;
    
    private List<Shoot> _shootReceived = new List<Shoot>();

    public void Shoot(ushort playerId, int shootId, Vector3 pos, Vector3 dir)
    {
        for (int i = 0; i < _shootReceived.Count; i++)
        {
            if (_shootReceived[i].ShootId == shootId) return;
        }

        _projectileMesh.transform.DOKill();
        _projectileMesh.transform.localScale = Vector3.zero;
        
        Shoot shoot = new Shoot(shootId, pos, dir);
        _shootReceived.Add(shoot);

        GameObject projectileInstance = Instantiate(_projectile, pos, Quaternion.identity);
        projectileInstance.transform.forward = dir;
        projectileInstance.GetComponent<ProjectileBehaviour>().PlayerId = GetComponent<PlayerIdentity>().Id;
        projectileInstance.GetComponentInChildren<ProjectileMeshRotation>().ChangeColor(GetComponent<PlayerIdentity>().ColorIndex);
        NetworkManager.Instance.ClientMessage.SendShootReceived(playerId, shootId);

        StartCoroutine(EndShoot());
    }
    
    private IEnumerator EndShoot()
    {
        yield return new WaitForSeconds(_couldown / 2);
        _projectileMesh.transform.DOScale(new Vector3(0.5f, 0.05f, 0.5f), _couldown / 2);
    }
}
