using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Gauge : MonoBehaviour
{
    [Tooltip("The gun loader this gauge ui displayed content.")][SerializeField] 
    private GunLoader gunLoader = null;
    
    private Resource currentResource;
    private Image gaugeUI;
    

    private void Awake()
    {
        gaugeUI = GetComponent<Image>();
        gaugeUI.fillAmount = gunLoader.CurrentResourceQuantity / gunLoader.Capacity;
        currentResource = gunLoader.CurrentResource;
        gaugeUI.color = gunLoader.CurrentResource.Color;
        
        if (currentResource.Type == ResourceType.normal) setGaugeToNormalResource();

        
        gunLoader.isConsuming += (consuming, quantity) =>
        {
            if (currentResource.Type == ResourceType.normal) setGaugeToNormalResource();
            else gaugeUI.fillAmount -= quantity / gunLoader.Capacity;
        };
        gunLoader.reload += (reloading, resource, quantity) =>
        {
           /* if (resource.Type != currentResource.Type)
            {*/
                currentResource = resource;
                gaugeUI.color = currentResource.Color;
            /*}*/

            if (currentResource.Type == ResourceType.normal) setGaugeToNormalResource();
            else gaugeUI.fillAmount += quantity / gunLoader.Capacity;
        }; 
    }

    private void setGaugeToNormalResource()
    {
        gaugeUI.color = currentResource.Color;
        gaugeUI.fillAmount = 1;
        
    }

}