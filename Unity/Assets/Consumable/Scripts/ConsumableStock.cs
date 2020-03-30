using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ConsumableStock", menuName = "ScriptableObjects/Consumable/ConsumableStock", order = 1)]
public class ConsumableStock : SingletonScriptableObject<ConsumableStock>
{
    [Tooltip("Max item quantity per slot.")][SerializeField]
    private int slotCapacity = 15;
    private  Dictionary<string, Consumable[]> stock;
    public  Dictionary<string, Consumable[]> Stock{get => stock;}

    private ConsumableStock()
    {
        stock = new Dictionary<string, Consumable[]>();
    }

   /* public void TakeConsumable(Consumable item)
    {
        if (!ContainsItem(item)) return;
        item.Use();
        stock[item]--;
        if (stock[item] <= 0) stock.Remove(item);
    }*/
    
    public void SetConsumable(Consumable item)
    {
        if (!stock.ContainsKey(item.Name))
        {
            Consumable[] slot = Enumerable.Repeat<Consumable>(null, slotCapacity).ToArray();
            slot[0] = item;
            stock.Add(item.Name, slot);
        }
        else
        {
            int index = SlotNextEmptySocket(stock[item.Name]);
            if ( index < slotCapacity)
            {
                stock[item.Name][index] = item;
            }
            else
            {
                Debug.Log("you already have to lot of " + item.Name); //TODO : event here !
            }
        }
    }

    public override string ToString()
    {
        return stock.Aggregate(" ------------------- Consumable Stock Debug -------------------\n",
            (current, element) => current + (element.Key + " : " + SlotNextEmptySocket(element.Value) +"\n"));
    }

    private int SlotNextEmptySocket(Consumable[] slot)
    {
        int counter = 0;
        foreach (Consumable item in slot)
        {
            if (item != null) counter++;
            else return counter;
        }
        return counter;
    }
}
