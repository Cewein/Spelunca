using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Gauge : MonoBehaviour
{
    [SerializeField] private GunLoader gunLoader = null;
    [SerializeField] private float emptyFillAmount = .14f;
  

    private Image gaugeUI;

    private void Awake()
    {
        gaugeUI = GetComponent<Image>();
        gaugeUI.fillAmount = emptyFillAmount;
        gunLoader.isConsuming += (consuming, quantity) => { gaugeUI.fillAmount -= quantity / gunLoader.Capacity; };
        gunLoader.reload += (reloading, resourceType, quantity) =>
        {
            // if (resourceType != currentResourceType) gaugeUI.Color = resourceType.GaugeColor
            gaugeUI.fillAmount += quantity / gunLoader.Capacity;
        }; 
    }

    
}