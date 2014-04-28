using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using DoodleEmpires.Engine.Utilities;
using Microsoft.Xna.Framework;
using System.IO;
using DoodleEmpires.Engine.Entities;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;
using DoodleEmpires.Engine.Economy;
using Lidgren.Network;

namespace DoodleEmpires.Engine.Terrain
{
    /// <summary>
    /// A terrain that is made up of small cubes, each having it's own texture and properties
    /// </summary>
    public class SPMap : VoxelMap
    {
        /// <summary>
        /// Gets the width of a single voxel tile
        /// </summary>
        public const int TILE_WIDTH = 16;
        /// <summary>
        /// Gets the height of a single voxel tile
        /// </summary>
        public const int TILE_HEIGHT = 16;
        /// <summary>
        /// Gets the number of trees per 32 tiles
        /// </summary>
        public const int TREE_DESNISTY = 4;
        /// <summary>
        /// Gets the alpha component for drawing zones
        /// </summary>
        public const float ZONE_ALPHA = 0.5f;
        
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
        /// The metadata for all the voxels
        /// </summary>
        protected byte[,] _meta;
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
        protected BasicEffect _basicEffect;

        protected TextureAtlas _atlas;
        protected TileManager _tileManager;

        protected Random _random;

        protected float _terrainHeightModifier;

        protected BackgroundWorker _updateThread;

        /// <summary>
        /// The transform color to multiply all tiles by
        /// </summary>
        protected Color _transformColor = Color.White;

        List<Zoning> _zones = new List<Zoning>();
        VertexPositionColor[] _zoneVerts = new VertexPositionColor[0];
        int[] _zoneIndices = new int[0];

        protected int _seed;

        #endregion

        int _maxX;
        int _maxY;

        /// <summary>
        /// Gets or sets the voxel material at the given x and y
        /// </summary>
        /// <param name="x">The x coordinate to set (chunk coords)</param>
        /// <param name="y">The y coordinate to set (chunk coords)</param>
        /// <returns>The voxel at (x, y)</returns>
        public override byte this[int x, int y]
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

        #region Public Properties

        /// <summary>
        /// Gets the graphics device for this terrain
        /// </summary>
        public GraphicsDevice Graphics
        {
            get { return _graphics; }
        }
        /// <summary>
        /// Gets or sets the texture atlas to use
        /// </summary>
        public TextureAtlas Atlas
        {
            get { return _atlas; }
            set { _atlas = value; }
        }
        /// <summary>
        /// Gets or sets the tile manager to use
        /// </summary>
        public TileManager TileManager
        {
            get { return _tileManager; }
            set { _tileManager = value; }
        }
        
        /// <summary>
        /// Gets the width of the world in world coords
        /// </summary>
        public int WorldWidth
        {
            get { return _width * TILE_WIDTH; }
        }
        /// <summary>
        /// Gets the height of the world in world coords
        /// </summary>
        public int WorldHeight
        {
            get { return _height * TILE_HEIGHT; }
        }

        /// <summary>
        /// Gets a list of zones in this voxel map
        /// </summary>
        public List<Zoning> Zones
        {
            get { return _zones; }
        }

