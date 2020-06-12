using System;
using UnityEngine.UI;
using System.Linq;
using UnityEngine;

public class LineMenu : MonoBehaviour
{
    [SerializeField] private Transform mainSlot;
    [SerializeField] private Transform leftSlot;
    [SerializeField] private Transform rightSlot;

    private void Start()
    {
        Inventory.Instance.selectConsumable += Refresh;
        Refresh();
    }

    private void Refresh()
    {
        try
        {
            if(Inventory.Instance.ConsumableStock.ElementAt(Inventory.Instance.IndexConsumable).Value>0)
                Set(mainSlot, Inventory.Instance.ConsumableStock.ElementAt(Inventory.Instance.IndexConsumable).Key.Icon);
            else Empty(mainSlot);
        }
        catch(ArgumentOutOfRangeException e){{Empty(mainSlot);}}

        try
        {
            if(Inventory.Instance.ConsumableStock.ElementAt(Inventory.Instance.IndexConsumable+1).Value>0)
                Set(rightSlot,Inventory.Instance.ConsumableStock.ElementAt(Inventory.Instance.IndexConsumable+1).Key.Icon);
            else Empty(rightSlot);
        }
        catch(ArgumentOutOfRangeException e){{Empty(rightSlot);}}

        try
        {
            if(Inventory.Instance.ConsumableStock.ElementAt(Inventory.Instance.IndexConsumable-1).Value>0)
                Set(leftSlot,Inventory.Instance.ConsumableStock.ElementAt(Inventory.Instance.IndexConsumable-1).Key.Icon);
            else Empty(leftSlot);
        }
        catch(ArgumentOutOfRangeException e){{Empty(leftSlot);}}
        
    }

    private void Empty(Transform slot)
    {
        slot.GetComponent<Image>().sprite = null;
        slot.GetComponent<Image>().enabled  = false;
    }

    private void Set(Transform slot, Sprite icon)
    {
        slot.GetComponent<Image>().sprite = icon;
        slot.GetComponent<Image>().enabled  = true;
    }
}
