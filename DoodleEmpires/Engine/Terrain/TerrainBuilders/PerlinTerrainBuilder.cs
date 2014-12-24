using DoodleEmpires.Engine.NoiseGeneration;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleEmpires.Engine.Terrain.TerrainBuilders
{
    public class PerlinTerrainBuilder : TerrainBuilder
    {
        public int TerrainHeight;
        public Color GroundColor = Color.Brown;
        public float SmoothFactor = 128.0f;
        public float FeatureSize = 50.0f;

        public PerlinTerrainBuilder(int seed, int terrainHeight = 200) : base(seed)
        {
            Noise.Seed = seed;
            Noise.Octaves = 8;
            Noise.Persistence = 0.08f;
            TerrainHeight = terrainHeight;
        }

        public override Color GetColorAtPosition(int x, int y)
        {
            int yy = (int)(((Noise.PerlinNoise_1D(x / (SmoothFactor + FeatureSize / 2)) + 1.0f) / 2.0f) * FeatureSize) + TerrainHeight;

            return y == yy ? Color.Black : 
                y > yy ? GroundColor : Color.TransparentBlack;
        }
    }
}
