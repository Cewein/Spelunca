using System;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(CharacterController), typeof(MinerInputHandler))]
public class MinerController : MonoBehaviour
{
    #region SerializedFields ============================================================================================

    [Header("Linked objects")]
    [Tooltip("Player view camera, if it's null, it will be the main camera.")][SerializeField]
    private Camera playerCamera;
    [Tooltip("List of weapons prefab ( gun and pickaxe so).")][SerializeField]
    private GameObject[] weapons;
    [Tooltip("Weapon point position when player is idling.")][SerializeField]
    private Transform  weaponDefaultPosition;
    [Tooltip("Weapon point position when player is aiming.")][SerializeField]
    private Transform weaponsAimingPosition;
    [Tooltip("Transform of the empty game object that is the weapon parent.")][SerializeField]
    private Transform weaponParent;
    [Tooltip("The speed of the aiming animation.")][SerializeField]
    private float aimingAcceleration = 2f;
    
  

    [Header("Jump physics")]
    [Tooltip("Force applied upward when jumping")][SerializeField]
    private float jumpForce = 9f;
    [Tooltip("Character weight is gravity multiply by it mass. It will be applied downward when it falling.")][SerializeField]
    private float weight = 9.81f*60.0f;
    [Tooltip("Layer used to seeking ground.")][SerializeField]
    private LayerMask groundLayer = -1;
    [Tooltip("Range to seek ground (from characters foot).")][SerializeField]
    private float seekGroundScope = 0.05f;
    [Tooltip("Time before the character can jump again.")][SerializeField]
    private float jumpRecoverTime = 0.2f;

    [Header("Movement Physics")]
    [Tooltip("Max speed when the character walk on the ground.")][SerializeField]
    private float maxWalkSpeed = 10f;
    [Tooltip("Speed factor the walk speed is multiply with when the character is crouching.")][SerializeField]
    [Range(0,1)]
    private float CrouchSpeedFactor = 0.5f;
    [Tooltip("The character acceleration constant")][SerializeField]
    private float acceleration = 15;
    [Tooltip("By how much the walk speed must be multiply to make the character running.")][SerializeField]
    private float sprintFactor = 2f;
    
    [Header("Air Control Physics")]
    [Tooltip("Max speed when the character is in the air.")][SerializeField]
    private float maxSpeedInAir = 10f;
    [Tooltip("Acceleration speed when character is in the air")][SerializeField]
    private float accelerationInAir = 25f;
    [Tooltip("Range to seek ground (from characters foot) when character is in the air.")] [SerializeField]
    private float seekGroundScopeAir = 0.07f;

    [Header("Rotation")]
    [Tooltip("The camera rotation speed.")][SerializeField]
    private float rotationSpeed = 200f;
    [Range(0.1f, 1f)]
    [Tooltip("Rotation speed factor to make more precise rotation while the character is aiming")][SerializeField]
    private float aimingRotationFactor = 0.4f;
    [Tooltip("The minimum and the maximum yaw value in degree.")][SerializeField]
    private Vector2 yawAmplitude = new Vector2(-45,45);
    [Tooltip("The minimum and the maximum pitch value in degree.")][SerializeField]
    private Vector2 pitchAmplitude = new Vector2(90,-45);
    [Tooltip("If selected, clamp vertical rotation between the two angles value defined in pitch amplitude.")][SerializeField]
    private bool clampVerticalRotation = true;
    [Tooltip("If selected, clamp horizontal rotation between the two angles value defined in yaw amplitude.")][SerializeField]
    private bool clampHorizontalRotation = false;

    [Header("Stance ( standing and crouching )")]
    [Tooltip("Percentage of the character height the camera will be positioned.")][SerializeField]
    [Range(0.0f,100.0f)]
    private float cameraPosition = 90.0f;
    [Tooltip("Character standing height")][SerializeField]
    private float standingHeight = 2f;
    [Tooltip("Character crouching height")][SerializeField]
    private float crouchingHeight = 0.7f;
    [Tooltip("Crouching smooth transition speed.")][SerializeField]
    private float crouchingAcceleration = 10f;
   
    #endregion
    
    #region Other Fields ===============================================================================================
    // -- Character states
    public bool IsCrouching { get; private set; }
    public bool IsRunning { get; private set; }
    
    // -- linked objects
    private MinerInputHandler minerInputs;
    private CharacterController characterController;
    
    // -- FPS view
    private float cameraHeightRatio => cameraPosition / 100.0f;
    private Vector3 HeadPosition => transform.position + (transform.up * characterController.radius);
    private float pitch = 0f;
    private float yaw = 0f;
    private float newHeight;
    
