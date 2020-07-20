using System.Collections.Generic;
using UnityEditor;
//[CustomPropertyDrawer(typeof(Inventory.Resource_Stock))]
//public class ResourceStockDrawer : DictionaryDrawer<ResourceType, float> { }

[CustomPropertyDrawer(typeof(Inventory.Consumable_Stock))]
public class ConsumableStockDrawer : DictionaryDrawer<Consumable, int> { }


