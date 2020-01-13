using UnityEngine;
[RequireComponent(typeof(Animator))]
public class CameraAnimationEventHandler : MonoBehaviour
{
    [Header("Configuration")] [Tooltip("The camera animator.")] [SerializeField]
    private Animator animator = null;
    private MinerController minerController = null;
    private void Awake()
    {
        minerController = GetComponentInParent<MinerController>();
        animator = GetComponent<Animator>();
        minerController.run += isRunning => { animator.SetBool("isRunning", isRunning); };
        minerController.jump += isJumping => { animator.SetBool("isJumping", isJumping); };
    }
}
