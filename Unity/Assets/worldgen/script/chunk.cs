using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chunk : MonoBehaviour
{
    public ChunkData chunkData = new ChunkData();

    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public MeshCollider meshCollider;

    ComputeBuffer pointsBuffer;
    ComputeBuffer triangleBuffer;
    ComputeBuffer trisCounterBuffer;

    List<ComputeBuffer> bufferList;

    //Create a the chunk with a given size
    public void createMarchingBlock(int size, Vector3 playerSpawn, ComputeShader densityShader, ComputeShader marchShader)
    {
        //init
        chunkData.meshData = new MeshData();
        MeshData masterMeshData = new MeshData();
        Vector3 pos = GetComponent<Transform>().position;

        //create the 3 buffer needed for the GPU gen
        createBuffer(size);

        //create denstiy 
        //TODO make it a compute shader
        //we make it size + 3 because it's for the normals
        DensityGenerator.find(pointsBuffer, size + 3, pos - Vector3.one, densityShader);

        //loop for creating the mesh
        //TODO make it a compute shader
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    chunkData.meshData.mergeMeshData(MeshGenerator.generateMesh(chunkData.density, x + 1, y + 1, z + 1, size, 0, masterMeshData.tcount));
                }
            }
        }
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
            chunkData.mesh = chunkData.meshData.createMesh();
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

        pointsBuffer = new ComputeBuffer(nump, sizeof(float) * 4);
        triangleBuffer = new ComputeBuffer(totalMaxTris, sizeof(float) * 3 * 3 * 2, ComputeBufferType.Append);
        trisCounterBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);

        bufferList.Add(pointsBuffer);
        bufferList.Add(triangleBuffer);
        bufferList.Add(trisCounterBuffer);
    }
}

public struct ChunkData
{
    //to use when make the world editable
    public bool update;

    public Mesh mesh;
    public float[,,] density;
    public MeshData meshData;
}
