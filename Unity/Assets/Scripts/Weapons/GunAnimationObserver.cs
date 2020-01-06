using UnityEngine;

[RequireComponent(typeof(GunController))]
public class GunAnimationObserver : MonoBehaviour
{
    [Header("Configuration")] [Tooltip("The gun animator.")] [SerializeField]
    private Animator animator = null;
    private GunController gunController = null;
    private PlayerController playerController = null;
    private void Awake()
    {
        gunController = GetComponent<GunController>();
        playerController = GetComponentInParent<PlayerController>();
        gunController.aim += isAiming => {animator.SetBool("isAiming", isAiming);};
        gunController.aim += _ =>
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Aim")) animator.SetBool("canAttack", true);
        };

        gunController.reload += isReloading => {animator.SetBool("isReloading",isReloading);};
        gunController.reload += _ =>
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Reload")) animator.SetBool("canAttack", false);
        };

        playerController.run += isRunning => { animator.SetBool("isRunning", isRunning); };
        playerController.run += _ =>
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Run")) animator.SetBool("canAttack", false);
        };

    }

}
