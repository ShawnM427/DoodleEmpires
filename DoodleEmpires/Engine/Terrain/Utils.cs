using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DoodleEmpires.Engine.Terrain
{
    public class PerlinNoise1D
    {
        /// <summary>
        /// Gets or sets how rough the terrain is
        /// </summary>
        public static float Persistance = 0.25F;
        public static int Octaves = 3;
        public static float Amplitude = 100.0F;

        static Random rand = new Random();

        static float Noise1(int x)
        {
            x = (x<<13) ^ x;
            return ( 1.0F - ( (x * (x * x * 15731 + 789221) + 1376312589) & int.MaxValue) / 1073741824.0F);    

        }

        static float SmoothedNoise1(int x)
        {
            return Noise1(x) / 2 + Noise1(x - 1) / 4 + Noise1(x + 1) / 4;
        }

        static float InterpolatedNoise1(float x)
        {
            int iX = (int)x;
            float fX = x - iX;

            float v1 = SmoothedNoise1(iX);
            float v2 = SmoothedNoise1(iX + 1);

            return MathHelper.Lerp(v1, v2, fX);

        }

        public static float PerlinNoise_1D(float x)
        {
            float total = 0;
            float p = Persistance;
            int n = Octaves - 1;

            for (int i = 0; i < n; i++)
            {
                int frequency = 2 * i;
                float amplitude = p * i;

                total = total + InterpolatedNoise1(x * frequency) * amplitude;
            }

            return total * Amplitude;
        }
    }

    public static class HeightmapExtensions
    {
        public static void Smooth(this float[] heightmap, int passes = 1)
        {
            for (int p = 0; p < passes; p++)
            {
                for (int i = 1; i < heightmap.Length - 1; i++)
                {
                    heightmap[i] = heightmap[i] / 2 + heightmap[i - 1] / 4 + heightmap[i + 1] / 4;
                }
            }
        }

        public static void Smooth(this float[] heightmap, int minX, int maxX, int passes = 1)
        {
            minX = minX < 0 ? 0 : minX <= heightmap.Length ? minX : heightmap.Length - 1;
            maxX = maxX >= heightmap.Length ? heightmap.Length - 1 : maxX;

            for (int p = 0; p < passes; p++)
            {
                for (int i = minX + 1; i < maxX; i++)
                {
                    heightmap[i] = heightmap[i] / 2 + heightmap[i - 1] / 4 + heightmap[i + 1] / 4;
                }
            }
        }
    }
}
