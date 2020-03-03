using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor.UI;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;


public enum GrapplingHookState {Expanding,Pulling,Retracting,Inactive}
public class Hook : MonoBehaviour
{
    public GrapplingHookState state = GrapplingHookState.Inactive;
    public Transform origin;
    public Vector3 target;
    public Rigidbody player;
    public float deploySpeed;
    public float retractSpeed;
    public float pullSpeed;
    public MeshRenderer renderer;
    private Vector3 direction;
    
    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            if (state == GrapplingHookState.Inactive)
            {
                print("Hook : Inactive");
                return;
            }
            if (state == GrapplingHookState.Expanding)
            {
                print("Hook : Expanding");
                Vector3 direction = target - transform.position;
                float magnitude = Mathf.Clamp(direction.magnitude,1f,20f);
                Vector3 normDir = direction.normalized;
                transform.position += normDir * magnitude * deploySpeed * Time.deltaTime; //
                RaycastHit hit;
                if (Physics.Raycast(transform.position, normDir, out hit, deploySpeed * Time.deltaTime))
                {
                    transform.position = hit.point;
                    target = hit.point;
                    transform.up = hit.normal;
                    state = GrapplingHookState.Pulling;
                }
            }
            if (state == GrapplingHookState.Pulling)
            {
                ///*
                print("Hook : Pulling");
                
                Vector3 direction = target - player.transform.position;
                float magnitude = Mathf.Clamp(direction.magnitude,10f,40f);
                Vector3 normDir = direction.normalized;
                Vector3 force = normDir * pullSpeed;
                print("direction.magnitude : " + direction.magnitude);
                player.transform.position = player.transform.position + ((Vector3.down*9.81f + force) * Time.deltaTime); //player.AddForce(force);
                if (direction.magnitude < 1f)
                {
                    state = GrapplingHookState.Retracting;
                }
                //*/

            }
        }

        if (state == GrapplingHookState.Retracting)
        {
            print("Hook : Retracting");
            Vector3 direction = origin.position - transform.position;
            float magnitude = Mathf.Clamp(direction.magnitude,1f,20f);
            Vector3 normDir = direction.normalized;
            transform.position += normDir * magnitude * retractSpeed * Time.deltaTime;
            RaycastHit hit;
            print("magnitude : " + direction.magnitude);
            if (direction.magnitude < 0.2)
            {
                print("Disabling grappling hook.");
                renderer.enabled = false;
                state = GrapplingHookState.Inactive;
            }
        }
    }
}
