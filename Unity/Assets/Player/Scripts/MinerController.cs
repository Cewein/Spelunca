using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController), typeof(MinerInputHandler))]
public class MinerController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the main camera used for the player")]
    public Camera playerCamera;
  

    [Header("General")]
    [Tooltip("Force applied downward when in the air")]
    public float weight = 20f;
    [Tooltip("Physic layers checked to consider the player grounded")]
    public LayerMask groundCheckLayers = -1;
    [Tooltip("distance from the bottom of the character controller capsule to test for grounded")]
    public float seekGroundScope = 0.05f;

    [Header("Movement")]
    [Tooltip("Max movement speed when grounded (when not sprinting)")]
    public float maxSpeedOnGround = 10f;
    [Tooltip("Sharpness for the movement when grounded, a low value will make the player accelerate and decelerate slowly, a high value will do the opposite")]
    public float movementSharpnessOnGround = 15;
    [Tooltip("Max movement speed when crouching")]
    [Range(0,1)]
    public float maxSpeedCrouchedRatio = 0.5f;
    [Tooltip("Max movement speed when not grounded")]
    public float maxSpeedInAir = 10f;
    [Tooltip("Acceleration speed when in the air")]
    public float accelerationSpeedInAir = 25f;
    [Tooltip("Multiplicator for the sprint speed (based on grounded speed)")]
    public float sprintSpeedModifier = 2f;
    [Tooltip("Height at which the player dies instantly when falling off the map")]
    public float killHeight = -50f;

    [Header("Rotation")]
    [Tooltip("Rotation speed for moving the camera")]
    public float rotationSpeed = 200f;
    [Range(0.1f, 1f)]
    [Tooltip("Rotation speed multiplier when aiming")]
    public float aimingRotationMultiplier = 0.4f;
    
    [Tooltip("The minimum and the maximum yaw value in degree.")][SerializeField]
    private Vector2 yawAmplitude = new Vector2(-45,45);
    
    [Tooltip("The minimum and the maximum pitch value in degree.")][SerializeField]
    private Vector2 pitchAmplitude = new Vector2(90,-45);
    [Tooltip("If selected, clamp vertical rotation between the two angles value defined in pitch amplitude.")][SerializeField]
    private bool clampVerticalRotation = true;
    
    [Tooltip("If selected, clamp horizontal rotation between the two angles value defined in yaw amplitude.")][SerializeField]
    private bool clampHorizontalRotation = false;
    [Header("Jump")]
    [Tooltip("Force applied upward when jumping")]
    public float jumpForce = 9f;

    [Header("Stance")]
    [Tooltip("Ratio (0-1) of the character height where the camera will be at")]
    public float cameraHeightRatio = 0.9f;
    [Tooltip("Height of character when standing")]
    public float capsuleHeightStanding = 1.8f;
    [Tooltip("Height of character when crouching")]
    public float capsuleHeightCrouching = 0.9f;
    [Tooltip("Speed of crouching transitions")]
    public float crouchingSharpness = 10f;

    public UnityAction<bool> onStanceChanged;

    public bool isOnGround { get; private set; }
    public bool hasJumpedThisFrame { get; private set; }
    public bool isDead { get; private set; }
    public bool isCrouching { get; private set; }
   
        
    MinerInputHandler minerInputs;
    CharacterController characterController;
    Vector3 normal;
    private Vector3 velocity;
    private float speedModifier;
    private Vector3 newVelocity;
    Vector3 m_LatestImpactSpeed;
    private RaycastHit hit;
    float lastTimeJumped = 0f;
    float pitch = 0f;
    float yaw = 0f;
    float m_footstepDistanceCounter;
    float newHeight;
    private bool isSprinting;

    const float jumpRecoverTime = 0.2f;
    const float seekGroundScopeAir = 0.07f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        minerInputs = GetComponent<MinerInputHandler>();
        characterController.enableOverlapRecovery = true;
        if (playerCamera == null) playerCamera = Camera.main;
        ForceStanding();
    }

    private void ForceStanding()
    {
        Crouch(false);
        characterController.height = newHeight;
        characterController.center = Vector3.up * characterController.height * 0.5f;
        playerCamera.transform.localPosition = Vector3.up * newHeight * cameraHeightRatio;
    }
    
    private void Update()
    {
        hasJumpedThisFrame = false;
        SeekGround();
        if (minerInputs.isCrouching()){Crouch(!isCrouching);}
        SetHeightSmoothly();
        Rotate();
        Move();
        Jump();
    }

  
    void SeekGround()
    {
        float chosenGroundCheckDistance = isOnGround ? (characterController.skinWidth + seekGroundScope) : seekGroundScopeAir;
        isOnGround = false;
        normal = Vector3.up;

        if (Time.time >= lastTimeJumped + jumpRecoverTime)
        {
            // if we're grounded, collect info about the ground normal with a downward capsule cast representing our character capsule
            if (Physics.CapsuleCast(HeadPosition(), FootsPosition(characterController.height), characterController.radius, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance, groundCheckLayers, QueryTriggerInteraction.Ignore))
            {
                // storing the upward direction for the surface found
                normal = hit.normal;

                // Only consider this a valid ground hit if the ground normal goes in the same direction as the character up
                // and if the slope angle is lower than the character controller's limit
                if (Vector3.Dot(hit.normal, transform.up) > 0f && Vector3.Angle(transform.up, normal) <= characterController.slopeLimit)
                {
                    isOnGround = true;

                    // handle snapping to the ground
                    if (hit.distance > characterController.skinWidth)
                    {
                        characterController.Move(Vector3.down * hit.distance);
                    }
                }
            }
        }
    }

    private void Rotate()
    {
        yaw += minerInputs.Azimuth * rotationSpeed;  
        pitch += minerInputs.Elevation * rotationSpeed ;

        if ( clampHorizontalRotation ) yaw =  Mathf.Clamp(yaw,yawAmplitude.x,yawAmplitude.y) ;
        if ( clampVerticalRotation ) pitch = Mathf.Clamp(pitch,pitchAmplitude.x,pitchAmplitude.y) ;
         
        transform.localEulerAngles = new Vector3(0f, yaw, 0f);
        playerCamera.transform.localEulerAngles = new Vector3(pitch, 0, 0);

    }

    private void Jump()
    {
        if (!isOnGround || !minerInputs.isJumping()) return;
        if (Crouch(false))
        {
            velocity.y = 0f;
            velocity += Vector3.up * jumpForce;
            lastTimeJumped = Time.time;
            hasJumpedThisFrame = true;
            isOnGround = false;
            //  normal = Vector3.up;
        }
    }

    private void AirControl()
    {
        velocity +=  transform.TransformVector(minerInputs.Movement) * accelerationSpeedInAir * Time.deltaTime;
        Vector3 horizontalVelocity = Vector3.ProjectOnPlane(velocity, Vector3.up);
        horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, maxSpeedInAir * speedModifier);
        velocity = horizontalVelocity + (Vector3.up * velocity.y);
        // apply the weight to the velocity
        velocity += Vector3.down * weight * Time.deltaTime;
    }

    private void Run()
    {
        isSprinting = minerInputs.isRunning();
        if (isSprinting) { isSprinting = Crouch(false); }
        speedModifier = isSprinting ? sprintSpeedModifier : 1f;
    }
    
    void Move()
    {
        Run();
        if (isOnGround)
        {
            newVelocity =  transform.TransformVector(minerInputs.Movement) * maxSpeedOnGround * speedModifier;
            if (isCrouching){ newVelocity *= maxSpeedCrouchedRatio; }
            newVelocity = SlideOnSlope(newVelocity.normalized, normal) * newVelocity.magnitude;
            velocity = Vector3.Lerp(velocity, newVelocity, movementSharpnessOnGround * Time.deltaTime);
        }
        else{AirControl();}

        Vector3 previousHeadPosition = HeadPosition();
        Vector3 previousFootsPosition = FootsPosition(characterController.height);
        characterController.Move(velocity * Time.deltaTime);
        AvoidObstacles(previousHeadPosition, previousFootsPosition);
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
    
    private Vector3 HeadPosition()
    {
        return transform.position + (transform.up * characterController.radius);
    }
    
    private Vector3 FootsPosition(float height)
    {
        return transform.position + (transform.up * (height - characterController.radius));
    }

    /// <summary>
    /// To not being stuck on sharped ground.
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="slopeNormal"></param>
    /// <returns></returns>
    private Vector3 SlideOnSlope(Vector3 direction, Vector3 slopeNormal)
    {
        return Vector3.Cross(slopeNormal, Vector3.Cross(direction, transform.up)).normalized;
    }

    void SetHeightSmoothly()
    {
        if (characterController.height == newHeight) return;
        characterController.height = Mathf.Lerp(characterController.height, newHeight, crouchingSharpness * Time.deltaTime);
        characterController.center = Vector3.up * characterController.height * 0.5f;
        playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, Vector3.up * newHeight * cameraHeightRatio, crouchingSharpness * Time.deltaTime);
    }

    bool Crouch(bool crouched)
    {
        newHeight = crouched ? capsuleHeightCrouching : capsuleHeightStanding;
        // Check for obstacles
        if (!crouched && Physics.OverlapCapsule(HeadPosition(), 
                                                FootsPosition(capsuleHeightStanding),
                                                       characterController.radius, -1,
                                                       QueryTriggerInteraction.Ignore
                                              )
                                .Any(c => c != characterController)){ return false; }

        isCrouching = crouched;
        return true;
    }
}
