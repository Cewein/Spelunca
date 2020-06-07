using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "ScriptableObjects/Inventory", order = 1)]

public class Inventory : SingletonScriptableObject<Inventory>
{
  #region Fields =======================================================================================================

  [Header("Resources stock")]
  public int ResourceStockCapacity = 100;
  [SerializeField]
  private Dictionary<ResourceType, float> resourceStock;
  public Dictionary<ResourceType, float> ResourceStock => resourceStock;
  
  
  [Header("Consumables stock")]
  [Tooltip("Max item quantity per slot.")]
  [SerializeField]
  private int ConsumableSlotCapacity = 6;

  [SerializeField]
  private Dictionary<string, List<Consumable>> consumablesStock;
  public Dictionary<string, List<Consumable>> ConsumablesStock => consumablesStock;

  [Header("Artifacts stock")] 
  [SerializeField]
  private List<Artefact> artifactsStock;
  public List<Artefact> ArtifactsStock => artifactsStock;
  private int ArtifactsStockCapacity = 4;

  #endregion ===========================================================================================================

  #region Initialization ===============================================================================================
  
  public Inventory()
  {
    InitResourcesStock();
    InitConsumablesStock();
    InitArtifactsStock();
  }
  private void InitResourcesStock()
  {
    resourceStock = new Dictionary<ResourceType, float>();
    foreach (ResourceType resourceType in Enum.GetValues(typeof(ResourceType)))
    {
      if(resourceType != ResourceType.normal)
        ResourceStock.Add(resourceType, ResourceStockCapacity );
    }
  }
  private void InitConsumablesStock()
  {
    consumablesStock = new Dictionary<string,List<Consumable>>();
  }
  private void InitArtifactsStock()
  {
    artifactsStock = Enumerable.Repeat<Artefact>(null, ArtifactsStockCapacity).ToList();
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
  
  public void AddConsumable(Consumable item)
  {
    // if there is no slot assigned to this item yet.
    if (!consumablesStock.ContainsKey(item.Name))
    {
      // create a new slot filled with null item.
      List<Consumable> slot = Enumerable.Repeat<Consumable>(null, ConsumableSlotCapacity).ToList(); 
      // set the item in this slot first socket. 
      slot[0] = item;
      // add this slot to the consumables stock
      consumablesStock.Add(item.Name, slot);
    }
    // if a slot is already assigned to this item.
    else
    {
      // find the first empty socket index of the item slot.
      int index = ConsumableNextEmptySocket(consumablesStock[item.Name]);
      if ( index < ConsumableSlotCapacity) 
        // stock an item in this empty slot.
        consumablesStock[item.Name][index] = item;
      else 
        // full slot notification.
        Debug.Log("you already have to lot of " + item.Name); //TODO : event here !
    }
  }
  
  public void TakeConsumable(Consumable item)
  {
    if (!consumablesStock.ContainsKey(item.Name)) return;
    consumablesStock[item.Name].RemoveAt(0);

    if (ConsumableNextEmptySocket(consumablesStock[item.Name]) <= 0) consumablesStock.Remove(item.Name);
  }
  
  private int ConsumableNextEmptySocket(List<Consumable> slot)
  {
    int counter = 0;
    foreach (Consumable item in slot)
    {
      if (item != null) counter++;
      else return counter;
    }
    return counter;
  }
  
  public string ConsumablesStockToString()
  {
    return consumablesStock.Aggregate(" ------------------- Consumables Stock -------------------\n",
      (current, element) => current + (element.Key + " : " + ConsumableNextEmptySocket(element.Value) +"\n"));
  }

  #endregion ===========================================================================================================

  #region Artifacts ====================================================================================================
  
  public void AddArtifact(Artefact a)
  {
    // Check if the artifacts stock is full
    if(artifactsStock.Count < ArtifactsStockCapacity)
      artifactsStock.Add(a);
    else
      // send a notification to the player.
      Debug.Log("Stock is full, choose an artifact to throw away"); //TODO : event here !
  }
  
  public void TakeArtifact(Artefact a)
  { artifactsStock.Remove(a); }
  
  public string ArtifactsStockToString()
  {
    return artifactsStock.Aggregate(" ------------------- Artifacts Stock -------------------\n",
    (current, element) => current + ("- "+element.name+"\n"));
  }

  #endregion ===========================================================================================================

  public override string ToString()
  {
    return "======= INVENTORY DEBUG =======\n"
           + ResourceStockToString()
           + ConsumablesStockToString()
           + ArtifactsStockToString();
  }
}
