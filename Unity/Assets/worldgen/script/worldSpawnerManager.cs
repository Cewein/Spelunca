using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class worldSpawnerManager : MonoBehaviour, PoolSpawner
{
    public ChunkManager ckManager;

    public SpawnData getNewSpawnPosition()
    {
        Vector3[] arr = ckManager.getPositionOnChunks();

        return new SpawnData(arr[0], arr[1]);
    }
}
