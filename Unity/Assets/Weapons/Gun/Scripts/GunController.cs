using System;
using UnityEngine;

public class GunController : MonoBehaviour
{
    #region SerializeFields ==========
    
    [Header("Inputs")]
    
    [Tooltip("The fire input name as it defined in Edit > Project Settings > Inputs Manager.")] [SerializeField]
    private String fireInputName = "Fire";
    
    [Tooltip("The aim input name as it defined in Edit > Project Settings > Inputs Manager.")] [SerializeField]
    private String aimInputName = "Aim";
    
    [Tooltip("The reload input name as it defined in Edit > Project Settings > Inputs Manager.")] [SerializeField]
    private String reloadInputName = "Reload";
    
    [Header("Effects")]
    
    [Tooltip("The default damage effect particle system")][SerializeField] 
    private ParticleSystem damageEffect = null;
    [Tooltip("The muzzle flash particle system")] [SerializeField]
    private ParticleSystem  muzzleFlashEffect = null;
    [Tooltip("The muzzle flash spawn position.")] [SerializeField]
    private Transform muzzleFlashTransform = null;
    
    [Header("Raycast")]
    
    [Tooltip("The reticle that perform raycast.")] [SerializeField]
    private Raycast raycastReticle = null;
    
    #endregion ==========
    
    #region Action ==========

    public event Action<bool> shoot;
    public event Action<bool> aim;
    public event Action<bool> reload;

    #endregion ==========
    
    #region  Methodes ==========

    private void Awake()
    {
        muzzleFlashEffect = Instantiate(muzzleFlashEffect, muzzleFlashTransform.position, transform.rotation, transform);
        Cursor.visible = false;
        shoot += isShooting => fire(isShooting);
    }

    private void Update()
    {
        isShooting(Input.GetButtonDown(fireInputName));
        isAiming(Input.GetButton(aimInputName));
        isReloading(Input.GetButtonDown(reloadInputName));
    }

    private bool isShooting(bool isShooting)
    {
        shoot?.Invoke(isShooting);
        return isShooting;
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

    private bool fire(bool isShooting)
    {
        if (isShooting && GetComponent<Animator>().GetBool("canAttack"))
        {
            muzzleFlashEffect.Play();
            try
            {
                raycastReticle.Hit.transform.gameObject.GetComponent<IDamageable>().setDamage(raycastReticle.Hit, damageEffect);
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
