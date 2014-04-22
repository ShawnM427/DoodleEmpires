using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using DoodleEmpires.Engine.Utilities;
using Microsoft.Xna.Framework;

namespace DoodleEmpires.Engine.Terrain
{
    public class Leaves : Tile
    {
        public Leaves(byte type)
            : base(type, 40, RenderType.Land, false)
        {
            Color = Color.Green;
        }

        public override void Draw(SpriteBatch spriteBatch, TextureAtlas atlas, Rectangle bounds, byte mooreState, byte meta)
        {
            if (meta == 1)
            {
                spriteBatch.Draw(atlas.Texture, bounds, atlas.GetSource(80 + mooreState) ,Color);
            }
            else
                spriteBatch.Draw(atlas.Texture, bounds, atlas.GetSource(TextureID + mooreState), Color);
        }
    }

    public class WoodSpike : Tile
    {
        public WoodSpike(byte type)
            : base(type, 100, RenderType.Prop, false)
        {
            Color = Color.White;
        }

        public override void Draw(SpriteBatch spriteBatch, TextureAtlas atlas, Rectangle bounds, byte mooreState, byte meta)
        {
            MooreNeighbours neighbours = (MooreNeighbours)mooreState;

            if (neighbours.HasFlag(MooreNeighbours.TM))
                spriteBatch.Draw(atlas.Texture, bounds, atlas.GetSource(TextureID + 1), Color);
            else
                spriteBatch.Draw(atlas.Texture, bounds, atlas.GetSource(TextureID), Color);
        }
    }

    public class Ladder : Tile
    {
        public Ladder(byte type)
            : base(type, 102, RenderType.Prop, false)
        {
            Color = Color.White;
        }

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

    public class Door : Tile
    {
        public Door(byte type)
            : base(type, 105, RenderType.Prop, false)
        {
            Color = Color.White;
        }

        public override void Draw(SpriteBatch spriteBatch, TextureAtlas atlas, Rectangle bounds, byte mooreState, byte meta)
        {
            MooreNeighbours neighbours = (MooreNeighbours)mooreState;
            SpriteEffects flipped = meta >> 1 == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            if (neighbours.HasFlag(MooreNeighbours.BM))
                spriteBatch.Draw(atlas.Texture, bounds, atlas.GetSource(TextureID + 1 + (meta >> 7 << 7)), Color, 0f, Vector2.Zero, flipped, 0.0f);
            else
                spriteBatch.Draw(atlas.Texture, bounds, atlas.GetSource(TextureID + (meta >> 7 << 7)), Color, 0f, Vector2.Zero, flipped, 0.0f);
        }

        public override void AddToWorld(VoxelMap world, int x, int y)
        {
            world.SetTile(x, y, Type);
            world.SetMeta(x, y, 2);
            world.SetTile(x, y - 1, Type);
            world.SetMeta(x, y - 1, 2);
        }    
    }
}
