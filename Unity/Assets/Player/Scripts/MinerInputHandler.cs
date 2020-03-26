using UnityEngine;
using UnityEngine.Serialization;

public class MinerInputHandler : MonoBehaviour
{
    #region SerializedField ============================================================================================

    [Header("Inputs")]
    
    [Tooltip("The run input name as it defined in Edit > Project Settings > Inputs Manager.")] [SerializeField]
    private string runInputName = "Run";
    
    [Tooltip("The movement horizontally input name as it defined in Edit > Project Settings > Inputs Manager.")] [SerializeField]
    private string horizontalInputName = "Horizontal";
    
    [Tooltip("The movement vertically input name as it defined in Edit > Project Settings > Inputs Manager.")] [SerializeField]
    private string verticalInputName = "Vertical";
    
    [Tooltip("The rotate horizontally input name as it defined in Edit > Project Settings > Inputs Manager.")] [SerializeField]
    private string azimuthInputName = "Mouse X";
    
    [Tooltip("The rotate vertically input name as it defined in Edit > Project Settings > Inputs Manager.")] [SerializeField]
    private string elevationInputName = "Mouse Y";
    
    [Tooltip("The jump input name as it defined in Edit > Project Settings > Inputs Manager.")] [SerializeField]
    private string jumpInputName = "Jump";
    
    [Tooltip("The grapping hook input name as it defined in Edit > Project Settings > Inputs Manager.")] [SerializeField]
    private string grapplingInputName = "Grappling";
    
    [Tooltip("The fire input name as it defined in Edit > Project Settings > Inputs Manager.")] [SerializeField]
    private string fireInputName = "Fire";
    
    [Tooltip("The aim input name as it defined in Edit > Project Settings > Inputs Manager.")] [SerializeField]
    private string aimInputName = "Aim";
    
    [Tooltip("The reload input name as it defined in Edit > Project Settings > Inputs Manager.")] [SerializeField]
    private string reloadInputName = "Reload";
    
    [Tooltip("The crouch input name as it defined in Edit > Project Settings > Inputs Manager.")] [SerializeField]
    private string crouchInputName = "Cancel";
    
    [Tooltip("The switch weapon input name as it defined in Edit > Project Settings > Inputs Manager.")] [SerializeField]
    private string switchWeaponInputName = "Mouse ScrollWheel";
    
    [Header("Parameters")]
    
    [Tooltip("Sensitivity multiplier for moving the camera around")][SerializeField]
    [Range(1,100)]
    private float cameraSensitivity = 1f;
 
    #endregion
    
    #region Other fields ===============================================================================================
    
    private Vector3 movement;
    
    /// <summary>
    /// The miner movement quantity vector.
    /// </summary>
    public Vector3 Movement
    {
        get
        {
            if (!GetIfPlayerCanPlay()) movement.x =  movement.y = movement.z = 0f;
            else
            {
                movement.x = Input.GetAxisRaw(horizontalInputName);
                movement.y = 0f;
                movement.z = Input.GetAxisRaw(verticalInputName);
                // Avoid higher speed on diagonal movement
                movement = Vector3.ClampMagnitude(movement, 1);
            }
            return movement;
        }
    }
    
    public float Azimuth
    {
        get
        {
            if (GetIfPlayerCanPlay())
            {
                // NOTE : No need to multiply by delta by time because in Unity
                // mouse input is already calculate in function of delta time.
                return Input.GetAxis(azimuthInputName) * (cameraSensitivityAdjustement*cameraSensitivity)/100f;
            }

            return 0f;
        }
       
    }
    
    public float Elevation
    {
        get
        {
            if (GetIfPlayerCanPlay())
            {
                // NOTE : No need to multiply by delta by time because in Unity
                // mouse input is already calculate in function of delta time.
                return -Input.GetAxis(elevationInputName) *(cameraSensitivityAdjustement*cameraSensitivity)/100f;
            }

            return 0f;
        }
       
    }
    
    private float cameraSensitivityAdjustement = 0.03f;

    #endregion
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    /// <summary>
    /// Return if player inputs can be used. It will be useful to stop the game while menu UI are
    /// displayed or to paused the game.
    /// </summary>
    /// <returns>a boolean that indicate if player inputs can process.</returns>
    private bool GetIfPlayerCanPlay()
    {
        return Cursor.lockState == CursorLockMode.Locked;
    }
    public bool isJumping()
    {
        return GetIfPlayerCanPlay() && Input.GetButton(jumpInputName);
    }

    public bool isFiringDown()
    {
        return GetIfPlayerCanPlay() && Input.GetButtonDown(fireInputName);
    }

    public bool isFiringHeld()
    {
        return GetIfPlayerCanPlay() && Input.GetButton(fireInputName);
    }

    public bool isFiringUp()
    {
        return GetIfPlayerCanPlay() && Input.GetButtonUp(fireInputName);
    }

    public bool isAiming(bool perform)
    {
        return perform && GetIfPlayerCanPlay() && Input.GetButton(aimInputName);
    }

    public bool isReloading()
    {
        return GetIfPlayerCanPlay() && Input.GetButton(reloadInputName);
    }

    public bool isRunning()
    {
        return GetIfPlayerCanPlay() && Input.GetButton(runInputName);
    }

    // NOTE : crouch = s'accroupir
    public bool isCrouching()
    {
        return GetIfPlayerCanPlay() && Input.GetButtonDown(crouchInputName);
    }

    public int isSwitchingWeapon()
    {
        if (!GetIfPlayerCanPlay()) return 0;
        if (Input.GetAxis(switchWeaponInputName) > 0f)
            return -1;
        if (Input.GetAxis(switchWeaponInputName) < 0f)
            return 1;
        return 0;
    }
    
 
}
