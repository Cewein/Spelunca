using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ZoneSpawner : MonoBehaviour
{
    public Pool pool;
    public float radius = 10f;
    public int amount = 1000;
    public int ratePerSpawn = 10;
    private int amountToSpawn = 0;
    private bool spawning = false;

    private void Update()
    {
        if (spawning)
        {
            int toSpawnForFrame = ratePerSpawn;
            int minId = amount - amountToSpawn;
            int maxId = minId + toSpawnForFrame;
            if (maxId > pool.poolSize) maxId = pool.poolSize;
            for (int i = minId; i < maxId; i++)
            {
                //Debug.Log("spawning " + i);
                bool spawned = false;
                
                while (!spawned)
                {
                    RaycastHit hit = new RaycastHit();
                    Vector3 dir = Random.insideUnitSphere;
                    if (Physics.Raycast(transform.position, dir, out hit, radius))
                    {
                        spawned = true;
                        Vector3[] data = new Vector3[2];
                        data[0] = hit.point;
                        data[1] = hit.normal;
                        Array values = Enum.GetValues(typeof(ResourceType));
                        System.Random random = new System.Random();
                        ResourceType randomType = (ResourceType)values.GetValue(random.Next(values.Length));
                        pool.respawn(i, data,randomType);
                        amountToSpawn -= 1;
                        //Debug.Log("respawn " + i + " at " + data[0]);
                    }
                }

            }

            if (amountToSpawn <= 0)
            {
                
                spawning = false;
            }
        }
    }

    public void BeginSpawning()
    {
        //Debug.Log("BeginSpawning");
        spawning = true;
        amountToSpawn = amount;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
