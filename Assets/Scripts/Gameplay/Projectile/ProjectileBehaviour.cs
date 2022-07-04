using System.Security.Cryptography;
using DG.Tweening;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 2f;
    public int PlayerId;
    [SerializeField] private ParticleSystem _particleSystem;

    private void Update()
    {
        transform.position += transform.forward * _movementSpeed * Time.deltaTime;
    }

    private int index = 0;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Wall")
        {
            Vector3 normal = collision.contacts[0].normal;
            transform.forward = Vector3.Reflect(transform.forward, normal);
            transform.forward = new Vector3(transform.forward.x, 0, transform.forward.z);
        }
        
        if (collision.collider.tag == "CenterWall")
        {
            index++;
            if (index <= 1) return;
            _movementSpeed = 0;
            transform.DOMove(collision.contacts[0].point, 0.2f);
            transform.DOScale(Vector3.zero, 0.2f);
            Destroy(gameObject, 0.5f);
        }
            
        if (collision.collider.TryGetComponent<PlayerHealthController>(out PlayerHealthController playerHealth))
        {
            if (playerHealth.GetComponent<PlayerIdentity>().Id != PlayerId)
            {
                playerHealth.TakeDamage(20f);
                Destroy(gameObject);
                PlayFx();
            }
        }
    }

    private void PlayFx()
    {
        _particleSystem.transform.parent = null;
        _particleSystem.Play();
        Destroy(_particleSystem, 2f);
    }
}
