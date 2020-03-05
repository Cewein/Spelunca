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
    [Tooltip("The origin of the grappling hook.")] [SerializeField]
    private Hook HookPrefab;
    [Tooltip("The origin of the grappling hook.")] [SerializeField]
    private Transform GrapplingOrigin;
    [Tooltip("The player's camera.")][SerializeField]
    private Camera camera;
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
        hook = Instantiate(HookPrefab, transform.position, transform.rotation, transform);
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
            
        }else if (previousGrappingInput == false && isGrappling == true && hook.state != GrapplingHookState.Retracting)//Le vient d'appuyer sur le bouton, on doit déployer le grappin
        {
            Debug.Log("Attempt to throw the hook");
            RaycastHit hit;
            if(Physics.Raycast(GrapplingOrigin.position,camera.transform.forward,out hit,hook.maxDeployDistance)){
                Debug.Log("target found ! Reseting the hook!");
                hook.state = GrapplingHookState.Expanding;
                hook.renderer.enabled = true;
                
                hook.origin = GrapplingOrigin;
                hook.transform.position = GrapplingOrigin.position;
                hook.target = hit.point;
                hook.player = rb;
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
