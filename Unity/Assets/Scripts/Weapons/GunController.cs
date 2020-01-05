using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GunController : MonoBehaviour
{
    #region SerializeFields ==========
    
    [Header("Configuration")]
    [Tooltip("The fire input name as it defined in Edit > Project Settings > Inputs Manager.")] [SerializeField]
    private String fireInputName = "Fire";
    
    [Tooltip("The aim input name as it defined in Edit > Project Settings > Inputs Manager.")] [SerializeField]
    private String aimInputName = "Aim";
    
    [Tooltip("The muzzle flash particle system")] [SerializeField]
    private ParticleSystem  muzzleFlashEffect = null;

    private ParticleSystem a;
    
    [FormerlySerializedAs("muzzleFlashPosition")] [Tooltip("The muzzle flash spawn position.")] [SerializeField]
    private Transform muzzleFlashTransform = null;
    
    #endregion ==========

    #region Action ==========

    public event Action shoot;
    public event Action<bool> aim;



    #endregion ==========
    
    #region  Methodes ==========

    private void Awake()
    {
        muzzleFlashEffect = Instantiate(muzzleFlashEffect, muzzleFlashTransform.position, transform.rotation, transform);

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
    }

    private void isAiming(bool isAiming)
    {
        aim?.Invoke(isAiming);
    }
    
    #endregion ==========
}
