using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "ScriptableObjects/Inventory/Inventory", order = 1)]
public class Inventory : SingletonScriptableObject<Inventory>
{
  [Serializable] public class Resource_Stock : SerializableDictionary<ResourceType, float>{}
  [Serializable] public class Consumable_Stock  : SerializableDictionary<Consumable, int>{}
  
  #region Fields =======================================================================================================

  [Header("Resources stock")]
  public int ResourceStockCapacity = 100;
  [SerializeField]
  private Resource_Stock resourceStock;
  public Resource_Stock ResourceStock => resourceStock;
  
  
  [Header("Consumables stock")]
  [Tooltip("Max item quantity per slot.")]
  [SerializeField]
  private int ConsumableSlotCapacity = 6;

  [SerializeField]
  private Consumable_Stock consumableStock;
  public Consumable_Stock ConsumableStock => consumableStock;

  [Header("Artifacts stock")] 
  [SerializeField]
  private List<Artifact> artifactStock;
  public List<Artifact> ArtifactStock => artifactStock;
  private int ArtifactStockCapacity = 4;

  #endregion ===========================================================================================================

  #region Initialization ===============================================================================================
  
  public Inventory()
  {
    resourceStock = new Resource_Stock();
    consumableStock = new Consumable_Stock();
    artifactStock = new List<Artifact>();

  }

  private void OnEnable()
  {
    InitResourcesStock();
    InitConsumablesStock();
    InitArtifactsStock();
  }

  private void InitResourcesStock()
  {
    resourceStock = new Resource_Stock();
    foreach (ResourceType resourceType in Enum.GetValues(typeof(ResourceType)))
      if(resourceType != ResourceType.normal)
        ResourceStock.Add(resourceType, ResourceStockCapacity );
  }
  private void InitConsumablesStock()
  {
    consumableStock = new Consumable_Stock();
    foreach (Consumable c in ItemDataBase.Instance.consumables) 
      ConsumableStock.Add(c, 0 );
  }
  private void InitArtifactsStock()
  {
    artifactStock = Enumerable.Repeat<Artifact>(null, ArtifactStockCapacity).ToList();
  }

  #endregion ===========================================================================================================

  #region Resources ====================================================================================================

  public float TakeResource(ResourceType resource, float quantity)
  {
    float resourceTaken = ((resourceStock[resource] - quantity )> 0) ? quantity  : resourceStock[resource];
    resourceStock[resource] -= resourceTaken;
    return resourceTaken;
  }
  
  public void AddResource(ResourceType resource, float quantity)
  {
    float sum = resourceStock[resource] + quantity;
    resourceStock[resource] = (sum < ResourceStockCapacity)? sum : ResourceStockCapacity;
  }
  
  public string ResourceStockToString()
  {
    return resourceStock.Aggregate(" ------------------- Resources Stock -------------------\n",
      (current, element) => current + (element.Key + " : " + element.Value +"\n"));
  }  
  
  #endregion ===========================================================================================================

  #region Consumables ====================================================================================================

  public void NotifyConsomableStockUpdate()
  {
    updateConsomableStock?.Invoke();
  }
  public void AddConsumable(Consumable item)
  {
     // Check if the item slot is full
     if ( consumableStock[item] < ConsumableSlotCapacity) consumableStock[item]++;
     // full slot notification.
     else Debug.Log("you already have to lot of " + item.Name); //TODO : event here !
  }
  
  public void TakeConsumable(Consumable item)
  {
    if ( consumableStock[item] > 0) consumableStock[item]--;
  }

  public string ConsumablesStockToString()
  {
    return consumableStock.Aggregate(" ------------------- Consumables Stock -------------------\n",
      (current, element) => current + (element.Key + " : " + element.Value +"\n"));
  }

  #endregion ===========================================================================================================

  #region Artifacts ====================================================================================================
  
  public void AddArtifact(Artifact a)
  {
    // Check if the artifacts stock is full
    if(artifactStock.Count < ArtifactStockCapacity)
      artifactStock.Add(a);
    else
      // send a notification to the player.
      Debug.Log("Stock is full, choose an artifact to throw away"); //TODO : event here !
  }
  
  public void TakeArtifact(Artifact a)
  { artifactStock.Remove(a); }
  
  public string ArtifactsStockToString()
  {
    return artifactStock.Aggregate(" ------------------- Artifacts Stock -------------------\n",
    (current, element) => current + ("- "+element.name+"\n"));
  }

  #endregion ===========================================================================================================

  #region Notification =================================================================================================
  
  /*
   * Used in :
   *  - Line Menu
   */
  public Action updateConsomableStock;
  

  #endregion ===========================================================================================================
  public override string ToString()
  {
    return "======= INVENTORY DEBUG =======\n"
           + ResourceStockToString()
           + ConsumablesStockToString()
           + ArtifactsStockToString();
  }
}
