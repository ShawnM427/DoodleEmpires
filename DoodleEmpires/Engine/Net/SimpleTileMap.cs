﻿using System;
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
using DoodleEmpires.Engine.Terrain;
using DoodleEmpires.Engine.Entities.PathFinder;

namespace DoodleEmpires.Engine.Net
{
    /// <summary>
    /// A terrain that is made up of small cubes, each having it's own texture and properties
    /// </summary>
    public class SimpleTileMap : IDisposable
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
        protected byte[, ,] _tiles;
        /// <summary>
        /// The metadata for all the voxels
        /// </summary>
        protected byte[,,] _meta;
        /// <summary>
        /// The neighbour states for the corresponding voxels
        /// </summary>
        protected byte[,,] _neighbourStates;
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
        /// <summary>
        /// The basic effect for drawing complex geometry
        /// </summary>
        protected BasicEffect _basicEffect;
        /// <summary>
        /// The font used to render zone tags
        /// </summary>
        protected SpriteFont _zoneFont;
        /// <summary>
        /// A 1x1 blank texture
        /// </summary>
        protected Texture2D _pixelTex;

        /// <summary>
        /// The texture atlas to use
        /// </summary>
        protected TextureAtlas _atlas;
        /// <summary>
        /// The tile manager to use
        /// </summary>
        protected TileManager _tileManager;

        /// <summary>
        /// A random number generator used for random events
        /// </summary>
        protected Random _random;

        /// <summary>
        /// How far down to shift the terrain
        /// </summary>
        protected float _terrainHeightModifier;

        /// <summary>
        /// The thread handling tile updating
        /// </summary>
        protected BackgroundWorker _updateThread;

        /// <summary>
        /// The transform color to multiply all tiles by
        /// </summary>
        protected Color _transformColor = Color.White;

        List<Zoning> _zones = new List<Zoning>();
        VertexPositionColor[] _zoneVerts = new VertexPositionColor[0];
        int[] _zoneIndices = new int[0];

        /// <summary>
        /// This level's seed
        /// </summary>
        protected int _seed;

        /// <summary>
        /// True if this is a singleplayer map
        /// </summary>
        protected bool _isSinglePlayer;

        /// <summary>
        /// True if this map is in debug mode
        /// </summary>
        protected bool _debugging;

        BaseGrid _aiGrid;
        JumpPointParam _aiParam;
        JumpPointFinder _jpFinder;

        #endregion

        int _maxX;
        int _maxY;
        
