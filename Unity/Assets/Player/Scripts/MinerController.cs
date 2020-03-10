using System;
using UnityEngine;

public abstract class MinerController : MonoBehaviour
{
   #region Action ==========

    public event Action<bool> run;
    public event Action<bool> jump;
    public event Action<bool> grapplingHook;
    public event Action<float,float> move;
    public event Action<float,float> rotate;
    
    #endregion
    
    protected bool isRunning(bool isRunning)
    {
        run?.Invoke(isRunning);
        return isRunning;
    }
    
    protected bool isMoving(float x, float y)
    {
        move?.Invoke(x,y);
        return (x > 0 && y > 0);
    }
    
    protected bool isJumping(bool isJumping)
    {
        jump?.Invoke(isJumping);
        return isJumping;
    }
    protected bool isGrappling(bool isGrappling)
    {
        grapplingHook?.Invoke(isGrappling);
        return isGrappling;
    }

    protected bool isRotating(float x, float y)
    {
        rotate?.Invoke(x,y);
        return (x > 0 && y > 0);
    }

}
