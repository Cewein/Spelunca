using System.Collections.Generic;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "ResourcesStock", menuName = "ScriptableObjects/Resources/ResourcesStock", order = 1)]

public class ResourcesStock : ScriptableSingleton<ResourcesStock>
{
    private  Dictionary<Resource, float> stock; 
    public  Dictionary<Resource, float> Stock{get => stock;}
    

    private void Awake()
    {
        stock = new Dictionary<Resource, float>();
        foreach (Resource resourceType in Enum.GetValues(typeof(Resource)))
        {
            if(resourceType != Resource.normal) stock.Add(resourceType, 54 );
        }
    }

    
    public float takeResource(Resource resource, float quantity)
    {
        float resourceTaken = ((stock[resource] - quantity )> 0) ? quantity  : stock[resource];
        stock[resource] -= resourceTaken;

        return resourceTaken;
    }
    
    public void setResource(Resource resource, float quantity)
    {
        stock[resource] += quantity;
    }

    public override string ToString()
    {
        return stock.Aggregate(" ------------------- Resources Stock Debug -------------------\n",
                              (current, element) => current + (element.Key + " : " + element.Value +"\n"));
    }  

}