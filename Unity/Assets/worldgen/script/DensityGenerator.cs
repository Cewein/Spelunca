using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DensityGenerator
{
    public static FastNoise noise = new FastNoise();

    public static uint octaveNumber;
    public static float floor;
    public static Vector3 endZone;
    public static Vector3 playerSpawn;

    //density function
    //this is out put a value who is going to be use
    //in the mesh build procces
    public static float density(float k, float x, float y, float z)
    {
        if (y > 100)
            return 0;
        if (y < -100)
            return 1;

        //start zone
        float sphereStart = Vector3.Distance(playerSpawn, new Vector3(x, y, z));
        if (sphereStart < 20)
            return sphereStart - 20f;

        //end zone
        float sphereEnd = Vector3.Distance(endZone, new Vector3(x, y, z));
        if (sphereEnd < 50)
            return sphereEnd - 50f;

        //static tube need to be more flexible
        float tube1 = Vector3.Distance(new Vector3(x, y, z), new Vector3(Mathf.Sin(z / 10) * 6, Mathf.Cos(z / 10) * 6 - z / 6, z));
        float tube2 = Vector3.Distance(new Vector3(x, y, z), new Vector3(Mathf.Cos(z / 10) * 6, Mathf.Sin(z / 10) * 6 - z / 6, z));


        float densityValue = k;
        int octaveScale = 1;
        float octaveIntensity = 1.0f;

        for (int i = 0; i < octaveNumber; i++)
        {
            densityValue += noise.GetPerlin(x * octaveScale, y * octaveScale, z * octaveScale) * octaveIntensity;
            octaveIntensity *= 0.5f;
            octaveScale *= 2;
        }

        if (tube1 < 9 && densityValue > tube1 - 9.0f)
            return tube1 - 9.0f;
        if (tube2 < 9 && densityValue > tube2 - 9.0f)
            return tube2 - 9.0f;

        return densityValue;
    }

    //denstity algorithm loop, this will output
    //a 3D float array containing the density of the 
    //chunk, this array is use in the mesh build procces
    //and is  a crusial step in the marching algorithm
    public static float[,,] find(float size, Vector3 chunkPos)
    {
        float[,,] block = new float[(int)size, (int)size, (int)size];
        noise.SetNoiseType(FastNoise.NoiseType.Perlin);

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    block[x, y, z] = density(floor, x + chunkPos.x, y + chunkPos.y, z + chunkPos.z);
                }
            }
        }

        return block;
    }

    public static void find(ComputeBuffer pointsBuffer, int size, Vector3 chunkPos, ComputeShader densityShader)
    {
        int numThreadEachAxis = Mathf.CeilToInt(size / 8.0f);

        densityShader.SetBuffer(0, "points", pointsBuffer);
        densityShader.SetInt("numPointAxis", size);
        densityShader.SetVector("chunkPos", chunkPos);

        densityShader.Dispatch(0, numThreadEachAxis, numThreadEachAxis, numThreadEachAxis);

    }
}
