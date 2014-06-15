using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using DoodleEmpires.Engine.Utilities;
using Microsoft.Xna.Framework;
using DoodleEmpires.Engine.Net;

namespace DoodleEmpires.Engine.Terrain
{
    /// <summary>
    /// Represents the leaves tile
    /// </summary>
    public class Leaves : Tile
    {
        static Random _random = new Random();

        /// <summary>
        /// Gets whether this block needs updating.
        /// This is true for leaves.
        /// </summary>
        public override bool NeedsUpdate
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Creates a new instance of the leaves class, used for rendering tiles
        /// </summary>
        /// <param name="type">The typeID to assign to leaves</param>
        public Leaves(byte type)
            : base(type, 40, RenderType.Land, false)
        {
            Color = Color.Green;
        }

        /// <summary>
        /// Draws this tile
        /// </summary>
        /// <param name="spriteBatch">The spritebatch to draw with</param>
        /// <param name="atlas">The texture atlas to use</param>
        /// <param name="bounds">The bounds to render in</param>
        /// <param name="mooreState">The moore neighbours state</param>
        /// <param name="meta">The block's meta data</param>
        public override void Draw(SpriteBatch spriteBatch, TextureAtlas atlas, Rectangle bounds, byte mooreState, byte meta)
        {
            if (meta == 255)
            {
                spriteBatch.Draw(atlas.Texture, bounds, atlas.GetSource(80 + mooreState) ,Color);
            }
            else
                spriteBatch.Draw(atlas.Texture, bounds, atlas.GetSource(TextureID + mooreState), Color);
        }

        /// <summary>
        /// Handles adding this tile type to the world
        /// </summary>
        /// <param name="world">The world to add to</param>
        /// <param name="x">The x-coord of the tile to set</param>
        /// <param name="y">The y-coord of the tile to set</param>
        public override void AddToWorld(VoxelMap world, int x, int y)
        {
            base.AddToWorld(world, x, y);
            world.SetMeta(x, y, (byte)_random.Next(0, 256));
        }

        /// <summary>
        /// Called when a tile of this type is updated
        /// </summary>
        /// <param name="world">The world that this tile exists in</param>
        /// <param name="x">The x-coordinate in block coords</param>
        /// <param name="y">The y-coordinate in block coords</param>
        public override void OnTick(VoxelMap world, int x, int y)
        {
            byte meta = world.GetMeta(x, y);

            if (meta < 255)
            {
                meta += (byte)_random.Next(0, 2);
                world.SetMeta(x, y, meta);
            }
            if (meta == 255)
            {
                if (_random.NextDouble() < 0.01f)
                {
                    meta = 0;
                    world.SetMeta(x, y, meta);
                }
            }
        }
    }

    /// <summary>
    /// Represents a wooden spike tile
    /// </summary>
    public class WoodSpike : Tile
    {
        /// <summary>
        /// Creates a new wood spike tile type
        /// </summary>
        /// <param name="type">The ID of the tile</param>
        public WoodSpike(byte type)
            : base(type, 100, RenderType.Prop, false)
        {
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
        public override void Draw(SpriteBatch spriteBatch, TextureAtlas atlas, Rectangle bounds, byte mooreState, byte meta)
        {
            MooreNeighbours neighbours = (MooreNeighbours)mooreState;

            if (neighbours.HasFlag(MooreNeighbours.TM))
                spriteBatch.Draw(atlas.Texture, bounds, atlas.GetSource(TextureID + 1), Color);
            else
                spriteBatch.Draw(atlas.Texture, bounds, atlas.GetSource(TextureID), Color);
        }
    }

    /// <summary>
    /// Represents a ladder tile type
    /// </summary>
    public class Ladder : Tile
    {
        /// <summary>
        /// True if this tile type is climable
        /// </summary>
        public override bool Climable
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Creates a new ladder tile type
        /// </summary>
        /// <param name="type">The ID of the tile</param>
        public Ladder(byte type)
            : base(type, 102, RenderType.Prop, false)
        {
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
        public override void Draw(SpriteBatch spriteBatch, TextureAtlas atlas, Rectangle bounds, byte mooreState, byte meta)
        {
            MooreNeighbours neighbours = (MooreNeighbours)mooreState;

            if (neighbours.HasFlag(MooreNeighbours.L) && neighbours.HasFlag(MooreNeighbours.R))
                spriteBatch.Draw(atlas.Texture, bounds, atlas.GetSource(TextureID), Color);
            else if (neighbours.HasFlag(MooreNeighbours.L))
                spriteBatch.Draw(atlas.Texture, bounds, atlas.GetSource(TextureID + 1), Color);
            else if (neighbours.HasFlag(MooreNeighbours.R))
                spriteBatch.Draw(atlas.Texture, bounds, atlas.GetSource(TextureID + 2), Color);
            else
                spriteBatch.Draw(atlas.Texture, bounds, atlas.GetSource(TextureID), Color);
        }
    }

    /// <summary>
    /// Represents a door tile type
    /// </summary>
    public class Door : Tile
    {
        /// <summary>
        /// Creates a new door tile type
        /// </summary>
        /// <param name="type">The ID of the tile</param>
        public Door(byte type)
            : base(type, 105, RenderType.Prop, false)
        {
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
        public override void Draw(SpriteBatch spriteBatch, TextureAtlas atlas, Rectangle bounds, byte mooreState, byte meta)
        {
            MooreNeighbours neighbours = (MooreNeighbours)mooreState;
            SpriteEffects flipped = meta >> 1 == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            if (neighbours.HasFlag(MooreNeighbours.BM))
                spriteBatch.Draw(atlas.Texture, bounds, atlas.GetSource(TextureID + 1 + (meta >> 7 << 7)), Color, 0f, Vector2.Zero, flipped, 0.0f);
            else
                spriteBatch.Draw(atlas.Texture, bounds, atlas.GetSource(TextureID + (meta >> 7 << 7)), Color, 0f, Vector2.Zero, flipped, 0.0f);
        }

        /// <summary>
        /// Handles adding this tile type to the world
        /// </summary>
        /// <param name="world">The world to add to</param>
        /// <param name="x">The x-coord of the tile to set</param>
        /// <param name="y">The y-coord of the tile to set</param>
        public override void AddToWorld(VoxelMap world, int x, int y)
        {
            world.SetTile(x, y, Type);
            world.SetMeta(x, y, 1);
            world.SetTile(x, y - 1, Type);
            world.SetMeta(x, y - 1, 1);
        }

        /// <summary>
        /// Handles removing this tile type from the world
        /// </summary>
        /// <param name="world">The world to remove from</param>
        /// <param name="x">The x-coord of the tile to remove</param>
        /// <param name="y">The y-coord of the tile to remove</param>
        public override void RemovedFromWorld(VoxelMap world, int x, int y)
        {
            base.RemovedFromWorld(world, x, y);

            if (world[x, y - 1] == Type)
                world[x, y - 1] = 0;
            if (world[x, y + 1] == Type)
                world[x, y + 1] = 0;
        }
    }
}
