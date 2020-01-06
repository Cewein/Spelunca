using UnityEngine;
[RequireComponent(typeof(Animator))]
public class CameraAnimationEventHandler : MonoBehaviour
{
    [Header("Configuration")] [Tooltip("The camera animator.")] [SerializeField]
    private Animator animator = null;
    private PlayerController playerController = null;
    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        animator = GetComponent<Animator>();
        playerController.run += isRunning => { animator.SetBool("isRunning", isRunning); };
        playerController.jump += isJumping => { animator.SetBool("isJumping", isJumping); };
    }
}
