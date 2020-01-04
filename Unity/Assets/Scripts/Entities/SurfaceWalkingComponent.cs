using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class SurfaceWalkingComponent : MonoBehaviour
{
    
    public Collider col;
    public float detectionDistance = 1f;
    public float moovingSpeed = 3f;
    [Range(0f,1f)]
    public float lerpCoeff = 0.5f;
    public float avoidDistance = 0.1f;
    public float coeffAvoid = 0.1f;
    public float positionHeightOffset = 0.2f;
    public GameObject target;
    
    private bool isGrounded = false;
    void Update()
    {
        isGrounded = false;
        wallClimbBehaviour();
        if (!isGrounded)
        {
            transform.position = transform.position - Vector3.up * 9.81f * Time.deltaTime;
        }
        else
        {
            move();    
        }
        
    }

    private void LateUpdate()
    {
        if (transform.position.y < -6f)
        {
            //Destroy(gameObject);
        }
    }

    private void wallClimbBehaviour()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + transform.up*0.1f, -transform.up, out hit, detectionDistance, 1 << LayerMask.NameToLayer("Ground")))
        {
            isGrounded = true;
            transform.up = Vector3.Lerp(transform.up,hit.normal, lerpCoeff);
            transform.position = hit.point + hit.normal * positionHeightOffset;
        }
    }

    private void move()
    {
        Vector3 avoid = avoidanceBehaviourPosition() * Time.deltaTime;
        Vector3 direction = Vector3.ClampMagnitude(target.transform.position - transform.position, 1) * Time.deltaTime * moovingSpeed;
        transform.position += Vector3.ClampMagnitude(moovingSpeed* (direction*(1-coeffAvoid) + avoid*coeffAvoid),moovingSpeed);
    }
    private Vector3 avoidanceBehaviourPosition()
    {
        Vector3 avoidanceMove = Vector3.zero;
        int nAvoid = 0;
        
        if (Physics.Raycast(transform.position, transform.forward, avoidDistance,
            1 << LayerMask.NameToLayer("Enemy")))
        {
            avoidanceMove -= (transform.forward * avoidDistance);
            nAvoid++;
        }
        if (Physics.Raycast(transform.position, -transform.forward, avoidDistance,
            1 << LayerMask.NameToLayer("Enemy")))
        {
            avoidanceMove -= (-transform.forward * avoidDistance);
            nAvoid++;
        }
        if (Physics.Raycast(transform.position, transform.right, avoidDistance,
            1 << LayerMask.NameToLayer("Enemy")))
        {
            avoidanceMove -= (transform.right * avoidDistance);
            nAvoid++;
        }
        if (Physics.Raycast(transform.position, -transform.right, avoidDistance,
            1 << LayerMask.NameToLayer("Enemy")))
        {
            avoidanceMove -= (-transform.right * avoidDistance);
            nAvoid++;
        }


        if (nAvoid > 0)
        {
            avoidanceMove /= nAvoid;
        }
        /*
            Debug.DrawRay(transform.position,transform.forward,Color.red);
            Debug.DrawRay(transform.position,-transform.forward,Color.red);
            Debug.DrawRay(transform.position,transform.right,Color.red);
            Debug.DrawRay(transform.position,-transform.right,Color.red);
        */
        return avoidanceMove;
    }
}
