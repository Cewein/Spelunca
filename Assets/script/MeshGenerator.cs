using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator
{
    public struct gridcell
    {
        public Vector3[] p;
        public float[] val;
    }

    public static MeshData generateMesh(float[,,] block, int x, int y, int z, uint size, float isolevel, int tcount)
    {

        gridcell grid = new gridcell();
        grid.p = new Vector3[8];
        grid.val = new float[8];

        grid.val[0] = block[x, y, z];
        grid.val[1] = block[x + 1, y, z];
        grid.val[2] = block[x + 1, y, z + 1];
        grid.val[3] = block[x, y, z + 1];

        grid.val[4] = block[x, y + 1, z];
        grid.val[5] = block[x + 1, y + 1, z];
        grid.val[6] = block[x + 1, y + 1, z + 1];
        grid.val[7] = block[x, y + 1, z + 1];

        grid.p[0] = new Vector3(x, y, z);
        grid.p[1] = new Vector3(x + 1, y, z);
        grid.p[2] = new Vector3(x + 1, y, z + 1);
        grid.p[3] = new Vector3(x, y, z + 1);
        grid.p[4] = new Vector3(x, y + 1, z);
        grid.p[5] = new Vector3(x + 1, y + 1, z);
        grid.p[6] = new Vector3(x + 1, y + 1, z + 1);
        grid.p[7] = new Vector3(x, y + 1, z + 1);

        return polygonise(grid, isolevel, tcount);
                
    }

    private static MeshData polygonise(gridcell grid, float isolevel, int tcount)
    {
        int cubeindex;
        Vector3[] vertlist = new Vector3[12];

        cubeindex = 0;
        if (grid.val[0] < isolevel) cubeindex |= 1;
        if (grid.val[1] < isolevel) cubeindex |= 2;
        if (grid.val[2] < isolevel) cubeindex |= 4;
        if (grid.val[3] < isolevel) cubeindex |= 8;
        if (grid.val[4] < isolevel) cubeindex |= 16;
        if (grid.val[5] < isolevel) cubeindex |= 32;
        if (grid.val[6] < isolevel) cubeindex |= 64;
        if (grid.val[7] < isolevel) cubeindex |= 128;

        if ((LookUpTable.edgeTable[cubeindex] & 1) != 0)
            vertlist[0] =
               VertexInterp(isolevel, grid.p[0], grid.p[1], grid.val[0], grid.val[1]);
        if ((LookUpTable.edgeTable[cubeindex] & 2) != 0)
            vertlist[1] =
               VertexInterp(isolevel, grid.p[1], grid.p[2], grid.val[1], grid.val[2]);
        if ((LookUpTable.edgeTable[cubeindex] & 4) != 0)
            vertlist[2] =
               VertexInterp(isolevel, grid.p[2], grid.p[3], grid.val[2], grid.val[3]);
        if ((LookUpTable.edgeTable[cubeindex] & 8) != 0)
            vertlist[3] =
               VertexInterp(isolevel, grid.p[3], grid.p[0], grid.val[3], grid.val[0]);
        if ((LookUpTable.edgeTable[cubeindex] & 16) != 0)
            vertlist[4] =
               VertexInterp(isolevel, grid.p[4], grid.p[5], grid.val[4], grid.val[5]);
        if ((LookUpTable.edgeTable[cubeindex] & 32) != 0)
            vertlist[5] =
               VertexInterp(isolevel, grid.p[5], grid.p[6], grid.val[5], grid.val[6]);
        if ((LookUpTable.edgeTable[cubeindex] & 64) != 0)
            vertlist[6] =
               VertexInterp(isolevel, grid.p[6], grid.p[7], grid.val[6], grid.val[7]);
        if ((LookUpTable.edgeTable[cubeindex] & 128) != 0)
            vertlist[7] =
               VertexInterp(isolevel, grid.p[7], grid.p[4], grid.val[7], grid.val[4]);
        if ((LookUpTable.edgeTable[cubeindex] & 256) != 0)
            vertlist[8] =
               VertexInterp(isolevel, grid.p[0], grid.p[4], grid.val[0], grid.val[4]);
        if ((LookUpTable.edgeTable[cubeindex] & 512) != 0)
            vertlist[9] =
               VertexInterp(isolevel, grid.p[1], grid.p[5], grid.val[1], grid.val[5]);
        if ((LookUpTable.edgeTable[cubeindex] & 1024) != 0)
            vertlist[10] =
               VertexInterp(isolevel, grid.p[2], grid.p[6], grid.val[2], grid.val[6]);
        if ((LookUpTable.edgeTable[cubeindex] & 2048) != 0)
            vertlist[11] =
               VertexInterp(isolevel, grid.p[3], grid.p[7], grid.val[3], grid.val[7]);

        List<Vector3> vertices = new List<Vector3>();
        List<int> indices = new List<int>();
        int tcounttemp = 0;

        for(int i = 0; LookUpTable.triTable[cubeindex,i] != -1; i +=3)
        {
            vertices.Add(vertlist[LookUpTable.triTable[cubeindex,i + 2]]);
            vertices.Add(vertlist[LookUpTable.triTable[cubeindex,i + 1]]);
            vertices.Add(vertlist[LookUpTable.triTable[cubeindex,i]]);
        }

        return new MeshData(vertices, indices, tcounttemp);
    }

    private static Vector3 VertexInterp(float isolevel, Vector3 p1, Vector3 p2, float valp1, float valp2)
    {
       float mu;
       Vector3 p;

        if (Mathf.Abs(isolevel - valp1) < 0.00001)
            return (p1);
        if (Mathf.Abs(isolevel - valp2) < 0.00001)
            return (p2);
        if (Mathf.Abs(valp1 - valp2) < 0.00001)
            return (p1);
        mu = (isolevel - valp1) / (valp2 - valp1);
        p.x = p1.x + mu * (p2.x - p1.x);
        p.y = p1.y + mu * (p2.y - p1.y);
        p.z = p1.z + mu * (p2.z - p1.z);

        return p;
    }
}

public class MeshData
{
    public List<Vector3> vertices;
    //public List<int> triangles;
    public int tcount;

    public MeshData()
    {
        vertices = new List<Vector3>();
        //triangles = new List<int>();
        tcount = 0;
    }

    public MeshData(List<Vector3> vertices, List<int> triangles, int tcount)
    {
        this.vertices = vertices;
       // this.triangles = triangles;
        this.tcount = tcount;
    }

    public void mergeMeshData(MeshData meshData)
    {
        this.tcount += meshData.tcount;
        this.vertices.AddRange(meshData.vertices);
        //this.triangles.AddRange(meshData.triangles);
    }



    public Mesh createMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = Enumerable.Range(0, vertices.Count).ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }
}
