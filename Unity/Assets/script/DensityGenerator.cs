using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DensityGenerator
{
    public static FastNoise noise = new FastNoise();

    //density function
    public static float density(float k, float x, float y, float z)
    {

        if (y > 100)
            return 1;
        if (y < -100)
            return 1;

        return k + noise.GetPerlin(x * 2, y * 2, z * 2) - (noise.GetPerlin(x * 4, y * 4, z * 4) * 0.5f) - (noise.GetPerlin(x * 8, y * 8, z * 8) * 0.25f);
    }

    //marching algorithm
    public static float [,,] find(float[,,] block, float floor, float size, Vector3 chunkPos)
    {
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
