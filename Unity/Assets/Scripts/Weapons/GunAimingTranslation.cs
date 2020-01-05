using System;
using System.Globalization;
using UnityEngine;

[RequireComponent(typeof(GunController))]
public class GunAimingTranslation : MonoBehaviour
{
    #region SerializeFields ==========

    [Header("Configuration")] [Tooltip("The gun animator.")] [SerializeField]
    private Animator animator;

    private GunController gunController;

    #endregion ==========
    
    #region  Methodes ==========

    private void Awake()
    {
        gunController = GetComponent<GunController>();
        gunController.aim += isAiming => {animator.SetBool("isAiming", isAiming);};
    }

   
   

    #endregion ==========

}
