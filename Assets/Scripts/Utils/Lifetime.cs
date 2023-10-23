using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifetime : MonoBehaviour
{
    [SerializeField] private float objectLifeTime = 2f;

    private void Start()
    {
        Destroy(gameObject, objectLifeTime);
    }
}
