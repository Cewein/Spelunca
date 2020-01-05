using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityDestroyer : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        Destroy( other.collider.gameObject);
       
    }
}
