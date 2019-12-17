using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chunk : MonoBehaviour
{
    public Vector3 position;
    public float[,,] density;

    [HideInInspector]
    public MeshData meshData;

    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public MeshCollider meshCollider;

    public void createMarchingBlock(uint size)
    {
        density = new float[size + 1, size + 1, size + 1];

        MeshData masterMeshData = new MeshData();

        Vector3 pos = GetComponent<Transform>().position;

        density = DensityGenerator.find(density, 0, size + 1, pos);
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    MeshData meshData = MeshGenerator.generateMesh(density, x, y, z, size, 0, masterMeshData.tcount);
                    masterMeshData.mergeMeshData(meshData);
                }
            }
        }

        Mesh mesh = masterMeshData.createMesh();
        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;
    }
}
