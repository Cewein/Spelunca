using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRotation : MonoBehaviour
{ 
    public Transform Target;
    public Transform UpDirection;
    public Transform Self;
    public float sineScale = 1;
    public float sineOctaves = 1;
    public float directionErrorCoeff = 1;

    // Update is called once per frame
    void Update()
    {
        var toTarget = Target.position - Self.position;
        var toUp = UpDirection.position - Self.position;
        float sine = 0f;
        for (int i = 1; i <= sineOctaves; i++)
        {
            sine += Mathf.PerlinNoise(Time.time * (sineScale * i), i) * 2 - 1;
        }
        Self.rotation = Quaternion.LookRotation(toUp.normalized, -toTarget.normalized);
        Self.Rotate(Vector3.right, 90f, Space.Self);
        Vector3 dir = (sine * transform.right * directionErrorCoeff + transform.forward).normalized;
        Debug.DrawRay(transform.position,dir);
        Self.rotation = Quaternion.LookRotation(toUp.normalized, -dir.normalized);
        Self.Rotate(Vector3.right, 90f, Space.Self);
    }
    
    /*
        for (int i = 1; i <= sineOctaves; i++)
        {
            sine += Mathf.Sin(Time.time*(sineScale*i));
        }
     */
}
