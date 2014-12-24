using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleEmpires.Engine
{
    public static class ExtraMath
    {
        public static int Wrap(this int value, int min, int max)
        {
            int range = max - min;

            while (value < min)
                value += range;
            while (value >= max)
                value -= range;

            return value;
        }

        public static Color Add(this Color source, Color destination)
        {
            return new Color(source.R + destination.R, source.G + destination.G, source.B + destination.B, source.A + destination.A);
        }

        public static Color Sub(this Color source, Color destination)
        {
            return new Color(source.R - destination.R, source.G - destination.G, source.B - destination.B, source.A - destination.A);
        }
    }
}
