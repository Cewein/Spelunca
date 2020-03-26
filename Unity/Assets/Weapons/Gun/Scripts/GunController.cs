using System;
using UnityEngine;

public class GunController : MonoBehaviour
{
    #region SerializeFields ==========

    [Header("Linked Objects")] [SerializeField]
    private MinerInputHandler inputHandler;

    [Header("Effects")]
    
    
   
    
    [Header("Raycast")]
    
    [Tooltip("The reticle that perform raycast.")] [SerializeField]
    private Raycast raycastReticle = null;

    private GunLoader magazine;
    private bool canAttack = true;
    public RaycastHit Hit { get =>  raycastReticle.Hit; }

    #endregion ==========
    
    #region Action ==========

    public event Action<bool,bool,bool> trigger;
    public event Action<bool> aim;
    public event Action<bool> reload;
    

    #endregion ==========
    
    #region  Methodes ==========

    private void Awake()
    {
        Cursor.visible = false;
        trigger += (down,held,up) => Trigger(down, held, up);
        magazine = GetComponentInChildren<GunLoader>();
    }

    private void Update()
    {
        isShooting(inputHandler.isFiringDown(),inputHandler.isFiringHeld(), inputHandler.isFiringUp());
        isAiming(inputHandler.isAiming());
        isReloading(inputHandler.isReloading());
    }

    private void isShooting(bool down, bool held, bool up)
    {
        trigger?.Invoke(down, held, up);
    }

    private void isAiming(bool isAiming)
    {
        aim?.Invoke(isAiming);
    }
    
    private bool isReloading(bool isReloading)
    {
        reload?.Invoke(isReloading);
        return isReloading;
    }

    private void Trigger(bool down, bool held, bool up)
    {
        GunArtefact artefact = GetComponentInChildren<GunArtefact>();
        if (   ((artefact.ShootMode == ShootingMode.AUTO )    && held)
            || ((artefact.ShootMode == ShootingMode.MANUAL )  && down)
            || ((artefact.ShootMode == ShootingMode.CHARGE )  && up)
            && canAttack)
        {
            try
            {
                raycastReticle.PerformRaycast();
            }
            catch (NullReferenceException e){}
        }

    }

    private void onIdle()
    {
        canAttack = true;
    }
   
    
    #endregion ==========
}
