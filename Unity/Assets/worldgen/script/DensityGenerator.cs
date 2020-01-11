using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DensityGenerator
{
    public static FastNoise noise = new FastNoise();
    
    public static float[] octaveScale;
    public static float[] octaveIntensity;
    
    //density function
    public static float density(float k, float x, float y, float z)
    {
        if (octaveScale.Length != octaveIntensity.Length)
        {
            return k;
        }
        if (y > 100)
            return 0;
        if (y < -100)
            return 1;
        float densityValue = k;
        for (int i = 0; i < octaveScale.Length; i++)
        {
            densityValue += noise.GetPerlin(x * octaveScale[i], y * octaveScale[i], z * octaveScale[i]) *
                            octaveIntensity[i];
        }
        
        return densityValue;
        }

    //marching algorithm
    public static float [,,] find(float floor, float size, Vector3 chunkPos)
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
