using DoodleEmpires.Engine.Entities.PathFinder;
using DoodleEmpires.Engine.Net;
using DoodleEmpires.Engine.Terrain;
using DoodleEmpires.Engine.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmileyFaceWars.Engine
{
    public class TileTerrain
    {
        /// <summary>
        /// Gets the width of a single voxel tile
        /// </summary>
        public const int TILE_WIDTH = 16;
        /// <summary>
        /// Gets the height of a single voxel tile
        /// </summary>
        public const int TILE_HEIGHT = 16;

        int _width;
        int _height;
        int _levels;

        int _maxX;
        int _maxY;

        bool _debugging = false;

        byte[, ,] _tiles;
        byte[, ,] _neighbours;
        BaseGrid _aiGrid;
        JumpPointParam _aiParam;
        JumpPointFinder _jpFinder;

        TileManager _tileManager;
        TextureAtlas _atlas;

        GraphicsDevice _graphics;
        SpriteBatch _spriteBatch;

        Color _transformColor = Color.White;
        Texture2D _pixelTex;

        public int WorldWidth
        {
            get { return _width * TILE_WIDTH; }
        }
        public int WorldHeight
        {
            get { return _height * TILE_HEIGHT; }
        }

        public Texture2D BackDrop
        {
            get;
            set;
        }

        /// <summary>
        /// An array holding all of the bounding rectangles to draw voxels in
        /// </summary>
        protected Rectangle[,] _voxelBounds;
        
        public TileTerrain(GraphicsDevice graphics, int width, int height, int levels, TileManager tileManager, TextureAtlas atlas, byte defaultID = 0, byte defaultMooreState = 0)
        {
            _width = width;
            _height = height;
            _levels = levels;

            _tiles = new byte[width, height, levels];
            _neighbours = new byte[width, height, levels];
            _voxelBounds = new Rectangle[width, height];

            BuildVoxelBouds();

            _aiGrid = new DynamicGridWPool(new NodePool());
            _aiParam = new JumpPointParam(_aiGrid, true, true, true, HeuristicMode.EUCLIDEAN);
            _jpFinder = new JumpPointFinder();

            _tileManager = tileManager;
            _atlas = atlas;

            _maxX = width - 1;
            _maxY = height - 1;

            _graphics = graphics;
            _spriteBatch = new SpriteBatch(graphics);

            _pixelTex = new Texture2D(graphics, 1, 1);
            _pixelTex.SetData(new[] { Color.White });

            for(int x = 0; x < width; x ++)
            {
                for(int y = 0; y < height; y ++)
                {
                    _tiles[x, y, 0] = defaultID;
                    _neighbours[x, y, 0] = defaultMooreState;
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

        int _MINX, _MINY, _MAXX, _MAXY;
        public void Render(ICamera2D camera)
        {
            camera.BeginDraw();

            if (BackDrop != null)
            {
                _spriteBatch.Begin(SpriteSortMode.Texture, null, SamplerState.LinearWrap, null, null);

                _spriteBatch.Draw(BackDrop, new Rectangle(0, -1, _graphics.Viewport.Width, _graphics.Viewport.Height),
                    camera.ViewBounds, Color.FromNonPremultiplied(255, 255, 255, 64));

                _spriteBatch.End();
            }
            
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap, null, null, null, camera.Transform);
            
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
                    for (int level = 0; level < _levels; level++)
                    {
                        _tileManager.RenderTile(_spriteBatch, _voxelBounds[x, y], _atlas, _neighbours[x, y, level],
                            _tiles[x, y, level], 0, _transformColor);
                    }
                    if (_debugging)
                    {
                        if (_aiGrid.IsWalkableAt(x, y))
                            _spriteBatch.Draw(_pixelTex, _voxelBounds[x, y], Color.Green * 0.25f);
                    }
                }
            }

            _spriteBatch.End();

            camera.EndDraw();
        }

        public void Set(int x, int y, int level, byte ID)
        {
            if (x >= 0 && x < _width && y >= 0 && y < _height && level >= 0 && level < _levels)
            {
                _tiles[x, y, level] = ID;
                UpdateNeighbours(x, y, level);
            }
        }

        private void UpdateNeighbours(int x, int y, int level, bool doNeighbours = true)
        {
            if (x >= 0 && y >= 0 && x < _width && y < _height)
            {
                ImmediateNeighbours neighbours = GetNeighbours(x, y, level);

                _neighbours[x, y, level] = (byte)neighbours;

                if (doNeighbours)
                {
                    UpdateNeighbours(x - 1, y - 1, level, false);
                    UpdateNeighbours(x, y - 1, level, false);
                    UpdateNeighbours(x + 1, y - 1, level, false);

                    UpdateNeighbours(x - 1, y, level, false);
                    UpdateNeighbours(x + 1, y, level, false);

                    UpdateNeighbours(x - 1, y + 1, level, false);
                    UpdateNeighbours(x, y + 1, level, false);
                    UpdateNeighbours(x + 1, y + 1, level, false);
                }
            }
        }

        protected ImmediateNeighbours GetNeighbours(int x, int y, int level)
        {
            ImmediateNeighbours ret = ImmediateNeighbours.None;
            
            if (CanConnect(x, y, level, x, y - 1, level))
                ret = ret | ImmediateNeighbours.T;
            if (CanConnect(x, y, level, x - 1, y, level))
                ret = ret | ImmediateNeighbours.L;
            if (CanConnect(x, y, level, x + 1, y, level))
                ret = ret | ImmediateNeighbours.R;
            if (CanConnect(x, y, level, x, y + 1, level))
                ret = ret | ImmediateNeighbours.B;

            if (ret == ImmediateNeighbours.All)
            {
                if (!CanConnect(x, y, level, x - 1, y - 1, level))
                {
                    ret = ImmediateNeighbours.InnerCorner;

                }
                else if (!CanConnect(x, y, level, x + 1, y - 1, level))
                {
                    ret = ImmediateNeighbours.InnerCorner | ImmediateNeighbours.T;

                }
                else if (!CanConnect(x, y, level, x - 1, y + 1, level))
                {
                    ret = ImmediateNeighbours.InnerCorner | ImmediateNeighbours.L;

                }
                else if (!CanConnect(x, y, level, x + 1, y + 1, level))
                {
                    ret = ImmediateNeighbours.InnerCorner | ImmediateNeighbours.T | ImmediateNeighbours.L;

                }
            }

            return ret;
        }

        protected bool CanConnect(int x1, int y1, int z1, int x2, int y2, int z2)
        {
            return GetMaterial(x1, y1, z1) == GetMaterial(x2, y2, z2);
        }

        protected bool CanConnect(byte mat1, byte mat2)
        {
            return mat1 == mat2;
        }

        protected byte GetMaterial(int x, int y, int level)        
        {
            if (x >= 0 && y >= 0 && x < _width && y < _height)
                return _tiles[x, y, level];
            else
                return 0;
        }
    }
}
