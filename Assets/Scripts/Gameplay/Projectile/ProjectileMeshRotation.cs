using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMeshRotation : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(0, 550 * Time.deltaTime,0);
    }
}
