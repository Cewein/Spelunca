using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickaxeAnimationEventHandler: MonoBehaviour
{
    [Header("Configuration")] [Tooltip("The gun animator.")] [SerializeField]
    private Animator animator = null;
    private PickaxeController ctrl = null;

    
    
    private void Awake()
    {
        ctrl = GetComponent<PickaxeController>();
        ctrl.pick += isPicking => {animator.SetBool("isPicking", isPicking);};
    }
}
