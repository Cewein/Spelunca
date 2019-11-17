using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class MarchingCubeMaster : MonoBehaviour
{
    public MeshFilter meshFilter;
    public MeshRenderer MeshRenderer;

    [Range(0, 1)]
    public float floor;
    private float floor_temp = -1;

    public uint size;

    public void Awake()
    {
        meshFilter = gameObject.AddComponent<MeshFilter>();
    }

    public void createMarchingBlock()
    {
        float[,,] block = new float[size, size, size];

        MeshData masterMeshData = new MeshData();

        Vector3 pos = GetComponent<Transform>().position;

        block = DensityGenerator.find(block, 0, size, pos);
        for (int x = 0; x < size - 1; x++)
        {
            for (int y = 0; y < size - 1; y++)
            {
                for (int z = 0; z < size - 1; z++)
                {
                    MeshData meshData = MeshGenerator.generateMesh(block, x, y, z,size, floor,masterMeshData.tcount);
                    masterMeshData.mergeMeshData(meshData);
                }
            }
        }

        Mesh mesh = masterMeshData.createMesh();
        meshFilter.sharedMesh = mesh;
    }

    private void Update()
    {
        if(floor_temp != floor)
        {
            floor_temp = floor;
            createMarchingBlock();
        }

    }

}
