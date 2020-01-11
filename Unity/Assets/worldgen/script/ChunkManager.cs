using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{

    //Config
    public Transform player;
    public uint chunkSize;
    public uint viewRange = 5;
    public GameObject chunk;

    //chunk data
    private Vector3 playerChunk;
    private GameObject[,,] chunks;
    private Dictionary<Vector3, ChunkData> chunkDictionary;

    //frustum cull of the chunks
    Plane[] planes;

    //Chunk Density
    [SerializeField] private float[] octaveScale;
    [SerializeField] private float[] octaveIntensity;
    
    private void Awake()
    {
        chunks = new GameObject[viewRange,viewRange,viewRange];
        chunkDictionary = new Dictionary<Vector3, ChunkData>();
    }

    void Start()
    {
        playerChunk.x = Mathf.Floor(player.position.x / chunkSize);
        playerChunk.y = Mathf.Floor(player.position.y / chunkSize);
        playerChunk.z = Mathf.Floor(player.position.z / chunkSize);
        DensityGenerator.octaveScale = octaveScale;
        DensityGenerator.octaveIntensity = octaveIntensity;
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
            //print("in a new chunk");
            //print("new chunk is : " + playerChunk.ToString());
            Vector3 direction = (temp - playerChunk);
            playerChunk = temp;
            updateChunks(direction);
        }

        frustumCulling();

    }

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

    void generateChunks()
    {
        int half = (int)viewRange / 2;

        for (int x = 0; x < viewRange; x++)
        {
            for (int y = 0; y < viewRange; y++)
            {
                for (int z = 0; z < viewRange; z++)
                {
                    //print("gen chunk : " + x + " " + y + " " + z);
                    Vector3 arr = new Vector3(x - half, y - half, z - half);
                    chunks[x, y, z] = Instantiate(chunk, (arr + playerChunk) * chunkSize, new Quaternion());
                    chunks[x, y, z].GetComponent<chunk>().createMarchingBlock(chunkSize);
                    chunkDictionary.Add(arr + playerChunk, chunks[x, y, z].GetComponent<chunk>().chunkData);
                }
            }
        }
    }

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

                    if (chunkDictionary.TryGetValue(chunkPos + direction, out tempData))
                    {
                        chunks[x, y, z].transform.position += direction * chunkSize;
                        chunks[x, y, z].GetComponent<chunk>().chunkData = tempData;
                        chunks[x, y, z].GetComponent<chunk>().makeMeshFromChunkData();
                    }
                    else
                    {
                        chunks[x, y, z].transform.position += direction * chunkSize;
                        chunks[x, y, z].GetComponent<chunk>().createMarchingBlock(chunkSize);
                        chunkDictionary.Add(chunks[x, y, z].transform.position / chunkSize, chunks[x, y, z].GetComponent<chunk>().chunkData);
                    }
                }
            }
        }
    }

    bool inArray(Vector3 newChunkArrayPosition)
    {
        print(newChunkArrayPosition);
        int x = (int)newChunkArrayPosition.x;
        int y = (int)newChunkArrayPosition.y;
        int z = (int)newChunkArrayPosition.z;

        if (x > 0 && y > 0 && z > 0 && x <= viewRange && y <= viewRange && z <= viewRange)
            return true;

        return false;
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
