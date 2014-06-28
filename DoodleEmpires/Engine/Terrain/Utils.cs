using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DoodleEmpires.Engine.Terrain
{
    /// <summary>
    /// Some basic extansions for manipulating heightmap
    /// </summary>
    public static class HeightmapExtensions
    {
        /// <summary>
        /// Smooths a height map
        /// </summary>
        /// <param name="heightmap">The heightmap to smooth</param>
        /// <param name="passes">The number of passes to make, default 1</param>
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

        /// <summary>
        /// Smooths a height map
        /// </summary>
        /// <param name="heightmap">The heightmap to smooth</param>
        /// <param name="minX">The minimum x coord to smooth from</param>
        /// <param name="maxX">The maximum x coord to smooth from</param>
        /// <param name="passes">The number of passes to make, default 1</param>
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
