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

    [Header("Mob Setting")]
    public Pool pool;
    [Range(0, 1)]
    [Tooltip("ratio of rejection of chunks, less mean more chunk are selected ")]
    public float ratioOfRejectionForSpider = 0.97f;
    public int maxSpiderPerChunk = 50;

    [Header("Minerals setting")]
    [Range(0, 1)]
    [Tooltip("ratio of rejection of chunks, less mean more chunk are selected ")]
    public float ratioOfRejectionForMineral = 0.97f;
    public int maxMineralPerChunk = 200;
    public Structure[] minerals;

    [Header("Fluff setting")]
    [Range(0, 1)]
    [Tooltip("ratio of rejection of chunks, less mean more chunk are selected ")]
    public float ratioOfRejectionForFluff = 0.90f;
    public int maxFluffPerChunk = 200;
    public Structure[] Fluffs;

    [Header("Rare spawn setting")]
    [Range(0, 1)]
    [Tooltip("ratio of rejection of chunks, less mean more chunk are selected ")]
    public float ratioOfRejectionForRareStruct = 0.90f;
    public Structure[] rare;

    //chunks 
    [HideInInspector]
    public Vector3 playerChunk;
    private GameObject[,,] chunks;
    [HideInInspector]
    public Dictionary<Vector3, ChunkData> chunkDictionary;

    [HideInInspector]
    public static Transform playerPos;
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
        DensityGenerator.seed = seed;
        DensityGenerator.precision = precision;

        portal.spawnCoord = playerSpawn;
    }

    void Start()
    {
        //get the player chunk
        playerChunk.x = Mathf.Floor(player.position.x / chunkSize);
        playerChunk.y = Mathf.Floor(player.position.y / chunkSize);
        playerChunk.z = Mathf.Floor(player.position.z / chunkSize);
        
        //create chunk (see function below)
        generateChunks(playerChunk);

        playerPos = player;
    }

    void Update()
    {
        playerPos = player;

        playerChunk.x = Mathf.Floor(player.position.x / chunkSize);
        playerChunk.y = Mathf.Floor(player.position.y / chunkSize);
        playerChunk.z = Mathf.Floor(player.position.z / chunkSize);

        StartCoroutine(UpdateChunks());

        cheat();

        frustumCulling();
    }

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

    /// <summary>
    /// 
    ///with a AABB plane we can see if a mesh
    ///is inside the view frustum, if it not inside
    ///it's not rendered
    /// </summary>
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
                    else if (AroundMiddle(x, y, z))
                        chunks[x, y, z].GetComponent<MeshRenderer>().enabled = true;
                    else
                        chunks[x, y, z].GetComponent<MeshRenderer>().enabled = false;
                }
            }
        }
    }


    /// <summary>
    ///this generate the chunks
    ///<para />
    ///for genertating chunk during runtime
    ///see updateChunks function
    /// </summary>
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

                    SpawnStructures(chunks[x, y, z]);

                    chunkDictionary.Add(arr + playerChunk, chunks[x, y, z].GetComponent<chunk>().chunkData);
                }
            }
        }
    }

    /// <summary>
    ///update the chunk during runtime, create new
    ///chunk if they are not inside the dictionnary
    ///<para />
    ///gen update is done everyframe
    /// </summary>
    IEnumerator UpdateChunks()
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
                    chunk.GetComponent<chunk>().chunkData.toggle(false);
                    chunk.transform.position += direction * chunkSize;
                    chunk.GetComponent<chunk>().chunkData = tempData;
                    chunk.GetComponent<chunk>().makeMeshFromChunkData();
                    chunk.GetComponent<chunk>().chunkData.lastPlayerPos = temp;

                }
            }
            chunk.GetComponent<chunk>().chunkData.toggle(true);

        }

        //there is a corouting in that chunk but it's need, it's spread out
        //the computation on time, it compute on chunk per frame so normally
        //60 chunks per second (or more if you have a powerfull cpu + gpu)
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
                    chunk.GetComponent<chunk>().chunkData.toggle(false);
                    chunk.transform.position += direction * chunkSize;
                    chunk.GetComponent<chunk>().createMarchingBlock(chunkSize, playerSpawn, densityShader, MeshGeneratorShader, useDefaultNormal);
                    chunk.GetComponent<chunk>().chunkData.lastPlayerPos = temp;

                    SpawnStructures(chunk);

                    chunkDictionary.Add(chunk.transform.position / chunkSize, chunk.GetComponent<chunk>().chunkData);
                    chunk.GetComponent<chunk>().chunkData.toggle(true);
                }
                yield return null;
            }
        }
    }

    void SpawnStructures(GameObject chunk)
    {
        float ckHash = Hash(chunk.transform.position);
        if (ckHash > ratioOfRejectionForMineral)
            SpawnStructures(chunk, minerals, maxMineralPerChunk);
        if (ckHash > ratioOfRejectionForFluff)
            SpawnStructures(chunk, Fluffs, maxFluffPerChunk, true);
        if (ckHash > ratioOfRejectionForSpider)
            SpawnSpiders(chunk, maxSpiderPerChunk);
        if (ckHash > ratioOfRejectionForRareStruct)
            SpawnStructures(chunk, rare, 1);
    }

    /// <summary> when doing view frustum culling this function let a 3x3 chunks box around the player </summary>
    bool AroundMiddle(int x, int y, int z)
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

    /// <summary>
    /// hash function, warning might collide a lot not tested properly
    /// because it the not the goal of the function we just need 
    /// value between zero and one
    /// </summary>
    float Hash(Vector3 vec)
    {
        double val = (1299689.0f * Math.Abs(vec.x) + 611953.0f * Math.Abs(vec.y)) / 898067 * Math.Abs(vec.z);
        return (float)(val - Math.Truncate(val));
    }


    /// <summary>
    /// return a array, first value is the position and the second is the rotation !
    /// </summary>
    Vector3[] GetPositionOnChunks(GameObject chunk)
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

    /// <summary>
    /// spawn a structre on a chunk with the given structure array and number of maximum object in that chunk 
    /// </summary>
    void SpawnStructures(GameObject ck, Structure[] strct, int maxStruct, bool isFluff = false)
    {
        int size = strct.Length;
        int s = UnityEngine.Random.Range(0, size);
        Dictionary<Vector3, GameObject> dico = new Dictionary<Vector3, GameObject>();

        for (int i = 0; i < maxStruct && size > 0; i++)
        {

            Vector3[] data = GetPositionOnChunks(ck);

            if (!dico.ContainsKey(data[0]))
            {
                float angle = Vector3.Dot(data[1], Vector3.up);

                if (isFluff) s = UnityEngine.Random.Range(0, size);

                Vector3 area = strct[s].area;

                if (data[0] != Vector3.zero && area.x <= angle && area.y >= angle)
                {
                    GameObject o = Instantiate(strct[s].gameObject, data[0], Quaternion.FromToRotation(Vector3.up, data[1]) * transform.rotation);
                    dico.Add(o.transform.position, o);
                }
            }
        }

        if (isFluff) ck.GetComponent<chunk>().chunkData.flufflDictionary = dico;
        else ck.GetComponent<chunk>().chunkData.mineralDictionary = dico;
            
        ck.GetComponent<chunk>().chunkData.hasSpawnResources = true; 
    }

    void SpawnSpiders(GameObject ck, int maxStruct)
    {
        int size = Enum.GetNames(typeof(ResourceType)).Length;
        int s = UnityEngine.Random.Range(1, size);
        for (int i = 0; i < maxStruct; i++)
        {

            Vector3[] data = GetPositionOnChunks(ck);

            if (data[0] != Vector3.zero)
            {
                pool.spawn(1, data, (ResourceType)s);
            }
        }
    }
}

[System.Serializable]
public struct Structure
{
    //the gameobject we want to spawn
    public GameObject gameObject;
    public Vector2 area;
}
