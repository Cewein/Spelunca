using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class GunController : MonoBehaviour
{
    #region SerializeFields ==========
    
    [Header("Inputs")]
    
    [Tooltip("The fire input name as it defined in Edit > Project Settings > Inputs Manager.")] [SerializeField]
    private String fireInputName = "Fire";
    
    [Tooltip("The aim input name as it defined in Edit > Project Settings > Inputs Manager.")] [SerializeField]
    private String aimInputName = "Aim";
    
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

    public event Action shoot;
    public event Action<bool> aim;

    #endregion ==========
    
    #region  Methodes ==========

    private void Awake()
    {
        muzzleFlashEffect = Instantiate(muzzleFlashEffect, muzzleFlashTransform.position, transform.rotation, transform);
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetButtonDown(fireInputName)) isShooting();
        if (Input.GetButton(aimInputName)) isAiming(true);
        else isAiming(false);

    }

    private void isShooting()
    {
        shoot?.Invoke();
        muzzleFlashEffect.Play();
        try
        {
            raycastReticle.Hit.transform.gameObject.GetComponent<IDamageable>().setDamage(raycastReticle.Hit, damageEffect);
        }
        catch (NullReferenceException e)
        {}
    }

    private void isAiming(bool isAiming)
    {
        aim?.Invoke(isAiming);
    }
    
    #endregion ==========
}
