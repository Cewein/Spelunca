using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml.Linq;

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
    public int viewRange = 5;
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

    //this variable is the stat of if we are in continue mode or new game mode
    public static bool randomSeed = false;
    public static bool useDefaultConfig = true;

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
    private int arraySize;
    private GameObject[] chunks;
    private DensityGenerator densityGenerator;
    [HideInInspector]
    public Dictionary<Vector3, ChunkData> chunkDictionary;

    public static Transform playerPos;
    //zone of spawn
    [HideInInspector]
    public Vector3 playerSpawn;
    [Header("Boss position")]
    //end zone
    public Transform boss;

    //frustum cull of the chunks
    Plane[] planes;

    private void Awake()
    {
        densityGenerator = new DensityGenerator();

        if (randomSeed)
        {
            seed = UnityEngine.Random.Range(-20f, 20f);

            if(!useDefaultConfig)
            {
                loadConfig();
            }

            //set variable for the density generator
            SetDensityValue();
        }
        else if (Load())
        {
            isoLevel = densityGenerator.isoLevel;
            boss.position = densityGenerator.endZone;
            playerSpawn = densityGenerator.playerSpawn;
            lacunarity = densityGenerator.lacunarity;
            octave = densityGenerator.octave;
            persistence = densityGenerator.persistence;
            spawnSize = densityGenerator.spawnSize;
            bossSize = densityGenerator.bossSize;
            tunnelSize = densityGenerator.tunnelSize;
            seed = densityGenerator.seed;
            precision = densityGenerator.precision;
            chunkSize = densityGenerator.size;
            player.position = densityGenerator.playerPos;
        }
        else
        {
            SetDensityValue();
        }

        //init data for runtime
        arraySize = viewRange * viewRange * viewRange;
        chunks = new GameObject[arraySize];
        chunkDictionary = new Dictionary<Vector3, ChunkData>();

        //set static variable for the density generator
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
        densityGenerator.playerPos = player.transform.position;

        playerChunk.x = Mathf.Floor(player.position.x / chunkSize);
        playerChunk.y = Mathf.Floor(player.position.y / chunkSize);
        playerChunk.z = Mathf.Floor(player.position.z / chunkSize);

        StartCoroutine(UpdateChunks());

        cheat();

        frustumCulling();
    }

    void Save()
    {
        Directory.CreateDirectory("C:\\ProgramData\\spelunca\\");
        File.WriteAllBytes("C:\\ProgramData\\spelunca\\world.json", System.Text.Encoding.ASCII.GetBytes(JsonUtility.ToJson(densityGenerator, true)));
        new XDocument(
            new XElement("world",
                new XAttribute("isoLevel", densityGenerator.isoLevel),
                new XAttribute("endZone", densityGenerator.endZone),
                new XAttribute("playerSpawn", densityGenerator.playerSpawn),
                new XAttribute("playerPos", densityGenerator.playerPos),
                new XAttribute("lacunarity", densityGenerator.lacunarity),
                new XAttribute("octave", densityGenerator.octave),
                new XAttribute("persistence", densityGenerator.persistence),
                new XAttribute("spawnSize", densityGenerator.spawnSize),
                new XAttribute("bossSize", densityGenerator.bossSize),
                new XAttribute("tunnelSize", densityGenerator.tunnelSize),
                new XAttribute("seed", densityGenerator.seed),
                new XAttribute("precision", densityGenerator.precision),
                new XAttribute("size", densityGenerator.size)
            )
        )
        .Save("C:\\ProgramData\\spelunca\\world.xml");
    }

    public bool Load()
    {
        Directory.CreateDirectory("C:\\ProgramData\\spelunca\\");
        if (File.Exists("C:\\ProgramData\\spelunca\\world.xml"))
        {
            densityGenerator = new DensityGenerator();
            XDocument doc = XDocument.Load("C:\\ProgramData\\spelunca\\world.xml");
            XElement world =  doc.Element("world");
            densityGenerator.isoLevel = float.Parse(world.Attribute("isoLevel").Value, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            densityGenerator.endZone = Spelunca.Utils.StringToVector3(world.Attribute("endZone").Value);
            densityGenerator.playerSpawn = Spelunca.Utils.StringToVector3(world.Attribute("playerSpawn").Value);
            densityGenerator.playerPos = Spelunca.Utils.StringToVector3(world.Attribute("playerPos").Value);
            densityGenerator.lacunarity = float.Parse(world.Attribute("lacunarity").Value, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            densityGenerator.octave = int.Parse(world.Attribute("octave").Value);
            densityGenerator.persistence = float.Parse(world.Attribute("persistence").Value, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            densityGenerator.spawnSize = float.Parse(world.Attribute("spawnSize").Value, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            densityGenerator.bossSize = float.Parse(world.Attribute("bossSize").Value, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            densityGenerator.tunnelSize = float.Parse(world.Attribute("tunnelSize").Value, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            densityGenerator.precision = float.Parse(world.Attribute("precision").Value, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            densityGenerator.size = int.Parse(world.Attribute("size").Value);
            densityGenerator.seed = float.Parse(world.Attribute("seed").Value, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            return true;
        }
        return false;  
    }

    public bool loadConfig()
    {
        Directory.CreateDirectory("C:\\ProgramData\\spelunca\\");
        if (File.Exists("C:\\ProgramData\\spelunca\\world.xml"))
        {
            densityGenerator = new DensityGenerator();
            XDocument doc = XDocument.Load("C:\\ProgramData\\spelunca\\config.xml");
            XElement config = doc.Element("config");
            isoLevel = float.Parse(config.Attribute("isoLevel").Value, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            lacunarity = float.Parse(config.Attribute("lacunarity").Value, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            octave = int.Parse(config.Attribute("octave").Value);
            persistence = float.Parse(config.Attribute("persistence").Value, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            precision = float.Parse(config.Attribute("precision").Value, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            chunkSize = int.Parse(config.Attribute("size").Value);
            viewRange = int.Parse(config.Attribute("viewDistance").Value);
            return true;
        }
        return false;
    }

    void SetDensityValue()
    {
        densityGenerator.isoLevel = isoLevel;
        densityGenerator.endZone = boss.position;
        densityGenerator.playerSpawn = playerSpawn = player.position;
        densityGenerator.lacunarity = lacunarity;
        densityGenerator.octave = octave;
        densityGenerator.persistence = persistence;
        densityGenerator.spawnSize = spawnSize;
        densityGenerator.bossSize = bossSize;
        densityGenerator.tunnelSize = tunnelSize;
        densityGenerator.seed = seed;
        densityGenerator.precision = precision;
        densityGenerator.size = chunkSize;
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

            if (Input.GetKeyUp(KeyCode.F4))
            {
                Save();
            }
        }
    }

    int Fatten(int x, int y, int z)
    {
        return x + viewRange * (y + viewRange * z);
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

        for (int x = 0; x < arraySize; x++)
        {
            //here the is two this appening, the first is checking if the chunks is between the planes 
            //of the camera frustum and the second is checking if the chunk is near from the player
            //the maximum distance is one chunk, if both test fails it hide the chunk
            if (GeometryUtility.TestPlanesAABB(planes, chunks[x].GetComponent<Collider>().bounds))
                chunks[x].GetComponent<MeshRenderer>().enabled = true;
            else if (AroundMiddle(chunks[x].transform.position / 16))
                chunks[x].GetComponent<MeshRenderer>().enabled = true;
            else
                chunks[x].GetComponent<MeshRenderer>().enabled = false;
        }
    }


    /// <summary>
    ///this generate the chunks, it's the only 3D for loops
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
                    chunks[Fatten(x, y, z)] = Instantiate(chunk, (arr + playerChunk) * chunkSize, new Quaternion());
                    //Two compute shader are pass
                    chunks[Fatten(x, y, z)].GetComponent<chunk>().createMarchingBlock(densityGenerator, playerSpawn, densityShader, MeshGeneratorShader, useDefaultNormal);
                    chunks[Fatten(x, y, z)].GetComponent<chunk>().chunkData.lastPlayerPos = playerChunk;

                    SpawnStructures(chunks[Fatten(x, y, z)]);

                    chunkDictionary.Add(arr + playerChunk, chunks[Fatten(x, y, z)].GetComponent<chunk>().chunkData);
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
        Queue<GameObject> toGen = new Queue<GameObject>();

        Vector3 temp = new Vector3();
        temp = new Vector3();
        temp.x = Mathf.Floor(player.position.x / chunkSize);
        temp.y = Mathf.Floor(player.position.y / chunkSize);
        temp.z = Mathf.Floor(player.position.z / chunkSize);

        for (int x = 0; x < arraySize; x++)
        {
            chunkPos = chunks[x].transform.position / chunkSize;
            chunkPlayerPos = chunks[x].GetComponent<chunk>().chunkData.lastPlayerPos;

            if (chunkPlayerPos != temp)
            {
                ChunkData tempData;
                Vector3 direction = (temp - chunkPlayerPos);

                //look if it find the chunk into the dictionary
                //if not it create a new chunk
                if (chunkDictionary.TryGetValue(chunkPos + direction, out tempData))
                {
                    chunks[x].GetComponent<chunk>().chunkData.toggle(false);
                    chunks[x].transform.position += direction * chunkSize;
                    chunks[x].GetComponent<chunk>().chunkData = tempData;
                    chunks[x].GetComponent<chunk>().makeMeshFromChunkData();
                    chunks[x].GetComponent<chunk>().chunkData.lastPlayerPos = temp;

                }
                else
                {
                    toGen.Enqueue(chunks[x]);
                }
            }
            chunks[x].GetComponent<chunk>().chunkData.toggle(true);

        }

        //there is a corouting in that chunk but it's need, it's spread out
        //the computation on time, it compute on chunk per frame so normally
        //60 chunks per second (or more if you have a powerfull cpu + gpu)
        foreach(var ch in toGen)
        {
            chunkPos = ch.transform.position / chunkSize;
            chunkPlayerPos = ch.GetComponent<chunk>().chunkData.lastPlayerPos;

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
                    ch.GetComponent<chunk>().chunkData.toggle(false);
                    ch.transform.position += direction * chunkSize;
                    ch.GetComponent<chunk>().createMarchingBlock(densityGenerator, playerSpawn, densityShader, MeshGeneratorShader, useDefaultNormal);
                    ch.GetComponent<chunk>().chunkData.lastPlayerPos = temp;

                    SpawnStructures(ch);

                    chunkDictionary.Add(ch.transform.position / chunkSize, ch.GetComponent<chunk>().chunkData);
                    ch.GetComponent<chunk>().chunkData.toggle(true);
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
    bool AroundMiddle(Vector3 pos)
    {

        float dist = Vector3.Distance(playerChunk, pos);

        if (dist > 2) return true;
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

        if (isFluff) Spelunca.Utils.AddRange(ck.GetComponent<chunk>().chunkData.flufflDictionary, dico);
        else Spelunca.Utils.AddRange(ck.GetComponent<chunk>().chunkData.mineralDictionary, dico);
            
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
