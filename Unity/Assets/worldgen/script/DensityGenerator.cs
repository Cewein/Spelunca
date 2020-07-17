using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DensityGenerator
{
    public int octave;
    public float lacunarity;
    public float persistence;
    public float isoLevel;
    public float spawnSize;
    public float bossSize;
    public float tunnelSize;
    public int size;
    public Vector3 endZone;
    public Vector3 playerSpawn;
    public Vector3 playerPos; //not use only for data saving
    public float seed;
    public float precision;

    //use a compute buffer and un compute shader to generate the 
    //density array for the world gen
    //it's done on the gpu (see shader/densityShader.copute)
    public void find(ComputeBuffer pointsBuffer, int size, Vector3 chunkPos, ComputeShader densityShader)
    {
        int numThreadEachAxis = Mathf.CeilToInt(size / 8.0f);

        densityShader.SetBuffer(0, "points", pointsBuffer);
        densityShader.SetInt("numPointAxis", size);
        densityShader.SetVector("chunkPos", chunkPos);
        densityShader.SetVector("chunkCoord", (chunkPos + Vector3.one) / (size - 1));
        densityShader.SetFloat("precision", precision);

        densityShader.SetVector("playerSpawn", playerSpawn);
        densityShader.SetVector("endZone", endZone);

        densityShader.SetFloat("lacunarity", lacunarity);
        densityShader.SetFloat("persistence", persistence);
        densityShader.SetFloat("seed", seed);

        densityShader.SetFloat("spawnSize", spawnSize);
        densityShader.SetFloat("bossSize", bossSize);
        densityShader.SetFloat("tunnelSize", tunnelSize);

        densityShader.SetInt("octave", octave);


        densityShader.Dispatch(0, numThreadEachAxis, numThreadEachAxis, numThreadEachAxis);

    }
}
