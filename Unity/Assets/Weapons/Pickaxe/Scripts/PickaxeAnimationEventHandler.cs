using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickaxeAnimationEventHandler: MonoBehaviour
{
    [Header("Configuration")] [Tooltip("The gun animator.")] [SerializeField]
    private Animator animator = null;
    private GunController ctrl = null;

    
    
    private void Awake()
    {
        ctrl = GetComponent<GunController>();
        ctrl.trigger += (d,h,u) => {animator.SetBool("isPicking", d||h||u);};
    }
}
