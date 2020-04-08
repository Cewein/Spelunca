using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EntityGenerator : MonoBehaviour, PoolSpawner
{
    public GameObject prefabToSpawn;
    public string layerToSpawn = "Default";
    public int    amount = 500;
    public float  radius = 10f;
    public float  spawnDuration = 10f;
    public GameObject player;

    private float spawnDistance = 0.2f;
    private Pool pool;         //the other entities on the map
    private float spawnedPerSeconds;//the amount of entity to spawn for each seconds
    private int spawnedAmount; //the number of entities already spawned

    private void Awake()
    {
        EnemyComponent ec = prefabToSpawn.GetComponent<EnemyComponent>();
        spawnDistance = ec.surfaceWalkingHeightOffset;
        spawnedPerSeconds = amount / spawnDuration;
        //Debug.Log(amount + " / " + spawnDuration + " = " + spawnedPerSeconds);
        
        pool = this.gameObject.AddComponent<Pool>();
        pool.poolSize = amount;
        pool.poolSpawner = this;
        pool.enemy = ec;
        pool.spawnDistance = spawnDistance;
        pool.name = "Araignée";
        pool.layerToSpawn = "Default";
        pool.player = player;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position,radius);
    }



    public SpawnData getNewSpawnPosition()
    {
        bool didHit = false;
        RaycastHit hit;
        int counter = 0;
        while (!didHit)
        {
            if (Physics.Raycast(transform.position, Random.insideUnitSphere, out hit, radius,
                1 << LayerMask.NameToLayer(layerToSpawn)))
            {
                didHit = true;
                SpawnData spawnData = new SpawnData(hit.point,hit.normal);
                return spawnData;
            }
            /*
            else
            {
                counter++;
                Debug.Log(counter + " attempts failed.");
            }*/
        }

        throw new Exception("Error, trying to spawn entities in an empty space.");
    }
    
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            float rand = Random.value;
            if (rand < 1 / 3f)
            {
                pool.spawn(50,ResourceType.fire);
            }else if (rand > 2 / 3f)
            {
                pool.spawn(50,ResourceType.water);
            }else
            {
                pool.spawn(50,ResourceType.plant);
            }
                
            //Debug.Log("GOING TO SPAWN");
            pool.spawnAll();
        }
    }
}
