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

    private int maxHP;
    
    private int HP = 0;
    private List<HealthPoint> points;
    private int amountToFill = 0;
    private float secondsBetweenNewHP = 0.1f;
    private bool invincible = false;
    
    // Start is called before the first frame update
  

    void Start()
    {
        if (player == null) player = FindObjectOfType<PlayerStats>();

        HP = 0;
        maxHP = player.MaxLife;
        invincible = true;
        amountToFill = 0;
        points = new List<HealthPoint>();
        for (int i = 0; i < maxHP; i++)
        {
            Vector3 position = new Vector3(transform.position.x + healthPointXOffset * i,transform.position.y);
            points.Add(Instantiate(healthPointPrefab, position, transform.rotation, transform));
            
            points[i].enable = false;
        }
        
        player.hurt += (damage,_,__) => { removeHP(damage); };
        player.heal += addHP; 
        addHP(player.Life);

    }

    void addHP(int amount)
    {
        amountToFill += amount;
        StartCoroutine(fillUpHP());
    }
    void removeHP(int amount)
    {
        int toRemove = amount;
        while (toRemove > 0)
        {
            points[HP - 1].hit = true;
            toRemove -= 1;
            HP -= 1;
        }
        
    }
    IEnumerator fillUpHP()
    {
        WaitForSeconds wait = new WaitForSeconds(secondsBetweenNewHP);
        while (amountToFill > 0)
        {
            points[HP].hit = false;
            points[HP].enable = true;
            HP += 1;
            amountToFill -= 1;
            yield return wait;
        }
        StartCoroutine(invincibleState(2f));
        StopCoroutine(fillUpHP());
    }
    
    IEnumerator invincibleState(float seconds)
    {
        invincible = true;
        yield return new WaitForSeconds(seconds);
        invincible = false;
        StopCoroutine(invincibleState(seconds));
    }
}
