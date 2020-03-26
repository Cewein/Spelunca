using UnityEngine;
using System;

public class PickaxeController : MonoBehaviour
{
    [Header("Inputs")] [Tooltip("The picking input name but it's the same as fire input name")] [SerializeField]
    private MinerInputHandler inputHandler = null;
    public event Action<bool> pick;
    [Header("Effects")]
    [Tooltip("The default damage effect particle system")][SerializeField] 
    private ParticleSystem damageEffect = null;
    [Header("Raycast")]
    
    [Tooltip("The reticle that perform raycast.")] [SerializeField]
    private Raycast raycastReticle = null;

    private RaycastHit hit;
    private void Awake()
    {
        pick += isPicking => attack(isPicking);
    }

    private void Update()
    {
        isPicking(inputHandler.isFiringDown());
    }

    private bool isPicking(bool isPicking)
    {
        pick?.Invoke(isPicking);
        return isPicking;
    }
    
    private bool attack(bool isShooting)
    {
        if (isShooting && GetComponent<Animator>().GetBool("canAttack"))
        {
            try
            {
                raycastReticle.PerformRaycast();
                raycastReticle.Hit.transform.gameObject.GetComponent<IPickable>().Pickax(raycastReticle.Hit, 5);
            }
            catch (NullReferenceException e){}

            try
            {
                raycastReticle.Hit.transform.gameObject.GetComponent<ResourceCollectible>().pick += CollectResource;
            }
            catch (NullReferenceException e){}
        }

        return isShooting && GetComponent<Animator>().GetBool("canAttack");
    }

    private void CollectResource(ResourceType type, float quantity)
    {
        Debug.Log("You earn "+ quantity+ " of "+ type +" ! ");
    }


}
