using System;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(MinerController))]
public class PlayerPhysic : MonoBehaviour
{
    #region SerializeFields ==========

    [Header("Configuration")]
    [Tooltip("The player walk speed in unit per frame.")] [SerializeField]
    private float walkSpeed = 150f;
    
    [Tooltip("The player run speed in unit per frame.")][SerializeField]
    private float runSpeed = 500f;
    
    [Tooltip("The player jump force in unit per frame.")][SerializeField]
    private float jumpForce = 500f;
    
    [Tooltip("The number of jump the player can do before re-land.")][SerializeField]
    private int additionnalJump = 1;

    [Header("Grappling Hook parameters")]
    [Tooltip("The hook.")] [SerializeField]
    private Hook hook;
    [Tooltip("The origin of the grappling hook.")] [SerializeField]
    private Transform GHOrigin;
    [Tooltip("The player's camera.")][SerializeField]
    private Camera GHCamera;
    [Tooltip("The maximum distance of the grappling hook's range'.")][SerializeField]
    private float GHMaxRangeDistance = 100f;
    [Tooltip("The minimum distance of the grappling hook's acceleration.")][SerializeField]
    private float GHMinDistance = 10f;
    [Tooltip("The maximum distance of the grappling hook's acceleration.")][SerializeField]
    private float GHMaxDistance = 40f;
    [Tooltip("The base speed of the grappling's pull.")][SerializeField]
    private float GHBasePullSpeed = 2f;
    [Tooltip("The deploying speed of the grappling hook.")][SerializeField]
    private float GHDeploySpeed = 5f;
    [Tooltip("The retracting speed of the grappling hook.")][SerializeField]
    private float GHRetractSpeed = 5f;
    [Tooltip("A prefab to spawn at target's position when shooting'")][SerializeField]
    private GameObject GHDebugTargetPrefab;
    
    #endregion
    
    #region Fields ==========

    private MinerController minerController = null;
    private Vector3 newVelocity = Vector3.zero;
    private Vector3 jumpVelocity = Vector3.zero;
    private float currentSpeed = 0;
    private Rigidbody rb;
    
    //Grappling hook variables
    private bool previousGrappingInput = false;//The grappling hook's input during the previous state
    #endregion
    
    public void Awake()
    {
        minerController = GetComponent<MinerController>();
        rb = gameObject.GetComponent<Rigidbody>();

        minerController.move += move;
        minerController.jump += isJumping => { jump(isJumping);};
        minerController.grapplingHook += isGrappling => { grapplingHook(isGrappling);};
        minerController.run += isRunning => { run(isRunning);};
    }
    
    void FixedUpdate(){setVelocity();}

    private void move(float xMov, float zMov)
    {
        Vector3 moveHorizontal = transform.right * xMov;
        Vector3 moveVertical = transform.forward * zMov;
        newVelocity = (moveHorizontal + moveVertical).normalized * currentSpeed;
    }

    private bool run(bool isRunning)
    {
        currentSpeed = isRunning ? runSpeed : walkSpeed;
        return isRunning;
    }

    private bool jump(bool isJumping)
    {
        if (isJumping && additionnalJump == 0)
        {
            jumpVelocity = transform.up * (jumpForce * rb.mass);
        }
        else jumpVelocity = Vector3.zero;
        return isJumping;
    }
    
    private bool grapplingHook(bool isGrappling)
    {
        if (previousGrappingInput == true && isGrappling == false)//Le joueur a relaché la touche, on doit arreter le grappin
        {
            hook.state = GrapplingHookState.Retracting;
            
        }else if (previousGrappingInput == false && isGrappling == true)//Le vient d'appuyer sur le bouton, on doit déployer le grappin
        {
            RaycastHit hit;
            if(Physics.Raycast(GHOrigin.position,GHCamera.transform.forward,out hit,GHMaxRangeDistance)){
                Debug.Log("target found ! Reseting the hook!");
                hook.state = GrapplingHookState.Expanding;
                hook.renderer.enabled = true;
                hook.origin = GHOrigin;
                hook.transform.position = hook.origin.position;
                hook.target = hit.point;
                hook.deploySpeed = GHDeploySpeed;
                hook.retractSpeed = GHRetractSpeed;
                hook.pullSpeed = GHBasePullSpeed;
            }
        }
        previousGrappingInput = isGrappling;
        return isGrappling;
    }
    
    void setVelocity()
    {
        if (hook.state != GrapplingHookState.Pulling)
        {
            rb.velocity = transform.up * rb.velocity.y + newVelocity * Time.fixedDeltaTime;
            rb.AddForce(jumpVelocity);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        additionnalJump--;
    }

    private void OnCollisionExit(Collision collision)
    {
        additionnalJump++;
    }
}
