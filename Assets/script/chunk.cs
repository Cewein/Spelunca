using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chunk : MonoBehaviour
{
    public ChunkData chunkData = new ChunkData();

    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public MeshCollider meshCollider;

    public void createMarchingBlock(uint size)
    {

        chunkData.density = new float[size + 1, size + 1, size + 1];
        chunkData.meshData = new MeshData();

        MeshData masterMeshData = new MeshData();

        Vector3 pos = GetComponent<Transform>().position;

        chunkData.density = DensityGenerator.find(chunkData.density, 0, size + 1, pos);
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

        Mesh mesh = chunkData.meshData.createMesh();
        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;
    }
}

public struct ChunkData
{ 
    public float[,,] density;
    public MeshData meshData;
}
