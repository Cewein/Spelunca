using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RessourceMachine : MonoBehaviour
{

    public GunMagazine gl;
    
    private float angle;
    private float baseAngle;
    private float currentAngle;
    private void Start()
    {
        baseAngle = 90f; 
        angle = 120f;
        currentAngle = 90f;
    }

    void Update()
    {
        if(gl.currentResource == null) return;
        if (gl.currentResource.Type == ResourceType.fire)
        {
            currentAngle = baseAngle;
        }else if(gl.currentResource.Type == ResourceType.plant)
        {
            currentAngle = ((baseAngle + angle)+720)%360;
            //currentAngle = baseAngle + angle;
        }else if(gl.currentResource.Type == ResourceType.water)
        {
            currentAngle = ((baseAngle - angle)+720)%360;
        }
        
        float z = Mathf.Lerp(transform.eulerAngles.z, currentAngle, 0.2f);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y,z);
    }
}
