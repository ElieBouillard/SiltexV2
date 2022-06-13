using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocalAnimationController : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        gameObject.GetComponent<Animator>();
    }
}
