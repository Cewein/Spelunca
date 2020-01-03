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

    //frustum cull of the chunks
    Plane[] planes;

    private void Awake()
    {
        chunks = new GameObject[viewRange,viewRange,viewRange];
    }

    void Start()
    {
        playerChunk.x = Mathf.Floor(player.position.x / chunkSize);
        playerChunk.y = Mathf.Floor(player.position.y / chunkSize);
        playerChunk.z = Mathf.Floor(player.position.z / chunkSize);

        generateChunks();

        planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
    }

    void Update()
    {
        Vector3 temp = new Vector3();
        temp.x = Mathf.Floor(player.position.x / chunkSize);
        temp.y = Mathf.Floor(player.position.y / chunkSize);
        temp.z = Mathf.Floor(player.position.z / chunkSize);

        if(playerChunk != temp)
        {
            print("in a new chunk");
            print("new chunk is : " + playerChunk.ToString());
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
                    chunks[x, y, z].SetActive(false);
                    if (GeometryUtility.TestPlanesAABB(planes, chunks[x, y, z].GetComponent<Collider>().bounds))
                        chunks[x, y, z].SetActive(true);
                    else if (aroundMiddle(x, y, z)) ;
                        //chunks[x, y, z].SetActive(true);
                        
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
                    Vector3 arr = new Vector3(x - half, y - half, z - half);
                    chunks[x, y, z] = Instantiate(chunk, (arr) * chunkSize + playerChunk * chunkSize, new Quaternion());
                    chunks[x, y, z].GetComponent<chunk>().createMarchingBlock(chunkSize);
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
                    Vector3 chunkArrayPosition = new Vector3(x, y, z);

                    if (inArray(chunkArrayPosition + direction))
                    {
                        int xb = (int)direction.x;
                        int yb = (int)direction.y;
                        int zb = (int)direction.z;

                        chunks[x, y, z] = chunks[x + xb, y + yb, z + zb];
                    }
                    else
                    {
                        chunks[x, y, z].transform.position += direction * chunkSize;
                        chunks[x, y, z].GetComponent<chunk>().createMarchingBlock(chunkSize);
                    }
                }
            }
        }
    }

    bool inArray(Vector3 newChunkArrayPosition)
    {
        int x = (int)newChunkArrayPosition.x;
        int y = (int)newChunkArrayPosition.y;
        int z = (int)newChunkArrayPosition.z;

        if (x < 0 || y < 0 || z < 0)
            return false;
        if (x >= viewRange || y >= viewRange || z >= viewRange)
            return false;

        return false;
    }

    bool aroundMiddle(int x, int y, int z)
    {
        int half = (int)viewRange / 2;

        x -= half;
        if (x >= -1 && x <= 1)
            return true;

        y -= half;
        if (y >= -1 && y <= 1)
            return true;

        z -= half;
        if (z >= -1 && z <= 1)
            return true;

        return false;
    }
}
