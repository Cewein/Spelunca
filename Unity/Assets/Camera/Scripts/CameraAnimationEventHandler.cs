using UnityEngine;
[RequireComponent(typeof(Animator))]
public class CameraAnimationEventHandler : MonoBehaviour
{
    [Header("Configuration")] [Tooltip("The camera animator.")] [SerializeField]
    private Animator animator = null;
    private MinerInputHandler inputHandler = null;
    private void Awake()
    {
        inputHandler = GetComponentInParent<MinerInputHandler>();
        animator = GetComponent<Animator>();
        animator.SetBool("isRunning", inputHandler.isRunning()); 
        animator.SetBool("isJumping", inputHandler.isJumping()); 
    }
}
