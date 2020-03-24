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
    
    [Header("Colliders parameters")]
    [Tooltip("The box collider used to detect if the player is on ground and can jump.")] [SerializeField]
    private Foots footCollider = null;
    [Header("Grappling Hook parameters")]
    [Tooltip("The origin of the grappling hook.")] [SerializeField]
    private Hook HookPrefab;
    [Tooltip("The origin of the grappling hook.")] [SerializeField]
    private Transform GrapplingOrigin;
    [Tooltip("The player's camera.")][SerializeField]
    private Camera camera;
    [Tooltip("ONLY FOR FRAME DEBUG")][SerializeField]
    private bool launchGrapplingHook;
    #endregion
    
    #region Fields ==========

    private MinerController minerController = null;
    private Vector3 newVelocity = Vector3.zero;
    private Vector3 jumpVelocity = Vector3.zero;
    private float currentSpeed = 0;
    private Rigidbody rb;
    private Hook hook;
    
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
        hook = Instantiate(HookPrefab, transform.position, transform.rotation);
        hook.origin = GrapplingOrigin;
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
       
        if (isJumping && footCollider.additionalJumps >= 0)
        {
            jumpVelocity = transform.up * (jumpForce * rb.mass);
            footCollider.additionalJumps--;
        }
        else jumpVelocity = Vector3.zero;
        return isJumping;
    }
    
    private bool grapplingHook(bool isGrappling)
    {
        bool grapplingControl = launchGrapplingHook || isGrappling; //FIXME: Will be removed
        if (previousGrappingInput == true && grapplingControl == false)//Le joueur a relaché la touche, on doit arreter le grappin
        {
            hook.state = GrapplingHookState.Retracting;
            
        }else if (previousGrappingInput == false && grapplingControl == true && hook.state != GrapplingHookState.Retracting)//Le vient d'appuyer sur le bouton, on doit déployer le grappin
        {
            RaycastHit hit;
            if(Physics.Raycast(GrapplingOrigin.position,camera.transform.forward,out hit,hook.maxDeployDistance)){
                hook.state = GrapplingHookState.Expanding;
                hook.renderer.enabled = true;
                
                hook.origin = GrapplingOrigin;
                hook.rope.origin = GrapplingOrigin;
                hook.transform.position = GrapplingOrigin.position;
                hook.target = hit.point;
                hook.player = rb;
            }
        }
        previousGrappingInput = grapplingControl;
        return grapplingControl;
    }
    
    void setVelocity()
    {
        if (hook.state != GrapplingHookState.Pulling)
        {
            rb.velocity = transform.up * rb.velocity.y + newVelocity * Time.fixedDeltaTime;
            rb.AddForce(jumpVelocity);
        }
    }

   
}
