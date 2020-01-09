using System.Collections.Generic;
using System;
using System.Globalization;
using System.Linq;
using UnityEngine;


public class ResourcesStock : MonoBehaviour
{
    public static ResourcesStock instance = null;


    private static Dictionary<Resource, float> stock;

    public static Dictionary<Resource, float> Stock{get => stock;}
    

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        
        stock = new Dictionary<Resource, float>();
        foreach (Resource resourceType in Enum.GetValues(typeof(Resource)))
        {
            if(resourceType != Resource.normal) stock.Add(resourceType, 54 );
        }
    }

    
    public static float takeResource(Resource resource, float quantity)
    {
        float resourceTaken = ((stock[resource] - quantity )> 0) ? quantity  : stock[resource];
        stock[resource] -= resourceTaken;

        return resourceTaken;
    }
    
    public static void setResource(Resource resource, float quantity)
    {
        stock[resource] += quantity;
    }

    public override string ToString()
    {
        return stock.Aggregate(" ------------------- Resources Stock Debug -------------------\n",
                              (current, element) => current + (element.Key + " : " + element.Value +"\n"));
    }  

}