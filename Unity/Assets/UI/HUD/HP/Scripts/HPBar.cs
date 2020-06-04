using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;
    public float pixelPerHP;
    [SerializeField] private RectTransform healthBar;
    [SerializeField] private Image healthBarContent;
    [SerializeField] private float speed;


    // Update is called once per frame
    void FixedUpdate()
    {
        float currentSizeX = healthBar.rect.width;
        float requiredSizeX = pixelPerHP * stats.MaxLife;
        float newSizeDeltaX = Mathf.Lerp(currentSizeX, requiredSizeX, speed * Time.fixedDeltaTime);

        float currentMaxLife = currentSizeX / pixelPerHP;
        float newMaxLife = Mathf.Lerp(currentMaxLife, stats.MaxLife, speed * Time.fixedDeltaTime); //pour empecher la barre de pv de bouger quand on gagne des pv max
        float maxLife = newMaxLife > 0 ? newMaxLife : 0.000001f;//on empeche la division par 0
        float fill = stats.Life / maxLife;
        healthBarContent.fillAmount = fill <=1 ? fill : 1;//on empeche de dépasser 1 si on a plus de pv que de maxpv
        
        healthBar.sizeDelta = new Vector2(newSizeDeltaX,healthBar.sizeDelta.y);
    }
}
