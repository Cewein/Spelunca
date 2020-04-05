using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chunk : MonoBehaviour
{
    public ChunkData chunkData = new ChunkData();

    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public MeshCollider meshCollider;

    private ComputeBuffer pointsBuffer;
    private ComputeBuffer triangleBuffer;
    private ComputeBuffer trisCounterBuffer;

    List<ComputeBuffer> bufferList;

    Vector4[] dataArray;

    //Create a the chunk with a given size
    public void createMarchingBlock(int size, Vector3 playerSpawn, ComputeShader densityShader, ComputeShader marchShader)
    {
        //init
        Vector3 pos = GetComponent<Transform>().position;
        bufferList = new List<ComputeBuffer>();

        //create the 3 buffer needed for the GPU gen
        createBuffer(size);

        //get the number of gpu group thread to run 
        //at the same time
        int numThreadEachAxis = Mathf.CeilToInt((size-1) / 8.0f);

        //create denstiy on the gpu
        //the data stay on the gpu with the compute buffer
        DensityGenerator.find(pointsBuffer,size + 1, pos - Vector3.one, densityShader);

        pointsBuffer.GetData(dataArray);

        //create the mesh on the gpu

        //set the conteur to 0 and pass value
        //to the compute shader
        triangleBuffer.SetCounterValue(0);
        marchShader.SetBuffer(0, "points", pointsBuffer);
        marchShader.SetBuffer(0, "triangles", triangleBuffer);
        marchShader.SetInt("size", size + 1);
        marchShader.SetFloat("isoLevel", DensityGenerator.isoLevel);

        //lauch the compute shader on each threadGroup
        marchShader.Dispatch(0, numThreadEachAxis, numThreadEachAxis, numThreadEachAxis);

        //setup for getting the data
        ComputeBuffer.CopyCount(triangleBuffer, trisCounterBuffer, 0);
        int[] triCountArray = { 0 };
        trisCounterBuffer.GetData(triCountArray);
        int numTris = triCountArray[0];

        // Get triangle data from shader
        Triangle[] tris = new Triangle[numTris];
        triangleBuffer.GetData(tris, 0, 0, numTris);

        //setup for mesh cpu creation
        Mesh mesh = new Mesh();
        mesh.Clear();
        Vector3[] vertices = new Vector3[numTris * 3];
        int[] meshTriangles = new int[numTris * 3];

        //but the data into the mesh
        for (int i = 0; i < numTris; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                meshTriangles[i * 3 + j] = i * 3 + j;
                vertices[i * 3 + j] = tris[i][j];
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = meshTriangles;

        mesh.RecalculateNormals();

        chunkData.mesh = mesh;

        chunkData.update = true;
        makeMeshFromChunkData();

        //release all the buffer
        foreach (var buffer in bufferList)
        {
            if(buffer != null)
                buffer.Release();
        }
    }

    //Make a mesh and then put it in a MeshFilter and a MeshCollider
    public void makeMeshFromChunkData()
    {
        if (chunkData.update)
        {
            //chunkData.mesh = chunkData.meshData.createMesh();
            chunkData.update = false;
        }

        meshFilter.sharedMesh = chunkData.mesh;
        meshCollider.sharedMesh = chunkData.mesh;
    }

    //create the compute buffer and add them to a list
    private void createBuffer(int size)
    {
        int nump = (int)Mathf.Pow((size + 3), 3);
        int voxPerAxe = size - 1;
        int totalVox = voxPerAxe * voxPerAxe * voxPerAxe;
        int totalMaxTris = totalVox * 5;

        dataArray = new Vector4[nump];

        //setup the compute buffer
        pointsBuffer = new ComputeBuffer(nump, sizeof(float) * 4);
        triangleBuffer = new ComputeBuffer(totalMaxTris, sizeof(float) * 3 * 3, ComputeBufferType.Append);
        trisCounterBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);

        bufferList.Add(pointsBuffer);
        bufferList.Add(triangleBuffer);
        bufferList.Add(trisCounterBuffer);
    }
}

public struct ChunkData
{
    public bool update;

    public Mesh mesh;
    public float[,,] density;
    public MeshData meshData;
}

public struct Triangle
{
    //if you use a new triangle struct
    //be sure to modify the compute size
    //and also the shader

#pragma warning disable 649 // disable unassigned variable warning
    public Vector3 a;
    public Vector3 b;
    public Vector3 c;

    public Vector3 this[int i]
    {
        get
        {
            switch (i)
            {
                case 0:
                    return a;
                case 1:
                    return b;
                default:
                    return c;
            }
        }
    }
}
