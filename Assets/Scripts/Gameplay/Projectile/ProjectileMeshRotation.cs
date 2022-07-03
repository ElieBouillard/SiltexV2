using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMeshRotation : MonoBehaviour
{
    [SerializeField] private ParticleSystemRenderer[] _particleSystems;
    private Renderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        transform.Rotate(0, 550 * Time.deltaTime,0);
    }

    public void ChangeColor(int colorIndex)
    {
        Color color = NetworkColorsManager.Instance.Colors[colorIndex];
            
        _renderer.material.SetColor("_EmissionColor", color);

        foreach (var particleSystem in _particleSystems)
        {
            particleSystem.materials[0].SetColor("_EmissionColor", color);
        }
    }
}