        /// <summary>
        /// Gets or sets the backdrop texture
        /// </summary>
        public Texture2D BackDrop
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// Creates a new voxel based terrain
        /// </summary>
        /// <param name="graphics">The graphics device to bind to</param>
        /// <param name="tileManager">The tile manager to use</param>
        /// <param name="atlas">The texture atlas to use</param>
        /// <param name="width">The width of the map, in tiles</param>
        /// <param name="height">The height of the map, in tiles</param>
        public SPMap(GraphicsDevice Graphics, TileManager tileManager, TextureAtlas atlas, int width, int height, int? seed = null, float terrainHeightModifier = 25)
        {
            _seed = seed.HasValue ? seed.Value : (int)DateTime.Now.Ticks;
            Noise.Seed = _seed;

            _random = new Random(_seed);
            _tiles = new byte[width, height];
            _meta = new byte[width, height];
            _neighbourStates = new byte[width, height];
            _width = width;
            _height = height;
            _terrainHeightModifier = terrainHeightModifier;

            _maxX = width - 1;
            _maxY = height - 1;

            _graphics = Graphics;

            _spriteBatch = new SpriteBatch(Graphics);

            _basicEffect = new BasicEffect(_graphics);
            _basicEffect.VertexColorEnabled = true;
            _basicEffect.Projection =
                Matrix.CreateOrthographicOffCenter(0, _graphics.Viewport.Width, _graphics.Viewport.Height, 0, 0, 1);
            _basicEffect.Alpha = ZONE_ALPHA;

            _atlas = atlas;
            _tileManager = tileManager;
            BuildVoxelBouds();

            GenTerrain();

            _updateThread = new BackgroundWorker();
            _updateThread.DoWork += BeginUpdateLoop;
            _updateThread.RunWorkerAsync();
        }

        private void BeginUpdateLoop(object sender, DoWorkEventArgs e)
        {
            System.Timers.Timer _updateTimer = new System.Timers.Timer();
            _updateTimer.Elapsed += new System.Timers.ElapsedEventHandler(_updateTimer_Elapsed);
            _updateTimer.Interval = 1000 / 5;
            _updateTimer.Start();
        }

        void _updateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    byte tID = GetMaterial(x, y);

