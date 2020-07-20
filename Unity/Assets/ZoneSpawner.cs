using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ZoneSpawner : MonoBehaviour
{
    public Pool pool;
    public float radius = 10f;
    public float amount = 1000f;
    public float ratePerSecond = 150f;
    private float amountToSpawn = 0f;
    private bool spawning = false;

    private void Update()
    {
        if (spawning)
        {
            int toSpawnForFrame = (int)(ratePerSecond * Time.deltaTime);
            for (int i = 0; i < toSpawnForFrame; i++)
            {
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
                    }
                }

            }
            spawning = false;
        }
    }

    public void BeginSpawning()
    {
        spawning = true;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
