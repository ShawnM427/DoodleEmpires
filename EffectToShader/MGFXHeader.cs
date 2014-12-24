using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EffectToShader
{
    class MGFXHeader
    {
        public int Signature;
        public int Version;
        public int Profile;
        public int EffectKey;
        public byte HeaderSize;
    }
}
