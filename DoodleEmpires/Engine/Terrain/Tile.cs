using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DoodleEmpires.Engine.Terrain
{
    public struct Tile
    {
        public byte Type;
        public short TextureID;
        public RenderType RenderType;
        public bool Solid;
        public Color Color;

        public Tile(byte type, short textureID, RenderType renderType = RenderType.Land, bool solid = false)
        {
            Type = type;
            TextureID = textureID;
            RenderType = renderType;
            Solid = solid;
            Color = Color.White;
        }
    }

    public enum RenderType
    {
        Land,
        Prop,
        None
    }
}
