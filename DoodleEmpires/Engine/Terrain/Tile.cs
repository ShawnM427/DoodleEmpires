using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoodleEmpires.Engine.Terrain
{
    public struct Tile
    {
        public byte Type;
        public short TextureID;
        public RenderType RenderType;
        public bool Solid;

        public Tile(byte type, short textureID, RenderType renderType = RenderType.Land, bool solid = false)
        {
            Type = type;
            TextureID = textureID;
            RenderType = renderType;
            Solid = solid;
        }
    }

    public enum RenderType
    {
        Land,
        Prop,
        None
    }
}
