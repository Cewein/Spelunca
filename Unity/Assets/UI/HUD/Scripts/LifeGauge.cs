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
        gaugeLifeUI.color = Color.green;
        player.hurt += (damage,_,__) =>
        {
            gaugeLifeUI.fillAmount -= damage / player.MaxLife;
            gaugeLifeUI.color = Color.Lerp(Color.red,Color.green, gaugeLifeUI.fillAmount);
        };
        player.heal += (hp) =>
        {
            gaugeLifeUI.fillAmount += hp / player.MaxLife;
            gaugeLifeUI.color = Color.Lerp(Color.red,Color.green, gaugeLifeUI.fillAmount);
        }; 
    }

}
