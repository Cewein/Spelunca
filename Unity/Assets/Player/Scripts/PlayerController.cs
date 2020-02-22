using System;
using UnityEngine;

public class PlayerController : MinerController
{
    #region SerializeFields ==========

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
    
    #endregion    
    
    private void Start()
    {
       
    }
    private void Update()
    {
        isMoving(Input.GetAxis(horizontalInputName),Input.GetAxis(verticalInputName));
        isRotating(Input.GetAxis(azimuthInputName),Input.GetAxis(elevationInputName));
        isRunning(Input.GetButton(runInputName));
        isJumping(Input.GetButtonDown(jumpInputName));
    }
    
}
