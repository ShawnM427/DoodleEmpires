using System;
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
        public virtual bool NeedsUpdate
        {
            get { return false; }
        }
        public virtual bool Climable
        {
            get { return false; }
        }

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

        public virtual bool Intersects(Rectangle check, Rectangle bounds)
        {
            return check.Intersects(bounds);
        }

        public virtual void AddToWorld(VoxelMap world, int x, int y)
        {
            world.SetTile(x, y, Type);
            world.SetMeta(x, y, 0);
        }

        public void RemovedFromWorld(VoxelMap world, int x, int y)
        {
            world.SetTile(x, y, 0);
            world.SetMeta(x, y, 0);
        }

        public virtual void OnTick(VoxelMap map, int x, int y) { }
    }

    public enum RenderType
    {
        Land,
        Prop,
        None
    }
}
