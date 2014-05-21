using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoodleEmpires.Engine.Terrain
{
    [Obsolete("Volcanic formation cannot be used on the new voxel terrain system.")]
    public class VolcanicFormation
    {
        static int VolcanoPerChunk = 4;
        static int InitialVolcanoSize = 128;
        static Random Random = new Random();

        public static float[] GenMap(int length, int passes, int smoothing = 1)
        {
            float[] map = new float[length];

            int vSize = InitialVolcanoSize;

            for (int p = 0; p < passes; p++)
            {
                vSize /= 2;

                for (int i = 0; i < (length) / 16 * VolcanoPerChunk; i++)
                {
                    int height = Random.Next(vSize);

                    int pos = Random.Next(length);

                    int left = pos - height / 2;
                    int right = pos + height / 2;

                    bool opp = Random.Next(2) == 1;

                    left = left < 0 ? 0 : left;
                    right = right > length ? length : right;

                    for (int x = left; x < right; x++)
                    {
                        map[x] += opp ? height - Math.Abs(pos - x) : - height - Math.Abs(pos - x);
                    }
                }
            }

            map.Smooth(smoothing);

            return map;
        }
    }
}
