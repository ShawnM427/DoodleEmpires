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
}
