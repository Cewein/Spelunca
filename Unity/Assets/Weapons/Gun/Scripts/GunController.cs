using System;
using UnityEngine;

public class GunController : MonoBehaviour
{
    #region SerializeFields ==========

    [Header("Linked Objects")] [SerializeField]
    private MinerInputHandler inputHandler;

    public bool IA = false;
    public IAInputHandler iaInputHandler;
    [Header("Raycast")]
    [Tooltip("The reticle that perform raycast.")] [SerializeField]
    private Raycast raycastReticle = null; // TODO : le récupérer boudiouuuuu !!!! mais ça va changer la gestion des reticles de la HUD alors je fait après

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
        if (!IA)
        {
            Cursor.visible = false;
            trigger += Trigger;
            magazine = GetComponentInChildren<GunLoader>();
            inputHandler = GetComponentInParent<MinerInputHandler>();
        }
        else
        {
            trigger += Trigger;
            magazine = GetComponentInChildren<GunLoader>();
            iaInputHandler = GetComponentInParent<IAInputHandler>();
        }
       
    }

    private void Update()
    {
        if (!IA)
        {
            isShooting(inputHandler.isFiringDown(),inputHandler.isFiringHeld(), inputHandler.isFiringUp());
            isAiming(inputHandler.isAiming(true));
            isReloading(inputHandler.isReloading());
        }
        else
        {
            isShooting(iaInputHandler.isFiringDown(),iaInputHandler.isFiringHeld(), iaInputHandler.isFiringUp());
            isAiming(iaInputHandler.isAiming(true));
            isReloading(iaInputHandler.isReloading());
        }
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
        if (((artefact.ShootMode != ShootingMode.AUTO) || !held) &&
            ((artefact.ShootMode != ShootingMode.MANUAL) || !down) &&
            (((artefact.ShootMode != ShootingMode.CHARGE) || !up) || !canAttack)) return;
        try { raycastReticle.PerformRaycast(); }
        catch (NullReferenceException e){}

    }

    private void onIdle()
    {
        canAttack = true;
    }
   
    
    #endregion ==========
}
