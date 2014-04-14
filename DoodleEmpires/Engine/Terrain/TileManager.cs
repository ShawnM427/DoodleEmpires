using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DoodleEmpires.Engine.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DoodleEmpires.Engine.Terrain
{
    public class TileManager
    {
        List<Tile> _tileTypes = new List<Tile>();
        bool[,] _connections = new bool[256,256];

        public TileManager()
        {
            RegisterTile(0, RenderType.None, false); //adds the air tile type
        }

        public byte RegisterTile(short texID, RenderType renderType = RenderType.Land, bool solid = false)
        {
            Tile tile = new Tile((byte)_tileTypes.Count, texID, renderType, solid);
            tile.Type = (byte)_tileTypes.Count;
            _tileTypes.Add(tile);
            return tile.Type;
        }

        public byte RegisterTile(short texID,  Color Color, RenderType renderType = RenderType.Land, bool solid = false)
        {
            Tile tile = new Tile((byte)_tileTypes.Count, texID, renderType, solid) { Color = Color };
            tile.Type = (byte)_tileTypes.Count;
            _tileTypes.Add(tile);
            return tile.Type;
        }

        public void RenderTile(SpriteBatch spriteBatch, Rectangle bounds, TextureAtlas atlas, byte mooreState, byte tileID)
        {
            switch (_tileTypes[tileID].RenderType)
            {
                case RenderType.Land:
                    spriteBatch.Draw(atlas.Texture, bounds, atlas.GetSource(_tileTypes[tileID].TextureID + mooreState), _tileTypes[tileID].Color);
                    break;
                case RenderType.Prop:
                    spriteBatch.Draw(atlas.Texture, bounds, atlas.GetSource(_tileTypes[tileID].TextureID), _tileTypes[tileID].Color);
                    break;
            }
        }

        public bool IsSolid(byte tileID)
        {
            return _tileTypes[tileID].Solid;
        }

        public bool CanConnect(byte sourceID, byte destinationID)
        {
            return _connections[sourceID, destinationID];
        }

        public void RegisterConnect(byte sourceID, byte destinationID, bool canConnect = true)
        {
            _connections[sourceID, destinationID] = canConnect;
            if (canConnect)
                _connections[destinationID, sourceID] = canConnect;
        }
    }
}
