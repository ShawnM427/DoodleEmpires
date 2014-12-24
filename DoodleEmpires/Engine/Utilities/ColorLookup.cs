using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleEmpires.Engine
{
    public class ColorLookup
    {
        Color[] _samples;

        public ColorLookup(Color[] samples)
        {
            _samples = samples;
        }

        public Color Sample(float percent)
        {
            int minID = (int)(_samples.Length * percent);
            float mid = (percent - ((1.0f / _samples.Length) * (float)minID)) * 100.0f;

            if (minID < _samples.Length - 1)
                return Color.Lerp(_samples[minID], _samples[minID + 1], mid);
            else
                return _samples.Last();
        }
    }
}
