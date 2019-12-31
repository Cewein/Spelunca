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

    private void Awake()
    {
        playerChunk = new Vector3();
        chunks = new GameObject[viewRange,viewRange,viewRange];

        generateChunks();
    }

    void Start()
    {
        playerChunk.x = Mathf.Floor(player.position.x / chunkSize);
        playerChunk.y = Mathf.Floor(player.position.y / chunkSize);
        playerChunk.z = Mathf.Floor(player.position.z / chunkSize);

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
                    chunks[x, y, z] = Instantiate(chunk, new Vector3(x - half, y - half, z - half) * chunkSize, new Quaternion());
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
}
