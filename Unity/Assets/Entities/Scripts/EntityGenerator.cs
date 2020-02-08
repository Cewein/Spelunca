using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using Random = UnityEngine.Random;

public class EntityGenerator : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public int amount = 500;

    public float radius = 10f;
    public float spawnDuration = 10f;

    public bool onSurface = true;
    public bool progressiveSpawn = true;

    public GameObject target;

    private float spawnDistance = 0.2f;

    private EnemyComponent[] pool;
    private float spawnedPerSeconds;//the amount of entity to spawn for each seconds
    private int spawnedAmount; //the number of entities already spawned

    private void Awake()
    {
        pool = new EnemyComponent[this.amount];
        EnemyComponent ec = prefabToSpawn.GetComponent<EnemyComponent>();
        spawnDistance = ec.surfaceWalkingHeightOffset;
        spawnedPerSeconds = amount / spawnDuration;
        Debug.Log(amount + " / " + spawnDuration + " = " + spawnedPerSeconds);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!progressiveSpawn)
        {
            //Debug.Log("Start spawning " + amount);
            spawn(amount);
        }
        
    }

    private void Update()
    {
        if (spawnedAmount == 0)
        {
            float rate = 1/spawnedPerSeconds;
            //Debug.Log("spawning rate : 1 for every " + rate + " seconds");
            StartCoroutine(ProgressiveSpawning(rate));
        }
/*
        foreach (var entity in pool)
        {
            if (entity.transform.position.y < -10)
            {
                entity.enabled = false;
            }
        }
        */
    }

    IEnumerator ProgressiveSpawning(float rate)
    {
        WaitForSeconds wait = new WaitForSeconds(rate);
        while (spawnedAmount < amount)
        {
            //Debug.Log("spawning 1 every " + rate);
            spawn(1);
            yield return wait;
        }
    }
    /*
    private void FixedUpdate()
    {
        if (progressiveSpawn)
        {
            Debug.Log("amount - spawnedAmount = " + (amount - spawnedAmount));
            if ( amount - spawnedAmount > 0)
            {
                int toSpawn = (int) (spawnedPerSeconds * Time.fixedDeltaTime);
                print("gonna spawn " + toSpawn);
                if (toSpawn > amount - spawnedAmount)
                {
                    toSpawn = amount - spawnedAmount;
                }

                Debug.Log("FixedUpdate spawning " + toSpawn);
                spawn(toSpawn);
            };
        }
    }*/
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position,radius);
    }

    private void spawn(int amountToSpawn)
    {
        //Debug.Log("Already spawned : " + spawnedAmount + " and amount to spawn : " + amountToSpawn);
        for (int i = spawnedAmount; i < spawnedAmount+amountToSpawn; i++)
        {
            if (onSurface)
            {
                bool didHit = false;
                RaycastHit hit;
                while (!didHit)
                {
                    if (Physics.Raycast(transform.position, Random.insideUnitSphere, out hit, radius,
                        1 << LayerMask.NameToLayer("Ground")))
                    {
                        didHit = true;
                        
                        //Debug.Log("Instantiating at index " + i);
                        pool[i] = Instantiate(prefabToSpawn, hit.point + hit.normal*spawnDistance,
                            Quaternion.Euler(0, 0, 0),transform).GetComponent<EnemyComponent>();;
                        pool[i].transform.up = hit.normal;
                        pool[i].name = "Entity " + i;
                        pool[i].target = target;
                    } 
                }
            }
            else
            {
                pool[i] = Instantiate(prefabToSpawn, Random.insideUnitSphere * radius + transform.position,
                    Quaternion.Euler(0, Random.Range(0, 360), 0),transform).GetComponent<EnemyComponent>();
                pool[i].name = "Entity " + i;
                pool[i].target = target;
            }
        }
        spawnedAmount += amountToSpawn;
    }
}
