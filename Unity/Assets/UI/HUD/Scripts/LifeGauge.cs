using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class LifeGauge : MonoBehaviour
{
    [Tooltip("The player stats this gauge ui displayed content.")][SerializeField] 
    private PlayerStats player = null;
    private Image gaugeLifeUI;
    
    private void Start()
    {
        gaugeLifeUI = GetComponent<Image>();
        gaugeLifeUI.fillAmount = player.Life / player.MaxLife;
        //gaugeLifeUI.color = gunLoader.CurrentResource.Color;
        

        
        player.hurt += (damage,_,__) =>
        {
            gaugeLifeUI.fillAmount -= damage / player.MaxLife;
        };
        player.heal += (hp) =>
        {
            gaugeLifeUI.fillAmount += hp / player.MaxLife;
        }; 
    }

}
