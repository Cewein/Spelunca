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
    public int poolSize = 200; //the number of entities in the pool
    public PoolSpawner poolSpawner;
    public EnemyComponent enemy;
    public string layerToSpawn;
    public string name;
    public float spawnDistance;
    public GameObject player;
    public int maxEntitiesAttacking = 3;

    private EnemyComponent[] pool; //the entities on the map
    private Dictionary<int,bool> disabled;
    private Dictionary<int,bool> attacking;
    void Start()
    {
        pool = new EnemyComponent[poolSize];
        disabled = new Dictionary<int,bool>();
        attacking = new Dictionary<int,bool>();
        for (int i = 0; i < poolSize; i++)
        {
            disabled[i] = true;
        }
    }

    /// <summary>
    /// return a int[2] first is attacking, second is disabled
    /// </summary>
    public int[] GetSize()
    {
        int[] size = new int[2];

        size[0] = attacking.Count;
        size[1] = disabled.Count;

        return size;
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
                    if (!disabled.ContainsKey(i))
                    {
                        disableEntity(i);
                    }
                }
                //Checking if entities are done attacking
                if (attacking.ContainsKey(i) && entity.state != EnemyBehaviourState.Fighting)
                {
                    attacking.Remove(i);
                }
                Vector3 playerPos = player.transform.position;
                float distance = Math.Abs((entity.transform.position - playerPos).magnitude);
//                Debug.DrawLine(entity.transform.position,playerPos);
//                Debug.Log("distance from entity n°" + i + " to player " + distance);
                if (entity.state == EnemyBehaviourState.Chasing && distance > entity.proximityOffset - entity.proximityPrecision && distance < entity.proximityOffset + entity.proximityPrecision)
                {
                    EntityAttackRequest(i);
                }
            }
            
        }
    }

    public bool EntityAttackRequest(int index)//Allow an entity to ask if it can attack the player
    {
        //Debug.Log("Attack request for entity n°" + index);
        if (pool[index].state != EnemyBehaviourState.Disabled)
        {
            if (attacking.Count < maxEntitiesAttacking && pool[index].state != EnemyBehaviourState.Fighting)//there is at least 1 spot left as an attacker
            {
                attacking[index] = true;
                pool[index].state = EnemyBehaviourState.Fighting;
                return true;
            }
        }
        return false;
    }
    private void disableEntity(int index)
    {
        disabled[index] = true;
        pool[index].meshRenderer.enabled = false;
        
    }

    public void spawn(int amount, Vector3[] spawnData, ResourceType type)
    {
        int counter = 0;
        foreach (var i in disabled.Keys.ToList())
        {
            if (disabled.ContainsKey(i))
            {
                spawnOnce(i, spawnData, type);
                counter++;
                //pool[i].target = target;
            }

            if (counter >= amount)
                break;
        }
    }
    public void respawn(int id, Vector3[] spawnData, ResourceType type)
    {
        spawnOnce(id,spawnData,type);
    }

    private void spawnOnce(int id, Vector3[] spawnData, ResourceType type)
    {
        if (pool[id] == null)
        {
            pool[id] = Instantiate(enemy, spawnData[0] + spawnData[1] * spawnDistance, Quaternion.FromToRotation(Vector3.up, spawnData[1]));
            pool[id].name = name + id;
            pool[id].groundLayer = this.layerToSpawn;
        }
        else
        {
            pool[id].transform.position = spawnData[0] + spawnData[1] * spawnDistance;
            pool[id].transform.localRotation = Quaternion.FromToRotation(Vector3.up, spawnData[1]);
            pool[id].meshRenderer.enabled = true;
        }

        pool[id].type = type;
        pool[id].refreshMaterial();
        pool[id].transform.up = spawnData[1];
        pool[id].state = EnemyBehaviourState.Idle;
        pool[id].player = player;
        disabled.Remove(id);
    }
}
