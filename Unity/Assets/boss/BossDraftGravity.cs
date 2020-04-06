using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class BossDraftGravity : MonoBehaviour
{
    [Tooltip("Character weight is gravity multiply by it mass. It will be applied downward when it falling.")][SerializeField]
    private float weight = 9.81f*60.0f;
    private CharacterController characterController;
    [Tooltip("Layer used to seeking ground.")][SerializeField]
    private LayerMask groundLayer = -1;
    [Tooltip("Range to seek ground (from characters foot).")][SerializeField]
    private float seekGroundScope = 0.05f;
    [Tooltip("Range to seek ground (from characters foot) when character is in the air.")] [SerializeField]
    private float seekGroundScopeAir = 0.07f;
    [Tooltip("The character acceleration constant")][SerializeField]
    private float acceleration = 15;
    
    [Tooltip("Max speed when the character is in the air.")][SerializeField]
    private float maxSpeedInAir = 10f;
    private Vector3 HeadPosition => transform.position + (transform.up * characterController.radius);
    private bool isOnGround;
    private Vector3 normal;
    private float speedFactor;
    public Vector3 velocity;
    private Vector3 newVelocity;
    private RaycastHit hit;

    public MinerController miner;
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        characterController.enableOverlapRecovery = true;
        
    }

    private void Update()
    {
        SeekGround();
        if(Vector3.Distance(miner.GetComponent<Transform>().position, transform.position) < 40) Move();

    }

    void SeekGround()
    {
        float scope = isOnGround ? (characterController.skinWidth + seekGroundScope) : seekGroundScopeAir;
        isOnGround = false;
        normal = Vector3.up;
        if (Physics.CapsuleCast(HeadPosition,
                FootsPositionAtHeight(characterController.height),
                characterController.radius, Vector3.down,
                out RaycastHit hit, scope, groundLayer,
                QueryTriggerInteraction.Ignore
            )
        )
        {
            normal = hit.normal;

            // Collision occured only if the normal co-linear to the character up direction
            // and if the slope angle is lower than the character controller's (the unity component) slope limit
            if (!(Vector3.Dot(hit.normal, transform.up) > 0f)
                || !(Vector3.Angle(transform.up, normal) <= characterController.slopeLimit)) return;
            isOnGround = true;
            // keep the character sticking to the floor.
            if (hit.distance > characterController.skinWidth){ characterController.Move(Vector3.down * hit.distance);}
        }
    }
    
    private Vector3 FootsPositionAtHeight(float height)
    {
        return transform.position + (transform.up * (height - characterController.radius));
    }
    
    void Move()
    {
        if (isOnGround)
        {
            velocity = Vector3.zero;
        }
        else{AirControl();}

        Vector3 previousHeadPosition = HeadPosition;
        Vector3 previousFootsPosition = FootsPositionAtHeight(characterController.height);
        characterController.Move(velocity * Time.deltaTime);
        AvoidObstacles(previousHeadPosition, previousFootsPosition);
    }
    
    private void AirControl()
    {
        Vector3 horizontalVelocity = Vector3.ProjectOnPlane(velocity, Vector3.up);
        horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, maxSpeedInAir * speedFactor);
        velocity = horizontalVelocity + (Vector3.up * velocity.y);
        // apply the weight to the velocity
        velocity += Vector3.down * weight * Time.deltaTime;
    }
    
    private Vector3 SlideOnSlope(Vector3 direction, Vector3 slopeNormal)
    {
        return Vector3.Cross(slopeNormal, Vector3.Cross(direction, transform.up)).normalized;
    }
    
    private void AvoidObstacles(Vector3 previousHeadPosition,Vector3 previousFootsPosition)
    {
        if (Physics.CapsuleCast(previousHeadPosition, previousFootsPosition, characterController.radius, 
            velocity.normalized, out hit, velocity.magnitude * Time.deltaTime,
            -1, QueryTriggerInteraction.Ignore)
        )
        {
            velocity = Vector3.ProjectOnPlane(velocity, hit.normal);
        }
    }
}
