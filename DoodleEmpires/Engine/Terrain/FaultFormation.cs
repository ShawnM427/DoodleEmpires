using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoodleEmpires.Engine.Terrain
{
    public static class FaultFormation
    {
        public static int FaultsPerChunk = 4;
        public static int InitialPassScale = 256;

        public static float VolcanoChance = 0.9F;
        public static int MaxVolcanoHeight = 300;
        public static int MinVolcanoHeight = 100;
        static Random Random = new Random();

        public static float[] GenMap(int length, int passes, int smoothing = 1)
        {
            float[] map = new float[length];

            for (int p = 0; p < passes; p++)
            {
                for (int i = 0; i < FaultsPerChunk * length / 128; i++)
                {
                    int x = Random.Next(length);
                    float scale = Random.Next(InitialPassScale / passes);
                    bool opp = Random.Next(2) == 1;

                    for (int x1 = 0; x1 < length; x1++)
                    {
                        if (opp)
                            map[x1] += x1 < x ? -scale : scale;
                        else
                            map[x1] += x1 > x ? -scale : scale;
                    }
                }
            }


            map.Smooth(smoothing);

            return map;
        }
    }
}
