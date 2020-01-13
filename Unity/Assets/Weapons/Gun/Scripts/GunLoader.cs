using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(GunController))]
public class GunLoader : MonoBehaviour
{
    #region SerializeFields ==========
    
    [Header("Settings")]
    
    [Tooltip("The maximum amount of resource the loader can take.")][SerializeField]
    private float capacity = 50;
    
    #endregion

    #region Fields ==========

    private Resource currentResource;
    private Resource normalResource;
    private float currentResourceQuantity = 0;
    public bool printDebug = false;

    #endregion
    
  #region Properties ==========
  
  public Resource CurrentResource
  {
      get
      {
          if (currentResource == null)
          {
              Resource[] list = Resources.FindObjectsOfTypeAll<Resource>();
              normalResource = list.First(item => item.Type == ResourceType.normal);
              currentResource = normalResource;
          }
          return currentResource;
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

        currentResourceQuantity = 1;
        gun.shoot += isShooting =>
        {
            if (!isShooting) return;
            isCurrentResourceConsuming(isShooting, 10);
            if (printDebug) Debug.Log(this);
        };
        gun.reload += isGunReloading =>
        {
            if (!isGunReloading ) return;
            if (currentResourceQuantity < capacity)
            {
                if (currentResource == normalResource)
                    isReloading(isGunReloading, currentResource, 0);
                else isReloading(isGunReloading, currentResource, ResourcesStock.Instance.takeResource(currentResource.Type, capacity));

            }
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
        if( currentResource != normalResource && (ResourcesStock.Instance.Stock[currentResource.Type] <= 0)) 
            currentResource = normalResource; 
        
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

        str += ResourcesStock.Instance;
        return str;  
    }
}


