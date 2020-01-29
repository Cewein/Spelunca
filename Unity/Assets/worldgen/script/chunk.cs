﻿using System.Collections;
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
        //we make it size + 3 because it's for the normals
        chunkData.density = DensityGenerator.find(size + 3, pos - Vector3.one);

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
}

public struct ChunkData
{
    //to use when make the world editable
    public bool update;

    public Mesh mesh;
    public float[,,] density;
    public MeshData meshData;
}