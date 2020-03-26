using System;
using UnityEngine;

public class GunController : MonoBehaviour
{
    #region SerializeFields ==========

    [Header("Linked Objects")] [SerializeField]
    private MinerInputHandler inputHandler;
    
    /*[Header("Inputs")]
    
    [Tooltip("The Trigger input name as it defined in Edit > Project Settings > Inputs Manager.")] [SerializeField]
    private String fireInputName = "Fire";
    
    [Tooltip("The aim input name as it defined in Edit > Project Settings > Inputs Manager.")] [SerializeField]
    private String aimInputName = "Aim";
    
    [Tooltip("The reload input name as it defined in Edit > Project Settings > Inputs Manager.")] [SerializeField]
    private String reloadInputName = "Reload";*/
    
    [Header("Effects")]
    
    [Tooltip("The default damage effect particle system")][SerializeField] 
    private ParticleSystem damageEffect = null;
   
    
    [Header("Raycast")]
    
    [Tooltip("The reticle that perform raycast.")] [SerializeField]
    private Raycast raycastReticle = null;

    private GunLoader magazine;
    
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
        trigger += (down,held,up) => Trigger(down || held || up);
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

    private bool isAiming(bool isAiming)
    {
        aim?.Invoke(isAiming);
        return isAiming;
    }
    
    private bool isReloading(bool isReloading)
    {
        reload?.Invoke(isReloading);
        return isReloading;
    }

    private bool Trigger(bool isShooting)
    {
        if (isShooting && GetComponent<Animator>().GetBool("canAttack"))
        {
            try
            {
                raycastReticle.PerformRaycast();
                raycastReticle.Hit.transform.gameObject.GetComponent<IDamageable>().setDamage(raycastReticle.Hit, damageEffect, 5, magazine.CurrentResource.Type);
            }
            catch (NullReferenceException e){}
        }

        return isShooting && GetComponent<Animator>().GetBool("canAttack");
    }

    private void onIdle()
    {
        GetComponent<Animator>().SetBool("canAttack",true);
    }
   
    
    #endregion ==========
}
