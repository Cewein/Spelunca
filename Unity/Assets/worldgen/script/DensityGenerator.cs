using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DensityGenerator
{
    public static int octave;
    public static float lacunarity;
    public static float persistence;
    public static float isoLevel;
    public static float spawnSize;
    public static float bossSize;
    public static float tunnelSize;
    public static Vector3 endZone;
    public static Vector3 playerSpawn;
    public static float seed;
    public static float presicion;

    //use a compute buffer and un compute shader to generate the 
    //density array for the world gen
    //it's done on the gpu (see shader/densityShader.copute)
    public static void find(ComputeBuffer pointsBuffer, int size, Vector3 chunkPos, ComputeShader densityShader)
    {
        int numThreadEachAxis = Mathf.CeilToInt(size / 8.0f);

        densityShader.SetBuffer(0, "points", pointsBuffer);
        densityShader.SetInt("numPointAxis", size);
        densityShader.SetVector("chunkPos", chunkPos);
        densityShader.SetVector("chunkCoord", (chunkPos + Vector3.one) / (size - 1));
        densityShader.SetFloat("presicion", presicion);

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
