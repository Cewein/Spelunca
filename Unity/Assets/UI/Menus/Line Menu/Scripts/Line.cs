using System;
using UnityEngine;
using UnityEngine.UI;


public class Line : MonoBehaviour
{
    [Tooltip("Image of a slot")][SerializeField] 
    private GameObject slot = null;
    [Tooltip("Number of slot visible ")][SerializeField] 
    private int displayedSlotsNumber = 5;

    private void Awake()
    {
        foreach (Consumable[] items in ConsumableStock.Instance.Stock.Values)
        {
            
        }
    }
}
