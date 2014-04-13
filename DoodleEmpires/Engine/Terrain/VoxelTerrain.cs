using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using DoodleEmpires.Engine.Utilities;
using Microsoft.Xna.Framework;
using System.IO;

namespace DoodleEmpires.Engine.Terrain
{
    public class VoxelTerrain
    {
        public const int TILE_WIDTH = 16;
        public const int TILE_HEIGHT = 16;

        protected Rectangle[,] _voxelBounds;
        protected byte[,] _tiles;
        protected byte[,] _neighbourStates;
        protected int _width;
        protected int _height;
        int _maxX;
        int _maxY;

        protected GraphicsDevice _graphics;
        protected SpriteBatch _spriteBatch;

        protected TextureAtlas _atlas;
        protected TileManager _tileManager;

        protected Color _transformColor = Color.White;

        public byte this[int x, int y]
        {
            get
            {
                return _tiles[x, y];
            }
            set
            {
                _tiles[x, y] = value;
                UpdateVoxel(x, y);
            }
        }

        public VoxelTerrain(GraphicsDevice Graphics, TileManager tileManager, TextureAtlas atlas, int width, int height)
        {
            _tiles = new byte[width, height];
            _neighbourStates = new byte[width, height];
            _width = width;
            _height = height;

            _maxX = width - 1;
            _maxY = height - 1;

            _graphics = Graphics;
            _spriteBatch = new SpriteBatch(Graphics);
            
            for (int x = 0; x < 800; x++)
            {
                for (int y = 200; y < 400; y++)
                {
                    _tiles[x, y] = 1;
                    _neighbourStates[x, y] = (byte)(MooreNeighbours.R | MooreNeighbours.BM | MooreNeighbours.L | MooreNeighbours.TM);
                }
                _neighbourStates[x, 199] = (byte)(MooreNeighbours.R | MooreNeighbours.BM | MooreNeighbours.L); ;
            }

            _atlas = atlas;
            _tileManager = tileManager;
            BuildVoxelBouds();
        }

        protected virtual void BuildVoxelBouds()
        {
            _voxelBounds = new Rectangle[_width, _height];

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    _voxelBounds[x, y] = new Rectangle(x * TILE_WIDTH, y * TILE_HEIGHT, TILE_WIDTH, TILE_HEIGHT);
                }
            }
        }

        protected virtual void UpdateVoxel(int x, int y, bool doNeighbours = true)
        {
            if (IsSolid(x, y))
            {
                MooreNeighbours neighbours = GetNeighbours(x, y);
                
                _neighbourStates[x, y] = (byte)neighbours;
            }

            if (doNeighbours)
            {
                UpdateVoxel(x - 1, y, false);
                UpdateVoxel(x + 1, y, false);
                UpdateVoxel(x, y - 1, false);
                UpdateVoxel(x, y + 1, false);
            }
        }

        protected MooreNeighbours GetNeighbours(int x, int y)
        {
            return (MooreNeighbours)(0 +
                //(IsSolid(x - 1, y - 1) ? MooreNeighbours.TL : 0) |
                (CanConnect(GetMaterial(x, y), GetMaterial(x, y - 1)) ? MooreNeighbours.TM : 0) |
                //(IsSolid(x + 1, y - 1) ? MooreNeighbours.TR : 0) |
                (CanConnect(GetMaterial(x, y), GetMaterial(x - 1, y)) ? MooreNeighbours.L : 0) |
                (CanConnect(GetMaterial(x, y), GetMaterial(x + 1, y)) ? MooreNeighbours.R : 0) |
                //(IsSolid(x - 1, y + 1) ? MooreNeighbours.BL : 0) |
                (CanConnect(GetMaterial(x, y), GetMaterial(x, y + 1)) ? MooreNeighbours.BM : 0)); //|
                //(IsSolid(x + 1, y + 1) ? MooreNeighbours.BR : 0));
        }

        protected bool IsSolid(int x, int y)
        {
            return x >= 0 & x < _width & y >= 0 & y < _height ? _tileManager.IsSolid(_tiles[x, y]) : false;
        }

        protected byte GetMaterial(int x, int y)
        {
            return x >= 0 & x < _width & y >= 0 & y < _height ? _tiles[x, y] : (byte)0;
        }

        protected bool CanConnect(byte sourceID, byte destinationID)
        {
            return _tileManager.CanConnect(sourceID, destinationID);
        }

        public virtual void SetTileSafe(int x, int y, byte id)
        {
            if (x >= 0 & x < _width & y >= 0 & y < _height)
                this[x, y] = id;
        }

        int _MINX, _MINY, _MAXX, _MAXY;
        public virtual void Render(ICamera2D camera)
        {
            _spriteBatch.Begin(SpriteSortMode.Texture, null, SamplerState.PointClamp, null, null, null, camera.Transform);
            _MINX = camera.ScreenBounds.X / TILE_WIDTH - 1;
            _MINY = camera.ScreenBounds.Y / TILE_HEIGHT - 1;
            _MAXX = camera.ScreenBounds.Right / TILE_WIDTH + 1;
            _MAXY = camera.ScreenBounds.Bottom / TILE_HEIGHT + 1;

            _MINX = _MINX < 0 ? 0 : _MINX;
            _MINY = _MINY < 0 ? 0 : _MINY;
            _MAXX = _MAXX > _maxX ? _maxX : _MAXX;
            _MAXY = _MAXY > _maxY ? _maxY : _MAXY;

            for (int y = _MINY; y <= _MAXY; y++)
            {
                for (int x = _MINX; x <= _MAXX; x++)
                {
                    _tileManager.RenderTile(_spriteBatch, _voxelBounds[x, y], _atlas, _neighbourStates[x, y], _tiles[x, y]);
                }
            }

            _spriteBatch.End();
        }

        public void SaveToStream(Stream stream)
        {
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write("Doodle Empires Voxel Terrain ");
            writer.Write("0.0.1");

            writer.Write(_width);
            writer.Write(_height);

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    writer.Write(_tiles[x, y]);
                    writer.Write(_neighbourStates[x, y]);
                }
            }

            writer.Dispose();
        }

        public static VoxelTerrain ReadFromStream(Stream stream, GraphicsDevice graphics, TileManager tileManager, TextureAtlas atlas)
        {
            BinaryReader reader = new BinaryReader(stream);

            string header = reader.ReadString();
            string version = reader.ReadString();

            switch (version)
            {
                case "0.0.1":
                    return LoadVersion_0_0_1(reader, graphics, tileManager, atlas);
                default:
                    reader.Dispose();
                    return null;
            }

        }

        private static VoxelTerrain LoadVersion_0_0_1(BinaryReader reader, GraphicsDevice graphics, TileManager tileManager, TextureAtlas atlas)
        {
            int width = reader.ReadInt32();
            int height = reader.ReadInt32();

            VoxelTerrain terrain = new VoxelTerrain(graphics, tileManager, atlas, width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    terrain._tiles[x, y] = reader.ReadByte();
                    terrain._neighbourStates[x, y] = reader.ReadByte();
                }
            }

            reader.Dispose();
            return terrain;
        }
    }

    [Flags]
    public enum MooreNeighbours : byte
    {
        None = 0,
        TM = 1 << 0,
        L = 1 << 1,
        R = 1 << 2,
        BM = 1 << 3,
    }

    public static class MooreNeighbourExtensions
    {
        public static byte Value(this MooreNeighbours value)
        {
            return (byte)value;
        }

        public static MooreNeighbours ToMooreNeighbours(this byte value)
        {
            return (MooreNeighbours)value;
        }
    }
}
