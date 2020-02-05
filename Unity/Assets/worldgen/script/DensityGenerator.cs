using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DensityGenerator
{
    public static FastNoise noise = new FastNoise();

    public static uint octaveNumber;
    public static float floor;
    
    //density function
    public static float density(float k, float x, float y, float z)
    {
        if (y > 100)
            return 0;
        if (y < -100)
            return 1;

        float densityValue = k;
        int octaveScale = 1;
        float octaveIntensity = 1.0f;

        for (int i = 0; i < octaveNumber; i++)
        {
            densityValue += noise.GetPerlin(x * octaveScale, y * octaveScale, z * octaveScale) * octaveIntensity;
            octaveIntensity *= 0.5f;
            octaveScale *= 2;
        }

        return densityValue;
    }

    //marching algorithm
    public static float [,,] find(float size, Vector3 chunkPos)
    {
        float[,,] block = new float[(int)size, (int)size, (int)size];
        noise.SetNoiseType(FastNoise.NoiseType.Perlin);

        for(int x = 0; x < size; x++)
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
}
