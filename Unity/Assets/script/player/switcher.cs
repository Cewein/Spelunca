using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class switcher : MonoBehaviour
{

    public Color hit;
    public Color notHit;

    bool isHit;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().material.color = notHit;
        isHit = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(isHit)
        {
            GetComponent<Renderer>().material.color = hit;
        }
        else
        {
            GetComponent<Renderer>().material.color = notHit;
        }

        isHit = false;
    }

    void HitByRay()
    {
        GetComponent<Renderer>().material.color = hit;
        isHit = true;
    }

}
