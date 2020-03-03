using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public LineRenderer lr;
    public Transform pointA;
    public Transform pointB;

    // Update is called once per frame
    void FixedUpdate()
    {
        lr.SetPosition(0,pointA.position);
        lr.SetPosition(1,pointB.position);
    }
}
