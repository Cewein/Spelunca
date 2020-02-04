using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyComponent : MonoBehaviour
{
    
    [Header("Base entity stats")]
    [SerializeField]private float startHP = 10f; //the entity's start health points
    public float baseDamage = 1f; //base damage dealed when attacking an other entiy (such as a player)
    public float movingSpeed = 3f; //Speed at which the entity is able to move

    [Header("Behaviours parameters")]
    public GameObject target; //The target of the entity
    public float surfaceDetectionDistance = 1f; //Distance from the center point up to which the Enemy is able to detect the presence of a ground surface 
    public float surfaceDetectionOffset = 1f; //Offset form which the detection raycast is casted (used if the center point is set at the bottom of the 3D model
    public float surfaceWalkingHeightOffset = 1f; //Offset used to make the 3D model touch the ground with its feet/lugs/paws etc
    public float avoidanceDistance = 0.1f; // Length of the raycasts casted in order to detect nearby fellow entities 

    public bool isGrounded = false;

    private Vector3 currentSurfaceNormal;
    //Private variables
    private float HP;
    

    public void hit(float damages)
    {
        HP -= damages;
    }
    
    

    private void Start()
    {
        HP = startHP;
        /*RaycastHit hit;
        if (Physics.SphereCast(transform.position, surfaceWalkingHeightOffset, Vector3.up, out hit, 0,
            LayerMask.NameToLayer("Ground")))
        {
            transform.up = hit.normal;
        }*/
    }
    void Update()
    {
        //isGrounded = false;
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
        if (HP <= 0)
        {
            Destroy(gameObject);
        }
        
    }

    public void wallClimbBehaviour()
    {
        //if (Physics.SphereCast(transform.position, positionHeightOffset*1.2f, Vector3.up, out hit, 0, 1 << LayerMask.NameToLayer("Ground"))) //Doesnt Work
        //if (Physics.Raycast(transform.position + transform.up*0.1f, -transform.up, out hit, detectionDistance, 1 << LayerMask.NameToLayer("Ground")))
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position + transform.up*surfaceDetectionOffset, -transform.up, out hit, surfaceDetectionDistance, 1 << LayerMask.NameToLayer("Ground"))){
            
            isGrounded = true;
            transform.up = Vector3.Lerp(transform.up,hit.normal, 0.5f);
            transform.position = hit.point + hit.normal * surfaceWalkingHeightOffset;
            currentSurfaceNormal = hit.normal;
        }
        else
        {
            isGrounded = false;
        }
    }

    public void move()
    {
        Vector3 direction = Vector3.zero;
        if (target != null)
        {
            direction = target.transform.position - transform.position;
            direction.Normalize();
            transform.LookAt(target.transform);
            transform.up = currentSurfaceNormal; //FIXME: Il faut réussir à le faire tourner autour de l'axe de la normale sans fournir de degrés genre avec un Look At
        }
        Vector3 avoid = Vector3.zero;//avoidanceBehaviourPosition();
        transform.position += Vector3.ClampMagnitude(Time.deltaTime * movingSpeed* Vector3.Lerp(direction,avoid, 0.5f),movingSpeed);
    }
    public Vector3 avoidanceBehaviourPosition()
    {
        Vector3 avoidanceMove = Vector3.zero;
        int nAvoid = 0;
        
        if (Physics.Raycast(transform.position, transform.forward, avoidanceDistance,
            1 << LayerMask.NameToLayer("Enemy")))
        {
            avoidanceMove -= (transform.forward * avoidanceDistance);
            nAvoid++;
        }
        if (Physics.Raycast(transform.position, -transform.forward, avoidanceDistance,
            1 << LayerMask.NameToLayer("Enemy")))
        {
            avoidanceMove -= (-transform.forward * avoidanceDistance);
            nAvoid++;
        }
        if (Physics.Raycast(transform.position, transform.right, avoidanceDistance,
            1 << LayerMask.NameToLayer("Enemy")))
        {
            avoidanceMove -= (transform.right * avoidanceDistance);
            nAvoid++;
        }
        if (Physics.Raycast(transform.position, -transform.right, avoidanceDistance,
            1 << LayerMask.NameToLayer("Enemy")))
        {
            avoidanceMove -= (-transform.right * avoidanceDistance);
            nAvoid++;
        }


        if (nAvoid > 0)
        {
            avoidanceMove /= nAvoid;
        }
        else
        {
            avoidanceMove = Vector3.zero;
        }
        return avoidanceMove;
    }

    private void OnDrawGizmosSelected()
    {    
        Gizmos.color=Color.red;
        Gizmos.DrawRay(transform.position + transform.up*surfaceDetectionOffset,-transform.up*surfaceDetectionDistance);

    }
}
