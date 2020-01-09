using System;
using UnityEngine;

[RequireComponent(typeof(GunController))]
public class GunLoader : MonoBehaviour
{
    #region SerializeFields ==========
    
    [Header("Settings")]
    
    [Tooltip("The maximum amount of resource the loader can take.")][SerializeField]
    private float capacity = 50;
    
    [Header("Linked objects")]
    
    [Tooltip("The resource gauge prefabs.")][SerializeField]
    private Gauge gauge = null;
    
    #endregion

    #region Fields ==========

    private Resource currentResource = Resource.normal;
    private float currentResourceQuantity = 0;
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
    
    public Action<bool,Resource, float> reload;
    public Action<bool> isEmpty;
    public Action<bool, float> isConsuming;
    public Action<bool, float> upgradeCapacity;
    
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
            if (currentResourceQuantity < capacity && (currentResource != Resource.normal)) 
                isReloading(isGunReloading, currentResource, ResourcesStock.takeResource(currentResource, capacity));
            if (printDebug) Debug.Log(this);
        };

        
    }
    
    private void isReloading(bool isReloading, Resource newResource, float quantity)
    {
        reload?.Invoke(isReloading,newResource, quantity);
        if (isReloading && (currentResourceQuantity < capacity)) currentResourceQuantity += quantity;
    }
    
    private void isLoaderEmpty(bool isGaugeEmpty)
    {
        isEmpty?.Invoke(isGaugeEmpty);
        if( currentResource != Resource.normal && (ResourcesStock.Stock[currentResource] <= 0)) 
            currentResource = Resource.normal; 
        
    }

    private void isCurrentResourceConsuming(bool isResourceConsuming, float quantity)
    {
        isConsuming?.Invoke(isResourceConsuming, quantity);
        if (currentResourceQuantity <= 0) isLoaderEmpty(true);
        else if (isResourceConsuming ) currentResourceQuantity -= ((currentResourceQuantity-quantity)>0) ?quantity: currentResourceQuantity;
    }

    private void isCapacityUpgrade(bool isUpgrade, float additionalSpace)
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


