using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Gauge : MonoBehaviour
{
    [Tooltip("The gun magazine this gauge ui displayed content.")] [SerializeField]
    private GunMagazine magazine = null;
    private Image gaugeUI;
    
    private void Start()
    {
        gaugeUI = GetComponent<Image>();
        gaugeUI.fillAmount = 1;
        UpdateUI();
        magazine.isConsuming += AdaptIsConsuming;
        magazine.reload += AdaptIsReloading;
    }

    private void AdaptIsConsuming(bool b, float f)
    {UpdateUI();}
    private void AdaptIsReloading(bool b, Resource r,float f)
    {UpdateUI();}

    private void UpdateUI()
    {
        gaugeUI.color = magazine.CurrentResource.Color; 
        gaugeUI.fillAmount = magazine.CurrentResourceQuantity / magazine.Capacity;
    }
    
    private void OnDestroy()
    {
        magazine.isConsuming -= AdaptIsConsuming;
        magazine.reload -= AdaptIsReloading;
    }
}