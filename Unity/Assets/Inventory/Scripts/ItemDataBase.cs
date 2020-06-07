using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "ScriptableObjects/Inventory/ItemDatabase", order = 1)]

public class ItemDataBase :  SingletonScriptableObject<ItemDataBase>
{
    public List<Consumable> consumables; 
    public List<Artifact> artifacts;
    public List<Resource> resources;

    public ItemDataBase()
    {
        consumables = new List<Consumable>();
        artifacts   = new List<Artifact>();
        resources   = new List<Resource>();
    }
}
