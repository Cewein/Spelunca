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
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!progressiveSpawn)
        {
            spawn(amount);
        }
        
    }
    
    private void Update()
    {
        if (progressiveSpawn)
        {
            StartCoroutine(ProgressiveSpawn());
        }
    }

    IEnumerator ProgressiveSpawn()
    {
        if (progressiveSpawn)
        {
            WaitForSeconds wait = new  WaitForSeconds(1 / spawnedPerSeconds);
            while (amount - spawnedAmount > 0)
            {
                Debug.Log("left to spawn : " + (amount - spawnedAmount));
                spawn(1);
                yield return wait;
            }
            
        }else{
            Debug.LogError("Inconsistency in code : progressiveSpawn called while progressiveSpawn = false");
        }
    }
/*
 * if ( amount - spawnedAmount > 0)
            {
                int toSpawn = (int) (spawnedPerSeconds * Time.deltaTime);
                print("gonna spawn " + toSpawn);
                if (toSpawn > amount - spawnedAmount)
                {
                    toSpawn = amount - spawnedAmount;
                }
            }
 */
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position,radius);
    }

    private void spawn(int amount)
    {
        for (int i = spawnedAmount; i < amount; i++)
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
        spawnedAmount += amount;
    }
}
