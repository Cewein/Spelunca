using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private HealthPoint healthPointPrefab;
    [SerializeField] private float healthPointXOffset;

    public int maxHP;

    private int HP;

    private List<HealthPoint> points;
    private int amountToFill = 0;
    private float secondsBetweenNewHP = 0.1f;
    private bool invincible = false;
    
    // Start is called before the first frame update
    void Start()
    {
        HP = 0;
        invincible = true;
        amountToFill = 0;
        points = new List<HealthPoint>();
        for (int i = 0; i < maxHP; i++)
        {
            Vector3 position = new Vector3(transform.position.x + healthPointXOffset * i,transform.position.y);
            points.Add(Instantiate(healthPointPrefab, position, transform.rotation, transform));
            
            points[i].enable = false;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("c"))
        {
            addHP(5);
        }else if (Input.GetKeyDown("v"))
        {
            StartCoroutine(fillUpHP());
        }else if (Input.GetKeyDown("b"))
        {
            removeHP(1);
        }
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
        Debug.Log("fillUpHP");
        Debug.Log("HP : " + HP);
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
