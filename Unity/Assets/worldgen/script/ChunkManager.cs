using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{

    //Config
    [Header("Player vision setting")]
    public Transform player;
    public uint chunkSize;
    public uint viewRange = 5;
    [SerializeField] private float floor = 0;
    public GameObject chunk;

    //chunks 
    private Vector3 playerChunk;
    private GameObject[,,] chunks;
    private Dictionary<Vector3, ChunkData> chunkDictionary;

    //zone of spawn
    private Vector3 playerSpawn;

    //frustum cull of the chunks
    Plane[] planes;
    

    [Header("Noise setting")]
    public uint octaveNumber = 5;

    [Header("Debug setting")]
    public bool debugNormals = false;
    
    
    private void Awake()
    {
        chunks = new GameObject[viewRange,viewRange,viewRange];
        chunkDictionary = new Dictionary<Vector3, ChunkData>();
        DensityGenerator.octaveNumber = octaveNumber;
        DensityGenerator.floor = floor;
    }

    void Start()
    {
        playerChunk.x = Mathf.Floor(player.position.x / chunkSize);
        playerChunk.y = Mathf.Floor(player.position.y / chunkSize);
        playerChunk.z = Mathf.Floor(player.position.z / chunkSize);
        playerSpawn = player.position;
        generateChunks();
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

        frustumCulling();

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
                    chunks[x, y, z].GetComponent<chunk>().createMarchingBlock(chunkSize, playerSpawn);
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
                        chunks[x, y, z].GetComponent<chunk>().createMarchingBlock(chunkSize, playerSpawn);
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
}
