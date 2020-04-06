using System;
using UnityEngine;

[RequireComponent(typeof(GunController))]
public class GunLoader : MonoBehaviour
{
    #region SerializeFields ==========
    
    [Header("Settings")]
    
    [Tooltip("The maximum amount of resource the loader can take.")][SerializeField]
    private float capacity = 50;
    [Tooltip("base resources")][SerializeField]
    private Resource normalResource = null;

    
    #endregion

    #region Fields ==========

    [HideInInspector] public Resource currentResource;
    private float currentResourceQuantity = 0;
    public bool printDebug = false;

    #endregion
    
  #region Properties ==========
  
  public Resource CurrentResource
  {
      get
      {
          if (currentResource != null) return currentResource;
          //Resource[] list = Resources.FindObjectsOfTypeAll<Resource>();
         // normalResource = list.First(item => item.Type == ResourceType.normal)
          currentResource = normalResource;
          return currentResource;
      }

      set
      {
          Resource old = currentResource;
          currentResource = value;
          if (currentResource.Type == ResourceType.normal)
          {
              isReloading(true, currentResource, 0);
          }
          else
          {
              if (old.Type != ResourceType.normal) ResourcesStock.Instance.setResource(old.Type,currentResourceQuantity);
              currentResourceQuantity = 0;
              isReloading(true, currentResource, ResourcesStock.Instance.takeResource(currentResource.Type, capacity));
          }
      } 
  }

  public Resource NormalResource => normalResource;

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
        gun.trigger += (down,held,up) =>
        {
            if (!(down || held || up)) return;
            isCurrentResourceConsuming(down || held || up, 10);
            if (printDebug) Debug.Log(this);
        };
        gun.reload += isGunReloading =>
        {
            if (!isGunReloading ) return;
            if (currentResourceQuantity < capacity)
            {
                if (currentResource.Type == ResourceType.normal)
                    isReloading(isGunReloading, currentResource, 0);
                else isReloading(isGunReloading, currentResource, ResourcesStock.Instance.takeResource(currentResource.Type, capacity));

            }
            if (printDebug) Debug.Log(this);
        };
    }
    
    private void isReloading(bool isReloading, Resource newResource, float quantity)
    {
        reload?.Invoke(isReloading, newResource, quantity);
        if (isReloading && (currentResourceQuantity < capacity)) currentResourceQuantity += quantity;
    }
    
    private void isLoaderEmpty(bool isGaugeEmpty)
    {
        isEmpty?.Invoke(isGaugeEmpty);
        if (currentResource.Type != ResourceType.normal && (ResourcesStock.Instance.Stock[currentResource.Type] <= 0))
        {
            isReloading(true,normalResource,0);
        }
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


