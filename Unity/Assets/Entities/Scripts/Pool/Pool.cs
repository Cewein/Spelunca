using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


public interface PoolSpawner
{
    SpawnData getNewSpawnPosition();
}

public struct SpawnData
{
    public Vector3 position;
    public Vector3 direction;

    public SpawnData (Vector3 position, Vector3 direction)
    {
        this.position = position;
        this.direction = direction;
    }
}
public class Pool:MonoBehaviour
{
    private EnemyComponent[] pool; //the entities on the map
    public bool[] disabled;
    public int poolSize = 200; //the number of entities in the pool
    public PoolSpawner poolSpawner;
    public EnemyComponent enemy;
    public string layerToSpawn;
    public string name;
    public float spawnDistance;

    void Start()
    {
        pool = new EnemyComponent[poolSize];
        disabled = new bool[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            disabled[i] = true;
        }
    }

    private void Update()
    {
        for(int i = 0; i < poolSize; i++)
        {
            EnemyComponent entity = pool[i];
            if (entity != null)
            {
                if (entity.state == EnemyBehaviourState.Disabled)
                {
                    if (disabled[i] == false)
                    {
                        disableEntity(i);
                    }
                }
            }
            
        }
    }

    private void disableEntity(int index)
    {
        disabled[index] = true;
        
    }
    
    public void spawnAll()
    {
        for(int i = 0; i < poolSize; i++)
        {
            if (disabled[i] == true)
            {
                var spawnPoint = poolSpawner.getNewSpawnPosition();
                if (pool[i] == null)
                {
                    pool[i] = Instantiate(enemy, spawnPoint.position + spawnPoint.direction * spawnDistance,Quaternion.Euler(0, 0, 0));
                    pool[i].name = name + i;
                    pool[i].groundLayer = this.layerToSpawn;
                }
                else
                {
                    pool[i].transform.position = spawnPoint.position + spawnPoint.direction * spawnDistance;
                }
                pool[i].transform.up = spawnPoint.direction;
                pool[i].state = EnemyBehaviourState.Idle;
                disabled[i] = false;
                //pool[i].target = target;
            }
        }
    }
    
    /*
     
     

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
        
            foreach (var entity in pool)
            {
                if (entity.transform.position.y < -10)
                {
                    entity.enabled = false;
                }
            }
        
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
    }
    */ 
     

}
