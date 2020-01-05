using System;
using System.Globalization;
using UnityEngine;

[RequireComponent(typeof(GunController))]
public class GunAimingTranslation : MonoBehaviour
{
    #region SerializeFields ==========
    [Header("Configuration")]
    
    
    [Tooltip("The default gun position.")] [SerializeField]
    private Vector3 defaultPosition = new Vector3(.66f,-.3f,.614f);
    
    [Tooltip("The gun position when the player is aiming.")] [SerializeField]
    private Vector3 aimingPosition = new Vector3(.0f,-.3f, .614f);

    private GunController gunController;

    #endregion ==========
    
    #region  Methodes ==========

    private void Awake()
    {
        gunController = GetComponent<GunController>();
        gunController.aim += makeTranslation;
    }

    private void makeTranslation(bool isAiming)
    {
        if (isAiming) defaultToAiming();
        else aimingToDefault();
    }
    private void defaultToAiming()
    {
        transform.localPosition = aimingPosition;
    }

    private void aimingToDefault()
    {
        transform.localPosition = defaultPosition;
    }

    #endregion ==========

}
