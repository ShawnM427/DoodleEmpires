using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DoodleEmpires.Engine.Utilities;

namespace DoodleEmpires.Engine.Terrain
{
    /// <summary>
    /// Represents a tile type
    /// </summary>
    public class Tile
    {
        /// <summary>
        /// The tile ID for this tile type
        /// </summary>
        public byte Type;
        /// <summary>
        /// The basic texture ID for this tile
        /// </summary>
        public short TextureID;
        /// <summary>
        /// The render tye for this tile
        /// </summary>
        public RenderType RenderType;
        /// <summary>
        /// True if this tile type is solid
        /// </summary>
        public bool Solid;
        /// <summary>
        /// The color multiplier for this tile type
        /// </summary>
        public Color Color;
        /// <summary>
        /// True if this tile type needs to be updated
        /// </summary>
        public virtual bool NeedsUpdate
        {
            get { return false; }
        }
        /// <summary>
        /// True if this tile type is climable
        /// </summary>
        public virtual bool Climable
        {
            get { return false; }
        }

        /// <summary>
        /// Creates a new instance of a tile type
        /// </summary>
        /// <param name="type">The type ID for this tile</param>
        /// <param name="textureID">The base texture ID for this tile type</param>
        /// <param name="renderType">The render type for this tile</param>
        /// <param name="solid">True if the tile is solid</param>
        public Tile(byte type, short textureID, RenderType renderType = RenderType.Land, bool solid = false)
        {
            Type = type;
            TextureID = textureID;
            RenderType = renderType;
            Solid = solid;
            Color = Color.White;
        }

        /// <summary>
        /// Draws this tile
        /// </summary>
        /// <param name="spriteBatch">The spritebatch to draw with</param>
        /// <param name="atlas">The texture atlas to use</param>
        /// <param name="bounds">The bounds to render in</param>
        /// <param name="mooreState">The moore neighbours state</param>
        /// <param name="meta">The block's meta data</param>
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

        /// <summary>
        /// Checks if this tile intersects a rectangle
        /// </summary>
        /// <param name="check">The rectangle to check</param>
        /// <param name="bounds">The current bounds of the tile</param>
        /// <returns>True if the tile intersects the checking rectangle</returns>
        public virtual bool Intersects(Rectangle check, Rectangle bounds)
        {
            return check.Intersects(bounds);
        }

        /// <summary>
        /// Handles adding this tile type to the world
        /// </summary>
        /// <param name="world">The world to add to</param>
        /// <param name="x">The x-coord of the tile to set</param>
        /// <param name="y">The y-coord of the tile to set</param>
        public virtual void AddToWorld(VoxelMap world, int x, int y)
        {
            world.SetTile(x, y, Type);
            world.SetMeta(x, y, 0);
        }

        /// <summary>
        /// Handles removing this tile type from the world
        /// </summary>
        /// <param name="world">The world to remove from</param>
        /// <param name="x">The x-coord of the tile to remove</param>
        /// <param name="y">The y-coord of the tile to remove</param>
        public virtual void RemovedFromWorld(VoxelMap world, int x, int y)
        {
            world.SetTile(x, y, 0);
            world.SetMeta(x, y, 0);
        }

        /// <summary>
        /// Called when a tile of this type is updated
        /// </summary>
        /// <param name="map">The world that this tile exists in</param>
        /// <param name="x">The x-coordinate in block coords</param>
        /// <param name="y">The y-coordinate in block coords</param>
        public virtual void OnTick(VoxelMap map, int x, int y) { }
    }

    /// <summary>
    /// Represents how the tile should be rendered
    /// </summary>
    public enum RenderType
    {
        /// <summary>
        /// This tile uses no special rendering, simple tile based rendering
        /// </summary>
        None,
        /// <summary>
        /// This tile uses standard land rendering
        /// </summary>
        Land,
        /// <summary>
        /// This tile uses prop style rendering
        /// </summary>
        Prop
    }
}
