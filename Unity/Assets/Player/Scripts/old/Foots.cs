using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Foots : MonoBehaviour
{
    public int additionalJumps = 0;
    private int maxJumps = 0;

    private void Awake()
    {
        maxJumps = additionalJumps;
    }

    private void OnTriggerEnter(Collider info)     
    {
        additionalJumps = maxJumps;
    }
    
}
