using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{

    public Transform player;
    public int chunkSize;

    public Transform chunkDebug;
    public MarchingCubeMaster chunkMarchingDebug;

    private Vector3[,,] chunkCoords;
    private Vector3 playerChunk;

    private void Awake()
    {
        chunkCoords = new Vector3[5,5,5];
        playerChunk = new Vector3();
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

            chunkDebug.position = playerChunk * 16;
            chunkMarchingDebug.createMarchingBlock();
        }
    }
}
