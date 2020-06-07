using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class portal : MonoBehaviour
{
    public static Vector3 spawnCoord;

    private void OnTriggerEnter(Collider other)
    {
        other.transform.position = spawnCoord;
    }
}
