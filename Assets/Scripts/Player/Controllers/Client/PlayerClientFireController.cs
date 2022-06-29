using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClientFireController : MonoBehaviour
{
    [SerializeField] private GameObject _projectile;

    private List<Shoot> _shootReceived = new List<Shoot>();

    public void Shoot(ushort playerId, int shootId, Vector3 pos, Vector3 dir)
    {
        for (int i = 0; i < _shootReceived.Count; i++)
        {
            if (_shootReceived[i].ShootId == shootId) return;
        }
        
        Shoot shoot = new Shoot(shootId, pos, dir);
        _shootReceived.Add(shoot);

        GameObject projectileInstance = Instantiate(_projectile, pos, Quaternion.identity);
        projectileInstance.transform.forward = dir;
        
        NetworkManager.Instance.ClientMessage.SendShootReceived(playerId, shootId);
    }
}
