using DoodleEmpires.Engine.Terrain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoodleEmpires.Engine.Net;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using DoodleEmpires.Engine.Utilities;

namespace SmileyFaceWars.Engine
{
    /// <summary>
    /// Represents a terrain tile
    /// </summary>
    public class TerrainTile : Tile
    {
        Dictionary<int, int> _textureMapping = new Dictionary<int, int>()
        {
            {0, 19},
            {1, 19},
            {2, 19},
            {3, 18},
            {4, 19},
            {5, 16},
            {6, 19},
            {7, 17},
            {8, 19},
            {9, 19},
            {10, 2},
            {11, 10},
            {12, 0},
            {13, 8},
            {14, 1},
            {15, 9},
            {16, 12},
            {17, 11},
            {18, 4},
            {19, 3}
        };

        /// <summary>
        /// Creates a new wood spike tile type
        /// </summary>
        /// <param name="type">The ID of the tile</param>
        public TerrainTile(byte type, short texID)
            : base(type, texID, RenderType.Prop, false)
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
            ImmediateNeighbours neighbours = (ImmediateNeighbours)mooreState;

            spriteBatch.Draw(atlas.Texture, bounds, atlas.GetSource(_textureMapping[mooreState] + TextureID), Color);
        }
    }
}
