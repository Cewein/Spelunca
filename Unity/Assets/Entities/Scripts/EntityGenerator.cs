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
    public string layerToSpawn = "Default";

    public float radius = 10f;
    public float spawnDuration = 10f;

    public bool instantSpawn = false;
    public bool onSurface = true;
    public bool progressiveSpawn = true;

    public GameObject target;

    private float spawnDistance = 0.2f;

    private EnemyComponent[] inactivePool; //the entities that are no longer available on the map
    private EnemyComponent[] fightingPool; //the entities that are attacking the player
    private EnemyComponent[] pool;         //the other entities on the map
    
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
        if (instantSpawn)
        {
            if (!progressiveSpawn)
            {
                //Debug.Log("Start spawning " + amount);
                spawn(amount);
            }
        }
        
    }

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Debug.Log("GOING TO SPAWN");
            StartSpawning();
        }
    }

    public void StartSpawning()
    {
        if (spawnedAmount == 0)
        {
            if (!progressiveSpawn)
            {
                //Debug.Log("Start spawning " + amount);
                spawn(amount);
            }
            else
            {
                float rate = 1/spawnedPerSeconds;
                //Debug.Log("spawning rate : 1 for every " + rate + " seconds");
                StartCoroutine(ProgressiveSpawning(rate));
            }
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
                        1 << LayerMask.NameToLayer(layerToSpawn)))
                    {
                        didHit = true;
                        
                        //Debug.Log("Instantiating at index " + i);
                        pool[i] = Instantiate(prefabToSpawn, hit.point + hit.normal*spawnDistance,
                            Quaternion.Euler(0, 0, 0),transform).GetComponent<EnemyComponent>();;
                        pool[i].transform.up = hit.normal;
                        pool[i].name = "Entity " + i;
                        pool[i].target = target;
                        pool[i].groundLayer = layerToSpawn;
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
