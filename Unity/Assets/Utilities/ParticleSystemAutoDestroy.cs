using System;
using UnityEngine;

public class ParticleSystemAutoDestroy : MonoBehaviour
{
    private ParticleSystem ps;

    private void Awake(){ps = GetComponent<ParticleSystem>();}
    private void Update()
    {
        if (ps.IsAlive()) return;
        Destroy(gameObject);
    }
}