    // -- Physics and jump
    private float speedFactor;
    private Vector3 normal;
    private Vector3 velocity;
    private Vector3 newVelocity;
    private bool hasAlreadyJump;
    private float lastTimeJumped = 0f;
    
    // -- Ground and objects detection
    private bool isOnGround;
    private RaycastHit hit;
    
    // -- Weapon
    private int weaponIndex = 0;
    private Vector3 weaponNewPosition; 
    
    #endregion

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        minerInputs = GetComponent<MinerInputHandler>();
        characterController.enableOverlapRecovery = true;
        if (playerCamera == null) playerCamera = Camera.main;
        ForceStanding();

        foreach (GameObject prefab in weapons)
        {
            Instantiate(prefab, weaponParent);
        }
        switcher();
        
    }
    
   

    // to debug the height at the beginning of a scene.
    private void ForceStanding()
    {
        Crouch(false);
        characterController.height = newHeight;
        characterController.center = Vector3.up * characterController.height * 0.5f;
        playerCamera.transform.localPosition = Vector3.up * newHeight * cameraHeightRatio;
    }
    
    private void Update()
    {
        hasAlreadyJump = false;
        SeekGround();
        if (minerInputs.isCrouching()){Crouch(!IsCrouching);}
        SetHeightSmoothly();
        Rotate();
        Move();
        Jump();
    }

    private void LateUpdate()
    {
        SwitchWeapon();
        Aim();
    }

    private void Aim()
    {
        
        weaponNewPosition = minerInputs.isAiming() ?
                                                 weaponsAimingPosition.position : weaponDefaultPosition.position;
        weaponParent.position = Vector3.Slerp(weaponParent.position, weaponNewPosition, aimingAcceleration * Time.deltaTime);
    }

    private void SwitchWeapon()
    {
        weaponIndex = Mathf.Abs( (weaponIndex - minerInputs.isSwitchingWeapon() ) % weapons.Length );
        switcher();
    }
    
    private void switcher()
    {
        weapons[ Mathf.Abs( (weaponIndex - 1) % weapons.Length ) ].gameObject.SetActive(false);
        weapons[weaponIndex].gameObject.SetActive(true);
    }
    private void SeekGround()
    {
        float scope = isOnGround ? (characterController.skinWidth + seekGroundScope) : seekGroundScopeAir;
        isOnGround = false;
        normal = Vector3.up;

        if (Time.time >= lastTimeJumped + jumpRecoverTime)
        {
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
            hasAlreadyJump = true;
            isOnGround = false;
            //  normal = Vector3.up;
        }
    }

    private void AirControl()
    {
        velocity +=  transform.TransformVector(minerInputs.Movement) * accelerationInAir * Time.deltaTime;
        Vector3 horizontalVelocity = Vector3.ProjectOnPlane(velocity, Vector3.up);
        horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, maxSpeedInAir * speedFactor);
        velocity = horizontalVelocity + (Vector3.up * velocity.y);
        // apply the weight to the velocity
        velocity += Vector3.down * weight * Time.deltaTime;
    }

    private void Run()
    {
        IsRunning = minerInputs.isRunning();
        if (IsRunning) { IsRunning = Crouch(false); }
        speedFactor = IsRunning ? sprintFactor : 1f;
    }
    
    void Move()
    {
        Run();
        if (isOnGround)
        {
            newVelocity =  transform.TransformVector(minerInputs.Movement) * maxWalkSpeed * speedFactor;
            if (IsCrouching){ newVelocity *= CrouchSpeedFactor; }
            newVelocity = SlideOnSlope(newVelocity.normalized, normal) * newVelocity.magnitude;
            velocity = Vector3.Lerp(velocity, newVelocity, acceleration * Time.deltaTime);
        }
        else{AirControl();}

        Vector3 previousHeadPosition = HeadPosition;
        Vector3 previousFootsPosition = FootsPositionAtHeight(characterController.height);
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
    

    private Vector3 FootsPositionAtHeight(float height)
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
        characterController.height = Mathf.Lerp(characterController.height, newHeight, crouchingAcceleration * Time.deltaTime);
        characterController.center = Vector3.up * characterController.height * 0.5f;
        playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, Vector3.up * newHeight * cameraHeightRatio, crouchingAcceleration * Time.deltaTime);
    }

    bool Crouch(bool crouched)
    {
        newHeight = crouched ? crouchingHeight : standingHeight;
        // Check for obstacles
        if (!crouched && Physics.OverlapCapsule(HeadPosition, 
                                                FootsPositionAtHeight(standingHeight),
                                                       characterController.radius, -1,
                                                       QueryTriggerInteraction.Ignore
                                              )
                                .Any(c => c != characterController)){ return false; }

        IsCrouching = crouched;
        return true;
    }
}
