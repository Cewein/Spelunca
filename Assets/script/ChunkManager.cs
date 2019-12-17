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

        int half = (int)viewRange / 2;

        for (int x = 0; x < viewRange; x++)
        {
            for (int y = 0; y < viewRange; y++)
            {
                for (int z = 0; z < viewRange; z++)
                {
                    chunks[x, y, z] = Instantiate(chunk,new Vector3(x-half,y-half,z-half) * chunkSize, new Quaternion());
                    chunks[x, y, z].GetComponent<chunk>().createMarchingBlock(chunkSize);
                }
            }
        }
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
            playerChunk = temp;
        }
    }

    void generateChunks()
    {

    }

    void updateChunks()
    {

    }
}
