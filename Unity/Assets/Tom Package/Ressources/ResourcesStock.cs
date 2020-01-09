using System.Collections.Generic;
using System;
using System.Globalization;
using System.Linq;
using UnityEngine;


public class ResourcesStock : MonoBehaviour
{
    public static ResourcesStock instance = null;


    private static Dictionary<Resource, int> stock;
    

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        
        stock = new Dictionary<Resource, int>();
        foreach (Resource resourceType in Enum.GetValues(typeof(Resource)))
        {
            stock.Add(resourceType, 54 );
        }
    }

    
    public static int takeResource(Resource resource, int quantity)
    {
        int resourceTaken = ((stock[resource] - quantity )> 0) ? quantity  : stock[resource];
        stock[resource] -= resourceTaken;

        return resourceTaken;
    }
    
    public static void setResource(Resource resource, int quantity)
    {
        stock[resource] += quantity;
    }

    public override string ToString()
    {
        return stock.Aggregate(" ------------------- Resources Stock Debug -------------------\n",
                              (current, element) => current + (element.Key + " : " + element.Value +"\n"));
    }  

}