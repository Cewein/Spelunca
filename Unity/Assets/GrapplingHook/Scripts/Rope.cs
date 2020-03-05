using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public LineRenderer renderer;
    public Transform origin;
    public Transform hook;

    // Update is called once per frame
    void FixedUpdate()
    {
        renderer.SetPosition(0,origin.position);
        renderer.SetPosition(1,hook.position);
    }
}
