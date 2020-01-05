using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class SurfaceWalkingComponent : MonoBehaviour
{
    
    public Collider col;
    public float detectionDistance = 1f;
    public float movingSpeed = 3f;
    [Range(0f,1f)]
    public float lerpCoeff = 0.5f;
    public float avoidDistance = 0.1f;
    public float coeffAvoid = 0.1f;
    public float positionHeightOffset = 0.2f;
    public GameObject target;

    private Vector3 currentNormal;
    
    private bool isGrounded = false;

    private void Start()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, positionHeightOffset, Vector3.up, out hit, 0,
            LayerMask.NameToLayer("Ground")))
        {
            transform.up = hit.normal;
        }
    }

    void Update()
    {
        isGrounded = false;
        wallClimbBehaviour();
        if (!isGrounded)
        {
            transform.position = transform.position - Vector3.up * 9.81f * Time.deltaTime;
            transform.up = Vector3.up;
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
        //if (Physics.SphereCast(transform.position, positionHeightOffset*1.2f, Vector3.up, out hit, 0, 1 << LayerMask.NameToLayer("Ground"))) //Doesnt Work
        //if (Physics.Raycast(transform.position + transform.up*0.1f, -transform.up, out hit, detectionDistance, 1 << LayerMask.NameToLayer("Ground")))
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position + transform.up*0.1f, -transform.up, out hit, detectionDistance, 1 << LayerMask.NameToLayer("Ground"))){
            isGrounded = true;
            transform.up = Vector3.Lerp(transform.up,hit.normal, lerpCoeff);
            transform.position = hit.point + hit.normal * positionHeightOffset;
            currentNormal = hit.normal;
        }
    }

    private void move()
    {
        Vector3 direction = Vector3.zero;
        if (target != null)
        {
            direction = target.transform.position - transform.position;
            direction.Normalize();
            transform.LookAt(target.transform);
            transform.up = currentNormal; //FIXME: Il faut réussir à le faire tourner autour de l'axe de la normale sans fournir de degrés genre avec un Look At
        }
        Vector3 avoid = avoidanceBehaviourPosition();
        transform.position += Vector3.ClampMagnitude(Time.deltaTime * movingSpeed* (direction*(1-coeffAvoid) + avoid*coeffAvoid),movingSpeed);
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
