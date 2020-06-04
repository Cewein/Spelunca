using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
public class HealthBar : MonoBehaviour
{
    [Tooltip("The player stats this gauge ui displayed content.")][SerializeField] 
    private PlayerStats player = null;
    [Tooltip("Prefab used to instantiate health point.")][SerializeField]
    private HealthPoint healthPointPrefab;
    [Tooltip("Pixel between health point.")][SerializeField]
    private float healthPointXOffset;
    
    [Tooltip("Seconds between each HP appear.")][SerializeField] private float secondsBetweenNewHP = 0.1f;

    private int barHP;
    
    private List<HealthPoint> points;
    private int amountToFill = 0; //le nombre de pv restant à rajouter a la barre de HP (permet de se heal plusieurs fois même si le premier heal est pas fini)


    private RectTransform t;
    
    // Start is called before the first frame update
  

    void Start()
    {
        t = gameObject.GetComponent<RectTransform>();
        
        if (player == null) player = FindObjectOfType<PlayerStats>();

        player.Life = 0;
        player.invincible = true;
        amountToFill = 0;
        points = new List<HealthPoint>();
        for (int i = 0; i < player.MaxLife; i++)
        {
            Vector3 position = new Vector3(transform.position.x + healthPointXOffset * i,transform.position.y-t.rect.height/4);
            points.Add(Instantiate(healthPointPrefab, position, transform.rotation, transform));
            points[i].enable = false;
            points[i].hit = false;
        }
        
        //player.hurt += (damage,_,__) => { removeHP(damage); };
        //player.heal += addHP; 
        addHP(player.MaxLife);

    }

    public void addHP(int amount)
    {
        amountToFill += amount;
        StartCoroutine(fillUpHP());
    }
    public void removeHP(int amount)
    {
        
        int toRemove = amount;
        Debug.Log("hit : toRemove = "+toRemove);
        while (toRemove > 0)
        {
            if (player.Life - 1 < player.MaxLife)
            {
                points[player.Life - 1].hit = true;
            }
            toRemove -= 1;
            player.Life -= 1;
        }
        
    }
    IEnumerator fillUpHP()
    {
        WaitForSeconds wait = new WaitForSeconds(secondsBetweenNewHP);
        StartCoroutine(invincibleState(2f));
        while (amountToFill > 0)
        {
            points[player.Life].hit = false;
            points[player.Life].enable = true;
            player.Life += 1;
            amountToFill -= 1;
            yield return wait;
        }
        StopCoroutine(fillUpHP());
    }
    
    IEnumerator invincibleState(float seconds)
    {
        player.invincible = true;
        yield return new WaitForSeconds(seconds);
        player.invincible = false;
        StopCoroutine(invincibleState(seconds));
    }
}
