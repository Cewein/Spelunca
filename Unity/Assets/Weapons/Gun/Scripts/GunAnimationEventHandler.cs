using UnityEngine;

[RequireComponent(typeof(GunController))]
public class GunAnimationEventHandler : MonoBehaviour
{
    [Header("Configuration")] [Tooltip("The gun animator.")] [SerializeField]
    private Animator animator = null;
    private GunController gunController = null;
    private MinerController minerController = null;
    private void Awake()
    {
        gunController = GetComponent<GunController>();
        minerController = GetComponentInParent<MinerController>();
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

        minerController.run += isRunning => { animator.SetBool("isRunning", isRunning); };
        minerController.run += _ =>
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Run")) animator.SetBool("canAttack", false);
        };

    }

}
