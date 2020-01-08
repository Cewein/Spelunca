using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chunk : MonoBehaviour
{
    public ChunkData chunkData = new ChunkData();

    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public MeshCollider meshCollider;

    //Create a the chunk with a given size
    public void createMarchingBlock(uint size)
    {
        //init
        chunkData.meshData = new MeshData();
        MeshData masterMeshData = new MeshData();
        Vector3 pos = GetComponent<Transform>().position;

        //create denstiy 
        //TODO make it a compute shader
        chunkData.density = DensityGenerator.find(0, size + 1, pos);

        //loop for creating the mesh
        //TODO make it a compute shader
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    chunkData.meshData.mergeMeshData(MeshGenerator.generateMesh(chunkData.density, x, y, z, size, 0, masterMeshData.tcount));
                }
            }
        }

        makeMeshFromChunkData();
    }

    //Make a mesh and then put it in a MeshFilter and a MeshCollider
    public void makeMeshFromChunkData()
    {
        Mesh mesh = chunkData.meshData.createMesh();
        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;
    }
}

public struct ChunkData
{
    //to use when make the world editable
    //bool update;

    public float[,,] density;
    public MeshData meshData;
}
