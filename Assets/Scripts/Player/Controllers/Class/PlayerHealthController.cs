using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthController : MonoBehaviour
{
    [SerializeField] private ParticleSystem _deathParticleSystem;
    [SerializeField] private float _initialHealth = 100f;

    public Image HealthFillImage;

    private float _currHealth;

    private void Start()
    {
        InitializeHealth();
    }

    public void InitializeHealth()
    {
        _currHealth = _initialHealth;
        HealthFillImage.fillAmount = 1;
    }
    
    public void Setlife(float life)
    {
        _currHealth = life;
        HealthFillImage.fillAmount = _currHealth / _initialHealth;
        if (_currHealth <= 0) Death();
    }

    public float GetLife()
    {
        return _currHealth;
    }
    public virtual void Death()
    {
        _deathParticleSystem.Play();
    }
}
