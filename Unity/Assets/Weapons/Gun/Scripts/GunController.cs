using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

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

    [Header("Animations")] [SerializeField]
    private Transform foreEndTransform = null;
    [SerializeField] private Transform maxPulledPosition = null;
    [SerializeField] private Transform minPullPosition = null;

    [SerializeField] private float reloadAnimationTime = 50;
    private bool triggerReloadAnimation;
    private float reloadTimer;
    private bool canAttack = true;
    public RaycastHit Hit { get =>  raycastReticle.Hit; }

    public bool TriggerReloadAnimation{get => triggerReloadAnimation;}

    #endregion ==========
    
    #region Action ==========

    public event Action<bool,bool,bool> trigger;
    public event Action<bool> aim;
    public event Action<bool> reload;
    

    #endregion ==========
    
    #region  Methodes ==========

    private void Start()
    {
        if (!IA)
        {
            Cursor.visible = false;
            trigger += Trigger;
            inputHandler = GetComponentInParent<MinerInputHandler>();
            iaInputHandler = null;
        }
        else
        {
            trigger += Trigger;
            iaInputHandler = GetComponentInParent<IAInputHandler>();
            inputHandler = null;
        }

        reload += trigger =>
        {
            if (trigger)
            {
                reloadTimer = reloadAnimationTime;
                triggerReloadAnimation = true;
            }
        };
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

        if(triggerReloadAnimation) ReloadAnimation();
    }

    private void isShooting(bool down, bool held, bool up)
    {
        trigger?.Invoke(down, held, up);
    }

    private void isAiming(bool isAiming)
    {
        aim?.Invoke(isAiming);
    }
    
    public bool isReloading(bool isReloading)
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

    private void ReloadAnimation()
    {
        if (triggerReloadAnimation && reloadTimer > reloadAnimationTime/2)
        {
            foreEndTransform.position = Vector3.Lerp(foreEndTransform.position , maxPulledPosition.position, (reloadAnimationTime - reloadTimer)/(reloadAnimationTime/2));
            reloadTimer--;

        }
        else if (triggerReloadAnimation && reloadTimer <= reloadAnimationTime/2)
        {
            foreEndTransform.position = Vector3.Slerp(foreEndTransform.position , minPullPosition.position, ((reloadAnimationTime/2) - reloadTimer)/(reloadAnimationTime/2));
            if (triggerReloadAnimation && reloadTimer < 0) triggerReloadAnimation = false;
            reloadTimer--;
        }
        

    }
    
    #endregion ==========
}
