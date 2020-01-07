using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region SerializeFields ==========

    [Header("Inputs")]
    
    [Tooltip("The run input name as it defined in Edit > Project Settings > Inputs Manager.")] [SerializeField]
    private String runInputName = "Run";
    
    [Tooltip("The move horizontally input name as it defined in Edit > Project Settings > Inputs Manager.")] [SerializeField]
    private String horizontalInputName = "Horizontal";
    
    [Tooltip("The move vertically input name as it defined in Edit > Project Settings > Inputs Manager.")] [SerializeField]
    private String verticalInputName = "Vertical";
    
    [Tooltip("The rotate horizontally input name as it defined in Edit > Project Settings > Inputs Manager.")] [SerializeField]
    private String azimuthInputName = "Mouse X";
    
    [Tooltip("The rotate vertically input name as it defined in Edit > Project Settings > Inputs Manager.")] [SerializeField]
    private String elevationInputName = "Mouse Y";
    
    [Tooltip("The jump input name as it defined in Edit > Project Settings > Inputs Manager.")] [SerializeField]
    private String jumpInputName = "Jump";
    
    #endregion    
    
    #region Action ==========

    public event Action<bool> run;
    public event Action<bool> jump;
    public event Action<float,float> move;
    public event Action<float,float> rotate;
    
    #endregion
    
    

    private void Update()
    {
        isMoving(Input.GetAxis(horizontalInputName),Input.GetAxis(verticalInputName));
        isRotating(Input.GetAxis(azimuthInputName),Input.GetAxis(elevationInputName));
        isRunning(Input.GetButton(runInputName));
        isJumping(Input.GetButtonDown(jumpInputName));
    }

    private bool isRunning(bool isRunning)
    {
        run?.Invoke(isRunning);
        return isRunning;
    }
    
    private bool isMoving(float x, float y)
    {
        move?.Invoke(x,y);
        return (x > 0 && y > 0);
    }
    
    private bool isJumping(bool isJumping)
    {
        jump?.Invoke(isJumping);
        return isJumping;
    }

    private bool isRotating(float x, float y)
    {
        rotate?.Invoke(x,y);
        return (x > 0 && y > 0);
    }

}
