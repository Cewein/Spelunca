using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using UnityEditor.UI;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;


public enum GrapplingHookState {Expanding,Pulling,Retracting,Inactive}
public class Hook : MonoBehaviour
{
    public MeshRenderer renderer;
    public Transform hookPoint;

    public Transform origin;
    public Rope rope;
    [Header("Behaviour parameters")]
    public float maxDeployDistance = 30f;
    public float deploySpeed;
    public float retractSpeed;
    public float pullSpeed;
    public float minPullClampDistance = 10f;
    public float maxPullClampDistance = 40f;
    public float rotationAngle = 180f;
    //[System.NonSerialized]
    public GrapplingHookState state = GrapplingHookState.Inactive;
    //[System.NonSerialized]
    public Vector3 target;
    //[System.NonSerialized]
    public Rigidbody player;
    //[System.NonSerialized]
    private Vector3 direction;

    void Start()
    {
        renderer.enabled = false;
        rope.renderer.enabled = false;
        rope.origin = this.origin;
        state = GrapplingHookState.Inactive;
    }
    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            if (state == GrapplingHookState.Inactive)
            {
                //print("Hook : Inactive");
                return;
            }
            if (state == GrapplingHookState.Expanding)
            {
                //print("Hook : Expanding");
                rope.renderer.enabled = true;
                Vector3 direction = target - transform.position;
                float magnitude = Mathf.Clamp(direction.magnitude,10f,20f);
                Vector3 normDir = direction.normalized;
                RaycastHit hit;
                //Debug.DrawRay(transform.position, normDir * magnitude * deploySpeed * Time.deltaTime);
                if (Physics.Raycast(transform.position, normDir, out hit, magnitude * deploySpeed * Time.deltaTime))
                {
                    //print("Hook : SURFACE TOUCHED");
                    transform.position = hit.point;
                    target = hit.point;
                    transform.forward = hit.normal;
                    transform.position -= hookPoint.position-transform.position;
                    state = GrapplingHookState.Pulling;
                }
                else
                {
                    transform.position += normDir * magnitude * deploySpeed * Time.deltaTime; //
                    transform.forward = normDir;
                }
            }
            if (state == GrapplingHookState.Pulling)
            {
                //print("Hook : Pulling");
                Vector3 direction = target - player.transform.position;
                float speed = Mathf.Clamp(direction.magnitude,minPullClampDistance,maxPullClampDistance);
                Vector3 normDir = direction.normalized;
                Vector3 force = speed * normDir * pullSpeed;
                //print("direction.magnitude : " + direction.magnitude);
                player.velocity += force * Time.deltaTime; //player.AddForce(force);
                if (direction.magnitude < 1f)
                {
                    state = GrapplingHookState.Inactive;
                    renderer.enabled = false;
                    rope.renderer.enabled = false;
                }
            }
        }

        if (state == GrapplingHookState.Retracting)
        {
            print("Hook : Retracting");
            Vector3 direction = rope.origin.position - transform.position;
            float magnitude = Mathf.Clamp(direction.magnitude,1f,20f);
            Vector3 normDir = direction.normalized;
            transform.position += normDir * magnitude * retractSpeed * Time.deltaTime;
            transform.forward = normDir;
            RaycastHit hit;
            print("magnitude : " + direction.magnitude);
            if (direction.magnitude < 0.2)
            {
                print("Disabling grappling hook.");
                renderer.enabled = false;
                rope.renderer.enabled = false;
                state = GrapplingHookState.Inactive;
            }
        }
    }

    public void setHookAnchor(Transform anchor)
    {
        this.rope.origin = anchor;
    }
}
