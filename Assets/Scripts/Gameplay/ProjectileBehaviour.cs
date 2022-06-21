using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.AssetImporters;
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
        }
    }
}
