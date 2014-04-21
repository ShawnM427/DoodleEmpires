﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DoodleEmpires.Engine.Utilities;

namespace DoodleEmpires.Engine.Terrain
{
    public class Tile
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

        public virtual void Draw(SpriteBatch spriteBatch, TextureAtlas atlas, Rectangle bounds, byte mooreState, byte meta)
        {
            switch (RenderType)
            {
                case RenderType.Land:
                    spriteBatch.Draw(atlas.Texture, bounds, atlas.GetSource(TextureID + mooreState), Color);
                    break;
                case RenderType.Prop:
                    spriteBatch.Draw(atlas.Texture, bounds, atlas.GetSource(TextureID), Color);
                    break;
            }
        }

        public bool Intersects(Rectangle check, Rectangle bounds)
        {
            return check.Intersects(bounds);
        }
    }

    public enum RenderType
    {
        Land,
        Prop,
        None
    }
}
