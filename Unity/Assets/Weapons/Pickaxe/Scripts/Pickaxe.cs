using System;
using UnityEngine;

public class Pickaxe : MonoBehaviour
{
    [Header("Linked element")]
    [SerializeField]
    private MinerInputHandler controller = null;

    [Tooltip("The reticle that perform raycast.")] [SerializeField]
    private Raycast raycastReticle = null;
    
    [Tooltip("Pickax damage")][SerializeField]
    private float damage = 5f;
    
    [Tooltip("Crosshair parameters")][SerializeField]
    private Crosshair crosshair;
    
    private Animator animator = null;

    public Crosshair Crosshair => crosshair;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }


    private void Update()
    {
        animator.SetBool("isPicking", controller.isFiringDown());
    }
    
    private void Pick(bool isShooting)
    {
        isShooting = controller.isFiringDown();
        if (!isShooting || !GetComponent<Animator>().GetBool("canAttack")) return;
        try
        {
            if (raycastReticle.Hit.transform != null)
            {
                       
                try
                {
                    raycastReticle.Hit.transform.gameObject.GetComponent<IPickable>().Pickax(raycastReticle.Hit, damage);
                }
                catch (NullReferenceException e){}

                try
                {
                    if (raycastReticle.Hit.transform.gameObject.GetComponent<ResourceCollectible>().pick.GetInvocationList()
                            .Length > 1)
                    {
                        raycastReticle.Hit.transform.gameObject.GetComponent<ResourceCollectible>().pick -= CollectResource;
                    }
              
                }
                catch (NullReferenceException e)
                { 
                    raycastReticle.Hit.transform.gameObject.GetComponent<ResourceCollectible>().pick += CollectResource;
                }
            }
        }
        catch (NullReferenceException e){}
    }

    private void CollectResource(ResourceType type, float quantity)
    {
        Inventory.Instance.AddResource(type,quantity);
    }
}