                    if (_tileManager[tID].NeedsUpdate)
                        _tileManager[tID].OnTick(this, x, y);
                }
            }
        }

        #region Zoning

        /// <summary>
        /// Defines a zone
        /// </summary>
        /// <param name="minX">The minimum X coord (chunk)</param>
        /// <param name="minY">The minimum Y coord (chunk)</param>
        /// <param name="maxX">The maximum X coord (chunk)</param>
        /// <param name="maxY">The maximum Y coord (chunk)</param>
        /// <param name="zone">The zone to add</param>
        public void DefineZone(int minX, int minY, int maxX, int maxY, Zoning zone)
        {
            zone.Bounds = new Rectangle(minX * TILE_WIDTH, minY * TILE_HEIGHT, (maxX - minX) * TILE_WIDTH, (maxY - minY) * TILE_HEIGHT);

            AddZone(zone);
        }

        /// <summary>
        /// Defines a zone
        /// </summary>
        /// <param name="min">The minimum bounds of the zone, in world coords</param>
        /// <param name="max">The maximum bounds of the zone, in world coords</param>
        /// <param name="zone">The zone to add</param>
        public void DefineZone(Vector2 min, Vector2 max, Zoning zone)
        {
            int minX = (int)Math.Floor(Math.Min(min.X, max.X) / SPMap.TILE_WIDTH);
            int minY = (int)Math.Floor(Math.Min(min.Y, max.Y) / SPMap.TILE_HEIGHT);

            int maxX = (int)Math.Ceiling(Math.Max(min.X, max.X) / SPMap.TILE_WIDTH);
            int maxY = (int)Math.Ceiling(Math.Max(min.Y, max.Y) / SPMap.TILE_HEIGHT);

            DefineZone(minX, minY, maxX, maxY, zone);
        }

        /// <summary>
        /// Deletes a specific zone
        /// </summary>
        /// <param name="zone">The zone to delete</param>
        public void DeleteZone(Zoning zone)
        {
            if (_zones.Contains(zone))
            {
                int id = _zones.IndexOf(zone);

                List<VertexPositionColor> temp = _zoneVerts.ToList();
                temp.RemoveRange(id * 4, 4);
                _zoneVerts = temp.ToArray();
                
                for (int iID = id * 6 + 6; iID < _zoneIndices.Length; iID += 6)
                {
                    _zoneIndices[iID] = _zoneIndices[iID] - 4;
                    _zoneIndices[iID + 1] = _zoneIndices[iID + 1] - 4;
                    _zoneIndices[iID + 2] = _zoneIndices[iID + 2] - 4;

                    _zoneIndices[iID + 3] = _zoneIndices[iID + 3] - 4;
                    _zoneIndices[iID + 4] = _zoneIndices[iID + 4] - 4;
                    _zoneIndices[iID + 5] = _zoneIndices[iID + 5] - 4;
                }

                List<int> temp2 = _zoneIndices.ToList();
                temp2.RemoveRange(id * 6, 6);
                _zoneIndices = temp2.ToArray();

                _zones.RemoveAt(id);
            }
        }

        /// <summary>
        /// Handles adding a pre-snapped zone to the zone list
        /// </summary>
        /// <param name="zone">The zone to add</param>
        protected void AddZone(Zoning zone)
        {
            _zones.Add(zone);

            int vID = _zoneVerts.Length;
            Array.Resize(ref _zoneVerts, _zoneVerts.Length + 4);
            _zoneVerts[vID] = new VertexPositionColor(new Vector3(zone.Bounds.Left, zone.Bounds.Top, 0f), zone.Color);
            _zoneVerts[vID + 1] = new VertexPositionColor(new Vector3(zone.Bounds.Right, zone.Bounds.Top, 0f), zone.Color);
            _zoneVerts[vID + 2] = new VertexPositionColor(new Vector3(zone.Bounds.Left, zone.Bounds.Bottom, 0f), zone.Color);
            _zoneVerts[vID + 3] = new VertexPositionColor(new Vector3(zone.Bounds.Right, zone.Bounds.Bottom, 0f), zone.Color);

            int iID = _zoneIndices.Length;
            Array.Resize(ref _zoneIndices, _zoneIndices.Length + 6);
            _zoneIndices[iID] = vID;
            _zoneIndices[iID + 1] = vID + 1;
            _zoneIndices[iID + 2] = vID + 2;

            _zoneIndices[iID + 3] = vID + 2;
            _zoneIndices[iID + 4] = vID + 1;
            _zoneIndices[iID + 5] = vID + 3;
        }

        #endregion

        #region Meta

        /// <summary>
        /// Sets the metadata at the given chunk coords
        /// </summary>
        /// <param name="x">The x coordinate (chunk)</param>
        /// <param name="y">The y coordinate (chunk)</param>
        /// <param name="meta">The metadata to set to</param>
        public override void SetMeta(int x, int y, byte meta)
        {
            if (x >= 0 & x < _width & y >= 0 & y < _height)
            {
                _meta[x, y] = meta;
                UpdateVoxel(x, y);
            }
        }

        /// <summary>
        /// Gets the metadata at the given chunk coords
        /// </summary>
        /// <param name="x">The x coordinate (chunk)</param>
        /// <param name="y">The y coordinate (chunk)</param>
        /// <returns>The metadata at the given x and y</returns>
        public override byte GetMeta(int x, int y)
        {
            if (x >= 0 & x < _width & y >= 0 & y < _height)
            {
                return _meta[x, y];
            }
            else { return 0; }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Generates the terrain
        /// </summary>
        protected virtual void GenTerrain()
        {
            float tHeight;
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    tHeight = Noise.PerlinNoise_1D(x / 16.0f) * _terrainHeightModifier;
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
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    if (_tiles[x, y] != 0)
                    {
                        _neighbourStates[x, y] = (byte)GetNeighbours(x, y);
                    }
                }
            }

            for (int i = 0; i < _width; i+= (32 / TREE_DESNISTY) + _random.Next(-3, 3))
            {
                GenTree(i, 200 + (int)(Noise.PerlinNoise_1D(i / 16.0f) * _terrainHeightModifier) - 1);
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

                for (int i = 0; i < 10; i++)
                {
                    int xx = _random.Next(x - radius, x + radius + 1);
                    int yy = _random.Next(y - height - radius, y - height);

                    if (IsInsideOctagon(x, y - height - radius + 1, radius, xx, yy))
                        SetMeta(xx, yy, 1);
                }

                for (int yy = y; yy > y - height + 1; yy--)
                {
                    SetTile(x, yy, 4);
                }

                for (int xx = x - radius / 2; xx <= x + radius / 2; xx++)
                    SetTile(xx, y - height + 1, 0);

                SetTile(x, y - height + 1, 4);

                if (_random.Next(0, 3) == 1 && IsNotAir(x - 1, y + 1)) //33% chance of left root
                    SetTile(x - 1, y, 4);
                if (_random.Next(0, 3) == 1 && IsNotAir(x + 1, y + 1)) //33% chance of right root
                    SetTile(x + 1, y, 4);
                if (_random.Next(0, 10) == 1) //10% chance of left branch
                    SetTile(x - 1, y - height + 1, 4);
                if (_random.Next(0, 10) == 1) //10% chance of right branch
                    SetTile(x + 1, y - height + 1, 4);
            }
        }

        #endregion

        #region Render

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
            byte cMaterial = GetMaterial(x, y);

            if (_tileManager.Tiles[cMaterial].RenderType == RenderType.Land)
            {
                if (!_tileManager.CanConnect(cMaterial,  GetMaterial(x - 1, y)) &&
                    !_tileManager.CanConnect(cMaterial, GetMaterial(x, y - 1)) &&
                    (_tileManager.CanConnect(cMaterial, GetMaterial(x - 1, y + 1)) ||
                     _tileManager.CanConnect(cMaterial, GetMaterial(x + 1, y - 1))) &&
                     _tileManager.CanConnect(cMaterial, GetMaterial(x, y + 1)) &&
                     _tileManager.CanConnect(cMaterial, GetMaterial(x + 1, y)))
                    return MooreNeighbours.Diag;

                if (!_tileManager.CanConnect(cMaterial,  GetMaterial(x + 1, y)) &&
                    !_tileManager.CanConnect(cMaterial, GetMaterial(x, y - 1)) &&
                    (_tileManager.CanConnect(cMaterial, GetMaterial(x - 1, y - 1)) ||
                     _tileManager.CanConnect(cMaterial, GetMaterial(x + 1, y + 1))) &&
                     _tileManager.CanConnect(cMaterial, GetMaterial(x, y + 1)) &&
                     _tileManager.CanConnect(cMaterial, GetMaterial(x - 1, y)))
                    return MooreNeighbours.Diag + 1;

                if (!_tileManager.CanConnect(cMaterial,  GetMaterial(x - 1, y)) &&
                    !_tileManager.CanConnect(cMaterial, GetMaterial(x, y + 1)) &&
                    (_tileManager.CanConnect(cMaterial, GetMaterial(x - 1, y - 1)) ||
                     _tileManager.CanConnect(cMaterial, GetMaterial(x + 1, y + 1))) &&
                     _tileManager.CanConnect(cMaterial, GetMaterial(x, y - 1)) &&
                     _tileManager.CanConnect(cMaterial, GetMaterial(x + 1, y)))
                    return MooreNeighbours.Diag + 2;

                if (!_tileManager.CanConnect(cMaterial,  GetMaterial(x + 1, y)) &&
                    !_tileManager.CanConnect(cMaterial, GetMaterial(x, y + 1)) &&
                    (_tileManager.CanConnect(cMaterial, GetMaterial(x - 1, y + 1)) ||
                     _tileManager.CanConnect(cMaterial, GetMaterial(x + 1, y - 1))) &&
                     _tileManager.CanConnect(cMaterial, GetMaterial(x, y - 1)) &&
                     _tileManager.CanConnect(cMaterial, GetMaterial(x - 1, y)))
                    return MooreNeighbours.Diag + 3;
            }

            return (MooreNeighbours)(0 +
                (CanConnect(GetMaterial(x, y), GetMaterial(x, y - 1)) ? MooreNeighbours.TM : 0) |
                (CanConnect(GetMaterial(x, y), GetMaterial(x - 1, y)) ? MooreNeighbours.L : 0) |
                (CanConnect(GetMaterial(x, y), GetMaterial(x + 1, y)) ? MooreNeighbours.R : 0) |
                (CanConnect(GetMaterial(x, y), GetMaterial(x, y + 1)) ? MooreNeighbours.BM : 0));
        }
        
        int _MINX, _MINY, _MAXX, _MAXY;
        /// <summary>
        /// Renders this voxel terrain
        /// </summary>
        /// <param name="camera">The camera to render with</param>
        public virtual void Render(ICamera2D camera)
        {
            if (BackDrop != null)
            {
                _spriteBatch.Begin(SpriteSortMode.Texture, null, SamplerState.LinearWrap, null, null);

                _spriteBatch.Draw(BackDrop, new Rectangle(0, -1, _graphics.Viewport.Width, _graphics.Viewport.Height),
                    camera.ViewBounds, Color.FromNonPremultiplied(255, 255, 255, 64));

                _spriteBatch.End();
            }

            if (_zones.Count > 0)
            {
                _basicEffect.View = camera.Transform;
                _basicEffect.CurrentTechnique.Passes[0].Apply();
                _graphics.RasterizerState = RasterizerState.CullNone;
                _graphics.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, _zoneVerts,
                    0, _zoneVerts.Length, _zoneIndices, 0, _zoneIndices.Length / 3);
            }

            _spriteBatch.Begin(SpriteSortMode.Texture, null, SamplerState.PointWrap, null, null, null, camera.Transform);

            _MINX = camera.ViewBounds.X / TILE_WIDTH - 1;
            _MINY = camera.ViewBounds.Y / TILE_HEIGHT - 1;
            _MAXX = camera.ViewBounds.Right / TILE_WIDTH + 1;
            _MAXY = camera.ViewBounds.Bottom / TILE_HEIGHT + 1;

            _MINX = _MINX < 0 ? 0 : _MINX;
            _MINY = _MINY < 0 ? 0 : _MINY;
            _MAXX = _MAXX > _maxX ? _maxX : _MAXX;
            _MAXY = _MAXY > _maxY ? _maxY : _MAXY;

            for (int y = _MINY; y <= _MAXY; y++)
            {
                for (int x = _MINX; x <= _MAXX; x++)
                {
                    _tileManager.RenderTile(_spriteBatch, _voxelBounds[x, y], _atlas, _neighbourStates[x, y],
                        _tiles[x, y], _meta[x, y], _transformColor);
                }
            }

            _spriteBatch.End();
        }

        #endregion

        #region Checks

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
        /// Checks if a rectangle intersects with this voxel map
        /// </summary>
        /// <param name="rect">The rectangle to check</param>
        /// <returns>True if the rectangle intersect this voxel map</returns>
        public bool Intersects(Rectangle rect)
        {
            int MinX = rect.Left / TILE_WIDTH;
            int MinY = rect.Top / TILE_HEIGHT;
            int MaxX = rect.Right / TILE_WIDTH;
            int MaxY = rect.Bottom / TILE_HEIGHT;

            for (int y = MinY < 0 ? 0 : MinY; y < MaxY; y++)
            {
                for (int x = MinX < 0 ? 0 : MinX; x < MaxX; x++)
                {
                    if (IsNotAir(x, y) && CheckCollision(x, y, rect))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks a tile collision at the given tile coords
        /// </summary>
        /// <param name="x">The x coordinate to check (chunk)</param>
        /// <param name="y">The y coordinate to check (chunk)</param>
        /// <param name="rect">The rectangle to check</param>
        /// <returns>True if the tile intersects the rectangle</returns>
        private bool CheckCollision(int x, int y, Rectangle rect)
        {
            return _tileManager.CheckCollision(GetMaterial(x, y), rect, _voxelBounds[x, y]);
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
        /// Checks is a point is inside of a octagon
        /// </summary>
        /// <param name="xx">The x coordinate of the octagon's centre</param>
        /// <param name="yy">The y coordinate of the octagon's centre</param>
        /// <param name="radius">The radius of the octagon</param>
        /// <param name="x">The x coordinate to check</param>
        /// <param name="y">The y coordinate to check</param>
        /// <returns>True if the point is inside of the octagon</returns>
        private bool IsInsideOctagon(int xx, int yy, int radius, int x, int y)
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

        #endregion

        #region Get and Set

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
        /// Safely sets the voxel material at the given position, should only be called by tile classes
        /// </summary>
        /// <param name="x">The x co-ordinate to check (Chunk Coords)</param>
        /// <param name="y">The y co-ordinate to check (Chunk Coords)</param>
        /// <param name="id">The material to set</param>
        public override void SetTile(int x, int y, byte id)
        {
            if (x >= 0 & x < _width & y >= 0 & y < _height)
                this[x, y] = id;
        }

        /// <summary>
        /// Safely sets the voxel material at the given position
        /// </summary>
        /// <param name="x">The x co-ordinate to check (Chunk Coords)</param>
        /// <param name="y">The y co-ordinate to check (Chunk Coords)</param>
        /// <param name="id">The material to set</param>
        public override void SetTileSafe(int x, int y, byte id)
        {
            if (id != 0)
                _tileManager[id].AddToWorld(this, x, y);
            else
                _tileManager[GetMaterial(x, y)].RemovedFromWorld(this, x, y);
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
                        SetTile(xx, yy, value);
                }
            }
        }

        #endregion

        #region Saving & Loading

        /// <summary>
        /// Saves this voxel terrain to a stream
        /// </summary>
        /// <param name="stream">The stream to save to</param>
        public void SaveToStream(Stream stream)
        {
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write("Doodle Empires Voxel Terrain ");
            writer.Write("0.0.4");

            writer.Write(_width);
            writer.Write(_height);

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    writer.Write(_tiles[x, y]);
                    writer.Write(_neighbourStates[x, y]);
                    writer.Write(_meta[x, y]);
                }
            }

            writer.Write(_zones.Count);

            foreach (Zoning zone in _zones)
            {
                zone.WriteToStream(writer);
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
        public static SPMap ReadFromStream(Stream stream, GraphicsDevice graphics, TileManager tileManager, TextureAtlas atlas)
        {
            BinaryReader reader = new BinaryReader(stream);

            string header = reader.ReadString();
            string version = reader.ReadString();

            switch (version)
            {
                case "0.0.1":
                    return LoadVersion_0_0_1(reader, graphics, tileManager, atlas);
                case "0.0.2":
                    return LoadVersion_0_0_2(reader, graphics, tileManager, atlas);
                case "0.0.3":
                    return LoadVersion_0_0_3(reader, graphics, tileManager, atlas);
                case "0.0.4":
                    return LoadVersion_0_0_4(reader, graphics, tileManager, atlas);
                default:
                    reader.Dispose();
                    return null;
            }

        }

        #region File Versions

        /// <summary>
        /// Reads a voxel terrain from the stream using Terrain Version 0.0.1
        /// </summary>
        /// <param name="reader">The stream to read from</param>
        /// <param name="graphics">The graphics device to bind to</param>
        /// <param name="tileManager">The tile manager to use</param>
        /// <param name="atlas">The texture atlas to use</param>
        /// <returns>A voxel terrain loaded from the stream</returns>
        private static SPMap LoadVersion_0_0_1(BinaryReader reader, GraphicsDevice graphics, TileManager tileManager, TextureAtlas atlas)
        {
            int width = reader.ReadInt32();
            int height = reader.ReadInt32();

            SPMap terrain = new SPMap(graphics, tileManager, atlas, width, height);

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

        /// <summary>
        /// Reads a voxel terrain from the stream using Terrain Version 0.0.2
        /// </summary>
        /// <param name="reader">The stream to read from</param>
        /// <param name="graphics">The graphics device to bind to</param>
        /// <param name="tileManager">The tile manager to use</param>
        /// <param name="atlas">The texture atlas to use</param>
        /// <returns>A voxel terrain loaded from the stream</returns>
        private static SPMap LoadVersion_0_0_2(BinaryReader reader, GraphicsDevice graphics, TileManager tileManager, TextureAtlas atlas)
        {
            int width = reader.ReadInt32();
            int height = reader.ReadInt32();

            SPMap terrain = new SPMap(graphics, tileManager, atlas, width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    terrain._tiles[x, y] = reader.ReadByte();
                    terrain._neighbourStates[x, y] = reader.ReadByte();
                    terrain._meta[x, y] = reader.ReadByte();
                }
            }

            reader.Dispose();
            return terrain;
        }

        /// <summary>
        /// Reads a voxel terrain from the stream using Terrain Version 0.0.3
        /// </summary>
        /// <param name="reader">The stream to read from</param>
        /// <param name="graphics">The graphics device to bind to</param>
        /// <param name="tileManager">The tile manager to use</param>
        /// <param name="atlas">The texture atlas to use</param>
        /// <returns>A voxel terrain loaded from the stream</returns>
        private static SPMap LoadVersion_0_0_3(BinaryReader reader, GraphicsDevice graphics, TileManager tileManager, TextureAtlas atlas)
        {
            int width = reader.ReadInt32();
            int height = reader.ReadInt32();

            SPMap terrain = new SPMap(graphics, tileManager, atlas, width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    terrain._tiles[x, y] = reader.ReadByte();
                    terrain._neighbourStates[x, y] = reader.ReadByte();
                    terrain._meta[x, y] = reader.ReadByte();
                }
            }

            int zoneCount = reader.ReadInt32();

            for (int i = 0; i < zoneCount; i++)
            {
                Rectangle bounds = new Rectangle(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());

                Zoning zone = new Zoning();
                zone.Bounds = bounds;
                terrain.AddZone(zone);
            }

                reader.Dispose();
            return terrain;
        }

        /// <summary>
        /// Reads a voxel terrain from the stream using Terrain Version 0.0.4
        /// </summary>
        /// <param name="reader">The stream to read from</param>
        /// <param name="graphics">The graphics device to bind to</param>
        /// <param name="tileManager">The tile manager to use</param>
        /// <param name="atlas">The texture atlas to use</param>
        /// <returns>A voxel terrain loaded from the stream</returns>
        private static SPMap LoadVersion_0_0_4(BinaryReader reader, GraphicsDevice graphics, TileManager tileManager, TextureAtlas atlas)
        {
            int width = reader.ReadInt32();
            int height = reader.ReadInt32();

            SPMap terrain = new SPMap(graphics, tileManager, atlas, width, height);

            try
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        terrain._tiles[x, y] = reader.ReadByte();
                        terrain._neighbourStates[x, y] = reader.ReadByte();
                        terrain._meta[x, y] = reader.ReadByte();
                    }
                }

                int zoneCount = reader.ReadInt32();

                for (int i = 0; i < zoneCount; i++)
                {
                    Zoning zone = Zoning.ReadFromStream(reader);
                    terrain.AddZone(zone);
                }
            }
            catch (EndOfStreamException)
            {
                Debug.WriteLine("[WARNING] File corrupt, attempted to rad past ond of stream. Attempting to bypass...");
            }

            reader.Dispose();
            return terrain;
        }

        #endregion

        #endregion

        #region Networking

        public static SPMap ReadFromMessage(NetIncomingMessage message, 
            GraphicsDevice graphics, TileManager tileManager, TextureAtlas textureAtlas)
        {
            int width = message.ReadInt16();
            int height = message.ReadInt16();

            int seed = message.ReadInt32();

            SPMap map = new SPMap(graphics, tileManager, textureAtlas, width, height, seed);

            int changeCount = message.ReadInt32();

            for (int i = 0; i < changeCount; i++)
            {
                DeltaMapChange m = new DeltaMapChange(message.ReadInt16(), message.ReadInt16(), message.ReadByte());
                map.SetTileSafe(m.X, m.Y, m.NewID);
            }

            return map;
        }

        #endregion
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
