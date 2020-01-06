using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(PlayerController))]
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
    
    #endregion
    
    #region Fields ==========

    private PlayerController playerController = null;
    private Vector3 newVelocity = Vector3.zero;
    private Vector3 jumpVelocity = Vector3.zero;
    private float currentSpeed = 0;
    private Rigidbody rb;
    
    #endregion
    
    public void Awake()
    {
        playerController = GetComponent<PlayerController>();
        rb = gameObject.GetComponent<Rigidbody>();

        playerController.move += move;
        playerController.jump += isJumping => { jump(isJumping);};
        playerController.run += isRunning => { run(isRunning);};
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
        if (isJumping)
        {
            jumpVelocity = transform.up * (jumpForce * rb.mass);
        }
        else jumpVelocity = Vector3.zero;
        return isJumping;
    }
    void setVelocity()
    {
        rb.velocity = transform.up * rb.velocity.y + newVelocity * Time.fixedDeltaTime;
        rb.AddForce(jumpVelocity);
    }
}
