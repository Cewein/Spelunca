using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public LineRenderer renderer;
    public Transform pointA;
    public Transform pointB;

    // Update is called once per frame
    void FixedUpdate()
    {
        renderer.SetPosition(0,pointA.position);
        renderer.SetPosition(1,pointB.position);
    }
}
