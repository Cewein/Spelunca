using UnityEngine;

public class MinerInputHandler : MonoBehaviour
{
    
    [Header("Inputs")]
    
    [Tooltip("The run input name as it defined in Edit > Project Settings > Inputs Manager.")] [SerializeField]
    private string runInputName = "Run";
    
    [Tooltip("The move horizontally input name as it defined in Edit > Project Settings > Inputs Manager.")] [SerializeField]
    private string horizontalInputName = "Horizontal";
    
    [Tooltip("The move vertically input name as it defined in Edit > Project Settings > Inputs Manager.")] [SerializeField]
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
    
    [Tooltip("The crouch input name as it defined in Edit > Project Settings > Inputs Manager.")] [SerializeField]
    private string crouchInputName = "Cancel";
    
    [Tooltip("The switch weapon input name as it defined in Edit > Project Settings > Inputs Manager.")] [SerializeField]
    private string switchWeaponInputName = "Mouse Scrollwheel";
    
    [Tooltip("Sensitivity multiplier for moving the camera around")]
    public float lookSensitivity = 1f;
    [Tooltip("Limit to consider an input when using a trigger on a controller")]
    public float triggerAxisThreshold = 0.4f;
 

    bool m_FireInputWasHeld;
    private Vector3 move;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        m_FireInputWasHeld = GetFireInputHeld();
    }

    public bool CanProcessInput()
    {
        return Cursor.lockState == CursorLockMode.Locked;
    }

    public Vector3 GetMoveInput()
    {
        if (!CanProcessInput()) return Vector3.zero;
        move.x = Input.GetAxisRaw(horizontalInputName);
        move.y = 0f;
        move.z = Input.GetAxisRaw(verticalInputName);

        // constrain move input to a maximum magnitude of 1, otherwise diagonal movement might exceed the max move speed defined
        move = Vector3.ClampMagnitude(move, 1);

        return move;

    }

    public float GetLookInputsHorizontal()
    {
        return GetMouseOrStickLookAxis(azimuthInputName, azimuthInputName);
    }

    public float GetLookInputsVertical()
    {
        return GetMouseOrStickLookAxis(elevationInputName, elevationInputName);
    }

    public bool GetJumpInputDown()
    {
        if (CanProcessInput())
        {
            return Input.GetButtonDown(jumpInputName);
        }

        return false;
    }

    public bool GetJumpInputHeld()
    {
        if (CanProcessInput())
        {
            return Input.GetButton(jumpInputName);
        }

        return false;
    }

    public bool GetFireInputDown()
    {
        return GetFireInputHeld() && !m_FireInputWasHeld;
    }

    public bool GetFireInputReleased()
    {
        return !GetFireInputHeld() && m_FireInputWasHeld;
    }

    public bool GetFireInputHeld()
    {
        return CanProcessInput() && Input.GetButton(fireInputName);
    }

    public bool GetAimInputHeld()
    {
        return CanProcessInput() && Input.GetButton(aimInputName);
    }

    public bool GetSprintInputHeld()
    {
        return CanProcessInput() && Input.GetButton(runInputName);
    }

    public bool GetCrouchInputDown()
    {
        return CanProcessInput() && Input.GetButtonDown(crouchInputName);
    }

    public bool GetCrouchInputReleased()
    {
        return CanProcessInput() && Input.GetButtonUp(crouchInputName);
    }

    public int GetSwitchWeaponInput()
    {
        if (CanProcessInput())
        {


            if (Input.GetAxis(switchWeaponInputName) > 0f)
                return -1;
            if (Input.GetAxis(switchWeaponInputName) < 0f)
                return 1;
        }

        return 0;
    }
    
    float GetMouseOrStickLookAxis(string mouseInputName, string stickInputName)
    {
        if (CanProcessInput())
        {
            // Check if this look input is coming from the mouse
            bool isGamepad = Input.GetAxis(stickInputName) != 0f;
            float i = isGamepad ? Input.GetAxis(stickInputName) : Input.GetAxisRaw(mouseInputName);

            if (mouseInputName.Equals(elevationInputName)) i *= -1f;
            // apply sensitivity multiplier
            i *= lookSensitivity;

            if (isGamepad)
            {
                // since mouse input is already deltaTime-dependant, only scale input with frame time if it's coming from sticks
                i *= Time.deltaTime;
            }
            else
            {
                // reduce mouse input amount to be equivalent to stick movement
                i *= 0.01f;

            }

            return i;
        }

        return 0f;
    }
}
