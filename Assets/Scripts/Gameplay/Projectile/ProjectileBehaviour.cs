using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 2f;
    
    private void Update()
    {
        transform.position += transform.forward * _movementSpeed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Wall")
        {
            Vector3 normal = collision.contacts[0].normal;
            transform.forward = Vector3.Reflect(transform.forward, normal);
            transform.forward = new Vector3(transform.forward.x, 0, transform.forward.z); 
        }
    }
}
