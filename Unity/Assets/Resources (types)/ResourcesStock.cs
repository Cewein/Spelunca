using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "ResourcesStock", menuName = "ScriptableObjects/Resources/ResourcesStock", order = 1)]

public class ResourcesStock : SingletonScriptableObject<ResourcesStock>
{
    private  Dictionary<ResourceType, float> stock;
    public  Dictionary<ResourceType, float> Stock{get => stock;}
    public int capacity = 100;
    
    private ResourcesStock()
    {
        stock = new Dictionary<ResourceType, float>();
        foreach (ResourceType resourceType in Enum.GetValues(typeof(ResourceType)))
        {
            if(resourceType != ResourceType.normal) stock.Add(resourceType, capacity );
        }
    }

    public float takeResource(ResourceType resource, float quantity)
    {
        float resourceTaken = ((stock[resource] - quantity )> 0) ? quantity  : stock[resource];
        stock[resource] -= resourceTaken;

        return resourceTaken;
    }
    
    public void setResource(ResourceType resource, float quantity)
    {
        stock[resource] += quantity;
    }

    public override string ToString()
    {
        return stock.Aggregate(" ------------------- Resources Stock Debug -------------------\n",
                              (current, element) => current + (element.Key + " : " + element.Value +"\n"));
    }  

}