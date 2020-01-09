using System;
using UnityEngine;

[RequireComponent(typeof(GunController))]
public class GunLoader : MonoBehaviour
{
    #region SerializeFields ==========
    
    [Header("Settings")]
    
    [Tooltip("The maximum amount of resource the loader can take.")][SerializeField]
    private int capacity = 50;
    
    [Header("Linked objects")]
    
    [Tooltip("The resource gauge prefabs.")][SerializeField]
    private Gauge gauge = null;
    
    #endregion

    #region Fields ==========

    private Resource currentResource = Resource.normal;
    private int currentResourceQuantity = 0;
    public bool printDebug = false;

    #endregion
    
  #region Properties ==========
  
  public Resource CurrentResource
  {
      get
      {
          if (currentResource == null) currentResource = Resource.normal;
          return currentResource;
      }

      
  }
  
  #endregion
    #region Actions ==========
    
    public Action<bool,Resource, int> reload;
    public Action<bool> isEmpty;
    public Action<bool, int> isConsuming;
    public Action<bool, int> upgradeCapacity;
    
    #endregion
    private void Awake()
    {
        GunController gun = GetComponent<GunController>();

        gun.shoot += isShooting =>
        {
            if (!isShooting) return;
            isCurrentResourceConsuming(isShooting, 10);
            if (printDebug) Debug.Log(this);
        };
        gun.reload += isGunReloading =>
        {
            if (!isGunReloading ) return;
            if (currentResourceQuantity < capacity) isReloading(isGunReloading, currentResource, ResourcesStock.takeResource(currentResource, capacity));
            if (printDebug) Debug.Log(this);
        };

        
    }
    
    private void isReloading(bool isReloading, Resource newResource, int quantity)
    {
        reload?.Invoke(isReloading,newResource, quantity);
        if (isReloading && (currentResourceQuantity < capacity)) currentResourceQuantity += quantity;
    }
    
    private void isGaugeEmpty(bool isGaugeEmpty)
    {
        isEmpty?.Invoke(isGaugeEmpty);
    }

    private void isCurrentResourceConsuming(bool isResourceConsuming, int quantity)
    {
        isConsuming?.Invoke(isResourceConsuming, quantity);
        if (currentResourceQuantity <= 0) isGaugeEmpty(true);
        else if (isResourceConsuming ) currentResourceQuantity -= ((currentResourceQuantity-quantity)>0) ?quantity: currentResourceQuantity;
    }

    private void isCapacityUpgrade(bool isUpgrade, int additionalSpace)
    {
        upgradeCapacity?.Invoke(isUpgrade, additionalSpace);
        capacity += additionalSpace;
    }

    public override string ToString()
    {
        string str = "--------------------- Gun Loader Debug ------------------------\n";
        str += "    - Resources type in loader : "+currentResource+"\n";
        str += "    - Resources quantity in loader : "+currentResourceQuantity+"\n";
        str += "    - Loader capacity : "+capacity+"\n";

        str += ResourcesStock.instance;
        return str;  
    }
}


