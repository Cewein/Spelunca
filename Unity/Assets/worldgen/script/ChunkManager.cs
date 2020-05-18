﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public GameObject resourceFirePrefab;
    //compute shader
    [Header("Compute shader file")]
    public ComputeShader densityShader;
    public ComputeShader MeshGeneratorShader;
    public bool useDefaultNormal = false;

    //Config
    [Header("Player vision setting")]
    public Transform player;
    public int chunkSize;
    public uint viewRange = 5;
    public float precision = 1.0f;
    public GameObject chunk;

    [Header("World setting")]
    [Range(1, 8)]
    public int octave = 2;
    [Range(1, 4)]
    public float lacunarity = 2.0f;
    [Range(0, 1)]
    public float persistence = 0.5f;
    public float seed = 0;

    [Range(0, 1)]
    public float isoLevel = 0f;

    [Header("Area setting")]
    public float spawnSize = 20.0f;
    public float bossSize = 50.0f;
    public float tunnelSize = 9.0f;

    [Header("Structures setting")]
    [Range(0,1)]
    public float ratioOfSpawn = 0.5f;
    public int maxNumberOfStructPerChunk = 200;
    public structure[] structures;

    //chunks 
    private Vector3 playerChunk;
    private GameObject[,,] chunks;
    private Dictionary<Vector3, ChunkData> chunkDictionary;

    //zone of spawn
    private Vector3 playerSpawn;
    [Header("Boss position")]
    //end zone
    public Transform boss;

    //frustum cull of the chunks
    Plane[] planes;

    private void Awake()
    {
        //init data for runtime
        chunks = new GameObject[viewRange,viewRange,viewRange];
        chunkDictionary = new Dictionary<Vector3, ChunkData>();

        ratioOfSpawn = 1.0f - ratioOfSpawn;

        //set static variable for the density generator
        DensityGenerator.isoLevel = isoLevel;
        DensityGenerator.endZone = boss.position;
        DensityGenerator.playerSpawn = playerSpawn = player.position;
        DensityGenerator.lacunarity = lacunarity;
        DensityGenerator.octave = octave;
        DensityGenerator.persistence = persistence;
        DensityGenerator.spawnSize = spawnSize;
        DensityGenerator.bossSize = bossSize;
        DensityGenerator.tunnelSize = tunnelSize;
        DensityGenerator.seed = seed;
        DensityGenerator.precision = precision;
    }

    void Start()
    {
        //get the player chunk
        playerChunk.x = Mathf.Floor(player.position.x / chunkSize);
        playerChunk.y = Mathf.Floor(player.position.y / chunkSize);
        playerChunk.z = Mathf.Floor(player.position.z / chunkSize);
        
        //create chunk (see function below)
        generateChunks(playerChunk);
    }

    void Update()
    {
        StartCoroutine(updateChunks());

        cheat();

        frustumCulling();
    }

    //cheat for moving faster
    void cheat()
    {
        if (true)
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                player.position = playerSpawn;
            }

            if (Input.GetKeyUp(KeyCode.F2))
            {
                player.position = new Vector3(boss.position.x, boss.position.y + 30, boss.position.z);
            }
        }
    }

    //with a AABB plane we can see if a mesh
    //is inside the view frustum, if it not inside
    //it's not rendered
    void frustumCulling()
    {
        planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

        for (int x = 0; x < viewRange; x++)
        {
            for (int y = 0; y < viewRange; y++)
            {
                for (int z = 0; z < viewRange; z++)
                {
                    //here the is two this appening, the first is checking if the chunks is between the planes 
                    //of the camera frustum and the second is checking if the chunk is near from the player
                    //the maximum distance is one chunk, if both test fails it hide the chunk
                    if (GeometryUtility.TestPlanesAABB(planes, chunks[x, y, z].GetComponent<Collider>().bounds))
                        chunks[x, y, z].GetComponent<MeshRenderer>().enabled = true;
                    else if (aroundMiddle(x, y, z))
                        chunks[x, y, z].GetComponent<MeshRenderer>().enabled = true;
                    else
                        chunks[x, y, z].GetComponent<MeshRenderer>().enabled = false;
                }
            }
        }
    }

    //this generate the chunks
    //for genertating chunk during runtime
    //see updateChunks function
    void generateChunks(Vector3 playerChunk)
    {
        int half = (int)viewRange / 2;

        for (int x = 0; x < viewRange; x++)
        {
            for (int y = 0; y < viewRange; y++)
            {
                for (int z = 0; z < viewRange; z++)
                {
                    Vector3 arr = new Vector3(x - half, y - half, z - half);
                    chunks[x, y, z] = Instantiate(chunk, (arr + playerChunk) * chunkSize, new Quaternion());
                    //Two compute shader are pass
                    chunks[x, y, z].GetComponent<chunk>().createMarchingBlock(chunkSize, playerSpawn, densityShader, MeshGeneratorShader, useDefaultNormal);
                    chunks[x, y, z].GetComponent<chunk>().chunkData.lastPlayerPos = playerChunk;
                    if(hash(chunks[x, y, z].transform.position) > ratioOfSpawn)
                        spawnStructures(chunks[x, y, z]);
                    chunkDictionary.Add(arr + playerChunk, chunks[x, y, z].GetComponent<chunk>().chunkData);
                }
            }
        }
    }

    //update the chunk during runtime, create new
    //chunk if they are not inside the dictionnary
    //
    //gen update is done everyframe
    IEnumerator updateChunks()
    {
        Vector3 chunkPos;
        Vector3 chunkPlayerPos;
        

        Vector3 temp = new Vector3();
        temp = new Vector3();
        temp.x = Mathf.Floor(player.position.x / chunkSize);
        temp.y = Mathf.Floor(player.position.y / chunkSize);
        temp.z = Mathf.Floor(player.position.z / chunkSize);

        foreach (var chunk in chunks)
        {
            chunkPos = chunk.transform.position / chunkSize;
            chunkPlayerPos = chunk.GetComponent<chunk>().chunkData.lastPlayerPos;

            if (chunkPlayerPos != temp)
            {
                ChunkData tempData;
                Vector3 direction = (temp - chunkPlayerPos);

                //look if it find the chunk into the dictionary
                //if not it create a new chunk
                if (chunkDictionary.TryGetValue(chunkPos + direction, out tempData))
                {
                    chunk.transform.position += direction * chunkSize;
                    chunk.GetComponent<chunk>().chunkData = tempData;
                    chunk.GetComponent<chunk>().makeMeshFromChunkData();
                    chunk.GetComponent<chunk>().chunkData.lastPlayerPos = temp;
                }
            }
        }

        foreach (var chunk in chunks)
        {
            chunkPos = chunk.transform.position / chunkSize;
            chunkPlayerPos = chunk.GetComponent<chunk>().chunkData.lastPlayerPos;

            temp = new Vector3();
            temp.x = Mathf.Floor(player.position.x / chunkSize);
            temp.y = Mathf.Floor(player.position.y / chunkSize);
            temp.z = Mathf.Floor(player.position.z / chunkSize);

            if (chunkPlayerPos != temp)
            {
                ChunkData tempData;
                Vector3 direction = (temp - chunkPlayerPos);

                if (!chunkDictionary.TryGetValue(chunkPos + direction, out tempData))
                {
                    chunk.transform.position += direction * chunkSize;
                    chunk.GetComponent<chunk>().createMarchingBlock(chunkSize, playerSpawn, densityShader, MeshGeneratorShader, useDefaultNormal);
                    chunkDictionary.Add(chunk.transform.position / chunkSize, chunk.GetComponent<chunk>().chunkData);
                    chunk.GetComponent<chunk>().chunkData.lastPlayerPos = temp;
                    if (hash(chunk.transform.position) > ratioOfSpawn)
                        spawnStructures(chunk);
                }
                yield return null;
            }
        }
    }

    bool aroundMiddle(int x, int y, int z)
    {
        int half = (int)viewRange / 2;

        x -= half;
        y -= half;
        z -= half;

        if (x >= -1 && x <= 1)
            if (y >= -1 && y <= 1)
                if (z >= -1 && z <= 1)
                    return true;

        return false;
    }

    //hash function, warning might collide a lot not tested properly
    //because it the not the goal of the function we just need 
    //value between zero and one
    // TODO move function into static class
    float hash(Vector3 vec)
    {
        double val = (1299689.0f * Math.Abs(vec.x) + 611953.0f * Math.Abs(vec.y)) / 898067 * Math.Abs(vec.z);
        return (float)(val - Math.Truncate(val));
    }

    //return a array, first value is the position and the second is the rotation !
    Vector3[] getPositionOnChunks(GameObject chunk)
    {
        Vector3[] rez = new Vector3[2];

        rez[0] = Vector3.zero;
        rez[1] = Vector3.zero;

        chunk ck = chunk.GetComponent<chunk>();
        Vector3 pos = chunk.transform.position;


        int len = ck.chunkData.mesh.vertices.Length;
        if (len > 0)
        {
            int v = (int)(UnityEngine.Random.Range(0, ck.chunkData.mesh.vertices.Length - 1));
            rez[0] = ck.chunkData.mesh.vertices[v] + pos;
            rez[1] = ck.chunkData.mesh.normals[v];
        }

        return  rez;
    }

    void spawnStructures(GameObject ck)
    {
            
        for (int i = 0; i < maxNumberOfStructPerChunk; i++)
        {

            Vector3[] data = getPositionOnChunks(ck);

            int size = structures.Length;
            int s = UnityEngine.Random.Range(0, structures.Length);

            float angle = Vector3.Dot(data[1], Vector3.up);

            GameObject obj = Instantiate(structures[s].gameObject, data[0], Quaternion.FromToRotation(Vector3.up, data[1]) * transform.rotation);
        }
            
        ck.GetComponent<chunk>().chunkData.canSpawnResources = false; 
    }
}

[System.Serializable]
public struct structure
{
    //the gameobject we want to spawn
    public GameObject gameObject;
    Vector3 position;
}
