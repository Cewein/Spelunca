using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Foots : MonoBehaviour
{
    public int additionalJumps = 0;
    private int maxJumps = 0;
    private RaycastHit hit;
    private Ray ray; y
    private void Awake()
    {
        maxJumps = additionalJumps;
        ray = new Ray(transform.position, -transform.up);
    }

    private void FixedUpdate()
    {
        ray.origin = transform.position;
        if (!Physics.SphereCast(ray, 10,out hit, 5)) return;
        additionalJumps = maxJumps;
        Debug.DrawRay(ray.origin,ray.direction,Color.yellow);
    }

}