        /// <summary>
        /// Gets or sets the voxel material at the given x and y
        /// </summary>
        /// <param name="x">The x coordinate to set (chunk coords)</param>
        /// <param name="y">The y coordinate to set (chunk coords)</param>
        /// <param name="level">The tile level to set</param>
        /// <returns>The voxel at (x, y)</returns>
        public byte this[int x, int y, int level]
        {
            get
            {
                return _tiles[x, y, level];
            }
            set
            {
                _tiles[x, y, level] = value;
                UpdateVoxel(x, y, level);
                if (_tileManager[value].Solid)
                    _aiGrid.SetWalkableAt(x, y, false);
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

        /// <summary>
        /// Gets whether this map is a singleplayer map
        /// </summary>
        public bool SinglePlayerMap
        {
            get { return _isSinglePlayer; }
        }

        /// <summary>
        /// Gets or sets whether this map is in debugging mode
        /// </summary>
        public bool Debugging
        {
            get { return _debugging; }
            set { _debugging = value; }
        }

        #endregion

        /// <summary>
        /// Creates a new voxel based terrain
        /// </summary>
        /// <param name="graphics">The graphics device to bind to</param>
        /// <param name="zoneFont">The font to use for rendering zone tags</param>
        /// <param name="tileManager">The tile manager to use</param>
        /// <param name="atlas">The texture atlas to use</param>
        /// <param name="width">The width of the map, in tiles</param>
        /// <param name="height">The height of the map, in tiles</param>
        /// <param name="seed">The seed to generate the map from, if null will pick a random seed</param>
        /// <param name="isMPMap">True if this map is bound to an online map</param>
        /// <param name="terrainHeightModifier">How many blocks to shift the terrain down</param>
        public SimpleTileMap(GraphicsDevice graphics, SpriteFont zoneFont, TileManager tileManager, TextureAtlas atlas, int width, int height, int levels, int? seed = null, bool isMPMap = false, float terrainHeightModifier = 25)
        {
            _seed = seed.HasValue ? seed.Value : (int)DateTime.Now.Ticks;
            Noise.Seed = _seed;

            _isSinglePlayer = !isMPMap;

            _random = new Random(_seed);
            _tiles = new byte[width, height, levels];
            _meta = new byte[width, height, levels];
            _neighbourStates = new byte[width, height, levels];
            _width = width;
            _height = height;
            _terrainHeightModifier = terrainHeightModifier;

            _maxX = width - 1;
            _maxY = height - 1;
            
            _graphics = graphics;

            _spriteBatch = new SpriteBatch(graphics);

            _pixelTex = new Texture2D(_graphics, 1, 1);
            _pixelTex.SetData<Color>(new Color[] { Color.White });

            _zoneFont = zoneFont;

            _basicEffect = new BasicEffect(_graphics);
            _basicEffect.VertexColorEnabled = true;
            _basicEffect.Projection =
                Matrix.CreateOrthographicOffCenter(0, _graphics.Viewport.Width, _graphics.Viewport.Height, 0, 0, 1);
            _basicEffect.Alpha = ZONE_ALPHA;

            _atlas = atlas;
            _tileManager = tileManager;
            BuildVoxelBouds();

            _aiGrid = new DynamicGridWPool(new NodePool());
            _aiParam = new JumpPointParam(_aiGrid, true, true, true, HeuristicMode.EUCLIDEAN);
            _jpFinder = new JumpPointFinder();
            
            GenTerrain();
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
        /// Defines a zone
        /// </summary>
        /// <param name="zone">The zone to add</param>
        public void DefineZone(Zoning zone)
        {
            int minX = (int)Math.Floor((float)zone.Bounds.X / SPMap.TILE_WIDTH);
            int minY = (int)Math.Floor((float)zone.Bounds.Y / SPMap.TILE_HEIGHT);

            int maxX = (int)Math.Ceiling((float)zone.Bounds.Right / SPMap.TILE_WIDTH);
            int maxY = (int)Math.Ceiling((float)zone.Bounds.Bottom / SPMap.TILE_HEIGHT);
            
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
        /// Deletes a specific zone
        /// </summary>
        /// <param name="x">The X-Coord to delete at</param>
        /// <param name="y">The Y-Coord to delete at</param>
        public void DeleteZone(int x, int y)
        {
            Zoning zone = _zones.Find(Zone => Zone.Bounds.Contains(x, y));

            if (zone != null)
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
        public void AddPrebuiltZone(Zoning zone)
        {
            AddZone(zone);
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
            _zoneVerts[vID] = new VertexPositionColor(new Vector3(zone.Bounds.Left, zone.Bounds.Top, 0f), zone.Info.Color);
            _zoneVerts[vID + 1] = new VertexPositionColor(new Vector3(zone.Bounds.Right, zone.Bounds.Top, 0f), zone.Info.Color);
            _zoneVerts[vID + 2] = new VertexPositionColor(new Vector3(zone.Bounds.Left, zone.Bounds.Bottom, 0f), zone.Info.Color);
            _zoneVerts[vID + 3] = new VertexPositionColor(new Vector3(zone.Bounds.Right, zone.Bounds.Bottom, 0f), zone.Info.Color);

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
        /// <param name="level">The level coordinate (chunk)</param>
        /// <param name="meta">The metadata to set to</param>
        public void SetMeta(int x, int y, int level, byte meta)
        {
            if (x >= 0 & x < _width & y >= 0 & y < _height)
            {
                _meta[x, y, level] = meta;
                UpdateVoxel(x, y, level);
            }
        }

        /// <summary>
        /// Gets the metadata at the given chunk coords
        /// </summary>
        /// <param name="x">The x coordinate (chunk)</param>
        /// <param name="y">The y coordinate (chunk)</param>
        /// <param name="level">The level coordinate (chunk)</param>
        /// <returns>The metadata at the given x and y</returns>
        public byte GetMeta(int x, int y, int level)
        {
            if (x >= 0 & x < _width & y >= 0 & y < _height)
            {
                return _meta[x, y, level];
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
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    for (int z = 0; z < _tiles.GetLength(2); z++)
                    {
                        float type = (float)(SimplexNoise.Noise(x, y, z) + 1.0) / 2.0f;

                        _aiGrid.SetWalkableAt(x, y, true);

                        if (type > 0.5f)
                            _tiles[x, y, z] = _tileManager["grass"];
                        else
                            _tiles[x, y, z] = _tileManager["water"];
                    }
                }
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
        public void GenTree(int x, int y, int level)
        {
            if (_random.Next(100) > 2)
            {
                int size = _random.Next(3);

                switch(size)
                {
                    case 0:
                        SetTile(x, y, level, _tileManager["smallTree"]);
                        break;
                    case 1:
                        SetTile(x, y, level, _tileManager["smallTree"]);
                        break;
                    case 2:
                        SetTile(x, y, level, _tileManager["smallTree"]);
                        break;
                }
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
        protected virtual void UpdateVoxel(int x, int y, int level, bool doNeighbours = true)
        {
            MooreNeighbours neighbours = GetNeighbours(x, y, level);
            _neighbourStates[x, y, level] = (byte)neighbours;

            if (doNeighbours)
            {
                UpdateVoxel(x - 1, y - 1, level, false);
                UpdateVoxel(x, y - 1, level, false);
                UpdateVoxel(x + 1, y - 1, level, false);

                UpdateVoxel(x - 1, y, level, false);
                UpdateVoxel(x + 1, y, level, false);

                UpdateVoxel(x - 1, y + 1, level, false);
                UpdateVoxel(x, y + 1, level, false);
                UpdateVoxel(x + 1, y + 1, level, false);
            }
        }

        /// <summary>
        /// Gets the neighbour state for the given position
        /// </summary>
        /// <param name="x">The x coord to get (chunk coords)</param>
        /// <param name="y">The y coord to get (chunk coords)</param>
        /// <returns>The moore neighbour state for the given block</returns>
        protected MooreNeighbours GetNeighbours(int x, int y, int level)
        {
            byte cMaterial = GetMaterial(x, y, level);

            if (_tileManager.Tiles[cMaterial].RenderType == RenderType.Land)
            {
                if (!_tileManager.CanConnect(cMaterial, GetMaterial(x - 1, y, level)) &&
                    !_tileManager.CanConnect(cMaterial, GetMaterial(x, y - 1, level)) &&
                    (_tileManager.CanConnect(cMaterial, GetMaterial(x - 1, y + 1, level)) ||
                     _tileManager.CanConnect(cMaterial, GetMaterial(x + 1, y - 1, level))) &&
                     _tileManager.CanConnect(cMaterial, GetMaterial(x, y + 1, level)) &&
                     _tileManager.CanConnect(cMaterial, GetMaterial(x + 1, y, level)))
                    return MooreNeighbours.Diag;

                if (!_tileManager.CanConnect(cMaterial, GetMaterial(x + 1, y, level)) &&
                    !_tileManager.CanConnect(cMaterial, GetMaterial(x, y - 1, level)) &&
                    (_tileManager.CanConnect(cMaterial, GetMaterial(x - 1, y - 1, level)) ||
                     _tileManager.CanConnect(cMaterial, GetMaterial(x + 1, y + 1, level))) &&
                     _tileManager.CanConnect(cMaterial, GetMaterial(x, y + 1, level)) &&
                     _tileManager.CanConnect(cMaterial, GetMaterial(x - 1, y, level)))
                    return MooreNeighbours.Diag + 1;

                if (!_tileManager.CanConnect(cMaterial, GetMaterial(x - 1, y, level)) &&
                    !_tileManager.CanConnect(cMaterial, GetMaterial(x, y + 1, level)) &&
                    (_tileManager.CanConnect(cMaterial, GetMaterial(x - 1, y - 1, level)) ||
                     _tileManager.CanConnect(cMaterial, GetMaterial(x + 1, y + 1, level))) &&
                     _tileManager.CanConnect(cMaterial, GetMaterial(x, y - 1, level)) &&
                     _tileManager.CanConnect(cMaterial, GetMaterial(x + 1, y, level)))
                    return MooreNeighbours.Diag + 2;

                if (!_tileManager.CanConnect(cMaterial, GetMaterial(x + 1, y, level)) &&
                    !_tileManager.CanConnect(cMaterial, GetMaterial(x, y + 1, level)) &&
                    (_tileManager.CanConnect(cMaterial, GetMaterial(x - 1, y + 1, level)) ||
                     _tileManager.CanConnect(cMaterial, GetMaterial(x + 1, y - 1, level))) &&
                     _tileManager.CanConnect(cMaterial, GetMaterial(x, y - 1, level)) &&
                     _tileManager.CanConnect(cMaterial, GetMaterial(x - 1, y, level)))
                    return MooreNeighbours.Diag + 3;
            }

            return (MooreNeighbours)(0 +
                (CanConnect(GetMaterial(x, y, level), GetMaterial(x, y - 1, level)) ? MooreNeighbours.TM : 0) |
                (CanConnect(GetMaterial(x, y, level), GetMaterial(x - 1, y, level)) ? MooreNeighbours.L : 0) |
                (CanConnect(GetMaterial(x, y, level), GetMaterial(x + 1, y, level)) ? MooreNeighbours.R : 0) |
                (CanConnect(GetMaterial(x, y, level), GetMaterial(x, y + 1, level)) ? MooreNeighbours.BM : 0));
        }
        
        int _MINX, _MINY, _MAXX, _MAXY;
        /// <summary>
        /// Renders this voxel terrain
        /// </summary>
        /// <param name="camera">The camera to render with</param>
        public virtual void Render(ICamera2D camera)
        {
            camera.BeginDraw();

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

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap, null, null, null, camera.Transform);
            
            foreach (Zoning zone in _zones)
                _spriteBatch.DrawString(_zoneFont, zone.Info.Name,
                    new Vector2(zone.Bounds.Center.X - _zoneFont.MeasureString(zone.Info.Name).X / 2,
                        zone.Bounds.Center.Y - _zoneFont.MeasureString(zone.Info.Name).Y / 2), 
                        Color.Black * ZONE_ALPHA);
            
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
                    for (int level = 0; level < _tiles.GetLength(2); level++)
                    {
                        _tileManager.RenderTile(_spriteBatch, _voxelBounds[x, y], _atlas, _neighbourStates[x, y, level],
                            _tiles[x, y, level], _meta[x, y, level], _transformColor);

                        if (_debugging)
                        {
                            if (_aiGrid.IsWalkableAt(x, y))
                                _spriteBatch.Draw(_pixelTex, _voxelBounds[x, y], Color.Green * 0.25f);
                        }
                    }
                }
            }

            _spriteBatch.End();

            camera.EndDraw();
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
            return x >= 0 & x < _width & y >= 0 & y < _height ? _aiGrid.IsWalkableAt(x, y) : false;
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
                    if (CheckCollision(x, y, rect))
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
            return _aiGrid.IsWalkableAt(x, y);
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

        #region Pathfinding

        /// <summary>
        /// Attempts to find a path between 2 points
        /// </summary>
        /// <param name="start">The start position</param>
        /// <param name="end">The end position</param>
        /// <returns>A list of grid positions. Note that these need to be translated to world space</returns>
        public List<GridPos> GetPath(Vector2 start, Vector2 end)
        {
            //_aiParam.Reset()
            return JumpPointFinder.FindPath(_aiParam);
        }
        
        /// <summary>
        /// Checks if a given tile may be passable
        /// </summary>
        /// <param name="x">The x coord (chunk)</param>
        /// <param name="y">The y coord (chunk)</param>
        /// <returns>True if the tile should be passable</returns>
        private bool IsPassable(int x, int y)
        {
            return _aiGrid.IsWalkableAt(x, y);
        }

        /// <summary>
        /// Checks if a given tile may be passable
        /// </summary>
        /// <param name="id">The ID of the tile</param>
        /// <returns>True if the tile should be passable</returns>
        private bool IsPassable(byte id)
        {
            return !_tileManager.IsSolid(id) || _tileManager.IsClimable(id);
        }

        #endregion

        #region Get and Set

        /// <summary>
        /// Gets the voxel material at the given position
        /// </summary>
        /// <param name="x">The x co-ordinate to check (Chunk Coords)</param>
        /// <param name="y">The y co-ordinate to check (Chunk Coords)</param>
        /// <returns>The material at [x,y]</returns>
        protected byte GetMaterial(int x, int y, int level)
        {
            return x >= 0 & x < _width & y >= 0 & y < _height ? _tiles[x, y, level] : (byte)0;
        }

        /// <summary>
        /// Safely sets the voxel material at the given position, should only be called by tile classes
        /// </summary>
        /// <param name="x">The x co-ordinate to check (Chunk Coords)</param>
        /// <param name="y">The y co-ordinate to check (Chunk Coords)</param>
        /// <param name="id">The material to set</param>
        public void SetTile(int x, int y, int level, byte id)
        {
            if (x >= 0 & x < _width & y >= 0 & y < _height)
            {
                this[x, y, level] = id;
            }
        }
        
        /// <summary>
        /// Safely sets the voxel material at the given position
        /// </summary>
        /// <param name="x">The x co-ordinate to check (Chunk Coords)</param>
        /// <param name="y">The y co-ordinate to check (Chunk Coords)</param>
        /// <param name="id">The material to set</param>
        public void SetTileSafe(int x, int y, int level, byte id)
        {
            _tiles[x, y, level] = id;
        }
        
        /// <summary>
        /// Sets a sphere to one tile ID
        /// </summary>
        /// <param name="x">The x coord of the centre of the circle</param>
        /// <param name="y">The y coord of the centre of the circle</param>
        /// <param name="radius">The radius of the circle</param>
        /// <param name="value">The tile ID to set</param>
        public void SetSphere(int x, int y, int level, int radius, byte value)
        {            
            for (int xx = x - radius - 1; xx < x + radius + 1; xx++)
            {
                for (int yy = y - radius - 1; yy < y + radius + 1; yy++)
                {
                    if (IsInsideOctagon(x, y, radius, xx, yy))
                        SetTile(xx, yy, level, value);
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

            writer.Write("Doodle Empires Multilevel Voxel Terrain ");
            writer.Write("0.0.1");

            writer.Write(_width);
            writer.Write(_height);
            writer.Write(_tiles.GetLength(2));

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    for (int z = 0; z < _tiles.GetLength(2); z++)
                    {
                        writer.Write(_tiles[x, y, z]);
                        writer.Write(_neighbourStates[x, y, z]);
                        writer.Write(_meta[x, y, z]);
                        writer.Write(_aiGrid.IsWalkableAt(x, y));
                    }
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
        /// <param name="labelFont">The font to use for rendering labels such as zone tags</param>
        /// <param name="tileManager">The tile manager to use</param>
        /// <param name="atlas">The texture atlas to use</param>
        /// <returns>A voxel terrain loaded from the stream</returns>
        public static SimpleTileMap ReadFromStream(Stream stream, GraphicsDevice graphics, SpriteFont labelFont, TileManager tileManager, TextureAtlas atlas)
        {
            BinaryReader reader = new BinaryReader(stream);

            string header = reader.ReadString();
            string version = reader.ReadString();

            switch (version)
            {
                case "0.0.1":
                    return LoadVersion_0_0_1(reader, labelFont, graphics, tileManager, atlas);
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
        /// <param name="labelFont">The font to use for rendering labels such as zone tags</param>
        /// <param name="graphics">The graphics device to bind to</param>
        /// <param name="tileManager">The tile manager to use</param>
        /// <param name="atlas">The texture atlas to use</param>
        /// <returns>A voxel terrain loaded from the stream</returns>
        private static SimpleTileMap LoadVersion_0_0_1(BinaryReader reader, SpriteFont labelFont, GraphicsDevice graphics, TileManager tileManager, TextureAtlas atlas)
        {
            int width = reader.ReadInt32();
            int height = reader.ReadInt32();
            int levels = reader.ReadInt32();

            SimpleTileMap terrain = new SimpleTileMap(graphics, labelFont, tileManager, atlas, width, height, levels);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < levels; z++)
                    {
                        terrain._tiles[x, y, z] = reader.ReadByte();
                        terrain._neighbourStates[x, y, z] = reader.ReadByte();
                    }
                }
            }

            reader.Dispose();
            return terrain;
        }

        #endregion

        #endregion

        #region Networking

        /// <summary>
        /// Reads a map out of a network message
        /// </summary>
        /// <param name="message">The message to read from</param>
        /// <param name="graphics">The graphics device to bind the level to</param>
        /// <param name="labelFont">The font to use for drawing labels</param>
        /// <param name="tileManager">The tile manager to use</param>
        /// <param name="textureAtlas">The texture atlas to use</param>
        /// <returns>A map loaded from the message</returns>
        public static SPMap ReadFromMessage(NetIncomingMessage message,
            GraphicsDevice graphics, SpriteFont labelFont, TileManager tileManager, TextureAtlas textureAtlas)
        {
            int width = message.ReadInt16();
            int height = message.ReadInt16();

            int seed = message.ReadInt32();

            SPMap map = new SPMap(graphics, labelFont, tileManager, textureAtlas, width, height, seed, true);
            
            int zoneCount = message.ReadInt32();

            for (int i = 0; i < zoneCount; i++)
            {
                map.AddPrebuiltZone(Zoning.ReadFromPacket(message));
            }

            return map;
        }

        #endregion

        /// <summary>
        /// Disposes of this object and free's it's resources
        /// </summary>
        public void Dispose()
        {
            _spriteBatch.Dispose();
            _basicEffect.Dispose();
            _pixelTex.Dispose();
            _updateThread.CancelAsync();
            _updateThread.Dispose();
        }
    }
}
