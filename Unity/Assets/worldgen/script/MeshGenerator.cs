using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
    public List<Vector3> vertices;
    public List<Vector3> normals;
    //public List<int> triangles;
    public int tcount;

    public MeshData()
    {
        vertices = new List<Vector3>();
        normals = new List<Vector3>();
        //triangles = new List<int>();
        tcount = 0;
    }

    public MeshData(List<Vector3> vertices, List<Vector3> normals, List<int> triangles, int tcount)
    {
        this.vertices = vertices;
        this.normals = normals;
        //this.triangles = triangles;
        this.tcount = tcount;
    }

    public void mergeMeshData(MeshData meshData)
    {
        this.tcount += meshData.tcount;
        this.vertices.AddRange(meshData.vertices);
        this.normals.AddRange(meshData.normals);
        //this.triangles.AddRange(meshData.triangles);
    }

    public Mesh createMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.triangles = Enumerable.Range(0, vertices.Count).ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }
}
