using System;
using System.Globalization;
using UnityEngine;

[RequireComponent(typeof(GunController))]
public class GunAnimationObserver : MonoBehaviour
{
    [Header("Configuration")] [Tooltip("The gun animator.")] [SerializeField]
    private Animator animator = null;
    private GunController gunController = null;
    private void Awake()
    {
        gunController = GetComponent<GunController>();
        gunController.aim += isAiming => {animator.SetBool("isAiming", isAiming);};
    }

}
