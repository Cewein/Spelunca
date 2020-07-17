using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ConsumableStock", menuName = "ScriptableObjects/Consumable/ConsumableStock", order = 1)]
public class ConsumableStock : SingletonScriptableObject<ConsumableStock>
{
    [Tooltip("Max item quantity per slot.")][SerializeField]
    private int slotCapacity = 15;
    private  Dictionary<string, List<Consumable>> stock;
    public  Dictionary<string, List<Consumable>> Stock{get => stock;}

    public Action update;

    private ConsumableStock()
    {
        stock = new Dictionary<string,List<Consumable>>();
    }

    public void TakeConsumable(Consumable item)
    {
        if (!stock.ContainsKey(item.Name)) return;
        stock[item.Name].RemoveAt(0);

        if (SlotNextEmptySocket(stock[item.Name]) <= 0) stock.Remove(item.Name); 
            update?.Invoke();

    }
    
    public void SetConsumable(Consumable item)
    {
        if (!stock.ContainsKey(item.Name))
        {
            List<Consumable> slot = Enumerable.Repeat<Consumable>(null, slotCapacity).ToList();
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

        
        update?.Invoke();
    }

    public override string ToString()
    {
        return stock.Aggregate(" ------------------- Consumable Stock Debug -------------------\n",
            (current, element) => current + (element.Key + " : " + SlotNextEmptySocket(element.Value) +"\n"));
    }

    public int SlotNextEmptySocket(List<Consumable> slot)
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
