using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
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
    public GameObject chunk;

    [Header("World setting")]
    [Range(1, 8)]
    public int octave = 2;
    [Range(1, 4)]
    public float lacunarity = 2.0f;
    [Range(0, 1)]
    public float persistence = 0.5f;

    [Range(0, 1)]
    public float isoLevel = 0f;

    [Header("Area setting")]
    public float spawnSize = 20.0f;
    public float bossSize = 50.0f;
    public float tunnelSize = 9.0f;



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
    }

    void Start()
    {
        //get the player chunk
        playerChunk.x = Mathf.Floor(player.position.x / chunkSize);
        playerChunk.y = Mathf.Floor(player.position.y / chunkSize);
        playerChunk.z = Mathf.Floor(player.position.z / chunkSize);
        
        //create chunk (see function below)
        generateChunks();

        //demo for spawning in a chunk
        for(int i = 0; i < 150; i++)
        {
            Vector3[] spawnPos = getPositionOnChunks();
            if(spawnPos[0] != Vector3.zero)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = spawnPos[0];
                cube.transform.rotation = Quaternion.FromToRotation(cube.transform.up, spawnPos[1]) * transform.rotation;
            }
        }

    }

    void Update()
    {
        Vector3 temp = new Vector3();
        temp.x = Mathf.Floor(player.position.x / chunkSize);
        temp.y = Mathf.Floor(player.position.y / chunkSize);
        temp.z = Mathf.Floor(player.position.z / chunkSize);

        if(playerChunk != temp)
        {
            float timeScaleTemp = Time.timeScale;
            float timeFixedScaleTemp = Time.fixedDeltaTime;
            Time.fixedDeltaTime = 0;
            Time.timeScale = 0;

            Vector3 direction = (temp - playerChunk);
            playerChunk = temp;
            updateChunks(direction);

            Time.fixedDeltaTime = timeFixedScaleTemp;
            Time.timeScale = timeScaleTemp;
        }

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

            if(Input.GetKeyUp(KeyCode.F3))
            {
                for (int i = 0; i < 1500; i++)
                {
                    Vector3[] spawnPos = getPositionOnChunks();
                    if (spawnPos[0] != Vector3.zero)
                    {
                        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube.transform.position = spawnPos[0];
                        cube.transform.rotation = Quaternion.FromToRotation(cube.transform.up, spawnPos[1]) * transform.rotation;
                    }
                }
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
    void generateChunks()
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
                    chunkDictionary.Add(arr + playerChunk, chunks[x, y, z].GetComponent<chunk>().chunkData);
                }
            }
        }
    }

    //update the chunk during runtime, create new
    //chunk if they are not inside the dictionnary
    void updateChunks(Vector3 direction)
    {
        for (int x = 0; x < viewRange; x++)
        {
            for (int y = 0; y < viewRange; y++)
            {
                for (int z = 0; z < viewRange; z++)
                {
                    Vector3 chunkPos = chunks[x, y, z].transform.position / chunkSize;
                    ChunkData tempData;

                    //look if it find the chunk into the dictionary
                    //if not it create a new chunk
                    if (chunkDictionary.TryGetValue(chunkPos + direction, out tempData))
                    {
                        chunks[x, y, z].transform.position += direction * chunkSize;
                        chunks[x, y, z].GetComponent<chunk>().chunkData = tempData;
                        chunks[x, y, z].GetComponent<chunk>().makeMeshFromChunkData();
                    }
                    else
                    {
                        chunks[x, y, z].transform.position += direction * chunkSize;
                        chunks[x, y, z].GetComponent<chunk>().createMarchingBlock(chunkSize, playerSpawn, densityShader, MeshGeneratorShader, useDefaultNormal);
                        chunkDictionary.Add(chunks[x, y, z].transform.position / chunkSize, chunks[x, y, z].GetComponent<chunk>().chunkData);
                    }
                }
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
        return (float)(val - Math.Truncate(val)) - 0.5f;
    }

    //return a array, first value is the position and the second is the rotation !
    Vector3[] getPositionOnChunks()
    {
        Vector3[] rez = new Vector3[2];

        rez[0] = Vector3.zero;
        rez[1] = Vector3.zero;

        int x = (int)UnityEngine.Random.Range(0, viewRange - 1);
        int y = (int)UnityEngine.Random.Range(0, viewRange - 1);
        int z = (int)UnityEngine.Random.Range(0, viewRange - 1);

        chunk ck = chunks[x, y, z].GetComponent<chunk>();
        Vector3 pos = chunks[x, y, z].transform.position;

        if (hash(pos) > 0)
        {
            int len = ck.chunkData.mesh.vertices.Length;
            if (len > 0)
            {
                int v = (int)(UnityEngine.Random.Range(0, ck.chunkData.mesh.vertices.Length - 1));
                rez[0] = ck.chunkData.mesh.vertices[v] + pos;
                rez[1] = ck.chunkData.mesh.normals[v];
            }
        }
        return  rez;
    }
}
