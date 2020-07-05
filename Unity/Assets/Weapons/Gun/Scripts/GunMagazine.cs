using System;
using UnityEngine;

[RequireComponent(typeof(GunController))]
public class GunMagazine : MonoBehaviour
{
    
    [Header("Settings")]
    [Tooltip("The maximum amount of resource the loader can take.")][SerializeField]
    private float capacity = 50;
    [Tooltip("Normal resource.")] [SerializeField]
    private Resource defaultResource = null;
    [HideInInspector] public Resource currentResource;
    private float currentResourceQuantity = 0;
    public bool printDebug = false;

    #region Properties ==========
  
  public Resource CurrentResource
  {
      get=> currentResource;
      set
      {
          /* Each time the current resource is changing, resource quantity in this magazine is
           * re-injected in the stock and the magazine is full with the new resource at its maximum capacity
           */
          float oldQuantity = currentResourceQuantity;
          Resource oldResource = currentResource;
          Inventory.Instance.AddResource(oldResource.Type,oldQuantity);
          currentResource = value;
          currentResourceQuantity = 0;
          isReloading(true, currentResource, Inventory.Instance.TakeResource(currentResource.Type, capacity));
      } 
  }

  public float CurrentResourceQuantity => currentResourceQuantity;
  public float Capacity => capacity;

  #endregion
  #region Actions ==========
    
    public Action<bool,Resource, float> reload;
    public Action<bool> isEmpty;
    public Action<bool, float> isConsuming;
    public Action<bool, float> upgradeCapacity;
    
  #endregion
    private void Awake()
    {
        GunController gun = GetComponent<GunController>();
        currentResource =  defaultResource;
        currentResourceQuantity = (currentResource!=null) ? Inventory.Instance.TakeResource(currentResource.Type, capacity) : 0;
        gun.reload += isGunReloading =>
        {
            if (!isGunReloading ) return;
            if (currentResourceQuantity < capacity)
            {
                isReloading(isGunReloading, currentResource, Inventory.Instance.TakeResource(currentResource.Type, capacity-currentResourceQuantity));
            }
            if (printDebug) Debug.Log(this);
        };
    }
    
    private void isReloading(bool isReloading, Resource newResource, float quantity)
    {
        if (isReloading) currentResourceQuantity += quantity;
        reload?.Invoke(isReloading, newResource, quantity);
    }
    
    private void isLoaderEmpty(bool isGaugeEmpty)
    {
        isEmpty?.Invoke(isGaugeEmpty);
    }

    public void isCurrentResourceConsuming(bool isResourceConsuming, float quantity)
    {
        if(!isResourceConsuming) return;
        currentResourceQuantity -= quantity;
        if (currentResourceQuantity <= 0) isLoaderEmpty(true);
        else if (isResourceConsuming ) currentResourceQuantity -= ((currentResourceQuantity-quantity)>0) ?quantity: currentResourceQuantity;
        isConsuming?.Invoke(isResourceConsuming, quantity);
    }

    private void isCapacityUpgrade(bool isUpgrade, float additionalSpace)
    {
        capacity += additionalSpace;
        upgradeCapacity?.Invoke(isUpgrade, additionalSpace);
    }

    public override string ToString()
    {
        string str = "--------------------- Gun Loader Debug ------------------------\n";
        str += "    - Resources type in loader : "+currentResource+"\n";
        str += "    - Resources quantity in loader : "+currentResourceQuantity+"\n";
        str += "    - Loader capacity : "+capacity+"\n";

        str += Inventory.Instance.ResourceStockToString();
        return str;  
    }
}


