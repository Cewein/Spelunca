using System;
using UnityEngine;

public class Gauge : MonoBehaviour
{
    [Tooltip("The resource that currently field the Gauge")][SerializeField]
    private Resource currentResource;

    [Tooltip("The maximum resource amount the gauge can take.")][SerializeField]
    private int capacity = 500;

    public Resource CurrentResource
    {
        get
        {
            if (currentResource == null) currentResource = Resource.normal;
            return currentResource;
        } 
    }

    private Action<bool,Resource> changeCurrentResource;
    private Action<bool> isEmpty;
    private Action<bool, float> isConsuming;

    private bool isCurrentResourceChanging(bool isChanging, Resource newResource)
    {
        changeCurrentResource?.Invoke(isChanging,newResource);
        return isChanging;
    }
    
    private bool isGaugeEmpty(bool isGaugeEmpty)
    {
        isEmpty?.Invoke(isGaugeEmpty);
        return isGaugeEmpty;
    }

    private bool isCurrentResourceConsuming(bool isResourceConsuming, float quantity)
    {
        isConsuming?.Invoke(isResourceConsuming, quantity);
        return isResourceConsuming;
    }

}
