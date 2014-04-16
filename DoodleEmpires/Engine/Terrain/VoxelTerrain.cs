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
    /// <summary>
    /// A terrain that is made up of small cubes, each having it's own texture and properties
    /// </summary>
    public class VoxelTerrain
    {
        /// <summary>
        /// Gets the width of a single voxel tile
        /// </summary>
        public const int TILE_WIDTH = 16;
        /// <summary>
        /// Gets the height of a single voxel tile
        /// </summary>
        public const int TILE_HEIGHT = 16;
        public const int TREE_DESNISTY = 1;

        #region Protected Vars
        /// <summary>
        /// An array holding all of the bounding rectangles to draw voxels in
        /// </summary>
        protected Rectangle[,] _voxelBounds;
        /// <summary>
        /// The materials for all the voxels
        /// </summary>
        protected byte[,] _tiles;
        /// <summary>
        /// The neighbour states for the corresponding voxels
        /// </summary>
        protected byte[,] _neighbourStates;
        /// <summary>
        /// The number of tiles along the x axis
        /// </summary>
        protected int _width;
        /// <summary>
        /// The number of tiles along the y axis
        /// </summary>
        protected int _height;

        /// <summary>
        /// The graphics device this terrain is bound to
        /// </summary>
        protected GraphicsDevice _graphics;
        /// <summary>
        /// The spritebatch this terrain uses for drawing
        /// </summary>
        protected SpriteBatch _spriteBatch;

        protected TextureAtlas _atlas;
        protected TileManager _tileManager;

        protected Random _random;

        /// <summary>
        /// The transform color to multiply all tiles by
        /// </summary>
        protected Color _transformColor = Color.White;
        #endregion

        int _maxX;
        int _maxY;

        /// <summary>
        /// Gets or sets the voxel material at the given x and y
        /// </summary>
        /// <param name="x">The x coordinate to set (chunk coords)</param>
        /// <param name="y">The y coordinate to set (chunk coords)</param>
        /// <returns>The voxel at (x, y)</returns>
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
        
        /// <summary>
        /// Creates a new voxel based terrain
        /// </summary>
        /// <param name="graphics">The graphics device to bind to</param>
        /// <param name="tileManager">The tile manager to use</param>
        /// <param name="atlas">The texture atlas to use</param>
        /// <param name="width">The width of the map, in tiles</param>
        /// <param name="height">The height of the map, in tiles</param>
        public VoxelTerrain(GraphicsDevice Graphics, TileManager tileManager, TextureAtlas atlas, int width, int height)
        {
            _random = new Random();
            _tiles = new byte[width, height];
            _neighbourStates = new byte[width, height];
            _width = width;
            _height = height;

            _maxX = width - 1;
            _maxY = height - 1;

            _graphics = Graphics;
            _spriteBatch = new SpriteBatch(Graphics);

            _atlas = atlas;
            _tileManager = tileManager;
            BuildVoxelBouds();
            
            float tHeight;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tHeight = Noise.PerlinNoise_1D(x / 16);
                    if (y > 200 + tHeight)
                    {
                        if (y > 200 + tHeight + 4)
                            _tiles[x, y] = 2;
                        else
                            _tiles[x, y] = 1;

                        _neighbourStates[x, y] = (byte)MooreNeighbours.All;
                    }
                }
            }
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (_tiles[x, y] != 0)
                    {
                        _neighbourStates[x, y] = (byte)GetNeighbours(x, y);
                    }
                }
            }

            for (int i = 0; i < TREE_DESNISTY * width / 32; i++)
            {
                int x = _random.Next(_width);
                GenTree(x, 200 + (int)Noise.PerlinNoise_1D(x / 16));
            }
        }

        /// <summary>
        /// Builds all the voxel bounds for this map
        /// </summary>
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

        /// <summary>
        /// Update the voxel at the given position
        /// </summary>
        /// <param name="x">The x co-ordinate to update (Chunk Coords)</param>
        /// <param name="y">The y co-ordinate to update (Chunk Coords)</param>
        /// <param name="doNeighbours">True if it should update it's neighbours</param>
        protected virtual void UpdateVoxel(int x, int y, bool doNeighbours = true)
        {
            if (IsNotAir(x, y))
            {
                MooreNeighbours neighbours = GetNeighbours(x, y);
                
                _neighbourStates[x, y] = (byte)neighbours;
            }

            if (doNeighbours)
            {
                UpdateVoxel(x - 1, y - 1, false);
                UpdateVoxel(x, y - 1, false);
                UpdateVoxel(x + 1, y - 1, false);
                UpdateVoxel(x - 1, y + 1, false);
                UpdateVoxel(x, y + 1, false);
                UpdateVoxel(x + 1, y + 1, false);
                UpdateVoxel(x - 1, y, false);
                UpdateVoxel(x + 1, y, false);
            }
        }

        /// <summary>
        /// Gets the neighbour state for the given position
        /// </summary>
        /// <param name="x">The x coord to get (chunk coords)</param>
        /// <param name="y">The y coord to get (chunk coords)</param>
        /// <returns>The moore neighbour state for the given block</returns>
        protected MooreNeighbours GetNeighbours(int x, int y)
        {
            byte cMaterial = GetMaterial(x,y);

            if (!IsNotAir(x - 1, y) && !IsNotAir(x, y - 1) && (IsNotAir(x - 1, y + 1) || IsNotAir(x + 1, y - 1)) &&
                 IsNotAir(x, y + 1) && IsNotAir(x + 1, y))
                return MooreNeighbours.Diag;

            if (!IsNotAir(x + 1, y) && !IsNotAir(x, y - 1) && (IsNotAir(x - 1, y - 1) || IsNotAir(x + 1, y + 1)) &&
                 IsNotAir(x, y + 1) && IsNotAir(x - 1, y))
                return MooreNeighbours.Diag + 1;

            if (!IsNotAir(x - 1, y) && !IsNotAir(x, y + 1) && (IsNotAir(x - 1, y - 1) || IsNotAir(x + 1, y + 1)) &&
                 IsNotAir(x, y - 1) && IsNotAir(x + 1, y))
                return MooreNeighbours.Diag + 2;

            if (!IsNotAir(x + 1, y) && !IsNotAir(x, y + 1) && (IsNotAir(x - 1, y + 1) || IsNotAir(x + 1, y - 1)) &&
                 IsNotAir(x, y - 1) && IsNotAir(x - 1, y))
                return MooreNeighbours.Diag + 3;

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

        /// <summary>
        /// Checks if the voxel the given position is solid
        /// </summary>
        /// <param name="x">The x co-ordinate to check (Chunk Coords)</param>
        /// <param name="y">The y co-ordinate to check (Chunk Coords)</param>
        /// <returns>True is the block at [x,y] is solid</returns>
        protected bool IsSolid(int x, int y)
        {
            return x >= 0 & x < _width & y >= 0 & y < _height ? _tileManager.IsSolid(_tiles[x, y]) : false;
        }

        /// <summary>
        /// Checks if the voxel the given position is not air
        /// </summary>
        /// <param name="x">The x co-ordinate to check (Chunk Coords)</param>
        /// <param name="y">The y co-ordinate to check (Chunk Coords)</param>
        /// <returns>True is the block at [x,y] is not air</returns>
        protected bool IsNotAir(int x, int y)
        {
            return x >= 0 & x < _width & y >= 0 & y < _height ? _tiles[x, y] != 0 : false;
        }

        /// <summary>
        /// Gets the voxel material at the given position
        /// </summary>
        /// <param name="x">The x co-ordinate to check (Chunk Coords)</param>
        /// <param name="y">The y co-ordinate to check (Chunk Coords)</param>
        /// <returns>The material at [x,y]</returns>
        protected byte GetMaterial(int x, int y)
        {
            return x >= 0 & x < _width & y >= 0 & y < _height ? _tiles[x, y] : (byte)0;
        }

        /// <summary>
        /// Checks if two voxel types can connect to each other
        /// </summary>
        /// <param name="sourceID">The source ID to check</param>
        /// <param name="destinationID">The destination ID to check</param>
        /// <returns>True if they can connect</returns>
        protected bool CanConnect(byte sourceID, byte destinationID)
        {
            return _tileManager.CanConnect(sourceID, destinationID);
        }

        /// <summary>
        /// Safely sets the voxel material at the given position
        /// </summary>
        /// <param name="x">The x co-ordinate to check (Chunk Coords)</param>
        /// <param name="y">The y co-ordinate to check (Chunk Coords)</param>
        /// <param name="id">The material to set</param>
        public virtual void SetTileSafe(int x, int y, byte id)
        {
            if (x >= 0 & x < _width & y >= 0 & y < _height)
                this[x, y] = id;
        }

        /// <summary>
        /// Generates a tree at the given x and y
        /// </summary>
        /// <param name="x">The x coordinate of the base of the tree</param>
        /// <param name="y">The y coordinate of the base of the tree</param>
        public void GenTree(int x, int y)
        {
            if (GetMaterial(x, y + 1) == 1)
            {
                int height = _random.Next(3, 7);
                int radius = _random.Next(3, 5);

                SetSphere(x, y - height - radius + 1, radius, 5); //generate leaves

                for (int yy = y; yy > y - height; yy--)
                {
                    for (int xx = x - radius; xx < x + radius; xx++ )
                        SetTileSafe(xx, yy, 0);

                    SetTileSafe(x, yy, 4);
                }

                if (_random.Next(0, 3) == 1) //33% chance of left root
                    SetTileSafe(x - 1, y, 4);
                if (_random.Next(0, 3) == 1) //33% chance of right root
                    SetTileSafe(x + 1, y, 4);
                if (_random.Next(0, 10) == 1) //10% chance of left branch
                    SetTileSafe(x - 1, y - height + 1, 4);
                if (_random.Next(0, 10) == 1) //10% chance of right branch
                    SetTileSafe(x + 1, y - height + 1, 4);
            }
        }

        /// <summary>
        /// Sets a sphere to one tile ID
        /// </summary>
        /// <param name="x">The x coord of the centre of the circle</param>
        /// <param name="y">The y coord of the centre of the circle</param>
        /// <param name="radius">The radius of the circle</param>
        /// <param name="value">The tile ID to set</param>
        public void SetSphere(int x, int y, int radius, byte value)
        {            
            for (int xx = x - radius - 1; xx < x + radius + 1; xx++)
            {
                for (int yy = y - radius - 1; yy < y + radius + 1; yy++)
                {
                    if (IsInsideOctagon(x, y, radius, xx, yy))
                        SetTileSafe(xx, yy, value);
                }
            }
        }

        /// <summary>
        /// Checks is a point is inside of a octagon
        /// </summary>
        /// <param name="xx">The x coordinate of the octagon's centre</param>
        /// <param name="yy">The y coordinate of the octagon's centre</param>
        /// <param name="radius">The radius of the octagon</param>
        /// <param name="x">The x coordinate to check</param>
        /// <param name="y">The y coordinate to check</param>
        /// <returns>True if the point is inside of the octagon</returns>
        public bool IsInsideOctagon(int xx, int yy, int radius, int x, int y)
        {
            int relX = x - xx;
            int relY = y - yy;

            //This simply determines the 
            int w = (int)(1.5f * radius);
            
            // check the easy ones first
            if (x < xx - radius) return false;
            if (x > xx + radius) return false;
            if (y < yy - radius) return false;
            if (y > yy + radius) return false;
            
            // then the diagonals
            if (relX + w <  relY)
                return false;
            if (relX - w > relY)
                return false;
            if ((relX - w) > -relY) 
                return false;
            if ((relX + w) < -relY) 
                return false;

            // wasn't outside so it must be inside
            return true; 
        }

        int _MINX, _MINY, _MAXX, _MAXY;
        /// <summary>
        /// Renders this voxel terrain
        /// </summary>
        /// <param name="camera">The camera to render with</param>
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
                    _tileManager.RenderTile(_spriteBatch, _voxelBounds[x, y], _atlas, _neighbourStates[x, y], _tiles[x, y], _transformColor);
                }
            }

            _spriteBatch.End();
        }

        /// <summary>
        /// Saves this voxel terrain to a stream
        /// </summary>
        /// <param name="stream">The stream to save to</param>
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

        /// <summary>
        /// Reads a voxel terrain from the stream
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        /// <param name="graphics">The graphics device to bind to</param>
        /// <param name="tileManager">The tile manager to use</param>
        /// <param name="atlas">The texture atlas to use</param>
        /// <returns>A voxel terrain loaded from the stream</returns>
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

        /// <summary>
        /// Reads a voxel terrain from the stream using Terrain Version 0.0.1
        /// </summary>
        /// <param name="reader">The stream to read from</param>
        /// <param name="graphics">The graphics device to bind to</param>
        /// <param name="tileManager">The tile manager to use</param>
        /// <param name="atlas">The texture atlas to use</param>
        /// <returns>A voxel terrain loaded from the stream</returns>
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

    /// <summary>
    /// Represents the neighbour states of a voxel
    /// </summary>
    [Flags]
    public enum MooreNeighbours : byte
    {
        /// <summary>
        /// No neighbours are set
        /// </summary>
        None = 0,
        /// <summary>
        /// The top middle neighbour is set
        /// </summary>
        TM = 1 << 0,
        /// <summary>
        /// The left neighbour is set
        /// </summary>
        L = 1 << 1,
        /// <summary>
        /// The right neighbour is set
        /// </summary>
        R = 1 << 2,
        /// <summary>
        /// The bottom middle neighbour is set
        /// </summary>
        BM = 1 << 3,
        /// <summary>
        /// The tile should be a diagonal
        /// </summary>
        Diag = 1 << 4,
        /// <summary>
        /// All 4 sides are set
        /// </summary>
        All = TM | L | R | BM
    }
}
